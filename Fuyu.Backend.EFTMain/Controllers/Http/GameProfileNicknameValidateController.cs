using System.Threading.Tasks;
using Fuyu.Backend.BSG.Models.Requests;
using Fuyu.Backend.BSG.Models.Responses;
using Fuyu.Backend.EFTMain.Networking;
using Fuyu.Backend.EFTMain.Services;
using Fuyu.Common.Serialization;

namespace Fuyu.Backend.EFTMain.Controllers.Http;

public class GameProfileNicknameValidateController : AbstractEftHttpController<GameProfileNicknameValidateRequest>
{
    private readonly ProfileService _profileService;

    public GameProfileNicknameValidateController() : base("/client/game/profile/nickname/validate")
    {
        _profileService = ProfileService.Instance;
    }

    public override Task RunAsync(EftHttpContext context, GameProfileNicknameValidateRequest request)
    {
        var result = _profileService.IsValidNickname(request.Nickname);

        // TODO: Handle error result.

        var response = new ResponseBody<GameProfileNicknameValidateResponse>()
        {
            data = new GameProfileNicknameValidateResponse()
            {
                status = result,
            }
        };

        return context.SendResponseAsync(response, true, true);
    }
}