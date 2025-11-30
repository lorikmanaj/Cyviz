using Cyviz.Core.Domain.Services;
using Moq;
using System.Diagnostics;
using Xunit;

namespace Cyviz.Tests.Unit
{
    public class RetryPolicyTests
    {
        private readonly RetryPolicy _policy = new RetryPolicy();

        // 1. Succeeds on first try
        [Fact]
        public async Task ExecuteAsync_SucceedsOnFirstAttempt_ReturnsTrue()
        {
            var logger = Mock.Of<ILogger>();

            var result = await _policy.ExecuteAsync(
                action: () => Task.CompletedTask,
                logger: logger,
                ct: CancellationToken.None);

            Assert.True(result);
        }

        // 2. Fails first, succeeds second attempt
        [Fact]
        public async Task ExecuteAsync_FailsThenSucceeds_ReturnsTrue()
        {
            var logger = Mock.Of<ILogger>();
            int attempts = 0;

            Task Action()
            {
                attempts++;
                if (attempts == 1)
                    throw new Exception("Fail first");

                return Task.CompletedTask; // succeed on 2nd try
            }

            var sw = Stopwatch.StartNew();

            var result = await _policy.ExecuteAsync(
                Action,
                logger,
                CancellationToken.None);

            sw.Stop();

            Assert.True(result);
            Assert.Equal(2, attempts);

            // The first delay should be ~100-200ms range
            Assert.InRange(sw.ElapsedMilliseconds, 100, 500);
        }

        // 3. Always fails → returns false after all attempts
        [Fact]
        public async Task ExecuteAsync_AlwaysFails_ReturnsFalse()
        {
            var logger = Mock.Of<ILogger>();
            int attempts = 0;

            Task Action()
            {
                attempts++;
                throw new Exception("Always fail");
            }

            var sw = Stopwatch.StartNew();

            var result = await _policy.ExecuteAsync(
                Action,
                logger,
                CancellationToken.None);

            sw.Stop();

            Assert.False(result);
            Assert.Equal(3, attempts); // 3 attempts total

            // Total delay should be roughly:
            // Attempt1: 100-200ms
            // Attempt2: 300-600ms
            // Attempt3: no delay (returns false)
            // Sum: >=400ms and <= ~900ms
            Assert.InRange(sw.ElapsedMilliseconds, 350, 1300);
        }

        // 4. Cancellation before retry
        [Fact]
        public async Task ExecuteAsync_CancelledDuringDelay_ThrowsOperationCanceledException()
        {
            var logger = Mock.Of<ILogger>();
            var cts = new CancellationTokenSource();

            int attempts = 0;

            Task Action()
            {
                attempts++;
                throw new Exception("Fail");
            }

            var task = Task.Run(async () =>
            {
                // Cancel after 50ms, before retry (100ms)
                await Task.Delay(50);
                cts.Cancel();
            });

            await Assert.ThrowsAsync<OperationCanceledException>(async () =>
            {
                await _policy.ExecuteAsync(
                    Action,
                    logger,
                    cts.Token);
            });

            Assert.True(attempts >= 1); // It must try once
        }

        // 5. Logs warnings per failed attempt
        [Fact]
        public async Task ExecuteAsync_LogsWarning_OnEachFailure()
        {
            var logger = new Mock<ILogger>();
            int attempts = 0;

            Task Action()
            {
                attempts++;
                throw new Exception("Fail");
            }

            await _policy.ExecuteAsync(
                Action,
                logger.Object,
                CancellationToken.None);

            // Replay calls: should have 3 warnings
            logger.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.Exactly(3));
        }

        // 6. Delay pattern verification (approx)
        [Fact]
        public async Task ExecuteAsync_UsesApproximateDelayPattern()
        {
            var logger = Mock.Of<ILogger>();
            int attempts = 0;

            Task Action()
            {
                attempts++;
                throw new Exception("Fail");
            }

            var sw = Stopwatch.StartNew();

            await _policy.ExecuteAsync(Action, logger, CancellationToken.None);

            sw.Stop();

            long elapsed = sw.ElapsedMilliseconds;

            // Very loose tolerance but verifies correct behavior:
            // Delays: ~100–200 + ~300–600 + ~700–1400  => ~1100–2200 total
            Assert.InRange(elapsed, 900, 2500);
        }
    }
}
