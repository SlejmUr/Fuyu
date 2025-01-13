using System.Runtime.Serialization;

namespace Fuyu.Backend.BSG.Models.Responses;

[DataContract]
public class GameStartResponse
{
    [DataMember(Name = "utc_time")]
    public double BackendTime { get; set; }
}