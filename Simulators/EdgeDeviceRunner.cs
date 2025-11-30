namespace Cyviz.Simulators
{
    public class EdgeDeviceRunner
    {
        private readonly DeviceSimulator _simulator;

        public EdgeDeviceRunner(DeviceSimulator simulator)
        {
            _simulator = simulator;
        }

        public Task StartDeviceAsync(string id, CancellationToken ct = default)
        {
            return _simulator.RunAsync(id, ct);
        }
    }
}
