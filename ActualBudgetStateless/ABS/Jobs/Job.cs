using System.Timers;
using Timer = System.Timers.Timer;

namespace ABS.Jobs;

internal interface IJob
{
    void Start(TimeSpan interval);
    ManualResetEventSlim Stop();
}

internal abstract class Job : IJob, IDisposable
{
    private readonly ManualResetEventSlim _complete = new();

    private Timer? _timer;

    public void Start(TimeSpan interval)
    {
        if (_timer == null)
        {
            _timer = new Timer(interval.TotalMilliseconds);
            _timer.Elapsed += Tick;
            _timer.Start();
        }
    }

    public void Dispose()
    {
        if (_timer != null)
        {
            _timer.Elapsed -= Tick;
            using (_timer)
            {
                _timer.Stop();
            }
        }

        GC.SuppressFinalize(this);
    }

    public ManualResetEventSlim Stop()
    {
        if (_timer != null)
        {
            _timer.Stop();
        }

        if (!_running)
        {
            _complete.Set();
        }

        return _complete;
    }

    private bool _running;
    private readonly Lock _locker = new();

    private async void Tick(object? p, ElapsedEventArgs e)
    {
        if (_running) return;
        
        var run = false;
        lock (_locker)
        {
            if (!_running)
            {
                _running = true;
                run = true;
            }
        }

        if (!run) return;
        
        try
        {
            await Run();
        }
        finally
        {
            lock (_locker)
            {
                _running = false;
            }
        }
    }

    private async Task Run()
    {
        _complete.Reset();
        try
        {
            await Process();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }

        _complete.Set();
    }

    protected abstract Task Process();
}

internal static class ServiceProviderExtensions
{
    public static T BuildJob<T>(this IServiceProvider @this, TimeSpan interval)
        where T : IJob
    {
        var job = @this.GetRequiredService<T>();
        job.Start(interval);
        return job;
    }
}