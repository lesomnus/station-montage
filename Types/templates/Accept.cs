namespace Loko.Station.Template
{
    public abstract class Accept : IApp
    {
        private IStation _station;
        private int _count;

        public Accept(int count)
        {
            _count = count;
        }

        public void Open(IStation station, string[] args) {
            _station = station;
            _station.Linked += _accept;
        }

        protected abstract void Accepted(IStation station, string message, StationDesc src);

        private void _accept(string message, StationDesc src)
        {
            if (--_count < 1){
                _station.Linked -= _accept;
                _station.Linked += _block;
            }
            Accepted(_station, message, src);
        }

        private void _block(string message, StationDesc src)
        {
            _station.Send(MsgType.Block).To(src);
        }
    }
}