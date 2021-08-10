using GlmNet;
using IVM.Studio.Services;
using System.ServiceModel;

namespace IVM.Studio.I3D
{
    public class I3DClientService : I3DClientContract
    {
        public static MainWindow w;

        public void OnOpen(string path)
        {
            w.vw.Open(path);
        }

        public void OnUpdateCamera(float px, float py, float pz, float ax, float ay, float s)
        {
            w.vw.param.CAMERA_POS = new vec3(px, py, pz);
            w.vw.param.CAMERA_ANGLE = new vec2(ax, ay);
            w.vw.param.CAMERA_SCALE_FACTOR = s;
            w.vw.scene.UpdateModelviewMatrix();
        }
    }
}

 