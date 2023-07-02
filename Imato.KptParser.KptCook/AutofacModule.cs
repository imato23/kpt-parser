using Autofac;
using Imato.KptParser.KptCook.Impl;

namespace Imato.KptParser.KptCook;

public class AutofacModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        base.Load(builder);
        builder.RegisterType<KptCookService>().As<IKptCookService>().SingleInstance();
    }
}