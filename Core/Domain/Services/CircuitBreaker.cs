using Cyviz.Core.Domain.Enums;

namespace Cyviz.Core.Domain.Services
{
    public class CircuitBreakerOptions
    {
        public int FailureThreshold { get; set; } = 5;
        public TimeSpan OpenToHalfOpenAfter { get; set; } = TimeSpan.FromSeconds(10);
    }

    public interface ICircuitBreaker
    {
        CircuitState State { get; }

        bool CanExecute();// true if Closed or HalfOpen in “test” state
        void OnSuccess();// reset failures, maybe close breaker
        void OnFailure(); // increment failure count, maybe open breaker
    }

    public class CircuitBreaker(CircuitBreakerOptions? options = null) : ICircuitBreaker
    {
        private readonly CircuitBreakerOptions _options = options ?? new CircuitBreakerOptions();

        private int _failureCount;
        private DateTime _lastFailureUtc;
        private bool _halfOpenTrialInProgress;

        public CircuitState State { get; private set; } = CircuitState.Closed;

        public bool CanExecute()
        {
            switch (State)
            {
                case CircuitState.Closed:
                    return true;

                case CircuitState.Open:
                    // Check if we can move to HalfOpen
                    if (DateTime.UtcNow - _lastFailureUtc >= _options.OpenToHalfOpenAfter)
                    {
                        State = CircuitState.HalfOpen;
                        _halfOpenTrialInProgress = false;
                        return true;
                    }
                    return false;

                case CircuitState.HalfOpen:
                    // Allow only one trial execution
                    if (_halfOpenTrialInProgress)
                        return false;

                    _halfOpenTrialInProgress = true;
                    return true;

                default:
                    return false;
            }
        }

        public void OnSuccess()
        {
            _failureCount = 0;
            _halfOpenTrialInProgress = false;
            State = CircuitState.Closed;
        }

        public void OnFailure()
        {
            _failureCount++;
            _lastFailureUtc = DateTime.UtcNow;
            _halfOpenTrialInProgress = false;

            if (_failureCount >= _options.FailureThreshold)
                State = CircuitState.Open;
        }
    }
}
