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

    private int? exitCode;

    public Worker(
        ILogger<Worker> logger,
        IHostApplicationLifetime hostApplicationLifetime,
        IOptions<CommandlineOptions> options)
    {
        this.logger = logger;
        this.hostApplicationLifetime = hostApplicationLifetime;
        this.options = options;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting application");

        try 
        {
            // Todo: Add code here
            exitCode = 0;
            await Task.CompletedTask.ConfigureAwait(false);
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
        Environment.ExitCode = exitCode.GetValueOrDefault(-1);
        logger.LogInformation("Shutting down the application with code {ExitCode}", Environment.ExitCode);
        return Task.CompletedTask;
    }
}