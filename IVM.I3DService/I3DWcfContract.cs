using System.ServiceModel;

namespace IVM.Studio.Services
{
    public class I3DWcfUrl
    {
        public static string serverUrl = "net.tcp://localhost:8089";
        public static string mainViewUrl = "net.tcp://localhost:8088";
        public static string sliceViewUrl = "net.tcp://localhost:8087";
    }

    public enum I3DViewType
    {
        MAIN_VIEW = 0,
        SLICE_VIEW = 1,
    }

    [ServiceContract]
    public interface I3DClientContract
    {
        [OperationContract]
        void OnOpen(string path);
    }

    [ServiceContract]
    public interface I3DServerContract
    {
        [OperationContract]
        void OnWindowLoaded(int viewtype);
    }
}
