using Autofac;
using Imato.KptParser.Mealie.Impl;

namespace Imato.KptParser.Mealie;

public class AutofacModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        base.Load(builder);
        builder.RegisterType<MealieService>().As<IMealieService>().SingleInstance();
    }
}