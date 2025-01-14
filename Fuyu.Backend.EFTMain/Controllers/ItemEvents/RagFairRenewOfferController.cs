using System;
using System.Threading.Tasks;
using Fuyu.Backend.BSG.Models.ItemEvents;
using Fuyu.Backend.BSG.Networking;
using Fuyu.Backend.EFTMain.Services;

namespace Fuyu.Backend.EFTMain.Controllers.ItemEvents;

public class RagFairRenewOfferController : AbstractItemEventController<RagFairRenewOfferItemEvent>
{
    private readonly RagfairService _ragfairService;

    public RagFairRenewOfferController() : base("RagFairRenewOffer")
    {
        _ragfairService = RagfairService.Instance;
    }

    public override Task RunAsync(ItemEventContext context, RagFairRenewOfferItemEvent request)
    {
        var offer = _ragfairService.GetOffer(request.OfferId);

        if (offer != null)
        {
            offer.EndTime += (request.RenewalTime * TimeSpan.MillisecondsPerHour) / 1000d;
        }
        else
        {
            context.AppendInventoryError("Offer not found");
        }

        return Task.CompletedTask;
    }
}