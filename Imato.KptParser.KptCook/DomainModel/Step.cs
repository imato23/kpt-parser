namespace Imato.KptParser.KptCook.DomainModel;

public class Step
{
    public LocalizedString? Title { get; set; }
    public List<StepIngredient>? Ingredients { get; set; }
}