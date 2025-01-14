using System.Runtime.Serialization;
using Fuyu.Common.Hashing;

namespace Fuyu.Backend.BSG.Models.Trading;

[DataContract]
public class ItemOffer
{
    [DataMember(Name = "id")]
    public MongoId Id { get; set; }

    [DataMember(Name = "count")]
    public int Count { get; set; }
}