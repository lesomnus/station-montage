using System;
using Grpc.Core;

using RouterCli = Loko.Metro.Api.Router.RouterClient;

namespace Loko.Station
{
    internal static class RouterConn
    {
        private static RouterCli _cli = null;
        private static string _host = "";
        private static UInt16 _port = 0;
        private static Channel _chan = null;
        static RouterConn()
        {
            var host = Environment.GetEnvironmentVariable("LOKO_METRO_HOST");
            if (String.IsNullOrWhiteSpace(host))
            {
                host = "0.0.0.0";
            }

            UInt16 port;
            if (!UInt16.TryParse(Environment.GetEnvironmentVariable("LOKO_METRO_PORT"), out port))
            {
                port = 50051;
            }

            _host = host;
            _port = port;

            Console.WriteLine($"Metro server on {_host}:{_port}");
        }

        public static void Connect()
        {
            _chan = new Channel(_host, _port, ChannelCredentials.Insecure);
            _cli = new RouterCli(_chan);
        }

        public static RouterCli Client
        {
            get
            {
                if (_cli == null) Connect();
                return _cli;
            }
        }
    }
}