using SharpGL;
using GlmNet;
using System.Windows.Threading;
using System;

namespace ivm
{
    public class ViewScene
    {
        ImageStackView view;
        OpenGL ogl;
        DispatcherTimer tUpd; // 업데이트 타이머

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

            tUpd = new DispatcherTimer();
            tUpd.Tick += UpdateTick;
            tUpd.Start();
        }

        private void UpdateTick(object sender, EventArgs e)
        {
            view.camera.Update();

            view.RenderTarget.DoRender();
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
                view.param.BOX_HEIGHT = (float)tex3D.GetDepth() / (float)tex3D.GetWidth();
                view.param.BOX_HEIGHT *= view.scene.meta.pixelPerUM_Z / view.scene.meta.pixelPerUM_X;
            }
            else
            {
                view.param.BOX_HEIGHT = h;
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
            float s = view.param.CAMERA_SCALE_FACTOR;
            float rx = view.param.CAMERA_ANGLE.x;
            float ry = view.param.CAMERA_ANGLE.y;

            // camera transform
            mat4 viewRot = glm.rotate(mat4.identity(), ViewCommon.Deg2Rad(0), new vec3(1, 0, 0));
            mat4 viewTrn = glm.translate(mat4.identity(), view.param.CAMERA_POS);
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
            mat4 axisTrn = glm.translate(mat4.identity(), view.param.AXIS_POS);
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
            gl.ClearColor(view.param.BG_COLOR.x, view.param.BG_COLOR.y, view.param.BG_COLOR.z, 1.0f);
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT | OpenGL.GL_STENCIL_BUFFER_BIT);

            if (view.param.RENDER_MODE == ViewRenderMode.SLICE)
            {
                oblique.RenderOblique(gl, matProjOrtho, matSliceZView, matSliceZRot, view.param.SLICE_DEPTH.z * view.param.BOX_HEIGHT);
                oblique.RenderOblique(gl, matProjOrtho, matSliceYView, matSliceYRot, view.param.SLICE_DEPTH.y);
                oblique.RenderOblique(gl, matProjOrtho, matSliceXView, matSliceXRot, view.param.SLICE_DEPTH.x);

                if (view.param.SHOW_BOX)
                {
                    box.RenderOutline(gl, matProjOrtho, matSliceZView, view.param.SLICE_LINE_COLOR_Z);
                    box.RenderOutline(gl, matProjOrtho, matSliceYView, view.param.SLICE_LINE_COLOR_Y);
                    box.RenderOutline(gl, matProjOrtho, matSliceXView, view.param.SLICE_LINE_COLOR_X);

                    oblique.RenderDepthLine(gl, matProjOrtho, matSliceZView, view.param.SLICE_LINE_COLOR_X, ViewAxisDirection.X, view.param.SLICE_DEPTH.x);
                    oblique.RenderDepthLine(gl, matProjOrtho, matSliceZView, view.param.SLICE_LINE_COLOR_Y, ViewAxisDirection.Y, view.param.SLICE_DEPTH.y);
                    oblique.RenderDepthLine(gl, matProjOrtho, matSliceXView, view.param.SLICE_LINE_COLOR_Z, ViewAxisDirection.Z1, view.param.SLICE_DEPTH.z * view.param.BOX_HEIGHT);
                    oblique.RenderDepthLine(gl, matProjOrtho, matSliceYView, view.param.SLICE_LINE_COLOR_Z, ViewAxisDirection.Z2, view.param.SLICE_DEPTH.z * view.param.BOX_HEIGHT);
                }

                // grid-text
                if (view.param.SHOW_GRID && view.param.SHOW_GRID_TEXT)
                    box.RenderGridText(gl, matProjOrtho, matSliceZView);
            }
            else
            {
                // draw box outline
                if (view.param.SHOW_GRID)
                    box.RenderGrid(gl, matProj, matGridView, matGridObj);

                if (view.param.SHOW_BOX)
                    box.RenderOutline(gl, matProj, matGridView, view.param.BOX_COLOR);

                // 3d-volume rendering
                if (view.param.RENDER_MODE == ViewRenderMode.BLEND || view.param.RENDER_MODE == ViewRenderMode.ADDED)
                    box.RenderVolume3D(gl, matProj, matModelView);
                else if (view.param.RENDER_MODE == ViewRenderMode.OBLIQUE)
                    oblique.RenderOblique(gl, matProj, matModelView, matModelRot, view.param.OBLIQUE_DEPTH);
                
                // grid-text
                if (view.param.SHOW_GRID && view.param.SHOW_GRID_TEXT)
                    box.RenderGridText(gl, matProj, matGridView);

                // draw axis
                if (view.param.SHOW_AXIS)
                    axis.Render(gl, matAxisView);
            }
        }
    }
}
