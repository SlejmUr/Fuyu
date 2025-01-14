using System.Threading.Tasks;
using Fuyu.Backend.BSG.Models.ItemEvents;
using Fuyu.Backend.BSG.Networking;
using Fuyu.Backend.EFTMain.Services;

namespace Fuyu.Backend.EFTMain.Controllers.ItemEvents;

public class RagFairRemoveOfferItemEventController : AbstractItemEventController<RagFairRemoveOfferItemEvent>
{
    private readonly RagfairService _ragfairService;

    public RagFairRemoveOfferItemEventController() : base("RagFairRemoveOffer")
    {
        _ragfairService = RagfairService.Instance;
    }

    public override Task RunAsync(ItemEventContext context, RagFairRemoveOfferItemEvent request)
    {
        var offer = _ragfairService.GetOffer(request.OfferId);

        if (offer == null)
        {
            context.AppendInventoryError("Failed to find offer");
        }
        else
        {
            // TODO: do not remove offer immediately, match live behavior
            // -- nexus4880, 2025-1-13
            _ragfairService.RemoveOffer(offer);
        }

        return Task.CompletedTask;
    }
}