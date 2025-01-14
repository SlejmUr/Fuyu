using System.Collections.Generic;
using System.Threading.Tasks;
using Fuyu.Backend.BSG.ItemTemplates;
using Fuyu.Backend.BSG.Models.ItemEvents;
using Fuyu.Backend.BSG.Models.Items;
using Fuyu.Backend.BSG.Networking;
using Fuyu.Backend.BSG.Services;
using Fuyu.Backend.EFTMain.Services;
using Fuyu.Common.IO;
using Fuyu.Common.Serialization;
using Newtonsoft.Json;

namespace Fuyu.Backend.EFTMain.Controllers.ItemEvents;

public class RagFairBuyOfferItemEventController : AbstractItemEventController<RagFairBuyOfferItemEvent>
{
    private readonly EftOrm _eftOrm;
    private readonly RagfairService _ragfairService;

    public RagFairBuyOfferItemEventController() : base("RagFairBuyOffer")
    {
        _eftOrm = EftOrm.Instance;
        _ragfairService = RagfairService.Instance;
    }

    // TODO: logic for buying is wrong
    public override Task RunAsync(ItemEventContext context, RagFairBuyOfferItemEvent request)
    {
        var profile = _eftOrm.GetActiveProfile(context.SessionId);

        foreach (var buyOffer in request.BuyOffers)
        {
            for (var i = 0; i < buyOffer.Count; i++)
            {
                foreach (var itemOffer in buyOffer.Items)
                {
                    var stack = profile.Pmc.Inventory.Items.Find(it => it.Id == itemOffer.Id);
                    stack.Updatable.StackObjectsCount -= itemOffer.Count;
                    if (stack.Updatable.StackObjectsCount <= 0)
                    {
                        profile.Pmc.Inventory.Items.Remove(stack);
                        context.Response.ProfileChanges[profile.Pmc._id].Items.Delete.Add(stack);
                    }
                    else
                    {
                        context.Response.ProfileChanges[profile.Pmc._id].Items.Change.Add(stack);
                    }

                    var ragfairOffer = _ragfairService.GetOffer(buyOffer.Id);
                    foreach (var itemToBuy in ragfairOffer.Items)
                    {
                        var itemProperties = ItemFactoryService.Instance.GetItemProperties<ItemProperties>(itemToBuy.TemplateId);
                        var locationInGrid = profile.Pmc.Inventory.GetNextFreeSlot(itemProperties.Width, itemProperties.Height, out var gridName);
                        itemToBuy.ParentId = profile.Pmc.Inventory.Stash;
                        itemToBuy.Location = locationInGrid;
                        itemToBuy.SlotId = gridName;
                        profile.Pmc.Inventory.Items.Add(itemToBuy);
                        context.Response.ProfileChanges[profile.Pmc._id].Items.New.Add(itemToBuy);
                    }

                    _ragfairService.RemoveOffer(ragfairOffer);
                }
            }
        }

        return Task.CompletedTask;
    }
}