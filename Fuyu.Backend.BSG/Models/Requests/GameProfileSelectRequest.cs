using System.Runtime.Serialization;

namespace Fuyu.Backend.BSG.Models.Requests;

[DataContract]
public class GameProfileSelectRequest
{
    [DataMember(Name = "uid")]
    public string Uid { get; set; }
}
