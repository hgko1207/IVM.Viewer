using IVM.Studio.Services;
using System.ServiceModel;
using System.ServiceModel.Description;

namespace IVM.Studio.I3D
{
    public class I3DWcfClient
    {
        ServiceHost host;
        public I3DServerContract channel;

        public void Listen(int viewtype)
        {
            string url = "";
            if (viewtype == (int)I3DViewType.MAIN_VIEW)
                url = I3DWcfUrl.mainViewUrl;
            else
                url = I3DWcfUrl.sliceViewUrl;

            host = new ServiceHost(typeof(I3DClientService));
            host.AddServiceEndpoint(typeof(I3DClientContract), new NetTcpBinding(), url);
            host.Open();
        }

        public void Connect()
        {
            ChannelFactory<I3DServerContract> factory = new ChannelFactory<I3DServerContract>();
            factory.Endpoint.Address = new EndpointAddress(I3DWcfUrl.serverUrl);
            factory.Endpoint.Binding = new NetTcpBinding();
            factory.Endpoint.Contract.ContractType = typeof(I3DServerContract);

            // server channel
            channel = factory.CreateChannel();
        }
    }
}

