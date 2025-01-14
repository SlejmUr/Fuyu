using System.Runtime.Serialization;
using Fuyu.Common.Hashing;

namespace Fuyu.Backend.BSG.Models.ItemEvents;

[DataContract]
public class RagFairRenewOfferItemEvent : BaseItemEvent
{
    [DataMember(Name = "offerId")]
    public MongoId OfferId { get; set; }

    // NOTE: this is not used in the client there is no priority
    // checkbox on the extend offer window and the value never gets set
    // -- nexus4880, 2025-1-13
    /*[DataMember(Name = "priority")]
    public bool Priority { get; set; }*/

    [DataMember(Name = "renewalTime")]
    public int RenewalTime { get; set; }
}