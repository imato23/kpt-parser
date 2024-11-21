using System.Reflection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Imato.KptParser.Console.DomainModel;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Imato.KptParser.Console;

[Command(
    Name = "Imato.KptParser.Console.exe"
    , Description = "Parses KptCook favorite recipes and imports it to the Mealie recipe database")]
[HelpOption(
    "-h"
    , LongName = "help"
    , Description = "Get program help")]
[VersionOptionFromMember(
    "-v"
    , MemberName = nameof(GetVersion))]
internal class Program
{
    public static Task<int> Main(string[] args)
    {
        return CommandLineApplication.ExecuteAsync<Program>(args);
    }

    public async Task<int> OnExecuteAsync(CancellationToken cancellationToken)
    {
        IHostBuilder hostBuilder = CreateHostBuilder();

        hostBuilder.UseServiceProviderFactory(new AutofacServiceProviderFactory());
        hostBuilder.ConfigureContainer<ContainerBuilder>(
            containerBuilder =>
            {
                // ReSharper disable once RedundantNameQualifier
                containerBuilder.RegisterModule<Console.AutofacModule>();
                containerBuilder.RegisterModule<Common.AutofacModule>();
                containerBuilder.RegisterModule<KptCook.AutofacModule>();
                containerBuilder.RegisterModule<Mealie.AutofacModule>();
            });

        await hostBuilder.RunConsoleAsync(cancellationToken).ConfigureAwait(false);
        return Environment.ExitCode;
    }

    private IHostBuilder CreateHostBuilder()
    {
        IHostBuilder hostBuilder = Host.CreateDefaultBuilder()
            .ConfigureLogging(logging => { logging.ClearProviders(); })
            .UseSerilog((hostContext, loggerConfiguration) =>
            {
                loggerConfiguration.ReadFrom.Configuration(hostContext.Configuration);
            })
            .ConfigureServices(
                services =>
                {
                    services.Configure<CommandlineOptions>(_ => { });
                    services.AddHostedService<Worker>();
                });

        return hostBuilder;
    }

    private static string GetVersion()
    {
        return typeof(Program).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()
            ?.InformationalVersion ?? "Unknown";
    }
}