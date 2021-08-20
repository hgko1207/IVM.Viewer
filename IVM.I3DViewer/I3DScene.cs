using SharpGL;
using GlmNet;
using System.Windows.Threading;
using System;
using SharpGL.SceneGraph;
using System.Threading.Tasks;

namespace IVM.Studio.I3D
{
    public class I3DScene
    {
        I3DViewer view;
        DispatcherTimer timer; // 업데이트 타이머

        public I3DTex3D tex3D;
        public I3DAxis axis;
        public I3DBox box;
        public I3DOblique oblique;
        public I3DMeta meta;

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

        public delegate void FirstRenderDelegate();
        public FirstRenderDelegate firstRenderFunc = null;
        bool firstRenderd = false;

        public I3DScene(I3DViewer v)
        {
            view = v;

            axis = new I3DAxis(v);
            box = new I3DBox(v);
            oblique = new I3DOblique(v);
            tex3D = new I3DTex3D(v);
            meta = new I3DMeta(v);

            timer = new DispatcherTimer();
            timer.Tick += UpdateTick;
            timer.Start();
        }

        ~I3DScene()
        {
            timer.Stop();
        }

        private void UpdateTick(object sender, EventArgs e)
        {
            view.RenderTarget.DoRender();
        }

        public async Task<bool> Open(string imgPath, int lower, int upper, bool reverse)
        {
            loadedTexture = await tex3D.Load(view.gl, imgPath, lower, upper, reverse);

            loadedMeta = meta.Load(imgPath);

            UpdateHeight();

            return loadedTexture;
        }

        public bool IsLoaded()
        {
            return loadedTexture;
        }

        public void SetRenderMode(uint m)
        {
            if (m == I3DRenderMode.SLICE)
            {
                UpdateHeight(0.25f);
            }
            else
            {
                UpdateHeight();
            }
        }

        public void UpdateHeight(float h = -1.0f)
        {
            // calculate box-height
            if (h == -1.0f)
            {
                view.param.BOX_HEIGHT = (float)tex3D.GetDepth() / (float)tex3D.GetWidth();

                if (view.scene.meta.pixelPerUM_Z > 0)
                    view.param.BOX_HEIGHT *= view.scene.meta.pixelPerUM_Z / view.scene.meta.pixelPerUM_X;
            }
            else
            {
                view.param.BOX_HEIGHT = h;
            }
            
            view.scene.UpdateMesh();
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
            //float rx = view.param.CAMERA_ANGLE.x;
            //float ry = view.param.CAMERA_ANGLE.y;
            //float rz = view.param.CAMERA_ANGLE.z;

            // camera transform
            mat4 viewRot = glm.rotate(mat4.identity(), I3DCommon.Deg2Rad(0), new vec3(1, 0, 0));
            mat4 viewTrn = glm.translate(mat4.identity(), view.param.CAMERA_POS);
            mat4 viewMatrix = viewTrn * viewRot;

            // world transform
            mat4 scale = glm.scale(mat4.identity(), new vec3(s, s, s));
            //mat4 rotY = glm.rotate(mat4.identity(), I3DCommon.Deg2Rad(ry), new vec3(1, 0, 0));
            //mat4 rotZ = glm.rotate(mat4.identity(), I3DCommon.Deg2Rad(rx), new vec3(0, 0, 1));
            //mat4 rot = rotY * rotZ;

            Matrix mrot3 = view.camera.Matrix3FromEuler(view.param.CAMERA_ANGLE);
            mat4 rot = view.camera.Matrix4fSetRotationFromMatrix3f(mrot3);

            //rot = view.camera.transformMatrix;

            mat4 modelMatrix = scale * rot;
            matModelView = viewMatrix * modelMatrix;
            matModelRot = rot;

            // slice transform
            float slf = 0.25f;
            float sx = -0.05f;
            float sy = 0.05f;
            mat4 sls = glm.scale(mat4.identity(), new vec3(slf, slf, slf));
            mat4 vtz = glm.translate(mat4.identity(), new vec3(sx, sy, -5));
            matSliceZView = vtz * viewRot * sls;
            matSliceZRot = mat4.identity();

            mat4 sry = glm.rotate(mat4.identity(), I3DCommon.Deg2Rad(-90), new vec3(0, 1, 0));
            mat4 vty = glm.translate(mat4.identity(), new vec3(sx + 0.35f, sy, -5));
            matSliceYView = vty * viewRot * sls * sry;
            matSliceYRot = sry;

            mat4 srx = glm.rotate(mat4.identity(), I3DCommon.Deg2Rad(-90), new vec3(1, 0, 0));
            mat4 vtx = glm.translate(mat4.identity(), new vec3(sx, sy - 0.35f, -5));
            matSliceXView = vtx * viewRot * sls * srx;
            matSliceXRot = srx;

            // grid must be bigger more than object.
            float sgridf = 1.001f;
            mat4 sgrid = glm.scale(mat4.identity(), new vec3(s * sgridf, s * sgridf, s * sgridf));
            mat4 gridMatrix = sgrid * rot;
            matGridView = viewMatrix * gridMatrix;
            matGridObj = gridMatrix;

            // axis must be screen space matrix.
            mat4 axisTrn = glm.translate(mat4.identity(), view.param.AXIS_POS);
            matAxisView = axisTrn * rot;
        }

