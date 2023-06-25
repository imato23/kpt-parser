using Imato.KptCookImporter.KptCook.Impl;
using Imato.KptParser.Mealie;
using Imato.KptParser.Mealie.Impl;

namespace Imato.KptCookImporter.Console;

internal class Program
{
    private static void Main(string[] args)
    {
        System.Console.WriteLine("Hello, World!");
        MainAsync().Wait();
    }

    private static async Task MainAsync()
    {
        // var kptCookService = new KptCookService();
        // var ids = await kptCookService.GetFavoriteIdsAsync().ConfigureAwait(false);
        // var response = await kptCookService.GetRecipesAsync(ids).ConfigureAwait(false);

        IMealieService mealieService = new MealieService();
        await mealieService.LoginAsync();
        var recipes = await mealieService.GetAllRecipes();

    }
}