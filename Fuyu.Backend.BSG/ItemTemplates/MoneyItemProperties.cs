using System.Runtime.Serialization;
using Fuyu.Backend.BSG.Models.Trading;

namespace Fuyu.Backend.BSG.ItemTemplates;

[DataContract]
public class MoneyItemProperties : StackableItemItemProperties
{
    [DataMember(Name = "type")]
    public ECurrencyType Type { get; set; }

    [DataMember(Name = "eqMin")]
    public int eqMin { get; set; }

    [DataMember(Name = "eqMax")]
    public int eqMax { get; set; }

    [DataMember(Name = "rate")]
    public float Rate { get; set; }

    [DataMember(Name = "IsRagfairCurrency")]
    public bool IsRagfairCurrency { get; set; }
}