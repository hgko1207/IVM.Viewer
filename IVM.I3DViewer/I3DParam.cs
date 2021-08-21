using GlmNet;

namespace IVM.Studio.I3D
{
    public class I3DParam
    {
        // render
        public vec3 BG_COLOR = new vec3(0.0f, 0.0f, 0.0f);
        public vec4 THRESHOLD_INTENSITY_MIN = new vec4(0, 0, 0, 0);
        public vec4 THRESHOLD_INTENSITY_MAX = new vec4(255, 255, 255, 255);
        public float PER_PIXEl_ITERATION = 800;
        public vec4 ALPHA_WEIGHT = new vec4(10.0f, 10.0f, 10.0f, 10.0f);
        public vec4 ALPHA_BLEND = new vec4(0.1f, 0.1f, 0.1f, 0.1f);
        public vec4 BAND_ORDER = new vec4(0, 1, 2, 3);
        public vec4 BAND_VISIBLE = new vec4(1, 1, 1, 0);
        public float RENDER_MODE = 1; // 0: blending 1: added-color
        public float IS_COLOCALIZATION = 0; // 0: disable, 1: enable

        // slice
        public float OBLIQUE_DEPTH = 0;
        public vec3 SLICE_DEPTH = new vec3(0.0f, 0.0f, 0.0f);
        public vec4 SLICE_LINE_COLOR_X = new vec4(1.0f, 0.0f, 0.0f, 0.5f);
        public vec4 SLICE_LINE_COLOR_Y = new vec4(0.0f, 1.0f, 0.0f, 0.5f);
        public vec4 SLICE_LINE_COLOR_Z = new vec4(0.0f, 0.0f, 1.0f, 0.5f);

        // box
        public bool SHOW_BOX = true;
        public vec4 BOX_COLOR = new vec4(1.0f, 1.0f, 1.0f, 1.0f);
        public float BOX_HEIGHT = 1.0f;
        public float BOX_THICKNESS = 3.0f;

        // grid
        public bool SHOW_GRID = false;
        public bool SHOW_GRID_TEXT = true;
        public vec4 GRID_COLOR = new vec4(1.0f, 1.0f, 1.0f, 1.0f);
        public float GRID_MAJOR_DIST = 40.0f; // um
        public float GRID_MINOR_DIST = 10.0f; // um
        public int GRID_TEXT_SIZE = 0;
        public vec4 GRID_TEXT_COLOR = new vec4(1.0f, 1.0f, 1.0f, 1.0f);
        public float GRID_THICKNESS = 3.0f;

        // axis
        public bool SHOW_AXIS = true;
        public float AXIS_HEIGHT = 0.2f;
        public vec3 AXIS_POS = new vec3(0.75f, 0.75f, 0);
        public int AXIS_TEXT_SIZE = 0;
        public float AXIS_THICKNESS = 2.0f;

        // timelapse info
        public bool SHOW_TIMELAPSE = true;
        public vec3 TIMELAPSE_POS = new vec3(0.05f, 0.95f, 0);
        public int TIMELAPSE_TEXT_SIZE = 12;
        public vec4 TIMELAPSE_TEXT_COLOR = new vec4(1.0f, 1.0f, 1.0f, 1.0f);
        public string TIMELAPSE_FORMAT = "yy-MM-dd HH:mm:ss";
        public float TIMELAPSE_TEXTURE_DELAY = 100.0f; // mili-sec

        // camera
        public float CAMERA_SCALE_FACTOR = 3.0f;
        static public float CAMERA_DIST = -10.0f;
        public vec3 CAMERA_POS = new vec3(0, 0, CAMERA_DIST);
        public vec3 CAMERA_ANGLE = new vec3(0, 0, 0);
        public vec2 CAMERA_VELOCITY = new vec2(0, 0);
    }
}
