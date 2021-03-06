using System;
using System.Threading;
using System.Threading.Tasks;

namespace Loko.Station
{
    public interface IStation
    {
        string FlowID { get; }
        string Name { get; }

        event EventListener Signaled;
        event EventListener Linked;
        event EventListener Blocked;

        IMessageSender Send(Loko.Station.MsgType type);
        IMessageSender Send(Loko.Station.MsgType type, string message);

        IEventGrabber Grab(StationDesc station);

        Task<(string, StationDesc)> When(EventType type);
        Task<(string, StationDesc)> When(EventType type, CancellationToken cancellationToken);

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

        Task<string> When(MsgType type);
        Task<string> When(MsgType type, CancellationToken cancellationToken);
    }
}
