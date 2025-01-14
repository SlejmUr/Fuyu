using System;
using System.Linq;
using System.Threading.Tasks;
using Fuyu.Backend.BSG.Models.ItemEvents;
using Fuyu.Backend.BSG.Models.Trading;
using Fuyu.Backend.BSG.Networking;
using Fuyu.Backend.EFTMain.Services;

namespace Fuyu.Backend.EFTMain.Controllers.ItemEvents;

public class RagFairAddOfferItemEventController : AbstractItemEventController<RagFairAddOfferItemEvent>
{
    private readonly RagfairService _ragfairService;
    private readonly EftOrm _eftOrm;

    public RagFairAddOfferItemEventController() : base("RagFairAddOffer")
    {
        _ragfairService = RagfairService.Instance;
        _eftOrm = EftOrm.Instance;
    }

    public override Task RunAsync(ItemEventContext context, RagFairAddOfferItemEvent request)
    {
        var profile = _eftOrm.GetActiveProfile(context.SessionId);
        var ragfairUser = new RagfairPlayerUser(profile.Pmc);
        var items = profile.Pmc.Inventory.Items.Where(i => request.Items.Contains(i.Id)).ToList();

        var offer = _ragfairService.CreateAndAddOffer(ragfairUser, items, request.SellAsPack, request.Requirements,
            TimeSpan.FromMinutes(30d), false);

        if (offer != null)
        {
            context.Response.ProfileChanges[profile.Pmc._id].Items.Delete.AddRange(items);
        }
        else
        {
            context.AppendInventoryError("Failed to create offer");
        }

        return Task.CompletedTask;
    }
}