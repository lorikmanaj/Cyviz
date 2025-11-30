using Cyviz.Core.Application.Interfaces;
using Cyviz.Core.Domain.Entities;
using Cyviz.Core.Domain.Enums;

namespace Cyviz.Core.Domain.Services
{
    /// <summary>
    /// ///////////////
    /// The Followings are the sample/demo examples of how the specified concepts would be integrated
    /// </summary>
    public interface IHttpJsonAdapter : IDeviceProtocolAdapter { }
    public interface ITcpLineAdapter : IDeviceProtocolAdapter { }
    public interface IEdgeSignalRAdapter : IDeviceProtocolAdapter { }
    /// <summary>
    /// ///////////////////////////////
    /// </summary>
    public interface IDeviceProtocolAdapterResolver
    {
        IDeviceProtocolAdapter ResolveAdapter(Device device);
    }

    public class DeviceProtocolAdapterResolver(
        IHttpJsonAdapter httpJsonAdapter,
        ITcpLineAdapter tcpLineAdapter,
        IEdgeSignalRAdapter edgeSignalRAdapter) : IDeviceProtocolAdapterResolver
    {
        private readonly IHttpJsonAdapter _httpJsonAdapter = httpJsonAdapter;
        private readonly ITcpLineAdapter _tcpLineAdapter = tcpLineAdapter;
        private readonly IEdgeSignalRAdapter _edgeSignalRAdapter = edgeSignalRAdapter;

        public IDeviceProtocolAdapter ResolveAdapter(Device device)
        {
            return device.Protocol switch
            {
                DeviceProtocol.HttpJson => _httpJsonAdapter,
                DeviceProtocol.TcpLine => _tcpLineAdapter,
                DeviceProtocol.EdgeSignalR => _edgeSignalRAdapter,
                _ => _httpJsonAdapter
            };
        }
    }
}
