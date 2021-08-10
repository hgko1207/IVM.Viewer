using GlmNet;

namespace IVM.Studio.I3D
{
    public class I3DParam
    {
        // render
        public vec3 BG_COLOR = new vec3(0.075f, 0.075f, 0.075f);
        public vec3 THRESHOLD_INTENSITY = new vec3(0.0f, 0.0f, 0.0f);
        public float PER_PIXEl_ITERATION = 800;
        public vec3 ALPHA_WEIGHT = new vec3(20.0f, 20.0f, 20.0f);
        public float RENDER_MODE = 1; // 0: blending 1: added-color
        public int TEXT_SIZE = 12;
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
        public vec4 BOX_COLOR = new vec4(1.0f, 1.0f, 1.0f, 0.5f);
        public float BOX_HEIGHT = 1.0f;

        // grid
        public bool SHOW_GRID = false;
        public bool SHOW_GRID_TEXT = true;
        public vec4 GRID_COLOR = new vec4(0.5f, 0.5f, 0.5f, 0.2f);
        public float GRID_DIST = 16.0f; // pixels

        // axis
        public bool SHOW_AXIS = false;
        public float AXIS_HEIGHT = 0.2f;
        public vec3 AXIS_POS = new vec3(0.75f, 0.75f, 0);

        // camera
        public float CAMERA_SCALE_FACTOR = 0.8f;
        public vec3 CAMERA_POS = new vec3(0, 0, -5.0f);
        public vec2 CAMERA_ANGLE = new vec2(0, 0);
        public vec2 CAMERA_VELOCITY = new vec2(0, 0);
    }
}
