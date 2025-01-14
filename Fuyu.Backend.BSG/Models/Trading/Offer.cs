using System.Collections.Generic;
using System.Runtime.Serialization;
using Fuyu.Backend.BSG.Models.Items;
using Fuyu.Common.Hashing;

namespace Fuyu.Backend.BSG.Models.Trading;

[DataContract]
public class Offer
{
    // NOTE: I think this might be a MongoId with '1' appended
    // -- nexus4880, 2025-1-12
    [DataMember(Name = "_id")]
    public MongoId Id { get; set; }

    [DataMember(Name = "intId")]
    public long IntId { get; set; }

    [DataMember(Name = "user")]
    public IRagfairUser User { get; set; }

    /// <summary>
    /// The root item (more than likely will be Items[0].Id)
    /// </summary>
    [DataMember(Name = "root")]
    public MongoId Root { get; set; }

    [DataMember(Name = "items")]
    public List<ItemInstance> Items { get; set; }

    /// <summary>
    /// This is the cost of the item(s) that are being sold
    /// </summary>
    [DataMember(Name = "itemsCost")]
    public int ItemsCost { get; set; }

    [DataMember(Name = "requirementsCost")]
    public int RequirementsCost { get; set; }

    [DataMember(Name = "requirements")]
    public List<HandoverRequirement> Requirements { get; set; }

    [DataMember(Name = "startTime")]
    public double StartTime { get; set; }

    [DataMember(Name = "endTime")]
    public double EndTime { get; set; }

    [DataMember(Name = "wasEdit")]
    public bool WasEdit { get; set; }

    [DataMember(Name = "exchange")]
    public bool Exchange { get; set; }

    // NOTE: Always 1 for player offers
    // -- nexus4880, 2025-1-12
    [DataMember(Name = "loyaltyLevel")]
    public int LoyaltyLevel { get; set; }

    [DataMember(Name = "locked")]
    public bool Locked { get; set; }

    [DataMember(Name = "unlimitedCount")]
    public bool UnlimitedCount { get; set; }

    [DataMember(Name = "buyRestrictionMax")]
    public int BuyRestrictionMax { get; set; }

    [DataMember(Name = "buyRestrictionCurrent")]
    public int BuyRestrictionCurrent { get; set; }

    /// <summary>
    /// If this is a bulk listing
    /// </summary>
    [DataMember(Name = "sellInOnePiece")]
    public bool SellInOnePiece { get; set; }

    [DataMember(Name = "summaryCost")]
    public int SummaryCost { get; set; }
}