using System;

namespace Loko.Station
{
    public delegate void EventListener(string message, StationDesc src);

    public enum EventType
    {
        _ = 0,
        Closed,
        Linked,
        Blocked,
        Signaled,
    }

    public enum MsgType
    {
        Signal = 0,
        Link,
        Block,
    }

}