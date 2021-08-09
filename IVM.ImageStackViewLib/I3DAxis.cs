using SharpGL;
using SharpGL.Enumerations;
using GlmNet;

namespace IVM.Studio.I3D
{
    public class I3DAxis
    {
        ImageStackView view;

        vec3[] vertices;
        const int vertCount = 6;

        public I3DAxis(ImageStackView v)
        {
            view = v;
        }

        public void CreateMesh(OpenGL gl)
        {
            vertices = new vec3[vertCount];

            //------------------------- X -------------------------
            vertices[0].x = 0.0f; // front
            vertices[0].y = 0.0f;
            vertices[0].z = 0;
            vertices[1].x = view.param.AXIS_HEIGHT; // back
            vertices[1].y = 0.0f;
            vertices[1].z = 0;

            //------------------------- Y -------------------------
            vertices[2].x = 0.0f; // front
            vertices[2].y = 0.0f;
            vertices[2].z = 0;
            vertices[3].x = 0.0f; // back
            vertices[3].y = view.param.AXIS_HEIGHT;
            vertices[3].z = 0;

            //------------------------- Z -------------------------
            vertices[4].x = 0.0f; // front
            vertices[4].y = 0.0f;
            vertices[4].z = 0;
            vertices[5].x = 0.0f; // back
            vertices[5].y = 0.0f;
            vertices[5].z = view.param.AXIS_HEIGHT;
        }

        public void Render(OpenGL gl, mat4 mview)
        {
            gl.MatrixMode(OpenGL.GL_PROJECTION);
            gl.LoadIdentity();
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.LoadIdentity();
            gl.MultMatrix(mview.to_array());

            gl.PushAttrib(OpenGL.GL_CURRENT_BIT | OpenGL.GL_ENABLE_BIT |
                OpenGL.GL_LINE_BIT | OpenGL.GL_POLYGON_BIT | OpenGL.GL_POLYGON_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
            gl.Disable(OpenGL.GL_LIGHTING);
            gl.Disable(OpenGL.GL_TEXTURE_2D);
            gl.DepthFunc(OpenGL.GL_ALWAYS);
            gl.LineWidth(2.0f);

            gl.PolygonMode(FaceMode.FrontAndBack, PolygonMode.Lines);
            gl.Begin(BeginMode.Lines);

            //------------------------- X -------------------------
            gl.Color(1.0f, 0.0f, 0.0f, 0.5f);
            gl.Vertex(vertices[0].x, vertices[0].y, vertices[0].z);
            gl.Vertex(vertices[1].x, vertices[1].y, vertices[1].z);

            //------------------------- Y -------------------------
            gl.Color(0.0f, 1.0f, 0.0f, 0.5f);
            gl.Vertex(vertices[2].x, vertices[2].y, vertices[2].z);
            gl.Vertex(vertices[3].x, vertices[3].y, vertices[3].z);

            //------------------------- Z -------------------------
            gl.Color(0.0f, 0.0f, 1.0f, 0.5f);
            gl.Vertex(vertices[4].x, vertices[4].y, vertices[4].z);
            gl.Vertex(vertices[5].x, vertices[5].y, vertices[5].z);

            gl.End();            
            gl.PopAttrib();

            //---------------------------------------------------------------------------
            // render text
            float aw = (float)view.ActualWidth;
            float ah = (float)view.ActualHeight;
            int mg = 4;
            int fs = view.param.TEXT_SIZE;

            vec4 px = mview * new vec4(vertices[1].x, vertices[1].y, vertices[1].z, 1);
            px.x = (px.x + 1.0f) / 2.0f * aw;
            px.y = (px.y + 1.0f) / 2.0f * ah;
            gl.DrawText((int)px.x + mg, (int)px.y, 1.0f, 0.0f, 0.0f, "Courier New", fs, "X");

            vec4 py = mview * new vec4(vertices[3].x, vertices[3].y, vertices[3].z, 1);
            py.x = (py.x + 1.0f) / 2.0f * aw;
            py.y = (py.y + 1.0f) / 2.0f * ah;
            gl.DrawText((int)py.x + mg, (int)py.y, 0.0f, 1.0f, 0.0f, "Courier New", fs, "Y");

            vec4 pz = mview * new vec4(vertices[5].x, vertices[5].y, vertices[5].z, 1);
            pz.x = (pz.x + 1.0f) / 2.0f * aw;
            pz.y = (pz.y + 1.0f) / 2.0f * ah;
            gl.DrawText((int)pz.x + mg, (int)pz.y, 0.0f, 0.0f, 1.0f, "Courier New", fs, "Z");
        }
    }
}
