using System;
using System.Threading.Tasks;

namespace Loko.Station
{
    public interface IStation
    {
        event EventListener Signaled;
        event EventListener Linked;
        event EventListener Blocked;

        IMessageSender Send(Loko.Station.MsgType type);
        IMessageSender Send(Loko.Station.MsgType type, string message);

        IEventGrabber Grab(StationDesc station);

        void Log(string message);

        void Close();
    }

    public interface IMessageSender
    {
        Task To(StationDesc destination);
    }

    public interface IEventGrabber
    {
        event EventListener Signaled;
        event EventListener Linked;
        event EventListener Blocked;
    }
}
