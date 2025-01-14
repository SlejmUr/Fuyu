using System.Collections.Generic;
using System.Runtime.Serialization;
using Fuyu.Backend.BSG.Models.Trading;
using Fuyu.Common.Hashing;

namespace Fuyu.Backend.BSG.Models.Responses;

[DataContract]
public class OffersListResponse
{
    [DataMember(Name = "offers")]
    public List<Offer> Offers { get; set; }

    [DataMember(Name = "offersCount")]
    public int OffersCount;

    [DataMember(Name = "categories")]
    public Dictionary<MongoId, int> Categories = [];

    [DataMember(Name = "selectedCategory")]
    public MongoId SelectedCategory;
}