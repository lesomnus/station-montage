using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Loko.Station
{
    using Emitter = Dictionary<EventType, EventListener>;
    using Grabbed = Dictionary<MsgType, EventListener>;
    using GrabbedSet = Dictionary<StationDesc, Dictionary<MsgType, EventListener>>;

    partial class Station : IStation
    {

        private readonly string _flowID = null;
        private readonly Metro.Api.Station _stMsg = null;
        private readonly Emitter _internalEmitter = null;
        private readonly Emitter _externalEmitter = null;
        private readonly GrabbedSet _grabbeds = null;
        public Station(string flowID, string name, Emitter emitter)
        {
            _flowID = flowID;
            _stMsg = new Metro.Api.Station { Id = flowID, Name = name };
            _internalEmitter = emitter;
            _externalEmitter = new Emitter(){
                {EventType.Signaled, delegate{}},
                {EventType.Linked, delegate{}},
                {EventType.Blocked, delegate{}},
                {EventType.Closed, delegate{}}
            };
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

        public IMessageSender Send(Loko.Station.MsgType type, string message) => new MessageSender(type, message, _stMsg);

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
            Console.WriteLine(message);
        }

        public void Close()
        {
            var listeners = _internalEmitter[EventType.Closed].GetInvocationList();

            Parallel.ForEach(listeners, listener => (listener as EventListener).Invoke("", new StationDesc()));
        }

        private EventListener _filter(EventType type)
        {
            Debug.Assert(type == EventType.Closed, "Only 'Closed' event type allowed");

            return (string msg, StationDesc src) =>
            {
                var listeners = _externalEmitter[type].GetInvocationList();
                Parallel.ForEach(listeners, listener => (listener as EventListener).Invoke(msg, src));
            };
        }
        private EventListener _filter(EventType eType, MsgType mType)
        {
            Debug.Assert(eType != EventType.Closed, "'Closed' event type not allowd.");
            Debug.Assert(eType == EventType.Signaled ? mType == MsgType.Signal : true, "Event type and message type should be matched");
            Debug.Assert(eType == EventType.Linked ? mType == MsgType.Link : true, "Event type and message type should be matched");
            Debug.Assert(eType == EventType.Blocked ? mType == MsgType.Block : true, "Event type and message type should be matched");

            return (string msg, StationDesc src) =>
            {
                var listeners = _grabbeds.ContainsKey(src)
                    ? _externalEmitter[eType].GetInvocationList()
                    : _grabbeds[src][mType].GetInvocationList();

                Parallel.ForEach(listeners, listener => (listener as EventListener).Invoke(msg, src));
            };
        }
    }
}