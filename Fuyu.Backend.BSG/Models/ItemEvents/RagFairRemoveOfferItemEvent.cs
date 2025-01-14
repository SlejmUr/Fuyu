using System.Runtime.Serialization;
using Fuyu.Common.Hashing;

namespace Fuyu.Backend.BSG.Models.ItemEvents;

[DataContract]
public class RagFairRemoveOfferItemEvent : BaseItemEvent
{
    [DataMember(Name = "offerId")]
    public MongoId OfferId { get; set; }
}