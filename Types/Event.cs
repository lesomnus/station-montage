using System;

namespace Loko
{
    public delegate void ReceiveListener(String message, StationDesc src);

    public enum EventName
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