using System.Threading.Tasks;
using Fuyu.Backend.BSG.Models.Responses;
using Fuyu.Backend.EFTMain.Networking;

namespace Fuyu.Backend.EFTMain.Controllers.Http;

public class GameProfileNicknameReservedController : AbstractEftHttpController
{
    private readonly EftOrm _eftOrm;

    public GameProfileNicknameReservedController() : base("/client/game/profile/nickname/reserved")
    {
        _eftOrm = EftOrm.Instance;
    }

    public override Task RunAsync(EftHttpContext context)
    {
        var sessionId = context.SessionId;
        var account = _eftOrm.GetAccount(sessionId);

        var response = new ResponseBody<string>()
        {
            data = account.Username
        };

        return context.SendResponseAsync(response, true, true);
    }
}