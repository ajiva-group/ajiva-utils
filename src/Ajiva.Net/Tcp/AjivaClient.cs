using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Ajiva.Wrapper.Logger;

namespace Ajiva.Net.Tcp
{
    public class AjivaClient<T> where T : Enum
    {
        private IPEndPoint? Endpoint { get; init; }
        private T HandShakeValue { get; }
        private TcpClient TcpClient { get; }
        public int ClientId { get; private set; }
        public bool Server { get; private set; }

        public Queue<Body<T>> SendPackets { get; } = new();
        public Queue<Body<T>> ReceivedPackets { get; } = new();

        public PacketReceivedDelegate? PacketReceived;

        public event Action? OnHandShakeSucceeded;
        public event Action? OnHandShakeFailed;

        /// <returns>if packet should be added to ReceivedPackets</returns>
        public delegate bool PacketReceivedDelegate(Body<T> body);

        public static AjivaClient<T> ServerClient(TcpClient client, T handShake, int clientId) => new(handShake, client) {ClientId = clientId, Server = true};
        public static AjivaClient<T> ClientClient(IPEndPoint iEndpoint, T handShake) => new(handShake, new()) {ClientId = 0, Endpoint = iEndpoint, Server = false};

        private AjivaClient(T handShakeValue, TcpClient tcpClient)
        {
            HandShakeValue = handShakeValue;
            TcpClient = tcpClient;
        }

        public async Task Connect(CancellationToken cancellationToken)
        {
            if (!TcpClient.Connected && Endpoint is not null)
            {
                TcpClient.Connect(Endpoint);

                await Task.Factory.StartNew(async () =>
                {
                    if (await HandShake(false, cancellationToken))
                    {
                        await RunClient(cancellationToken);
                    }
                }, cancellationToken);
            }
        }

        public async Task RunClient(CancellationToken cancellationToken)
        {
            var header = AjivaNetUtils.NewHeader();

            while (TcpClient.Connected && !cancellationToken.IsCancellationRequested)
            {
                while (SendPackets.Count != 0) await SendPacket(SendPackets.Dequeue(), cancellationToken);

                if (TcpClient.Available > 0) await ReceivePacket(cancellationToken, header);

                await Task.Delay(1, cancellationToken);
            }
            if (TcpClient.Connected)
                TcpClient.Close();
        }

        private async Task SendPacket(Body<T> dequeue, CancellationToken cancellationToken)
        {
            await AjivaNetUtils.SendPaket(TcpClient, new() {Type = (int)(object)dequeue.Type, Version = 1, ClientId = ClientId}, dequeue.Memory, cancellationToken);
        }

        public async Task ReceivePacket(CancellationToken cancellationToken, AjivaMemory? header = null)
        {
            var (head, data) = await AjivaNetUtils.GetPaket(TcpClient, cancellationToken, header);
            var body = new Body<T>(head.GetType<T>(), data);

            if (!Server && ClientId == 0)
                ClientId = head.ClientId;

            if (PacketReceived == null || PacketReceived.Invoke(body)) ReceivedPackets.Enqueue(body);
        }

        public async Task<bool> HandShake(bool server, CancellationToken cancellationToken)
        {
            bool SucceedHandshake()
            {
                OnHandShakeSucceeded?.Invoke();
                return true;
            }

            bool FailHandshake()
            {
                OnHandShakeFailed?.Invoke();
                return false;
            }

            var support = NewHeader(HandShakeValue);

            bool VersionMisMatch(AjivaNetHead remote)
            {
                LogHelper.WriteLine($"Versions: support:{support.Version}, remote:{remote.Version}");
                return support.Version != remote.Version;
            }

            var trust = Guid.NewGuid();

            if (server)
                await AjivaNetUtils.SendPaket(TcpClient, support, AjivaMemory.With(trust), cancellationToken);

            var (head, data) = await AjivaNetUtils.GetPaket(TcpClient, cancellationToken);

            var href = 0;
            Guid check;
            if (server)
            {
                if (VersionMisMatch(head)) 
                    return FailHandshake();

                check = data.Read<Guid>(ref href);
                LogHelper.WriteLine($"Handshake Part Server: {trust}, Client: {check}");
                return check == trust ? SucceedHandshake() : FailHandshake();
            }
            if (VersionMisMatch(head)) return FailHandshake();

            check = data.Read<Guid>(ref href);
            LogHelper.WriteLine($"Handshake Part Server: {check}");
            await AjivaNetUtils.SendPaket(TcpClient, support, AjivaMemory.With(check), cancellationToken);
            return SucceedHandshake();
        }

        private int headerCount;
        private AjivaNetHead NewHeader(T type) => new() {Type = (int)(object)type, Version = 1, Index = headerCount++, ClientId = ClientId};
    }

    public class Body<T> where T : Enum
    {
        public T Type { get; set; }
        public AjivaMemory Memory { get; set; }

        public Body(T type, AjivaMemory memory)
        {
            Type = type;
            Memory = memory;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{nameof(Type)}: {Type}, {nameof(Memory)}: {Memory.AsString()}";
        }
    }
}
