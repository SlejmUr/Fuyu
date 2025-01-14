using System.Runtime.Serialization;
using Fuyu.Common.Hashing;

namespace Fuyu.Backend.BSG.Models.Trading;

[DataContract]
public class HandbookCategory
{
    [DataMember(Name = "Id")]
    public MongoId Id { get; set; }

    [DataMember(Name = "ParentId", EmitDefaultValue = false)]
    public MongoId? ParentId { get; set; }

    [DataMember(Name = "Icon")]
    public string Icon { get; set; }

    /// <summary>
    /// Hex string or empty <example>"#284e64"</example>
    /// </summary>
    [DataMember(Name = "Color")]
    public string Color { get; set; }

    // NOTE: It is a string in dumps but the client has
    // it as an int and it will be easier to use as an int
    // -- nexus4880, 2025-1-12
    [DataMember(Name = "Order")]
    public int Order { get; set; }
}