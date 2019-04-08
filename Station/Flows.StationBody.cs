using System;
using System.Collections.Generic;

namespace Loko.Station
{
    internal static partial class Flows
    {
        internal class StationBody
        {
            public Dictionary<Loko.Station.EventType, Loko.Station.EventListener> Emitter;
            public Station Station;

            public StationBody(Station station, Dictionary<Loko.Station.EventType, Loko.Station.EventListener> emitter){
                this.Emitter = emitter;
                this.Station = station;
            }
        }
    }
}