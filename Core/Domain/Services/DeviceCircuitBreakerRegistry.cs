using System.Collections.Concurrent;

namespace Cyviz.Core.Domain.Services
{
    public interface IDeviceCircuitBreakerRegistry
    {
        ICircuitBreaker GetCircuitBreaker(string deviceId);
    }

    public class DeviceCircuitBreakerRegistry(CircuitBreakerOptions? options = null) : IDeviceCircuitBreakerRegistry
    {
        private readonly ConcurrentDictionary<string, ICircuitBreaker> _breakers = new();

        private readonly CircuitBreakerOptions _options = options ?? new CircuitBreakerOptions();

        public ICircuitBreaker GetCircuitBreaker(string deviceId)
        {
            return _breakers.GetOrAdd(deviceId, _ => new CircuitBreaker(_options));
        }
    }
}
