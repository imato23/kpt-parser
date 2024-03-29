using Autofac;
using Imato.KptParser.Console.Impl;

namespace Imato.KptParser.Console;

public class AutofacModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        base.Load(builder);
        builder.RegisterType<Importer>().As<IImporter>().SingleInstance();
    }
}