namespace Loko.Station.Template
{
    public abstract class Accept : IApp
    {
        protected IStation Station;
        private int _count;
        private EventListener _listener;

        public Accept(int count)
        {
            _count = count;

            _listener = _accept;
        }

        public void Open(IStation station, string[] args) {
            this.Station = station;
            station.Linked += _listener;
        }

        public abstract void Start(string message, StationDesc src);

        private void _accept(string message, StationDesc src)
        {
            if (--_count == 0) _listener = _block;
            Start(message, src);
        }

        private void _block(string message, StationDesc src)
        {
            this.Station.Send(MsgType.Block).To(src);
        }
    }
}