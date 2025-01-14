using System.Collections.Generic;
using System.Runtime.Serialization;
using Fuyu.Backend.BSG.ItemTemplates;
using Fuyu.Backend.BSG.Models.Trading;
using Fuyu.Common.Hashing;

namespace Fuyu.Backend.BSG.Models.Requests;

[DataContract]
public class RagfairFindRequest
{
    [DataMember(Name = "page")]
    public int Page { get; set; }

    [DataMember(Name = "limit")]
    public int Limit { get; set; }

    [DataMember(Name = "sortType")]
    public int SortType { get; set; }

    [DataMember(Name = "sortDirection")]
    public int SortDirection { get; set; }

    [DataMember(Name = "currency")]
    public ECurrencyType Currency { get; set; }

    [DataMember(Name = "priceFrom")]
    public int PriceFrom { get; set; }

    [DataMember(Name = "priceTo")]
    public int PriceTo { get; set; }

    [DataMember(Name = "quantityFrom")]
    public int QuantityFrom { get; set; }

    [DataMember(Name = "quantityTo")]
    public int QuantityTo { get; set; }

    [DataMember(Name = "conditionFrom")]
    public int ConditionFrom { get; set; }

    [DataMember(Name = "conditionTo")]
    public int ConditionTo { get; set; }

    [DataMember(Name = "oneHourExpiration")]
    public bool OneHourExpiration { get; set; }

    [DataMember(Name = "removeBartering")]
    public bool RemoveBartering { get; set; }

    [DataMember(Name = "offerOwnerType")]
    public int OfferOwnerType { get; set; }

    [DataMember(Name = "onlyFunctional")]
    public bool OnlyFunctional { get; set; }

    [DataMember(Name = "updateOfferCount")]
    public bool UpdateOfferCount { get; set; }

    [DataMember(Name = "handbookId")]
    public MongoId HandbookId { get; set; }

    [DataMember(Name = "linkedSearchId")]
    public string LinkedSearchId { get; set; }

    [DataMember(Name = "neededSearchId")]
    public string NeededSearchId { get; set; }

    [DataMember(Name = "buildItems")]
    public Dictionary<MongoId, int> BuildItems { get; set; }

    [DataMember(Name = "buildCount")]
    public int BuildCount { get; set; }

    [DataMember(Name = "tm")]
    public int Tm { get; set; }

    [DataMember(Name = "reload")]
    public int Reload { get; set; }
}