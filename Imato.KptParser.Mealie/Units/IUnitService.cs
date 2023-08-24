using Imato.KptParser.Mealie.Units.DomainModel;

namespace Imato.KptParser.Mealie.Units;

public interface IUnitService
{
    Task<Unit> GetOrAddUnitAsync(string name, string abbreviation);
}