namespace Cyviz.Core.Domain.Services
{
    public interface IRetryPolicy
    {
        Task<bool> ExecuteAsync(Func<Task> action, ILogger logger, CancellationToken ct);
    }

    public class RetryPolicy : IRetryPolicy
    {
        private readonly int[] _delays = new[] { 100, 300, 700 };
        private readonly Random _rnd = new();

        public async Task<bool> ExecuteAsync(Func<Task> action, ILogger logger, CancellationToken ct)
        {
            for (int attempt = 0; attempt < _delays.Length; attempt++)
            {
                try
                {
                    await action();
                    return true;
                }
                catch (OperationCanceledException) when (ct.IsCancellationRequested)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex,
                        "RetryPolicy: attempt {Attempt} failed",
                        attempt + 1);

                    if (attempt == _delays.Length - 1)
                        return false;

                    var baseDelay = _delays[attempt];
                    var jitter = _rnd.Next(0, baseDelay);
                    var delayMs = baseDelay + jitter;

                    try
                    {
                        await Task.Delay(delayMs, ct);
                    }
                    catch (OperationCanceledException) when (ct.IsCancellationRequested)
                    {
                        throw;
                    }
                }
            }

            return false;
        }
    }
}
