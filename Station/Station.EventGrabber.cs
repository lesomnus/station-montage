using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Loko.Station
{
    partial class Station
    {
        public struct EventGrabber : IEventGrabber
        {
            private Dictionary<MsgType, EventListener> _grabbed;

            // TODO: receive lock.
            internal EventGrabber(Dictionary<MsgType, EventListener> grabbed)
            {
                _grabbed = grabbed;
            }

            // TODO: lock required.
            public event EventListener Signaled
            {
                add
                {
                    _grabbed[MsgType.Signal] += value;
                }
                remove
                {
                    _grabbed[MsgType.Signal] -= value;
                }
            }
            public event EventListener Linked
            {
                add
                {
                    _grabbed[MsgType.Link] += value;
                }
                remove
                {
                    _grabbed[MsgType.Link] -= value;
                }
            }
            public event EventListener Blocked
            {
                add
                {
                    _grabbed[MsgType.Block] += value;
                }
                remove
                {
                    _grabbed[MsgType.Block] -= value;
                }
            }

            public Task<string> When(MsgType type) => When(type, default(CancellationToken));
            public Task<string> When(MsgType type, CancellationToken cancellationToken)
            {
                var tcs = new TaskCompletionSource<string>();
                if (cancellationToken != default(CancellationToken))
                    cancellationToken.Register(tcs.SetCanceled);

                var grabbed = _grabbed[type];
                EventListener listener = null;

                grabbed += listener = (string msg, StationDesc _) =>
                {
                    grabbed -= listener;
                    tcs.TrySetResult(msg);
                };

                return tcs.Task;
            }
        }
    }
}