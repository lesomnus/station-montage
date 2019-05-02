using System;
using System.Threading;
using System.Threading.Tasks;

namespace Loko.Station.Template
{
    public abstract class Runner : Accept
    {
        private IStation _station = null;
        private TaskCompletionSource<string> _tcs = null;

        protected StationDesc Next;

        public Runner() : this(new StationDesc()) { }
        public Runner(StationDesc next) : base(1)
        {
            Next = next;
            _tcs = new TaskCompletionSource<string>();
        }

        public Task<string> Turn() => Turn(default(CancellationToken));
        public Task<string> Turn(CancellationToken cancellationToken)
        {
            if (cancellationToken != default(CancellationToken))
                cancellationToken.Register(_tcs.SetCanceled);
            return _tcs.Task;
        }

        public Task Prepare(StationDesc destination, string message = "") => _station.Send(MsgType.Link, message).To(destination);

        protected abstract Task<string> Invoked(string message);

        protected sealed override async void Accepted(IStation station, string message, StationDesc src)
        {
            _station = station;
            station.Signaled += (string msg, StationDesc _) =>
            {
                _tcs.TrySetResult(msg);
            };

            Next = src;

            var rst = await Invoked(message);
            await station.Send(MsgType.Signal, rst).To(Next);
            station.Close();
        }

        protected void Log(string message) => _station.Log(message);
    }
}