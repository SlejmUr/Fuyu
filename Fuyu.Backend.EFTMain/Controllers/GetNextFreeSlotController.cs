using System.Net;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Fuyu.Backend.BSG.Models.Items;
using Fuyu.Backend.EFTMain.Networking;
using Fuyu.Common.IO;
using Fuyu.Common.Networking;
using Fuyu.Common.Serialization;

namespace Fuyu.Backend.EFTMain.Controllers;

[DataContract]
public class GetNextFreeSlotRequest
{
    [DataMember(Name = "width")]
    public int Width { get; set; }

    [DataMember(Name = "height")]
    public int Height { get; set; }

    [DataMember(Name = "rotation")]
    public EItemRotation Rotation { get; set; }
}

// TODO: Delete later
// -- nexus4880, 2024-11-26
public class GetNextFreeSlotController : AbstractEftHttpController<GetNextFreeSlotRequest>
{
    private readonly EftOrm _eftOrm;

    public GetNextFreeSlotController() : base("/get/next/free/slot")
    {
        _eftOrm = EftOrm.Instance;
    }

    public override Task RunAsync(EftHttpContext context, GetNextFreeSlotRequest body)
    {
        var profile = _eftOrm.GetActiveProfile(context.SessionId);
        var freeSlot = profile.Pmc.Inventory.GetNextFreeSlot(body.Width, body.Height, out var gridName, body.Rotation);
        if (freeSlot == null)
        {
            return context.SendStatus(HttpStatusCode.InternalServerError);
        }

        var json = Json.Stringify(new { freeSlot, gridName });
        Terminal.WriteLine(json);
        return context.SendJsonAsync(Json.Stringify(new { fuck = "you" }), true, true);
    }
}