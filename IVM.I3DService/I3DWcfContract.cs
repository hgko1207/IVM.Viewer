using System.ServiceModel;

namespace IVM.Studio.Services
{
    public class I3DWcfUrl
    {
        public static string serverUrl = "net.tcp://localhost:8099";
        public static string mainViewUrl = "net.tcp://localhost:8098";
        public static string sliceViewUrl = "net.tcp://localhost:8097";
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

        [OperationContract]
        void OnUpdateCamera(float px, float py, float pz, float ax, float ay, float az, float s);
    }

    [ServiceContract]
    public interface I3DServerContract
    {
        [OperationContract]
        void OnWindowLoaded(int viewtype);

        [OperationContract]
        void OnUpdateCamera(int viewtype, float px, float py, float pz, float ax, float ay, float az, float s);
    }
}
