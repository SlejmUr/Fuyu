using System.Runtime.Serialization;
using Fuyu.Common.Hashing;

namespace Fuyu.Backend.BSG.Models.Trading;

[DataContract]
public class HandoverRequirement
{
    [DataMember(Name = "count")]
    public int Count { get; set; }

    [DataMember(Name = "_tpl")]
    public MongoId TemplateId { get; set; }
}