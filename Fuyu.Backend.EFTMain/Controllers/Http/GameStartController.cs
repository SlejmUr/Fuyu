using System;
using System.Threading.Tasks;
using Fuyu.Backend.BSG.Models.Requests;
using Fuyu.Backend.BSG.Models.Responses;
using Fuyu.Backend.EFTMain.Networking;

namespace Fuyu.Backend.EFTMain.Controllers.Http;

public class GameStartController : AbstractEftHttpController
{
    public GameStartController() : base("/client/game/start")
    {
    }

    public override Task RunAsync(EftHttpContext context)
    {
        var response = new ResponseBody<GameStartResponse>()
        {
            data = new GameStartResponse()
            {
                // TODO: update with TimeService later
                BackendTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() / 1000d
            }
        };

        return context.SendResponseAsync(response, true, true);
    }
}