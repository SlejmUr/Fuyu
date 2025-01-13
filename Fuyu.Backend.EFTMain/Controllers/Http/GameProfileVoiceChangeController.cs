using System.Threading.Tasks;
using Fuyu.Backend.BSG.Models.Requests;
using Fuyu.Backend.BSG.Models.Responses;
using Fuyu.Backend.EFTMain.Networking;
using Fuyu.Backend.EFTMain.Services;

namespace Fuyu.Backend.EFTMain.Controllers.Http;

public class GameProfileVoiceChangeController : AbstractEftHttpController<GameProfileVoiceChangeRequest>
{
    private readonly ProfileService _profileService;
    private readonly EftOrm _eftOrm;

    public GameProfileVoiceChangeController() : base("/client/game/profile/voice/change")
    {
        _profileService = ProfileService.Instance;
        _eftOrm = EftOrm.Instance;
    }

    public override Task RunAsync(EftHttpContext context, GameProfileVoiceChangeRequest body)
    {
        var profile = _eftOrm.GetActiveProfile(context.SessionId);

        profile.Pmc.Info.Voice = body.Voice;

        _profileService.WriteToDisk(profile);

        var response = new ResponseBody<object>()
        {
            data = null
        };

        return context.SendResponseAsync(response, true, true);
    }
}