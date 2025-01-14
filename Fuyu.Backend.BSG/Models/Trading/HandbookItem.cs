using System.Runtime.Serialization;
using Fuyu.Common.Hashing;

namespace Fuyu.Backend.BSG.Models.Trading;

[DataContract]
public class HandbookItem
{
    [DataMember(Name = "Id")]
    public MongoId Id { get; set; }

    [DataMember(Name = "ParentId")]
    public MongoId ParentId { get; set; }

    [DataMember(Name = "Price")]
    public int Price { get; set; }
}