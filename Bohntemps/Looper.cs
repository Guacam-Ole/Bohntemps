using Microsoft.Extensions.Logging;

namespace Bohntemps;

public class Looper
{
    private readonly BeansConverter _converter;
    private readonly ILogger<Looper> _logger;

    public Looper(BeansConverter converter, ILogger<Looper> logger)
    {
        _converter = converter;
        _logger = logger;
    }

    private const int MaxRetries = 5;

    public async Task Loop()
    {
        var retries = MaxRetries;
        while (true)
        {
            try
            {
                Thread.Sleep(TimeSpan.FromMinutes(5));
                retries--;
                var now = DateTime.Now;
                await _converter.RetrieveAndSend();
                _logger.LogDebug("Bohntemps finished. Tooks '{Seconds}' seconds", (DateTime.Now - now).TotalSeconds);
                retries = MaxRetries;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error om loop '{retries}/{max}' retries", retries, MaxRetries);
                if (retries == 0) throw;
            }
        }
    }
}