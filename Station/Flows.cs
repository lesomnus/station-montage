using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Loko
{
    using StationCollection = ConcurrentDictionary<String, Flows.StationBody>;
    using FlowCollection = ConcurrentDictionary<String, ConcurrentDictionary<String, Flows.StationBody>>;

    internal static partial class Flows
    {
        private static FlowCollection flows = new FlowCollection();

        private static StationBody createStationBody(Metro.Api.Station station) => createStationBody(station.Id, station.Name);
        private static StationBody createStationBody(String flowID, String name)
        {
            var body = new StationBody(
                new Station(flowID, name, new Dictionary<Loko.EventName, Loko.ReceiveListener>()),
                new System.Collections.Generic.Dictionary<EventName, ReceiveListener>()
            );

            return body;
        }

        public static StationBody Create(Metro.Api.Station station) => Create(station.Id, station.Name);
        public static StationBody Create(String flowID, String name)
        {
            StationCollection stations;

            if (!flows.TryGetValue(flowID, out stations))
            {
                flows.TryAdd(flowID, stations = new StationCollection());
            }

            var body = createStationBody(flowID, name);

            if (!stations.TryAdd(name, body))
            {
                stations.TryGetValue(name, out body);
            }

            return body;
        }

        public static Boolean Has(Metro.Api.Station station) => Has(station.Id, station.Name);
        public static Boolean Has(String flowID, String name)
        {
            StationCollection stations;

            if (!flows.TryGetValue(flowID, out stations))
            {
                return false;
            }

            return stations.ContainsKey(name);
        }

        public static Boolean TryGet(Metro.Api.Station station, out StationBody body) => TryGet(station.Id, station.Name, out body);
        public static Boolean TryGet(String flowID, String name, out StationBody body)
        {
            body = null;

            StationCollection stations;

            if (!flows.TryGetValue(flowID, out stations))
            {
                return false;
            }

            if (!stations.TryGetValue(name, out body))
            {
                return false;
            }

            return true;
        }

        public static void Del(Metro.Api.Station station) => TryDel(station);
        public static void Del(string flowID, String name) => TryDel(flowID, name);
        public static Boolean TryDel(Metro.Api.Station station) => TryDel(station.Id, station.Name);
        public static Boolean TryDel(String flowID, String name)
        {
            StationCollection stations;

            if (!flows.TryGetValue(flowID, out stations))
            {
                return false;
            }

            if (!stations.TryRemove(name, out _))
            {
                return false;
            }

            if (stations.IsEmpty)
            {
                flows.TryRemove(flowID, out _);
            }

            return true;
        }


    }
}