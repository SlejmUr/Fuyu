using System;
using System.Threading.Tasks;
using Fuyu.Backend.BSG.Models.Responses;
using Fuyu.Backend.EFTMain.Networking;
using Fuyu.Common.Serialization;

namespace Fuyu.Backend.EFTMain.Controllers.Http;

public class GameKeepaliveController : AbstractEftHttpController
{
    public GameKeepaliveController() : base("/client/game/keepalive")
    {
    }

    public override Task RunAsync(EftHttpContext context)
    {
        // TODO: generate this
        // --seionmoya, 2024-11-18
        var response = new ResponseBody<GameKeepaliveResponse>
        {
            data = new GameKeepaliveResponse()
            {
                msg = "OK",
                // TODO: update with TimeService later
                utc_time = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() / 1000d
            }
        };

        return context.SendResponseAsync(response, true, true);
    }
}