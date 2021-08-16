using GlmNet;
using IVM.Studio.Services;
using System.ServiceModel;
using System.Threading.Tasks;

namespace IVM.Studio.I3D
{
    public class I3DClientService : I3DClientContract
    {
        public static MainWindow w;

        public void OnOpen(string path)
        {
            w.vw.Open(path);

            if (w.viewtype == (int)I3DViewType.MAIN_VIEW)
            {
                int width = (int)w.vw.scene.tex3D.GetWidth();
                int height = (int)w.vw.scene.tex3D.GetHeight();
                float umWidth = w.vw.scene.meta.umWidth;
                float umHeight = w.vw.scene.meta.umHeight;

                Task.Run(() => w.wcfclient.channel.OnMetaLoaded(width, height, umWidth, umHeight));
            }
        }

        public void OnUpdateCamera(float px, float py, float pz, float ax, float ay, float az, float s)
        {
            w.vw.param.CAMERA_POS = new vec3(px, py, pz);
            w.vw.param.CAMERA_ANGLE = new vec3(ax, ay, az);
            w.vw.param.CAMERA_SCALE_FACTOR = s;
            w.vw.scene.UpdateModelviewMatrix();
        }
        public void OnChangeRenderMode(int m)
        {
            w.vw.param.RENDER_MODE = m;
        }

        public void OnChangeObliqueDepth(float d)
        {
            w.vw.param.OBLIQUE_DEPTH = d;
        }

    }
}

 