using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Emitter = System.Collections.Generic.Dictionary<Loko.Station.EventType, Loko.Station.EventListener>;

namespace Loko.Station
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

            _externalEmitter[EventType.Signaled] = delegate { };
            _externalEmitter[EventType.Linked] = delegate { };
            _externalEmitter[EventType.Blocked] = delegate { };
            _externalEmitter[EventType.Closed] = delegate { };

            emitter[EventType.Signaled] += _filter(EventType.Signaled);
            emitter[EventType.Linked] += _filter(EventType.Linked);
            emitter[EventType.Blocked] += _filter(EventType.Blocked);
            emitter[EventType.Closed] += _filter(EventType.Closed);
        }

        public IMessageSender Send(Loko.Station.MsgType type, String message)
        {
            return new MessageSender(type, message, _stMsg);
        }

        public event EventListener Signaled
        {
            add
            {
                lock (_externalEmitter)
                {
                    _externalEmitter[Loko.Station.EventType.Signaled] += value;
                }
            }
            remove
            {
                lock (_externalEmitter)
                {
                    _externalEmitter[Loko.Station.EventType.Signaled] -= value;
                }
            }
        }

        public event EventListener Linked
        {
            add
            {
                lock (_externalEmitter)
                {
                    _externalEmitter[Loko.Station.EventType.Linked] += value;
                }
            }
            remove
            {
                lock (_externalEmitter)
                {
                    _externalEmitter[Loko.Station.EventType.Linked] -= value;
                }
            }
        }

        public event EventListener Blocked
        {
            add
            {
                lock (_externalEmitter)
                {
                    _externalEmitter[Loko.Station.EventType.Blocked] += value;
                }
            }
            remove
            {
                lock (_externalEmitter)
                {
                    _externalEmitter[Loko.Station.EventType.Blocked] -= value;
                }
            }
        }

        public void Log(String message)
        {
            Console.WriteLine(message);
        }

        private EventListener _filter(EventType e)
        {
            return (String msg, StationDesc src) =>
            {
                var listeners = _externalEmitter[e].GetInvocationList();
                Parallel.ForEach(listeners, listener => (listener as EventListener).Invoke(msg, src));
            };
        }
    }
}