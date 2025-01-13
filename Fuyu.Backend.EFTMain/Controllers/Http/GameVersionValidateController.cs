using System.Threading.Tasks;
using Fuyu.Backend.BSG.Models.Requests;
using Fuyu.Backend.BSG.Services;
using Fuyu.Backend.EFTMain.Networking;

namespace Fuyu.Backend.EFTMain.Controllers.Http;

public class GameVersionValidateController : AbstractEftHttpController<GameVersionValidateRequest>
{
    private readonly ResponseService _responseService;

    public GameVersionValidateController() : base("/client/game/version/validate")
    {
        _responseService = ResponseService.Instance;
    }

    public override Task RunAsync(EftHttpContext context, GameVersionValidateRequest body)
    {
        // TODO: handle this
        // --seionmoya, 2024-11-18



        return context.SendJsonAsync(_responseService.EmptyJsonResponse, true, true);
    }
}