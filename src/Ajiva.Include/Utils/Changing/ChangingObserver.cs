using System;

namespace ajiva.Utils.Changing
{
    public class ChangingObserver : IChangingObserver
    {
        public ChangingObserver(int changeThreshold)
        {
            ChangeThreshold = changeThreshold;
        }

        /// <inheritdoc />
        public int ChangeThreshold { get; }

        /// <inheritdoc />
        public int ChangedAmount { get; set; } = 0;

        private object _lock { get; } = new();

        /// <inheritdoc />
        public event OnChangedDelegate? OnChanged;

        /// <inheritdoc />
        public void Changed()
        {
            lock (_lock)
            {
                ChangedAmount++;
                if (ChangedAmount > ChangeThreshold)
                {
                    ChangedAmount = 0;

                    OnChanged?.Invoke(this);
                }
            }
        }
    }
    public class ChangingObserverOnlyAfter<TSender, TValue> : IChangingObserverOnlyAfter<TSender, TValue> where TSender : class where TValue : struct
    {
        public ChangingObserverOnlyAfter(TSender owner, Func<TValue> result, int changeThreshold)
        {
            Owner = owner;
            Result = result;
            ChangeThreshold = changeThreshold;
        }

        public TSender Owner { get; }

        /// <inheritdoc />
        public Func<TValue> Result { get; }

        /// <inheritdoc />
        public int ChangeThreshold { get; }

        /// <inheritdoc />
        public int ChangedAmount { get; set; }

        /// <inheritdoc />
        public event IChangingObserverOnlyAfter<TSender, TValue>.OnChangedDelegate? OnChanged;

        /// <inheritdoc />
        public void Changed(TValue after)
        {
            lock (_lock)
            {
                ChangedAmount++;
                if (ChangedAmount > ChangeThreshold)
                {
                    ChangedAmount = 0;
                    OnChanged?.Invoke(Owner, after);
                }
            }
        }

        private object _lock { get; } = new();
    }
    public class ChangingObserver<TSender, TValue> : IChangingObserver<TSender, TValue> where TSender : class where TValue : struct
    {
        public ChangingObserver(TSender owner, Func<TValue> result, int changeThreshold)
        {
            Owner = owner;
            Result = result;
            ChangeThreshold = changeThreshold;
        }

        public TSender Owner { get; }

        /// <inheritdoc />
        public Func<TValue> Result { get; }

        /// <inheritdoc />
        public int ChangeThreshold { get; }

        /// <inheritdoc />
        public int ChangedAmount { get; set; }

        /// <inheritdoc />
        public event IChangingObserver<TSender, TValue>.OnChangedDelegate? OnChanged;

        /// <inheritdoc />
        public void Changed(TValue before, TValue after)
        {
            lock (_lock)
            {
                ChangedAmount++;
                if (ChangedAmount > ChangeThreshold)
                {
                    ChangedAmount = 0;

                    OnChanged?.Invoke(Owner, before, after);
                }
            }
        }

        private object _lock { get; } = new();
    }

    public class OverTimeChangingObserver : IOverTimeChangingObserver
    {
        public OverTimeChangingObserver(int delayUpdateFor)
        {
            DelayUpdateFor = delayUpdateFor;
        }

        /// <inheritdoc />
        public int DelayUpdateFor { get; }

        /// <inheritdoc />
        public int ChangedAmount { get; set; } = 0;

        /// <inheritdoc />
        public long ChangeBeginCycle { get; set; } = 0;

        private object _lock { get; } = new();

        /// <inheritdoc />
        public event IOverTimeChangingObserver.OnChangedDelegate? OnChanged;

        /// <inheritdoc />
        public event IOverTimeChangingObserver.OnUpdateDelegate? OnUpdate;

        /// <inheritdoc />
        public void Changed()
        {
            lock (_lock)
            {
                ChangedAmount++;
            }
            OnChanged?.Invoke(this);
        }

        /// <inheritdoc />
        public void Updated()
        {
            lock (_lock)
            {
                ChangedAmount = 0;
                ChangeBeginCycle = 0;
            }
            OnUpdate?.Invoke(this);
        }

        public IDisposable BeginBigChange()
        {
            BigChangeCount++;
            Current ??= new BachChange(this);
            return Current;
        }

        public void EndBigChange()
        {
            BigChangeCount--;
            if (BigChangeCount <= 0)
            {
                BigChangeCount = 0;
                if (Current is not null)
                {
                    if (ChangedAmount > Current.BeginChange)
                    {
                        ChangeBeginCycle = Current.BeginCycle; // - DelayUpdateFor ?
                    }
                    Current = null;
                }
            }
        }

        /// <inheritdoc />
        public bool Locked => Current is not null;

        public int BigChangeCount { get; set; }
        public BachChange? Current { get; set; }
        public class BachChange : DisposingLogger
        {
            /// <inheritdoc />
            public BachChange(IOverTimeChangingObserver parent)
            {
                Parent = parent;
                BeginCycle = parent.ChangeBeginCycle;
                BeginChange = parent.ChangedAmount;
            }

            public long BeginCycle { get; set; }
            public int BeginChange { get; }
            private IOverTimeChangingObserver Parent { get; }

            /// <inheritdoc />
            protected override void ReleaseUnmanagedResources(bool disposing)
            {
                base.ReleaseUnmanagedResources(disposing);
                Parent.EndBigChange();
            }
        }
    }
}
