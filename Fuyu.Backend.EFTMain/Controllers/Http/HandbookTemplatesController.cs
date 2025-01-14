using System.Threading.Tasks;
using Fuyu.Backend.BSG.Models.Responses;
using Fuyu.Backend.BSG.Models.Trading;
using Fuyu.Backend.EFTMain.Networking;

namespace Fuyu.Backend.EFTMain.Controllers.Http;

public class HandbookTemplatesController : AbstractEftHttpController
{
    private readonly EftOrm _eftOrm;

    public HandbookTemplatesController() : base("/client/handbook/templates")
    {
        _eftOrm = EftOrm.Instance;
    }

    public override Task RunAsync(EftHttpContext context)
    {
        // TODO: generate this
        // --seionmoya, 2024-11-18
        var response = new ResponseBody<HandbookTemplates>()
        {
            data = _eftOrm.GetHandbook()
        };

        return context.SendResponseAsync(response, true, true);
    }
}