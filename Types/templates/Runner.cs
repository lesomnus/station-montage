using System;
using System.Threading;
using System.Threading.Tasks;

namespace Loko.Station.Template
{
    public abstract class Runner : Accept
    {
        private IStation _station;
        private SemaphoreSlim _turn = new SemaphoreSlim(0, 1);
        private string _rst;

        protected StationDesc Next;

        public Runner() : this(new StationDesc()) { }
        public Runner(StationDesc next) : base(1)
        {
            Next = next;
        }

        private async Task<string> _Turn()
        {
            await _turn.WaitAsync();
            return _rst;
        }

        private void _Prepare(StationDesc destination, string message = "")
        {
            _station.Send(MsgType.Link, message).To(destination);
        }

        protected abstract Task<string> Invoked(string message, System.Func<Task<string>> turn, System.Action<StationDesc, string> prepare);

        protected sealed override async void Accepted(IStation station, string message, StationDesc src)
        {
            _station = station;
            _station.Signaled += (string msg, StationDesc _) =>
            {
                _rst = msg;
                _turn.Release();
            };

            var rst = await Invoked(message, _Turn, _Prepare);
            await _station.Send(MsgType.Signal, rst).To(Next);
            _station.Close();
        }

        protected void Log(string message) => _station.Log(message);
    }
}