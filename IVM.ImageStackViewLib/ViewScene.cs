using SharpGL;
using GlmNet;

namespace ivm
{
    public class ViewScene
    {
        ImageStackView view;
        OpenGL ogl;

        public ViewTex3D tex3D;
        public ViewAxis axis;
        public ViewBox box;
        public ViewOblique oblique;
        public ViewMeta meta;

        bool loadedTexture = false;
        bool loadedMeta = false;

        mat4 matModelView = mat4.identity();
        mat4 matModelRot = mat4.identity();
        mat4 matAxisView = mat4.identity();
        mat4 matGridView = mat4.identity();
        mat4 matGridObj = mat4.identity();
        mat4 matProj = mat4.identity();
        mat4 matProjOrtho = mat4.identity();

        mat4 matSliceZView = mat4.identity();
        mat4 matSliceZRot = mat4.identity();

        mat4 matSliceYView = mat4.identity();
        mat4 matSliceYRot = mat4.identity();

        mat4 matSliceXView = mat4.identity();
        mat4 matSliceXRot = mat4.identity();

        public ViewScene(ImageStackView v)
        {
            view = v;

            axis = new ViewAxis(v);
            box = new ViewBox(v);
            oblique = new ViewOblique(v);
            tex3D = new ViewTex3D(v);
            meta = new ViewMeta(v);
        }

        public bool Open(OpenGL gl, string imgPath)
        {
            ogl = gl;

            loadedMeta = meta.Load(imgPath);
            loadedTexture = tex3D.Load(gl, imgPath);

            UpdateHeight(gl);

            return loadedTexture;
        }

        public bool IsLoaded()
        {
            return loadedTexture;
        }

        public void SetRenderMode(uint m)
        {
            if (m == ViewRenderMode.SLICE)
            {
                UpdateHeight(ogl, 0.25f);
            }
            else
            {
                UpdateHeight(ogl);
            }
        }

        public void UpdateHeight(OpenGL gl, float h = -1.0f)
        {
            // calculate box-height
            if (h == -1.0f)
            {
                ViewParam.BOX_HEIGHT = (float)tex3D.GetDepth() / (float)tex3D.GetWidth();
                ViewParam.BOX_HEIGHT *= view.scene.meta.pixelPerUM_Z / view.scene.meta.pixelPerUM_X;
            }
            else
            {
                ViewParam.BOX_HEIGHT = h;
            }
            
            view.scene.UpdateMesh(gl);
        }

        public void UpdateProjectionMatrix()
        {
            float screenWidth = (float)view.ActualWidth;
            float screenHeight = (float)view.ActualHeight;

            //  Create the projection matrix for our screen size.
            const float S = 0.46f;
            float H = S * screenHeight / screenWidth;
            matProj = glm.frustum(-S, S, -H, H, 1, 100);
            matProjOrtho = glm.ortho(-S, S, -H, H, 1, 100);
        }

        public void UpdateModelviewMatrix()
        {
            float s = ViewParam.CAMERA_SCALE_FACTOR;
            float rx = ViewParam.CAMERA_ANGLE.x;
            float ry = ViewParam.CAMERA_ANGLE.y;

            // camera transform
            mat4 viewRot = glm.rotate(mat4.identity(), ViewCommon.Deg2Rad(0), new vec3(1, 0, 0));
            mat4 viewTrn = glm.translate(mat4.identity(), ViewParam.CAMERA_POS);
            mat4 viewMatrix = viewTrn * viewRot;

            // world transform
            mat4 scale = glm.scale(mat4.identity(), new vec3(s, s, s));
            mat4 rotY = glm.rotate(mat4.identity(), ViewCommon.Deg2Rad(ry), new vec3(1, 0, 0));
            mat4 rotZ = glm.rotate(mat4.identity(), ViewCommon.Deg2Rad(rx), new vec3(0, 0, 1));
            mat4 modelMatrix = scale * rotY * rotZ;
            matModelView = viewMatrix * modelMatrix;
            matModelRot = rotY * rotZ;

            // slice transform
            float slf = 0.25f;
            float sx = -0.05f;
            float sy = 0.05f;
            mat4 sls = glm.scale(mat4.identity(), new vec3(slf, slf, slf));
            mat4 vtz = glm.translate(mat4.identity(), new vec3(sx, sy, -5));
            matSliceZView = vtz * viewRot * sls;
            matSliceZRot = mat4.identity();

            mat4 sry = glm.rotate(mat4.identity(), ViewCommon.Deg2Rad(-90), new vec3(0, 1, 0));
            mat4 vty = glm.translate(mat4.identity(), new vec3(sx + 0.35f, sy, -5));
            matSliceYView = vty * viewRot * sls * sry;
            matSliceYRot = sry;

            mat4 srx = glm.rotate(mat4.identity(), ViewCommon.Deg2Rad(-90), new vec3(1, 0, 0));
            mat4 vtx = glm.translate(mat4.identity(), new vec3(sx, sy - 0.35f, -5));
            matSliceXView = vtx * viewRot * sls * srx;
            matSliceXRot = srx;

            // grid must be bigger more than object.
            float sgridf = 1.001f;
            mat4 sgrid = glm.scale(mat4.identity(), new vec3(s * sgridf, s * sgridf, s * sgridf));
            mat4 gridMatrix = sgrid * rotY * rotZ;
            matGridView = viewMatrix * gridMatrix;
            matGridObj = gridMatrix;

            // axis must be screen space matrix.
            mat4 axisTrn = glm.translate(mat4.identity(), ViewParam.AXIS_POS);
            matAxisView = axisTrn * rotY * rotZ;
        }