        public void Init()
        {
            view.gl.Enable(OpenGL.GL_TEXTURE_3D);

            box.InitShader(view.gl);
            oblique.InitShader(view.gl);

            box.CreateMesh(view.gl);
            box.CreateGrid(view.gl);
            axis.CreateMesh(view.gl);
        }

        public void UpdateMesh()
        {
            box.CreateMesh(view.gl);
            box.CreateGrid(view.gl);
            axis.CreateMesh(view.gl);
        }

        public void Render()
        {
            OpenGL gl = view.gl;

            // Clear the color and depth buffers.
            gl.ClearColor(view.param.BG_COLOR.x, view.param.BG_COLOR.y, view.param.BG_COLOR.z, 1.0f);
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT | OpenGL.GL_STENCIL_BUFFER_BIT);

            if (!loadedTexture)
                return;

            if (view.param.RENDER_MODE == I3DRenderMode.SLICE)
            {
                oblique.RenderOblique(gl, matProjOrtho, matSliceZView, matSliceZRot, view.param.SLICE_DEPTH.z * view.param.BOX_HEIGHT);
                oblique.RenderOblique(gl, matProjOrtho, matSliceYView, matSliceYRot, view.param.SLICE_DEPTH.y);
                oblique.RenderOblique(gl, matProjOrtho, matSliceXView, matSliceXRot, view.param.SLICE_DEPTH.x);

                if (view.param.SHOW_BOX)
                {
                    box.RenderOutline(gl, matProjOrtho, matSliceZView, view.param.SLICE_LINE_COLOR_Z);
                    box.RenderOutline(gl, matProjOrtho, matSliceYView, view.param.SLICE_LINE_COLOR_Y);
                    box.RenderOutline(gl, matProjOrtho, matSliceXView, view.param.SLICE_LINE_COLOR_X);

                    oblique.RenderDepthLine(gl, matProjOrtho, matSliceZView, view.param.SLICE_LINE_COLOR_X, I3DAxisDirection.X, view.param.SLICE_DEPTH.x);
                    oblique.RenderDepthLine(gl, matProjOrtho, matSliceZView, view.param.SLICE_LINE_COLOR_Y, I3DAxisDirection.Y, view.param.SLICE_DEPTH.y);
                    oblique.RenderDepthLine(gl, matProjOrtho, matSliceXView, view.param.SLICE_LINE_COLOR_Z, I3DAxisDirection.Z1, view.param.SLICE_DEPTH.z * view.param.BOX_HEIGHT);
                    oblique.RenderDepthLine(gl, matProjOrtho, matSliceYView, view.param.SLICE_LINE_COLOR_Z, I3DAxisDirection.Z2, view.param.SLICE_DEPTH.z * view.param.BOX_HEIGHT);
                }

                // grid-text
                if (view.param.SHOW_GRID_TEXT)
                {
                    box.RenderSliceTextX(gl, matProjOrtho, matSliceXView);
                    box.RenderSliceTextY(gl, matProjOrtho, matSliceYView);
                    box.RenderSliceTextZ(gl, matProjOrtho, matSliceYView);
                }
            }
            else
            {
                // draw box outline
                if (view.param.SHOW_GRID)
                    box.RenderGrid(gl, matProj, matGridView, matGridObj);

                if (view.param.SHOW_BOX)
                    box.RenderOutline(gl, matProj, matGridView, view.param.BOX_COLOR);

                // 3d-volume rendering
                if (view.param.RENDER_MODE == I3DRenderMode.BLEND || view.param.RENDER_MODE == I3DRenderMode.ADDED)
                    box.RenderVolume3D(gl, matProj, matModelView);
                else if (view.param.RENDER_MODE == I3DRenderMode.OBLIQUE)
                    oblique.RenderOblique(gl, matProj, matModelView, matModelRot, view.param.OBLIQUE_DEPTH);
                
                // grid-text
                if (view.param.SHOW_GRID_TEXT)
                    box.RenderGridText(gl, matProj, matGridView);

                // draw axis
                if (view.param.SHOW_AXIS)
                    axis.Render(gl, matAxisView);
            }

            if (!firstRenderd && firstRenderFunc != null)
            {
                firstRenderFunc();
                firstRenderd = true;
            }
        }
    }
}
