namespace Cyviz.Simulators
{
    public static class DeviceTelemetryGenerator
    {
        private static readonly Random _rnd = new();

        public static string GenerateJson()
        {
            var payload = new
            {
                temp = _rnd.Next(20, 35),
                cpu = _rnd.Next(5, 90),
                ts = DateTime.UtcNow
            };

            return System.Text.Json.JsonSerializer.Serialize(payload);
        }
    }
}
