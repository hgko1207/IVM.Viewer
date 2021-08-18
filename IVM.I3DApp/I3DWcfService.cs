using GlmNet;
using IVM.Studio.Services;
using System;
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
                int depth = (int)w.vw.scene.tex3D.GetDepth();
                float umWidth = w.vw.scene.meta.umWidth;
                float umHeight = w.vw.scene.meta.umHeight;
                float umPerPixelZ = w.vw.scene.meta.pixelPerUM_Z;

                Task.Run(() => w.wcfclient.channel.OnMetaLoaded(width, height, depth, umWidth, umHeight, umPerPixelZ));
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

        public void OnChangeBandOrder(int r, int g, int b, int a)
        {
            w.vw.param.BAND_ORDER.x = (float)r;
            w.vw.param.BAND_ORDER.y = (float)g;
            w.vw.param.BAND_ORDER.z = (float)b;
            w.vw.param.BAND_ORDER.w = (float)a;
        }

        public void OnChangeBandVisible(bool r, bool g, bool b, bool a)
        {
            w.vw.param.BAND_VISIBLE.x = (float)(r ? 1 : 0);
            w.vw.param.BAND_VISIBLE.y = (float)(g ? 1 : 0);
            w.vw.param.BAND_VISIBLE.z = (float)(b ? 1 : 0);
            w.vw.param.BAND_VISIBLE.w = (float)(a ? 1 : 0);
        }

        public void OnChangeIntensityThreshold(float rmin, float rmax, float gmin, float gmax, float bmin, float bmax, float amin, float amax)
        {
            w.vw.param.THRESHOLD_INTENSITY_MIN.x = rmin;
            w.vw.param.THRESHOLD_INTENSITY_MIN.y = gmin;
            w.vw.param.THRESHOLD_INTENSITY_MIN.z = bmin;
            w.vw.param.THRESHOLD_INTENSITY_MIN.w = amin;

            w.vw.param.THRESHOLD_INTENSITY_MAX.x = rmax;
            w.vw.param.THRESHOLD_INTENSITY_MAX.y = gmax;
            w.vw.param.THRESHOLD_INTENSITY_MAX.z = bmax;
            w.vw.param.THRESHOLD_INTENSITY_MAX.w = amax;
        }

        public void OnChangeAlphaWeight(float r, float g, float b, float a)
        {
            w.vw.param.ALPHA_WEIGHT.x = r;
            w.vw.param.ALPHA_WEIGHT.y = g;
            w.vw.param.ALPHA_WEIGHT.z = b;
            w.vw.param.ALPHA_WEIGHT.w = a;
        }

        public void OnChangeAxisParam(bool visible, int textsize, float height, float thickness, float px, float py)
        {
            w.vw.param.SHOW_AXIS = visible;
            w.vw.param.AXIS_TEXT_SIZE = textsize;
            w.vw.param.AXIS_HEIGHT = height;
            w.vw.param.AXIS_THICKNESS = thickness;
            w.vw.param.AXIS_POS = new vec3(px, py, 0);

            w.vw.scene.UpdateModelviewMatrix();
            w.vw.scene.UpdateMesh();
        }

        public void OnChangeBoxParam(float r, float g, float b, float a, float thickness)
        {
            w.vw.param.BOX_THICKNESS = thickness;
            w.vw.param.GRID_THICKNESS = Math.Max(thickness - 1, 1);

            w.vw.param.BOX_COLOR = new vec4(r, g, b, a);
            w.vw.param.GRID_COLOR = new vec4(r, g, b, a);

            if (a <= 0.0f)
            {
                w.vw.param.SHOW_BOX = false;
                w.vw.param.SHOW_GRID = false;
            }
            else
            {
                w.vw.param.SHOW_BOX = true;
                w.vw.param.SHOW_GRID = true;
            }
        }

        public void OnChangeGridLabelParam(float r, float g, float b, float a, int fontsize)
        {
            w.vw.param.GRID_TEXT_COLOR = new vec4(r, g, b, a);
            w.vw.param.GRID_TEXT_SIZE = fontsize;

            if (a <= 0.0f)
                w.vw.param.SHOW_GRID_TEXT = false;
            else
                w.vw.param.SHOW_GRID_TEXT = true;
        }

        public void OnChangeBackgroundParam(float r, float g, float b, float a)
        {
            w.vw.param.BG_COLOR = new vec3(r, g, b);
        }

        public void OnChangeSliceDepth(float x, float y, float z)
        {
            w.vw.param.SLICE_DEPTH = new vec3(x, y, z);
        }

        public void OnChangeSliceScaleParam(bool visible, int fontsize)
        {
            w.vw.param.SHOW_SLICE_TEXT = visible;
            w.vw.param.SLICE_TEXT_SIZE = fontsize;
        }
    }
}

 