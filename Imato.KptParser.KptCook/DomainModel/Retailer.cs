namespace Imato.KptParser.KptCook.DomainModel;

public class Retailer : KptModel
{
    public string? Rkey { get; set; }
    public string? Key { get; set; }
    public string? Name { get; set; }
    public string? Status { get; set; }
    public string? Typ { get; set; }
    public string? Country { get; set; }
    public string? PriceUpdate { get; set; }
    public string? MapStatus { get; set; }
    public bool? OnlineOrderingState { get; set; }
    public KptDate? CreationDate { get; set; }
    public KptDate? UpdateDate { get; set; }
}