using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Emitter = System.Collections.Generic.Dictionary<Loko.EventName, Loko.ReceiveListener>;

namespace Loko
{
    partial class Station : IStation
    {

        private readonly String _flowID = null;
        private readonly Metro.Api.Station _stMsg = null;
        private readonly Emitter _internalEmitter = null;
        private readonly Emitter _externalEmitter = null;
        public Station(String flowID, String name, Emitter emitter)
        {
            _flowID = flowID;
            _internalEmitter = emitter;
            _externalEmitter = new Emitter();
            _stMsg = new Metro.Api.Station { Id = flowID, Name = name };

            _externalEmitter[EventName.Signaled] = delegate { };
            _externalEmitter[EventName.Linked] = delegate { };
            _externalEmitter[EventName.Blocked] = delegate { };
            _externalEmitter[EventName.Closed] = delegate { };

            emitter[EventName.Signaled] += _filter(EventName.Signaled);
            emitter[EventName.Linked] += _filter(EventName.Linked);
            emitter[EventName.Blocked] += _filter(EventName.Blocked);
            emitter[EventName.Closed] += _filter(EventName.Closed);
        }

        public IMessageSender Send(Loko.MsgType type, String message)
        {
            return new MessageSender(type, message, _stMsg);
        }

        public event ReceiveListener Signaled
        {
            add
            {
                lock (_externalEmitter)
                {
                    _externalEmitter[Loko.EventName.Signaled] += value;
                }
            }
            remove
            {
                lock (_externalEmitter)
                {
                    _externalEmitter[Loko.EventName.Signaled] -= value;
                }
            }
        }

        public event ReceiveListener Linked
        {
            add
            {
                lock (_externalEmitter)
                {
                    _externalEmitter[Loko.EventName.Linked] += value;
                }
            }
            remove
            {
                lock (_externalEmitter)
                {
                    _externalEmitter[Loko.EventName.Linked] -= value;
                }
            }
        }

        public event ReceiveListener Blocked
        {
            add
            {
                lock (_externalEmitter)
                {
                    _externalEmitter[Loko.EventName.Blocked] += value;
                }
            }
            remove
            {
                lock (_externalEmitter)
                {
                    _externalEmitter[Loko.EventName.Blocked] -= value;
                }
            }
        }

        public void Log(String message)
        {
            Console.WriteLine(message);
        }

        private ReceiveListener _filter(EventName e)
        {
            return (String msg, StationDesc src) =>
            {
                var listeners = _externalEmitter[e].GetInvocationList();
                Parallel.ForEach(listeners, listener => (listener as ReceiveListener).Invoke(msg, src));
            };
        }
    }
}