using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Loko.Station
{
    using Blocked = HashSet<StationDesc>;

    partial class Station
    {
        public struct MessageSender : IMessageSender
        {
            private MsgType _type;
            private string _msg;
            private readonly Metro.Api.Station _srcMsg;
            private readonly Blocked _blocked;

            internal MessageSender(MsgType type, string message, Metro.Api.Station srource, Blocked blocked)
            {
                _type = type;
                _msg = message == null ? "" : message;
                _srcMsg = srource;
                _blocked = blocked;
            }

            public async Task To(StationDesc destination)
            {
                var cli = RouterConn.Client;
                var dstMsg = new Metro.Api.Station { Name = destination.Name, Image = destination.Image };

                switch (_type)
                {
                    default: break;

                    case MsgType.Signal:
                        {
                            var req = new Metro.Api.TransmitRequest { Token = Token.Create(), Src = _srcMsg, Dst = dstMsg, Message = _msg };
                            var res = await RouterConn.Client.TransmitAsync(req);

                            switch (res.Code)
                            {
                                case 200: return;
                                case 403: throw new NotPermittedException(destination);
                                case 404: throw new ImageNotFoundException(destination);
                                default: throw new UnmanagedStatusCodeException(res.Code);
                            }
                        }

                    case MsgType.Link:
                        {
                            var req = new Metro.Api.LinkRequest { Token = Token.Create(), Src = _srcMsg, Dst = dstMsg, Message = _msg };
                            var res = await RouterConn.Client.LinkAsync(req);

                            switch (res.Code)
                            {
                                case 200: return;
                                case 404: throw new ImageNotFoundException(destination);
                                default: throw new UnmanagedStatusCodeException(res.Code);
                            }
                        }

                    case MsgType.Block:
                        {
                            _blocked.Add(destination);
                            var req = new Metro.Api.BlockRequest { Token = Token.Create(), Src = _srcMsg, Dst = dstMsg, Message = _msg };
                            var res = await RouterConn.Client.BlockAsync(req);

                            switch (res.Code)
                            {
                                case 200: return;
                                case 404: throw new ImageNotFoundException(destination);
                                default: throw new UnmanagedStatusCodeException(res.Code);
                            }
                        }
                }

                return;
            }
        }
    }
}