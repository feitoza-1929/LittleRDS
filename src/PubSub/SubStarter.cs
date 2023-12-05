using Microsoft.Extensions.Hosting;

public class SubStarter : IHostedService
{

    public SubStarter(AOFSub aofSub)
    {
        _ = aofSub;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}