        public void Init(OpenGL gl)
        {
            gl.Enable(OpenGL.GL_TEXTURE_3D);

            box.InitShader(gl);
            oblique.InitShader(gl);

            box.CreateMesh(gl);
            box.CreateGrid(gl);
            axis.CreateMesh(gl);
        }

        public void UpdateMesh(OpenGL gl)
        {
            box.CreateMesh(gl);
            box.CreateGrid(gl);
            axis.CreateMesh(gl);
        }

        public void Render(OpenGL gl)
        {
            if (!loadedTexture)
                return;

            // Clear the color and depth buffers.
            gl.ClearColor(ViewParam.BG_COLOR.x, ViewParam.BG_COLOR.y, ViewParam.BG_COLOR.z, 1.0f);
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT | OpenGL.GL_STENCIL_BUFFER_BIT);

            if (ViewParam.RENDER_MODE == ViewRenderMode.SLICE)
            {
                oblique.RenderOblique(gl, matProjOrtho, matSliceZView, matSliceZRot, ViewParam.SLICE_DEPTH.z * ViewParam.BOX_HEIGHT);
                oblique.RenderOblique(gl, matProjOrtho, matSliceYView, matSliceYRot, ViewParam.SLICE_DEPTH.y);
                oblique.RenderOblique(gl, matProjOrtho, matSliceXView, matSliceXRot, ViewParam.SLICE_DEPTH.x);

                if (ViewParam.SHOW_BOX)
                {
                    box.RenderOutline(gl, matProjOrtho, matSliceZView, ViewParam.SLICE_LINE_COLOR_Z);
                    box.RenderOutline(gl, matProjOrtho, matSliceYView, ViewParam.SLICE_LINE_COLOR_Y);
                    box.RenderOutline(gl, matProjOrtho, matSliceXView, ViewParam.SLICE_LINE_COLOR_X);

                    oblique.RenderDepthLine(gl, matProjOrtho, matSliceZView, ViewParam.SLICE_LINE_COLOR_X, ViewAxisDirection.X, ViewParam.SLICE_DEPTH.x);
                    oblique.RenderDepthLine(gl, matProjOrtho, matSliceZView, ViewParam.SLICE_LINE_COLOR_Y, ViewAxisDirection.Y, ViewParam.SLICE_DEPTH.y);
                    oblique.RenderDepthLine(gl, matProjOrtho, matSliceXView, ViewParam.SLICE_LINE_COLOR_Z, ViewAxisDirection.Z1, ViewParam.SLICE_DEPTH.z * ViewParam.BOX_HEIGHT);
                    oblique.RenderDepthLine(gl, matProjOrtho, matSliceYView, ViewParam.SLICE_LINE_COLOR_Z, ViewAxisDirection.Z2, ViewParam.SLICE_DEPTH.z * ViewParam.BOX_HEIGHT);
                }

                // grid-text
                if (ViewParam.SHOW_GRID && ViewParam.SHOW_GRID_TEXT)
                    box.RenderGridText(gl, matProjOrtho, matSliceZView);
            }
            else
            {
                // draw box outline
                if (ViewParam.SHOW_GRID)
                    box.RenderGrid(gl, matProj, matGridView, matGridObj);

                if (ViewParam.SHOW_BOX)
                    box.RenderOutline(gl, matProj, matGridView, ViewParam.BOX_COLOR);

                // 3d-volume rendering
                if (ViewParam.RENDER_MODE == ViewRenderMode.BLEND || ViewParam.RENDER_MODE == ViewRenderMode.ADDED)
                    box.RenderVolume3D(gl, matProj, matModelView);
                else if (ViewParam.RENDER_MODE == ViewRenderMode.OBLIQUE)
                    oblique.RenderOblique(gl, matProj, matModelView, matModelRot, ViewParam.OBLIQUE_DEPTH);
                
                // grid-text
                if (ViewParam.SHOW_GRID && ViewParam.SHOW_GRID_TEXT)
                    box.RenderGridText(gl, matProj, matGridView);

                // draw axis
                if (ViewParam.SHOW_AXIS)
                    axis.Render(gl, matAxisView);
            }
        }
    }
}
