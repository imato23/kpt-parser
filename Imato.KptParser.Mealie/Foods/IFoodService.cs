using Imato.KptParser.Mealie.Foods.DomainModel;

namespace Imato.KptParser.Mealie.Foods
{
    public interface IFoodService
    {
        Task<Food> GetOrAddFoodAsync(string name);
    }
}
