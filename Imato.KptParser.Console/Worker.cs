using Imato.KptParser.Console.DomainModel;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Imato.KptParser.Console;

public class Worker : IHostedService
{
    private readonly IHostApplicationLifetime hostApplicationLifetime;
    private readonly ILogger<Worker> logger;

    // ReSharper disable once NotAccessedField.Local
    private readonly IOptions<CommandlineOptions> options;
    private readonly IImportService importService;
    private int exitCode;

    public Worker(
        ILogger<Worker> logger,
        IHostApplicationLifetime hostApplicationLifetime,
        IOptions<CommandlineOptions> options,
        IImportService importService)
    {
        this.logger = logger;
        this.hostApplicationLifetime = hostApplicationLifetime;
        this.options = options;
        this.importService = importService;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting application");

        try
        {
            await importService.StartImportAsync().ConfigureAwait(false);
            exitCode = 0;
        }
        catch (OperationCanceledException)
        {
            logger.LogInformation("The application has been killed with CTRL+C");
            exitCode = -1;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred");
            exitCode = 1;
        }
        finally
        {
            hostApplicationLifetime.StopApplication();
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        Environment.ExitCode = exitCode;
        logger.LogInformation("Shutting down the application with code {ExitCode}", Environment.ExitCode);
        return Task.CompletedTask;
    }
}