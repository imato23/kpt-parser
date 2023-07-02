using Autofac;
using Imato.KptParser.Common.Config;
using Imato.KptParser.Common.Config.Impl;
using Imato.KptParser.Common.Http;
using Imato.KptParser.Common.Http.Impl;

namespace Imato.KptParser.Common;

public class AutofacModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        base.Load(builder);
        builder.RegisterType<HttpClientFactory>().As<IHttpClientFactory>().SingleInstance();
        builder.RegisterType<AppSettingsReader>().As<IAppSettingsReader>().SingleInstance();
    }
}