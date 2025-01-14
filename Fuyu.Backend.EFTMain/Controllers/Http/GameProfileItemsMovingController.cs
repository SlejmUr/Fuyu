using System.Threading.Tasks;
using Fuyu.Backend.BSG.Models.ItemEvents;
using Fuyu.Backend.BSG.Models.Responses;
using Fuyu.Backend.BSG.Networking;
using Fuyu.Backend.EFTMain.Controllers.ItemEvents;
using Fuyu.Backend.EFTMain.Networking;
using Fuyu.Common.Serialization;
using Newtonsoft.Json.Linq;

namespace Fuyu.Backend.EFTMain.Controllers.Http;

public class GameProfileItemsMovingController : AbstractEftHttpController<JObject>
{
    public ItemEventRouter ItemEventRouter { get; } = new ItemEventRouter();
    private readonly EftOrm _eftOrm;

    public GameProfileItemsMovingController() : base("/client/game/profile/items/moving")
    {
        ItemEventRouter.AddController<CustomizationBuyEventController>();
        ItemEventRouter.AddController<EatItemEventController>();
        ItemEventRouter.AddController<InsureEventController>();
        ItemEventRouter.AddController<InterGameTransferEventController>();
        ItemEventRouter.AddController<MoveItemEventController>();
        ItemEventRouter.AddController<ReadEncyclopediaEventController>();
        ItemEventRouter.AddController<SellAllFromSavageEventController>();
        ItemEventRouter.AddController<TraderRepairEventController>();
        ItemEventRouter.AddController<TradingConfirmEventController>();
        ItemEventRouter.AddController<ApplyInventoryChangesItemEventController>();
        ItemEventRouter.AddController<RemoveItemEventController>();
        ItemEventRouter.AddController<FoldItemEventController>();
        ItemEventRouter.AddController<BindItemEventController>();
        ItemEventRouter.AddController<UnbindItemEventController>();
        ItemEventRouter.AddController<AddToWishListItemEventController>();
        ItemEventRouter.AddController<RemoveFromWishListItemEventController>();
        ItemEventRouter.AddController<ChangeWishlistItemCategoryItemEventController>();
        ItemEventRouter.AddController<AddNoteItemEventController>();
        ItemEventRouter.AddController<EditNoteItemEventController>();
        ItemEventRouter.AddController<DeleteNoteItemEventController>();
        ItemEventRouter.AddController<ExamineItemEventController>();
        ItemEventRouter.AddController<RecodeItemEventController>();
        ItemEventRouter.AddController<TagItemEventController>();
        ItemEventRouter.AddController<ToggleItemEventController>();
        ItemEventRouter.AddController<RepairItemEventController>();
        ItemEventRouter.AddController<RagFairBuyOfferItemEventController>();
        ItemEventRouter.AddController<RagFairAddOfferItemEventController>();
        ItemEventRouter.AddController<RagFairRemoveOfferItemEventController>();
        ItemEventRouter.AddController<RagFairRenewOfferController>();

        _eftOrm = EftOrm.Instance;
    }

    public override async Task RunAsync(EftHttpContext context, JObject request)
    {
        if (!request.ContainsKey("data"))
        {
            return;
        }

        var sessionId = context.SessionId;
        var profile = _eftOrm.GetActiveProfile(sessionId);
        var requestData = request.Value<JArray>("data");
        var itemEventResponse = new ItemEventResponse();
        /*{
				
				ProfileChanges = {
					// NOTE: Possibly make this a method where we can do
					// context.GetProfileChange() and if it doesn't exist add it
					// -- nexus4880, 2024-10-22
					{ profile.Pmc._id, new ProfileChange() },
					{ profile.Savage._id, new ProfileChange() }
				},
				InventoryWarnings = []
				
			};*/

        itemEventResponse.ProfileChanges[profile.Pmc._id] = new ProfileChange();
        itemEventResponse.ProfileChanges[profile.Savage._id] = new ProfileChange();

        var requestIndex = 0;
        foreach (var itemRequest in requestData)
        {
            var action = itemRequest.Value<string>("Action");
            var itemEventContext = new ItemEventContext(sessionId, action, requestIndex, itemRequest, itemEventResponse);
            await ItemEventRouter.RouteAsync(itemEventContext);
            requestIndex++;
        }

        var response = new ResponseBody<ItemEventResponse>
        {
            data = itemEventResponse
        };

        await context.SendResponseAsync(response, true, true);
    }
}