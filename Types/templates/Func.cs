using System.Threading.Tasks;

namespace Loko.Station.Template
{
    public abstract class Func : Accept
    {
        private IStation _station;
        protected StationDesc Next;

        public Func() : this(new StationDesc()) { }
        public Func(StationDesc next) : base(1)
        {
            Next = next;
        }

        protected abstract Task<string> Invoked(string message);

        protected sealed override async void Accepted(IStation station, string message, StationDesc _)
        {
            _station = station;
            
            string rst = await Invoked(message);
            await station.Send(MsgType.Signal, rst).To(Next);
            station.Close();
        }

        protected void Log(string message) => _station.Log(message);

        protected Task<string> Call(StationDesc func, string message)
        {
            var finished = _station.Grab(func).When(MsgType.Signal);
            _station.Send(MsgType.Link, message).To(func);
            return finished;
        }
    }
}