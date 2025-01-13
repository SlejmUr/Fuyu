using System.Threading.Tasks;
using Fuyu.Backend.BSG.Models.Requests;
using Fuyu.Backend.BSG.Models.Responses;
using Fuyu.Backend.EFTMain.Networking;
using Fuyu.Common.IO;

namespace Fuyu.Backend.EFTMain.Controllers.Http;

public class GameProfileSelectController : AbstractEftHttpController<GameProfileSelectRequest>
{
    public GameProfileSelectController() : base("/client/game/profile/select")
    {
    }

    public override Task RunAsync(EftHttpContext context, GameProfileSelectRequest body)
    {
        // TODO: handle this
        // --seionmoya, 2024-11-18

        if (!VFS.FileExists($"./Fuyu/Profiles/EFT/{body.Uid}.json"))
        {
            // TODO: Handle error if profile not exists.
        }

        // Note: Game only checks if we return something but doesnt check the value.
        // also it handles error so we good if we return ok.
        var response = new ResponseBody<ProfileSelectResponse>()
        {
            data = new ProfileSelectResponse()
            {
                status = "ok"
            }
        };

        return context.SendResponseAsync(response, true, true);
    }
}