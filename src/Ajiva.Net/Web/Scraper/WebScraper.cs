using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Ajiva.Net.Web.Scraper
{
    public class WebScraper
    {
        public event Action<HttpRequestMessage>? OnRequestSend;
        public Uri BaseUri { get; }
        public CookieContainer? CookieContainer { get; }

        public HttpClientHandler ClientHandler { get; }

        public HttpClient Client { get; set; }

        public WebScraper(Uri baseUri, CookieContainer? cookieContainer, UserAgent userAgent, AcceptHeader acceptHeader)
        {
            BaseUri = baseUri;
            CookieContainer = cookieContainer;

            ClientHandler = new()
            {
                CookieContainer = cookieContainer ?? new(),
                UseCookies = cookieContainer is not null,
                AllowAutoRedirect = true,
                UseProxy = false,
                Proxy = null,
                AutomaticDecompression = DecompressionMethods.All,
                MaxConnectionsPerServer = int.MaxValue,
            };

            Client = new(ClientHandler, false)
            {
                BaseAddress = baseUri
            };

            Client.DefaultRequestHeaders.Add(HeaderStrings.Host, baseUri.Host);
            Client.DefaultRequestHeaders.Add(HeaderStrings.UserAgent, userAgent.Agent);
            Client.DefaultRequestHeaders.Add(HeaderStrings.Accept, acceptHeader.Accept);
        }

        public void WriteCookie(Action<string> writer)
        {
            if (CookieContainer is null) return;

            foreach (Cookie cookie in CookieContainer.GetCookies(BaseUri))
                writer($"{cookie.Name} = {cookie.Value}");
        }

        public async Task<HttpResponseMessage> SendRequest(HttpMethod method, string url, HttpContent? content, IEnumerable<(string name, string value)> headers)
        {
            var httpRequestMessage = new HttpRequestMessage
            {
                Method = method,
                RequestUri = new(BaseUri, url),
                Content = content
            };

            foreach (var (name, value) in headers)
                httpRequestMessage.Headers.Add(name, value);

            OnRequestSend?.Invoke(httpRequestMessage);
            return await Client.SendAsync(httpRequestMessage);
        }

        public async Task<HttpResponseMessage> SendPostRequest(string url, FormUrlEncodedContent formContent, AcceptHeader accept, bool xhr)
        {
            return await SendRequest(HttpMethod.Post, url, formContent,
                xhr
                    ? new[] {HeaderStrings.XhrRequestTuple, accept.Tuple}
                    : new[] {accept.Tuple});
        }

        public async Task<HttpResponseMessage> SendGetRequest(string url, AcceptHeader accept, bool xhr)
        {
            return await SendRequest(HttpMethod.Get, url, null,
                xhr
                    ? new[] {HeaderStrings.XhrRequestTuple, accept.Tuple}
                    : new[] {accept.Tuple});
        }
    }
}
