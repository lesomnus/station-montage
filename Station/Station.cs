using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Loko.Station
{
    using Emitter = Dictionary<EventType, EventListener>;
    using Blocked = HashSet<StationDesc>;
    using Grabbed = Dictionary<MsgType, EventListener>;
    using GrabbedSet = Dictionary<StationDesc, Dictionary<MsgType, EventListener>>;

    partial class Station : IStation
    {
        private readonly string _flowID = null;
        private readonly Metro.Api.Station _stMsg = null;
        private readonly Emitter _internalEmitter = null;
        private readonly Emitter _externalEmitter = null;
        private readonly Blocked _blocked = null;
        private readonly GrabbedSet _grabbeds = null;

        public string Name { get; }

        public Station(string flowID, string name, Emitter emitter)
        {
            Name = name;
            _flowID = flowID;
            _stMsg = new Metro.Api.Station { Id = flowID, Name = name };
            _internalEmitter = emitter;
            _externalEmitter = new Emitter(){
                {EventType.Signaled, delegate{}},
                {EventType.Linked, delegate{}},
                {EventType.Blocked, delegate{}},
                {EventType.Closed, delegate{}}
            };
            _blocked = new Blocked();
            _grabbeds = new GrabbedSet();

            emitter[EventType.Signaled] += _filter(EventType.Signaled, MsgType.Signal);
            emitter[EventType.Linked] += _filter(EventType.Linked, MsgType.Link);
            emitter[EventType.Blocked] += _filter(EventType.Blocked, MsgType.Block);
            emitter[EventType.Closed] += _filter(EventType.Closed);
        }


        #region events

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

        #endregion

        public IMessageSender Send(Loko.Station.MsgType type) => new MessageSender(type, "", _stMsg, _blocked);
        public IMessageSender Send(Loko.Station.MsgType type, string message) => new MessageSender(type, message, _stMsg, _blocked);

        public IEventGrabber Grab(StationDesc station)
        {
            if (!_grabbeds.ContainsKey(station))
            {
                var grabbed = new Grabbed(){
                    {MsgType.Signal, delegate{}},
                    {MsgType.Link, delegate{}},
                    {MsgType.Block, delegate{}}
                };
                _grabbeds.Add(station, grabbed);
            }

            return new EventGrabber(_grabbeds[station]);
        }

        public void Log(string message)
        {
            var utc = (long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalMilliseconds;
            Console.WriteLine($"{utc}|{Name}|{message}");
        }

        public void Close()
        {
            var listeners = _internalEmitter[EventType.Closed].GetInvocationList();

            Parallel.ForEach(listeners, listener => (listener as EventListener).Invoke("", new StationDesc()));
        }

        private EventListener _filter(EventType type)
        {
            Debug.Assert(type == EventType.Closed, "Only 'Closed' event type is allowed");

            return (string msg, StationDesc src) => _externalEmitter[type].Invoke(msg, src);
        }
        private EventListener _filter(EventType eType, MsgType mType)
        {
            Debug.Assert(eType != EventType.Closed, "'Closed' event type is not allowed.");
            Debug.Assert(eType == EventType.Signaled ? mType == MsgType.Signal : true, "Event type and message type should be matched");
            Debug.Assert(eType == EventType.Linked ? mType == MsgType.Link : true, "Event type and message type should be matched");
            Debug.Assert(eType == EventType.Blocked ? mType == MsgType.Block : true, "Event type and message type should be matched");

            return (string msg, StationDesc src) =>
            {
                if (_blocked.Contains(src)) return;

                var listeners = _grabbeds.ContainsKey(src)
                    ? _grabbeds[src][mType]
                    : _externalEmitter[eType];

                listeners.Invoke(msg, src);
            };
        }
    }
}