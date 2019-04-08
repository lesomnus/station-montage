using System;
using System.Threading.Tasks;

namespace Loko.Station
{
    partial class Station
    {
        struct MessageSender: IMessageSender
        {
            private MsgType _type;
            private String _msg;
            private readonly Metro.Api.Station _srcMsg;

            public MessageSender(MsgType type, String message, Metro.Api.Station srource)
            {
                _type = type;
                _msg = message == null ? "" : message;
                _srcMsg = srource;
            }

            public async Task<Metro.Api.Response> To(StationDesc destination)
            {
                var cli = RouterConn.Client;
                var dstMsg = new Metro.Api.Station { Name = destination.Name, Image = destination.Image };

                switch (_type)
                {
                    default: break;

                    case MsgType.Signal:
                        {
                            var req = new Metro.Api.TransmitRequest { Token = Token.Create(), Src = _srcMsg, Dst = dstMsg, Message = _msg };
                            return await RouterConn.Client.TransmitAsync(req);
                        }

                    case MsgType.Link:
                        {
                            var req = new Metro.Api.LinkRequest { Token = Token.Create(), Src = _srcMsg, Dst = dstMsg, Message = _msg };
                            return await RouterConn.Client.LinkAsync(req);
                        }

                    case MsgType.Block:
                        {
                            var req = new Metro.Api.BlockRequest { Token = Token.Create(), Src = _srcMsg, Dst = dstMsg, Message = _msg };
                            return await RouterConn.Client.BlockAsync(req);
                        }
                }

                return null;
            }
        }
    }
}