using System.Collections.Generic;
using System.Runtime.Serialization;
using Fuyu.Backend.BSG.Models.Trading;
using Fuyu.Common.Hashing;

namespace Fuyu.Backend.BSG.Models.ItemEvents;

[DataContract]
public class RagFairAddOfferItemEvent : BaseItemEvent
{
    [DataMember(Name = "sellInOnePiece")]
    public bool SellAsPack { get; set; }

    [DataMember(Name = "items")]
    public List<MongoId> Items { get; set; }

    [DataMember(Name = "requirements")]
    public List<HandoverRequirement> Requirements { get; set; }
}