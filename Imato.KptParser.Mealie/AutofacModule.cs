using Autofac;
using Imato.KptParser.Mealie.Authorization;
using Imato.KptParser.Mealie.Authorization.Impl;
using Imato.KptParser.Mealie.Categories;
using Imato.KptParser.Mealie.Categories.Impl;
using Imato.KptParser.Mealie.Common;
using Imato.KptParser.Mealie.Common.Impl;
using Imato.KptParser.Mealie.Foods;
using Imato.KptParser.Mealie.Foods.Impl;
using Imato.KptParser.Mealie.Recipes;
using Imato.KptParser.Mealie.Recipes.Impl;
using Imato.KptParser.Mealie.Units;
using Imato.KptParser.Mealie.Units.Impl;

namespace Imato.KptParser.Mealie;

public class AutofacModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        base.Load(builder);
        builder.RegisterType<AuthorizationService>().As<IAuthorizationService>().SingleInstance();
        builder.RegisterType<HelperService>().As<IHelperService>().SingleInstance();
        builder.RegisterType<RecipeService>().As<IRecipeService>().SingleInstance();
        builder.RegisterType<FoodService>().As<IFoodService>().SingleInstance();
        builder.RegisterType<UnitService>().As<IUnitService>().SingleInstance();
        builder.RegisterType<RecipeCategoryService>().As<IRecipeCategoryService>().SingleInstance();
    }
}