using System.Linq;
using System.Threading.Tasks;
using Fuyu.Backend.BSG.Models.Requests;
using Fuyu.Backend.BSG.Models.Responses;
using Fuyu.Backend.EFTMain.Networking;
using Fuyu.Backend.EFTMain.Services;

namespace Fuyu.Backend.EFTMain.Controllers.Http;

public class ClientRagfairItemMarketPriceController : AbstractEftHttpController<ClientRagfairItemMarketPriceRequest>
{
    private readonly RagfairService _ragfairService;

    public ClientRagfairItemMarketPriceController() : base("/client/ragfair/itemMarketPrice")
    {
        _ragfairService = RagfairService.Instance;
    }

    public override Task RunAsync(EftHttpContext context, ClientRagfairItemMarketPriceRequest body)
    {
        var offers = _ragfairService.Offers.Where(i => i.Items[0].TemplateId == body.TemplateId);
        var minimum = float.MaxValue;
        var maximum = float.MinValue;
        var count = 0;
        var average = 0f;

        foreach (var offer in offers)
        {
            count++;
            average += offer.ItemsCost;

            if (offer.ItemsCost < minimum)
            {
                minimum = offer.ItemsCost;
            }

            if (offer.ItemsCost > maximum)
            {
                maximum = offer.ItemsCost;
            }
        }

        average /= count;

        var response = new ResponseBody<ClientRagfairItemMarketPriceResponse>()
        {
            data = new ClientRagfairItemMarketPriceResponse()
            {
                Maximum = maximum,
                Minimum = minimum,
                Average = average
            }
        };

        return context.SendResponseAsync(response, true, true);
    }
}