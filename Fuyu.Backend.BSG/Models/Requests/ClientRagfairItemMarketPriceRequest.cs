using System.Runtime.Serialization;
using Fuyu.Common.Hashing;

namespace Fuyu.Backend.BSG.Models.Requests;

[DataContract]
public class ClientRagfairItemMarketPriceRequest
{
    [DataMember(Name = "templateId")]
    public MongoId TemplateId { get; set; }
}