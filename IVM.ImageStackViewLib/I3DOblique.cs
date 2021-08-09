using SharpGL;
using SharpGL.Shaders;
using SharpGL.Enumerations;
using GlmNet;
using SharpGL.VertexBuffers;
using System.Linq;
using System;

namespace ivm
{
    public class I3DOblique
    {
        ImageStackView view = null;

        vec3[] vertices = null;
        vec3[] uvs = null;
        int vertCount = 0;

        // grid
        vec3[] vertdepth = null;

        VertexBufferArray vertArray = null;
        VertexBuffer vertexBuffer = null;
        VertexBuffer uvsBuffer = null;

        // oblique shader
        ShaderProgram shader;

        public I3DOblique(ImageStackView v)
        {
            view = v;

            vertices = new vec3[6]; // maximum 6
            uvs = new vec3[6];
            vertdepth = new vec3[2];
        }

        public void InitShader(OpenGL gl)
        {
            string vert = @"Shaders\Oblique.vert";
            string frag = @"Shaders\Oblique.frag";

            // Create the per pixel shader.
            shader = new ShaderProgram();
            shader.Create(gl,
                ManifestResourceLoader.LoadTextFile(vert),
                ManifestResourceLoader.LoadTextFile(frag), null);
            shader.BindAttributeLocation(gl, I3DVertexAttributes2.Position, "vPosition");
            shader.BindAttributeLocation(gl, I3DVertexAttributes2.TexCoord, "vTexCoord");
        }

        public void UpdateMesh(OpenGL gl, mat4 mrot, float depth)
        {
            vec4 pdir = new vec4(0, 0, 1, depth);
            mat4 irot = glm.inverse(mrot);
            pdir = irot * pdir;

            I3DPlane pln = new I3DPlane(pdir.x, pdir.y, pdir.z, pdir.w);

            vec3 aabb_min = new vec3(-1.0f, -1.0f, -view.param.BOX_HEIGHT);
            vec3 aabb_max = new vec3(1.0f, 1.0f, view.param.BOX_HEIGHT);

            //vec3 aabb_min = new vec3(-1.0f, -1.0f, -1.0f);
            //vec3 aabb_max = new vec3(1.0f, 1.0f, 1.0f);

            vertices[0] = new vec3(0, 0, 0);
            vertices[1] = new vec3(0, 0, 0);
            vertices[2] = new vec3(0, 0, 0);
            vertices[3] = new vec3(0, 0, 0);
            vertices[4] = new vec3(0, 0, 0);
            vertices[5] = new vec3(0, 0, 0);

            I3DCommon.calc_plane_aabb_intersection_points(pln, aabb_min, aabb_max, ref vertices, ref vertCount);

            for (int i = 0; i < vertCount; ++i)
            {
                uvs[i].x = vertices[i].x * 0.5f + 0.5f;
                uvs[i].y = vertices[i].y * 0.5f + 0.5f;
                uvs[i].z = -(vertices[i].z / view.param.BOX_HEIGHT) * 0.5f + 0.5f;
            }

            // fill vertexbuffer
            if (vertArray == null)
            {
                vertArray = new VertexBufferArray();
                vertArray.Create(gl);

                vertexBuffer = new VertexBuffer();
                vertexBuffer.Create(gl);

                uvsBuffer = new VertexBuffer();
                uvsBuffer.Create(gl);
            }
            vertArray.Bind(gl);

            // Create the vertex data buffer.
            vertexBuffer.Bind(gl);
            vertexBuffer.SetData(gl, I3DVertexAttributes2.Position, vertices.SelectMany(v => v.to_array()).ToArray(), false, 3);

            uvsBuffer.Bind(gl);
            uvsBuffer.SetData(gl, I3DVertexAttributes2.TexCoord, uvs.SelectMany(v => v.to_array()).ToArray(), false, 3);

            vertArray.Unbind(gl);
        }

        public void RenderOblique(OpenGL gl, mat4 mproj, mat4 mview, mat4 mrot, float depth)
        {
            // update geometry. aabb vs plane intersection.
            UpdateMesh(gl, mrot, depth);

            // Use the shader program.
            shader.Bind(gl);

            // Set the matrices.
            shader.SetUniformMatrix4(gl, "matProj", mproj.to_array());
            shader.SetUniformMatrix4(gl, "matModelView", mview.to_array());
            shader.SetUniform3(gl, "BG_COLOR", view.param.BG_COLOR.x, view.param.BG_COLOR.y, view.param.BG_COLOR.z);

            view.scene.tex3D.Bind(gl);

            vertArray.Bind(gl);
            gl.DrawArrays(OpenGL.GL_TRIANGLE_FAN, 0, vertCount);

            view.scene.tex3D.Unbind(gl);

            // Unbind the shader.
            shader.Unbind(gl);
        }

        public void RenderDepthLine(OpenGL gl, mat4 mproj, mat4 mview, vec4 lcol, uint axis, float depth)
        {
            if (axis == I3DAxisDirection.Z1)
            {
                vertdepth[0] = new vec3(-1.0f, -1.0f, -depth);
                vertdepth[1] = new vec3( 1.0f, -1.0f, -depth);
            }
            else if (axis == I3DAxisDirection.Z2)
            {
                vertdepth[0] = new vec3( 1.0f, -1.0f, -depth);
                vertdepth[1] = new vec3( 1.0f,  1.0f, -depth);
            }
            else if (axis == I3DAxisDirection.X)
            {
                vertdepth[0] = new vec3(-1.0f, depth, view.param.BOX_HEIGHT);
                vertdepth[1] = new vec3( 1.0f, depth, view.param.BOX_HEIGHT);
            }
            else if (axis == I3DAxisDirection.Y)
            {
                vertdepth[0] = new vec3(depth, -1.0f, view.param.BOX_HEIGHT);
                vertdepth[1] = new vec3(depth,  1.0f, view.param.BOX_HEIGHT);
            }

            gl.MatrixMode(OpenGL.GL_PROJECTION);
            gl.LoadIdentity();
            gl.MultMatrix(mproj.to_array());

            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.LoadIdentity();
            gl.MultMatrix(mview.to_array());

            gl.PushAttrib(OpenGL.GL_CURRENT_BIT | OpenGL.GL_ENABLE_BIT |
                OpenGL.GL_LINE_BIT | OpenGL.GL_POLYGON_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
            gl.Disable(OpenGL.GL_LIGHTING);
            gl.Disable(OpenGL.GL_TEXTURE_2D);
            gl.LineWidth(2.0f);
            gl.DepthFunc(OpenGL.GL_ALWAYS);
            gl.Color(lcol.x, lcol.y, lcol.z, lcol.w);

            gl.PolygonMode(FaceMode.FrontAndBack, PolygonMode.Lines);

            gl.Begin(BeginMode.Lines);
            for (int index = 0; index < vertdepth.Length; ++index)
                gl.Vertex(vertdepth[index].x, vertdepth[index].y, vertdepth[index].z);
            gl.End();

            gl.PopAttrib();
        }
    }
}
