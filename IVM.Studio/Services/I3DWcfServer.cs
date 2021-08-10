using IVM.Studio.Models.Events;
using Prism.Events;
using System.ServiceModel;

namespace IVM.Studio.Services
{
    public class I3DWcfServer
    {
        ServiceHost host;
        public I3DClientContract channel;

        public void Init(IEventAggregator e)
        {
            I3DServerService.EventAggregator = e;
        }

        public void Listen()
        {
            host = new ServiceHost(typeof(I3DServerService));
            host.AddServiceEndpoint(typeof(I3DServerContract), new NetTcpBinding(), I3DWcfUrl.serverUrl);
            host.Open();
        }

        public void Connect(int viewtype)
        {
            string url = "";
            if (viewtype == (int)I3DViewType.MAIN_VIEW)
                url = I3DWcfUrl.mainViewUrl;
            else
                url = I3DWcfUrl.sliceViewUrl;

            ChannelFactory<I3DClientContract> factory = new ChannelFactory<I3DClientContract>();
            factory.Endpoint.Address = new EndpointAddress(url);
            factory.Endpoint.Binding = new NetTcpBinding();
            factory.Endpoint.Contract.ContractType = typeof(I3DClientContract);

            // server channel
            channel = factory.CreateChannel();
        }
    }
}

