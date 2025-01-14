using System.Runtime.Serialization;
using Fuyu.Backend.BSG.Models.Trading;

namespace Fuyu.Backend.BSG.Models.ItemEvents;

[DataContract]
public class RagFairBuyOfferItemEvent : BaseItemEvent
{
    [DataMember(Name = "offers")]
    public BuyOffer[] BuyOffers { get; set; }
}