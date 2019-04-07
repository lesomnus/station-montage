using System;
using System.Collections.Generic;

namespace Loko
{
    internal static partial class Flows
    {
        internal class StationBody
        {
            public Dictionary<Loko.EventName, Loko.ReceiveListener> Emitter;
            public Station Station;

            public StationBody(Station station, Dictionary<Loko.EventName, Loko.ReceiveListener> emitter){
                this.Emitter = emitter;
                this.Station = station;
            }
        }
    }
}