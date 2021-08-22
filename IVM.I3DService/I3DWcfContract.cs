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

    public enum I3DRenderMode
    {
        BLEND = 0,
        ADDED = 1,
        OBLIQUE = 2,
        SLICE = 3,
    }

    [ServiceContract]
    public interface I3DClientContract
    {
        [OperationContract]
        void OnOpen(string path, int lower, int upper, bool reverse);

        [OperationContract]
        void OnCaptureScreen(string path);

        [OperationContract]
        void StartRecordVideo(string path);

        [OperationContract]
        void StopRecordVideo();

        [OperationContract]
        void RebuildVideo(string path); // for test

        [OperationContract]
        void OnUpdateCamera(float px, float py, float pz, float ax, float ay, float az, float s);

        [OperationContract]
        void OnChangeRenderMode(int m);

        [OperationContract]
        void OnChangeObliqueDepth(float d);

        [OperationContract]
        void OnChangeBandOrder(int r, int g, int b, int a);

        [OperationContract]
        void OnChangeBandVisible(bool r, bool g, bool b, bool a);

        [OperationContract]
        void OnChangeIntensityThreshold(float rmin, float rmax, float gmin, float gmax, float bmin, float bmax, float amin, float amax);

        [OperationContract]
        void OnChangeAlphaWeight(float r, float g, float b, float a);

        [OperationContract]
        void OnChangeAxisParam(bool visible, int textsize, float height, float thickness, float px, float py);

        [OperationContract]
        void OnChangeBoxParam(float r, float g, float b, float a, float thickness);

        [OperationContract]
        void OnChangeGridLabelParam(float r, float g, float b, float a, int fontsize);

        [OperationContract]
        void OnChangeGridSizeParam(float major, float minor);

        [OperationContract]
        void OnChangeBackgroundParam(float r, float g, float b, float a);

        [OperationContract]
        void OnChangeSliceDepth(float x, float y, float z);

        [OperationContract]
        void OnChangeTimelapseLabelParam(bool visible, float r, float g, float b, float a, int fontsize, string format, float px, float py, int msec);
    }

    [ServiceContract]
    public interface I3DServerContract
    {
        [OperationContract]
        void OnWindowLoaded(int viewtype);

        [OperationContract]
        void OnMetaLoaded(int width, int height, int depth, float umWidth, float umHeight, float umPerPixelZ);


        [OperationContract]
        void OnFirstRender(int viewtype);


        [OperationContract]
        void OnUpdateCamera(int viewtype, float px, float py, float pz, float ax, float ay, float az, float s);
    }
}
