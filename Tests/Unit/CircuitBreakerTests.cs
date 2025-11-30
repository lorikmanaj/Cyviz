using Cyviz.Core.Domain.Enums;
using Cyviz.Core.Domain.Services;
using Xunit;

namespace Cyviz.Tests.Unit
{
    public class CircuitBreakerTests
    {
        [Fact]
        public void Starts_Closed_And_Allows_Execution()
        {
            var breaker = new CircuitBreaker();

            Assert.Equal(CircuitState.Closed, breaker.State);
            Assert.True(breaker.CanExecute());
        }

        [Fact]
        public void After_Failures_Below_Threshold_Remains_Closed()
        {
            var breaker = new CircuitBreaker();

            // Default threshold = 5
            breaker.OnFailure();
            breaker.OnFailure();
            breaker.OnFailure();
            breaker.OnFailure();

            Assert.Equal(CircuitState.Closed, breaker.State);
            Assert.True(breaker.CanExecute());
        }

        [Fact]
        public void After_Reaching_Threshold_Opens_Circuit()
        {
            var breaker = new CircuitBreaker();

            // 5 failures → OPEN
            for (int i = 0; i < 5; i++)
                breaker.OnFailure();

            Assert.Equal(CircuitState.Open, breaker.State);
            Assert.False(breaker.CanExecute());
        }

        [Fact]
        public void Open_State_Blocks_Execution()
        {
            var breaker = new CircuitBreaker();

            for (int i = 0; i < 5; i++)
                breaker.OnFailure();

            Assert.Equal(CircuitState.Open, breaker.State);

            // Should block execution
            Assert.False(breaker.CanExecute());
        }

        [Fact]
        public void HalfOpen_Allows_Single_Trial()
        {
            var breaker = new CircuitBreaker();

            // Force OPEN
            for (int i = 0; i < 5; i++)
                breaker.OnFailure();

            // Manually force breaker to HalfOpen using reflection
            typeof(CircuitBreaker)
                .GetProperty("State")
                .SetValue(breaker, CircuitState.HalfOpen);

            // First trial allowed
            Assert.True(breaker.CanExecute());

            // Second trial must be rejected
            Assert.False(breaker.CanExecute());
        }

        [Fact]
        public void Success_In_HalfOpen_Closes_Circuit()
        {
            var breaker = new CircuitBreaker();

            // Open it
            for (int i = 0; i < 5; i++)
                breaker.OnFailure();

            // Force HalfOpen
            typeof(CircuitBreaker)
                .GetProperty("State")
                .SetValue(breaker, CircuitState.HalfOpen);

            Assert.True(breaker.CanExecute());

            // Now success should fully close it
            breaker.OnSuccess();

            Assert.Equal(CircuitState.Closed, breaker.State);
            Assert.True(breaker.CanExecute());
        }

        [Fact]
        public void Failure_In_HalfOpen_Reopens_Circuit()
        {
            var breaker = new CircuitBreaker();

            // Open
            for (int i = 0; i < 5; i++)
                breaker.OnFailure();

            // Move to HalfOpen
            typeof(CircuitBreaker)
                .GetProperty("State")
                .SetValue(breaker, CircuitState.HalfOpen);

            Assert.True(breaker.CanExecute());

            // Failure should immediately reopen
            breaker.OnFailure();

            Assert.Equal(CircuitState.Open, breaker.State);
            Assert.False(breaker.CanExecute());
        }
    }
}
