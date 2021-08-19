using GlmNet;

namespace IVM.Studio.I3D
{
    public class I3DParam
    {
        // render
        public vec3 BG_COLOR = new vec3(0.075f, 0.075f, 0.075f);
        public vec4 THRESHOLD_INTENSITY_MIN = new vec4(0, 0, 0, 0);
        public vec4 THRESHOLD_INTENSITY_MAX = new vec4(255, 255, 255, 255);
        public float PER_PIXEl_ITERATION = 800;
        public vec4 ALPHA_WEIGHT = new vec4(20.0f, 20.0f, 20.0f, 20.0f);
        public vec4 BAND_ORDER = new vec4(0, 1, 2, 3);
        public vec4 BAND_VISIBLE = new vec4(1, 1, 1, 0);
        public float RENDER_MODE = 1; // 0: blending 1: added-color
        public float IS_COLOCALIZATION = 0; // 0: disable, 1: enable
        public float TIMELAPSE_TEXTURE_DELAY = 1000.0f / 30.0f;

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
        public float BOX_THICKNESS = 1.0f;

        // grid
        public bool SHOW_GRID = false;
        public bool SHOW_GRID_TEXT = true;
        public vec4 GRID_COLOR = new vec4(0.5f, 0.5f, 0.5f, 0.2f);
        public float GRID_DIST = 16.0f; // pixels
        public int GRID_TEXT_SIZE = 0;
        public vec4 GRID_TEXT_COLOR = new vec4(1.0f, 1.0f, 1.0f, 1.0f);
        public float GRID_THICKNESS = 1.0f;

        // axis
        public bool SHOW_AXIS = true;
        public float AXIS_HEIGHT = 0.2f;
        public vec3 AXIS_POS = new vec3(0.75f, 0.75f, 0);
        public int AXIS_TEXT_SIZE = 0;
        public float AXIS_THICKNESS = 2.0f;

        // camera
        public float CAMERA_SCALE_FACTOR = 0.8f;
        public vec3 CAMERA_POS = new vec3(0, 0, -5.0f);
        public vec3 CAMERA_ANGLE = new vec3(0, 0, 0);
        public vec2 CAMERA_VELOCITY = new vec2(0, 0);
    }
}
