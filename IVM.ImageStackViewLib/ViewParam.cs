using GlmNet;

namespace ivm
{
    public static class ViewParam
    {
        // render
        public static vec3 BG_COLOR = new vec3(0.0f, 0.0f, 0.0f);
        public static vec3 THRESHOLD_INTENSITY = new vec3(0.0f, 0.0f, 0.0f);
        public static float PER_PIXEl_ITERATION = 800;
        public static vec3 ALPHA_WEIGHT = new vec3(20.0f, 20.0f, 20.0f);
        public static float RENDER_MODE = 1; // 0: blending 1: added-color
        public static int TEXT_SIZE = 12;
        public static float IS_COLOCALIZATION = 0; // 0: disable, 1: enable
        public static float TIMELAPSE_TEXTURE_DELAY = 1000.0f / 30.0f;

        // slice
        public static float OBLIQUE_DEPTH = 0;
        public static vec3 SLICE_DEPTH = new vec3(0.0f, 0.0f, 0.0f);
        public static vec4 SLICE_LINE_COLOR_X = new vec4(1.0f, 0.0f, 0.0f, 0.5f);
        public static vec4 SLICE_LINE_COLOR_Y = new vec4(0.0f, 1.0f, 0.0f, 0.5f);
        public static vec4 SLICE_LINE_COLOR_Z = new vec4(0.0f, 0.0f, 1.0f, 0.5f);

        // box
        public static bool SHOW_BOX = true;
        public static vec4 BOX_COLOR = new vec4(1.0f, 1.0f, 1.0f, 0.5f);
        public static float BOX_HEIGHT = 1.0f;

        // grid
        public static bool SHOW_GRID = false;
        public static bool SHOW_GRID_TEXT = true;
        public static vec4 GRID_COLOR = new vec4(0.5f, 0.5f, 0.5f, 0.2f);
        public static float GRID_DIST = 16.0f; // pixels

        // axis
        public static bool SHOW_AXIS = false;
        public static float AXIS_HEIGHT = 0.2f;
        public static vec3 AXIS_POS = new vec3(0.75f, 0.75f, 0);

        // camera
        public static float CAMERA_SCALE_FACTOR = 0.8f;
        public static vec3 CAMERA_POS = new vec3(0, 0, -5.0f);
        public static vec2 CAMERA_ANGLE = new vec2(0, 0);
        public static vec2 CAMERA_VELOCITY = new vec2(0, 0);
    }
}
