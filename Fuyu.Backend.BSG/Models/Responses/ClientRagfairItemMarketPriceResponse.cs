using System.Runtime.Serialization;

namespace Fuyu.Backend.BSG.Models.Responses;

[DataContract]
public class ClientRagfairItemMarketPriceResponse
{
    [DataMember(Name = "min")]
    public float Minimum { get; set; }

    [DataMember(Name = "avg")]
    public float Average { get; set; }

    [DataMember(Name = "max")]
    public float Maximum { get; set; }
}