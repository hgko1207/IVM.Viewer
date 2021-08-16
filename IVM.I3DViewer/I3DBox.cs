using System;
using System.Linq;
using System.Collections.Generic;
using SharpGL;
using SharpGL.Enumerations;
using SharpGL.VertexBuffers;
using SharpGL.Shaders;
using GlmNet;

namespace IVM.Studio.I3D
{
    public class I3DBox
    {
        I3DViewer view = null;

        vec3[] vertices = null;
        vec3[] normals = null;
        vec3[] uvs = null;
        VertexBufferArray vertArray = null;
        const int vertCount = 24;

        // grid
        vec3[] vertgrid = null;
        vec3[] normgrid = null;

        // volume shader
        ShaderProgram shader;

        public I3DBox(I3DViewer v)
        {
            view = v;
        }

        public void InitShader(OpenGL gl)
        {
            string vert = @"Shaders\Volume.vert";
            string frag = @"Shaders\Volume.frag";

            // Create the per pixel shader.
            shader = new ShaderProgram();
            shader.Create(gl,
                ManifestResourceLoader.LoadTextFile(vert),
                ManifestResourceLoader.LoadTextFile(frag), null);
            shader.BindAttributeLocation(gl, I3DVertexAttributes1.Position, "vPosition");
            shader.BindAttributeLocation(gl, I3DVertexAttributes1.Normal, "vNormal");
            shader.BindAttributeLocation(gl, I3DVertexAttributes1.TexCoord, "vTexCoord");
        }

        public void CreateMesh(OpenGL gl)
        {
            int i = 0;
            vertices = new vec3[vertCount];
            //------------------------- TOP -------------------------
            vertices[i].x = -1.0f; // left-bottom
            vertices[i].y = -1.0f;
            vertices[i++].z = view.param.BOX_HEIGHT;
            vertices[i].x = -1.0f; // left-top
            vertices[i].y = 1.0f;
            vertices[i++].z = view.param.BOX_HEIGHT;
            vertices[i].x = 1.0f; // right-top
            vertices[i].y = 1.0f;
            vertices[i++].z = view.param.BOX_HEIGHT;
            vertices[i].x = 1.0f; // right-bottom
            vertices[i].y = -1.0f;
            vertices[i++].z = view.param.BOX_HEIGHT;
            //------------------------- BOTTOM -------------------------
            vertices[i].x = -1.0f; // left-bottom
            vertices[i].y = -1.0f;
            vertices[i++].z = -view.param.BOX_HEIGHT;
            vertices[i].x = -1.0f; // left-top
            vertices[i].y = 1.0f;
            vertices[i++].z = -view.param.BOX_HEIGHT;
            vertices[i].x = 1.0f; // right-top
            vertices[i].y = 1.0f;
            vertices[i++].z = -view.param.BOX_HEIGHT;
            vertices[i].x = 1.0f; // right-bottom
            vertices[i].y = -1.0f;
            vertices[i++].z = -view.param.BOX_HEIGHT;
            //------------------------- FRONT -------------------------
            vertices[i  ].x = -1.0f; // left-bottom
            vertices[i  ].y = -1.0f;
            vertices[i++].z = -view.param.BOX_HEIGHT;
            vertices[i  ].x = -1.0f; // left-top
            vertices[i  ].y = -1.0f;
            vertices[i++].z = view.param.BOX_HEIGHT;
            vertices[i  ].x = 1.0f; // right-top
            vertices[i  ].y = -1.0f;
            vertices[i++].z = view.param.BOX_HEIGHT;
            vertices[i  ].x = 1.0f; // right-bottom
            vertices[i  ].y = -1.0f;
            vertices[i++].z = -view.param.BOX_HEIGHT;
            //------------------------- BACK -------------------------
            vertices[i].x = -1.0f; // left-bottom
            vertices[i].y = 1.0f;
            vertices[i++].z = -view.param.BOX_HEIGHT;
            vertices[i].x = -1.0f; // left-top
            vertices[i].y = 1.0f;
            vertices[i++].z = view.param.BOX_HEIGHT;
            vertices[i].x = 1.0f; // right-top
            vertices[i].y = 1.0f;
            vertices[i++].z = view.param.BOX_HEIGHT;
            vertices[i].x = 1.0f; // right-bottom
            vertices[i].y = 1.0f;
            vertices[i++].z = -view.param.BOX_HEIGHT;
            //------------------------- LEFT -------------------------
            vertices[i].x = -1.0f; // left-bottom
            vertices[i].y = 1.0f;
            vertices[i++].z = -view.param.BOX_HEIGHT;
            vertices[i].x = -1.0f; // left-top
            vertices[i].y = 1.0f;
            vertices[i++].z = view.param.BOX_HEIGHT;
            vertices[i].x = -1.0f; // right-top
            vertices[i].y = -1.0f;
            vertices[i++].z = view.param.BOX_HEIGHT;
            vertices[i].x = -1.0f; // right-bottom
            vertices[i].y = -1.0f;
            vertices[i++].z = -view.param.BOX_HEIGHT;
            //------------------------- RIGHT -------------------------
            vertices[i].x = 1.0f; // left-bottom
            vertices[i].y = 1.0f;
            vertices[i++].z = -view.param.BOX_HEIGHT;
            vertices[i].x = 1.0f; // left-top
            vertices[i].y = 1.0f;
            vertices[i++].z = view.param.BOX_HEIGHT;
            vertices[i].x = 1.0f; // right-top
            vertices[i].y = -1.0f;
            vertices[i++].z = view.param.BOX_HEIGHT;
            vertices[i].x = 1.0f; // right-bottom
            vertices[i].y = -1.0f;
            vertices[i++].z = -view.param.BOX_HEIGHT;

            i = 0;
            uvs = new vec3[vertCount];
            //------------------------- TOP -------------------------
            uvs[i].x = 0.0f; // u left-bottom
            uvs[i].y = 0.0f; // v
            uvs[i++].z = 0.0f;
            uvs[i].x = 0.0f; // u left-top
            uvs[i].y = 1.0f; // v
            uvs[i++].z = 0.0f;
            uvs[i].x = 1.0f; // u right-top
            uvs[i].y = 1.0f; // v
            uvs[i++].z = 0.0f;
            uvs[i].x = 1.0f; // u right-bottom
            uvs[i].y = 0.0f; // v
            uvs[i++].z = 0.0f;
            //------------------------- BOTTOM -------------------------
            uvs[i].x = 0.0f; // u left-bottom
            uvs[i].y = 0.0f; // v
            uvs[i++].z = 1.0f;
            uvs[i].x = 0.0f; // u left-top
            uvs[i].y = 1.0f; // v
            uvs[i++].z = 1.0f;
            uvs[i].x = 1.0f; // u right-top
            uvs[i].y = 1.0f; // v
            uvs[i++].z = 1.0f;
            uvs[i].x = 1.0f; // u right-bottom
            uvs[i].y = 0.0f; // v
            uvs[i++].z = 1.0f;
            //------------------------- FRONT -------------------------
            uvs[i  ].x = 0.0f; // u left-bottom
            uvs[i  ].y = 0.0f; // v
            uvs[i++].z = 1.0f;
            uvs[i  ].x = 0.0f; // u left-top
            uvs[i  ].y = 0.0f; // v
            uvs[i++].z = 0.0f;
            uvs[i  ].x = 1.0f; // u right-top
            uvs[i  ].y = 0.0f; // v
            uvs[i++].z = 0.0f;
            uvs[i  ].x = 1.0f; // u right-bottom
            uvs[i  ].y = 0.0f; // v
            uvs[i++].z = 1.0f;
            //------------------------- BACK -------------------------
            uvs[i].x = 0.0f; // u left-bottom
            uvs[i].y = 1.0f; // v
            uvs[i++].z = 1.0f;
            uvs[i].x = 0.0f; // u left-top
            uvs[i].y = 1.0f; // v
            uvs[i++].z = 0.0f;
            uvs[i].x = 1.0f; // u right-top
            uvs[i].y = 1.0f; // v
            uvs[i++].z = 0.0f;
            uvs[i].x = 1.0f; // u right-bottom
            uvs[i].y = 1.0f; // v
            uvs[i++].z = 1.0f;
            //------------------------- LEFT -------------------------
            uvs[i].x = 0.0f; // u left-bottom
            uvs[i].y = 1.0f; // v
            uvs[i++].z = 1.0f;
            uvs[i].x = 0.0f; // u left-top
            uvs[i].y = 1.0f; // v
            uvs[i++].z = 0.0f;
            uvs[i].x = 0.0f; // u right-top
            uvs[i].y = 0.0f; // v
            uvs[i++].z = 0.0f;
            uvs[i].x = 0.0f; // u right-bottom
            uvs[i].y = 0.0f; // v
            uvs[i++].z = 1.0f;
            //------------------------- RIGHT -------------------------
            uvs[i].x = 1.0f; // u left-bottom
            uvs[i].y = 1.0f; // v
            uvs[i++].z = 1.0f;
            uvs[i].x = 1.0f; // u left-top
            uvs[i].y = 1.0f; // v
            uvs[i++].z = 0.0f;
            uvs[i].x = 1.0f; // u right-top
            uvs[i].y = 0.0f; // v
            uvs[i++].z = 0.0f;
            uvs[i].x = 1.0f; // u right-bottom
            uvs[i].y = 0.0f; // v
            uvs[i++].z = 1.0f;

            i = 0;
            normals = new vec3[vertCount];
            //------------------------- TOP -------------------------
            normals[i].x = 0.0f; // left-bottom
            normals[i].y = 0.0f; // 
            normals[i++].z = 1.0f;
            normals[i].x = 0.0f; // left-top
            normals[i].y = 0.0f; // 
            normals[i++].z = 1.0f;
            normals[i].x = 0.0f; // right-top
            normals[i].y = 0.0f; // 
            normals[i++].z = 1.0f;
            normals[i].x = 0.0f; // right-bottom
            normals[i].y = 0.0f; // 
            normals[i++].z = 1.0f;
            //------------------------- BOTTOM -------------------------
            normals[i].x = 0.0f; // left-bottom
            normals[i].y = 0.0f; // 
            normals[i++].z = -1.0f;
            normals[i].x = 0.0f; // left-top
            normals[i].y = 0.0f; // 
            normals[i++].z = -1.0f;
            normals[i].x = 0.0f; // right-top
            normals[i].y = 0.0f; // 
            normals[i++].z = -1.0f;
            normals[i].x = 0.0f; // right-bottom
            normals[i].y = 0.0f; // 
            normals[i++].z = -1.0f;
            //------------------------- FRONT -------------------------
            normals[i].x = 0.0f; // left-bottom
            normals[i].y = -1.0f; // 
            normals[i++].z = 0.0f;
            normals[i].x = 0.0f; // left-top
            normals[i].y = -1.0f; // 
            normals[i++].z = 0.0f;
            normals[i].x = 0.0f; // right-top
            normals[i].y = -1.0f; // 
            normals[i++].z = 0.0f;
            normals[i].x = 0.0f; // right-bottom
            normals[i].y = -1.0f; // 
            normals[i++].z = 0.0f;
            //------------------------- BACK -------------------------
            normals[i].x = 0.0f; // left-bottom
            normals[i].y = 1.0f; // 
            normals[i++].z = 0.0f;
            normals[i].x = 0.0f; // left-top
            normals[i].y = 1.0f; // 
            normals[i++].z = 0.0f;
            normals[i].x = 0.0f; // right-top
            normals[i].y = 1.0f; // 
            normals[i++].z = 0.0f;
            normals[i].x = 0.0f; // right-bottom
            normals[i].y = 1.0f; // 
            normals[i++].z = 0.0f;
            //------------------------- LEFT -------------------------
            normals[i].x = -1.0f; // left-bottom
            normals[i].y = 0.0f; // 
            normals[i++].z = 0.0f;
            normals[i].x = -1.0f; // left-top
            normals[i].y = 0.0f; // 
            normals[i++].z = 0.0f;
            normals[i].x = -1.0f; // right-top
            normals[i].y = 0.0f; // 
            normals[i++].z = 0.0f;
            normals[i].x = -1.0f; // right-bottom
            normals[i].y = 0.0f; // 
            normals[i++].z = 0.0f;
            //------------------------- RIGHT -------------------------
            normals[i].x = 1.0f; // left-bottom
            normals[i].y = 0.0f; // 
            normals[i++].z = 0.0f;
            normals[i].x = 1.0f; // left-top
            normals[i].y = 0.0f; // 
            normals[i++].z = 0.0f;
            normals[i].x = 1.0f; // right-top
            normals[i].y = 0.0f; // 
            normals[i++].z = 0.0f;
            normals[i].x = 1.0f; // right-bottom
            normals[i].y = 0.0f; // 
            normals[i++].z = 0.0f;

            // fill vertexbuffer
            vertArray = new VertexBufferArray();
            vertArray.Create(gl);
            vertArray.Bind(gl);

            //  Create the vertex data buffer.
            var vertexBuffer = new VertexBuffer();
            vertexBuffer.Create(gl);
            vertexBuffer.Bind(gl);
            vertexBuffer.SetData(gl, I3DVertexAttributes1.Position, vertices.SelectMany(v => v.to_array()).ToArray(), false, 3);

            var normalsBuffer = new VertexBuffer();
            normalsBuffer.Create(gl);
            normalsBuffer.Bind(gl);
            normalsBuffer.SetData(gl, I3DVertexAttributes1.Normal, normals.SelectMany(v => v.to_array()).ToArray(), false, 3);

            var uvsBuffer = new VertexBuffer();
            uvsBuffer.Create(gl);
            uvsBuffer.Bind(gl);
            uvsBuffer.SetData(gl, I3DVertexAttributes1.TexCoord, uvs.SelectMany(v => v.to_array()).ToArray(), false, 3);

            vertArray.Unbind(gl);
        }

        public void CreateGrid(OpenGL gl)
        {
            if (!view.scene.IsLoaded())
                return;

            float dw = (float)view.param.GRID_DIST / (float)view.scene.tex3D.GetWidth();
            float dh = (float)view.param.GRID_DIST / (float)view.scene.tex3D.GetDepth();
            List<vec3> vertlst = new List<vec3>();
            List<vec3> normlst = new List<vec3>();

            //------------------------- TOP -> BOTTOM -------------------------
            for (float i = dh; i <= 1.0f; i += dh)
            {
                vertlst.Add(new vec3(-1.0f, -1.0f, -1.0f * (i * 2.0f - 1.0f) * view.param.BOX_HEIGHT));
                vertlst.Add(new vec3(-1.0f,  1.0f, -1.0f * (i * 2.0f - 1.0f) * view.param.BOX_HEIGHT));

                vertlst.Add(new vec3(-1.0f,  1.0f, -1.0f * (i * 2.0f - 1.0f) * view.param.BOX_HEIGHT));
                vertlst.Add(new vec3( 1.0f,  1.0f, -1.0f * (i * 2.0f - 1.0f) * view.param.BOX_HEIGHT));
                                             
                vertlst.Add(new vec3( 1.0f,  1.0f, -1.0f * (i * 2.0f - 1.0f) * view.param.BOX_HEIGHT));
                vertlst.Add(new vec3( 1.0f, -1.0f, -1.0f * (i * 2.0f - 1.0f) * view.param.BOX_HEIGHT));

                vertlst.Add(new vec3( 1.0f, -1.0f, -1.0f * (i * 2.0f - 1.0f) * view.param.BOX_HEIGHT));
                vertlst.Add(new vec3(-1.0f, -1.0f, -1.0f * (i * 2.0f - 1.0f) * view.param.BOX_HEIGHT));

                normlst.Add(new vec3(-1.0f, 0.0f, 0.0f));
                normlst.Add(new vec3(-1.0f, 0.0f, 0.0f));

                normlst.Add(new vec3( 0.0f, 1.0f, 0.0f));
                normlst.Add(new vec3( 0.0f, 1.0f, 0.0f));

                normlst.Add(new vec3( 1.0f, 0.0f, 0.0f));
                normlst.Add(new vec3( 1.0f, 0.0f, 0.0f));

                normlst.Add(new vec3( 0.0f,-1.0f, 0.0f));
                normlst.Add(new vec3( 0.0f,-1.0f, 0.0f));
            }
            //------------------------- FRONT -> BACK -------------------------
            for (float i = dw; i <= 1.0f; i += dw)
            {
                vertlst.Add(new vec3(-1.0f, 1.0f * (i * 2.0f - 1.0f), -view.param.BOX_HEIGHT));
                vertlst.Add(new vec3(-1.0f, 1.0f * (i * 2.0f - 1.0f),  view.param.BOX_HEIGHT));
                
                vertlst.Add(new vec3(-1.0f, 1.0f * (i * 2.0f - 1.0f),  view.param.BOX_HEIGHT));
                vertlst.Add(new vec3( 1.0f, 1.0f * (i * 2.0f - 1.0f),  view.param.BOX_HEIGHT));
                
                vertlst.Add(new vec3( 1.0f, 1.0f * (i * 2.0f - 1.0f),  view.param.BOX_HEIGHT));
                vertlst.Add(new vec3( 1.0f, 1.0f * (i * 2.0f - 1.0f), -view.param.BOX_HEIGHT));

                vertlst.Add(new vec3( 1.0f, 1.0f * (i * 2.0f - 1.0f), -view.param.BOX_HEIGHT));
                vertlst.Add(new vec3(-1.0f, 1.0f * (i * 2.0f - 1.0f), -view.param.BOX_HEIGHT));

                normlst.Add(new vec3(-1.0f, 0.0f, 0.0f));
                normlst.Add(new vec3(-1.0f, 0.0f, 0.0f));

                normlst.Add(new vec3( 0.0f, 0.0f, 1.0f));
                normlst.Add(new vec3( 0.0f, 0.0f, 1.0f));
                                      
                normlst.Add(new vec3( 1.0f, 0.0f, 0.0f));
                normlst.Add(new vec3( 1.0f, 0.0f, 0.0f));
                                      
                normlst.Add(new vec3( 0.0f, 0.0f,-1.0f));
                normlst.Add(new vec3( 0.0f, 0.0f,-1.0f));
            }
            //------------------------- LEFT -> RIGHT -------------------------
            for (float i = dw; i <= 1.0f; i += dw)
            {
                vertlst.Add(new vec3( 1.0f * (i * 2.0f - 1.0f),  1.0f, -view.param.BOX_HEIGHT));
                vertlst.Add(new vec3( 1.0f * (i * 2.0f - 1.0f),  1.0f,  view.param.BOX_HEIGHT));

                vertlst.Add(new vec3( 1.0f * (i * 2.0f - 1.0f),  1.0f,  view.param.BOX_HEIGHT));
                vertlst.Add(new vec3( 1.0f * (i * 2.0f - 1.0f), -1.0f,  view.param.BOX_HEIGHT));
                
                vertlst.Add(new vec3( 1.0f * (i * 2.0f - 1.0f), -1.0f,  view.param.BOX_HEIGHT));
                vertlst.Add(new vec3( 1.0f * (i * 2.0f - 1.0f), -1.0f, -view.param.BOX_HEIGHT));
                
                vertlst.Add(new vec3( 1.0f * (i * 2.0f - 1.0f), -1.0f, -view.param.BOX_HEIGHT));
                vertlst.Add(new vec3( 1.0f * (i * 2.0f - 1.0f),  1.0f, -view.param.BOX_HEIGHT));

                normlst.Add(new vec3( 0.0f, 1.0f,  0.0f));
                normlst.Add(new vec3( 0.0f, 1.0f,  0.0f));
            
                normlst.Add(new vec3( 0.0f, 0.0f,  1.0f));
                normlst.Add(new vec3( 0.0f, 0.0f,  1.0f));

                normlst.Add(new vec3( 0.0f,-1.0f,  0.0f));
                normlst.Add(new vec3( 0.0f,-1.0f,  0.0f));

                normlst.Add(new vec3( 0.0f, 0.0f, -1.0f));
                normlst.Add(new vec3( 0.0f, 0.0f, -1.0f));
            }

            vertgrid = new vec3[vertlst.Count];
            for (int i = 0; i < vertlst.Count; ++i)
                vertgrid[i] = vertlst[i];

            normgrid = new vec3[normlst.Count];
            for (int i = 0; i < normlst.Count; ++i)
                normgrid[i] = normlst[i];
        }

        public void RenderOutline(OpenGL gl, mat4 mproj, mat4 mview, vec4 lcol)
        {
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
            gl.LineWidth(1.0f);
            gl.DepthFunc(OpenGL.GL_ALWAYS);
            gl.Color(lcol.x, lcol.y, lcol.z, lcol.w);

            gl.PolygonMode(FaceMode.FrontAndBack, PolygonMode.Lines);

            gl.Begin(BeginMode.Quads);
            for (int index = 0; index < vertices.Length; ++index)
                gl.Vertex(vertices[index].x, vertices[index].y, vertices[index].z);
            gl.End();
            
            gl.PopAttrib();
        }

        public void RenderGrid(OpenGL gl, mat4 mproj, mat4 mview, mat4 mobj)
        {
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
            gl.LineWidth(1.0f);
            gl.DepthFunc(OpenGL.GL_ALWAYS);
            gl.Color(view.param.GRID_COLOR.x, view.param.GRID_COLOR.y, view.param.GRID_COLOR.z, view.param.GRID_COLOR.w);

            gl.PolygonMode(FaceMode.FrontAndBack, PolygonMode.Lines);

            gl.Begin(BeginMode.Lines);
            for (int i = 0; i < vertgrid.Length / 2; ++i)
            {
                // 랜더링 전에 normal과 view vector cross 를 통한 culling 수행
                vec4 v = new vec4(0, 0, 1, 0);
                vec3 n = normgrid[i * 2];
                vec4 o = new vec4(n.x, n.y, n.z, 0);
                o = mobj * o;
                o = glm.normalize(o);

                if (glm.dot(v, o) > 0)
                    continue;

                gl.Vertex(vertgrid[i * 2 + 0].x, vertgrid[i * 2 + 0].y, vertgrid[i * 2 + 0].z);
                gl.Vertex(vertgrid[i * 2 + 1].x, vertgrid[i * 2 + 1].y, vertgrid[i * 2 + 1].z);
            }
            gl.End();

            gl.PopAttrib();
        }

        public void RenderGridText(OpenGL gl, mat4 mproj, mat4 mview)
        {
            float aw = (float)view.ActualWidth;
            float ah = (float)view.ActualHeight;
            int mg = 4;
            int fs = view.param.TEXT_SIZE;

            vec4 pw = mproj * mview * new vec4(0, -1, -view.param.BOX_HEIGHT, 1);
            pw = new vec4(pw.x / pw.w, pw.y / pw.w, pw.z / pw.w, 1);
            pw.x = (pw.x + 1.0f) / 2.0f * aw;
            pw.y = (pw.y + 1.0f) / 2.0f * ah;
            gl.DrawText((int)pw.x, (int)pw.y - mg - fs, 1.0f, 1.0f, 1.0f, "Courier New", fs, String.Format("w: {0} µm", view.scene.meta.umWidth));

            vec4 ph = mproj * mview * new vec4(1, 0, -view.param.BOX_HEIGHT, 1);
            ph = new vec4(ph.x / ph.w, ph.y / ph.w, ph.z / ph.w, 1);
            ph.x = (ph.x + 1.0f) / 2.0f * aw;
            ph.y = (ph.y + 1.0f) / 2.0f * ah;
            gl.DrawText((int)ph.x + mg, (int)ph.y, 1.0f, 1.0f, 1.0f, "Courier New", fs, String.Format("h: {0} µm", view.scene.meta.umHeight));

            vec4 pz = mproj * mview * new vec4(1, -1, 0, 1);
            pz = new vec4(pz.x / pz.w, pz.y / pz.w, pz.z / pz.w, 1);
            pz.x = (pz.x + 1.0f) / 2.0f * aw;
            pz.y = (pz.y + 1.0f) / 2.0f * ah;
            gl.DrawText((int)pz.x + mg, (int)pz.y - fs / 2, 1.0f, 1.0f, 1.0f, "Courier New", fs, String.Format("d: {0} µm", (int)(view.scene.tex3D.GetDepth() * view.scene.meta.pixelPerUM_Z)));
        }

        public void RenderVolume3D(OpenGL gl, mat4 matProj, mat4 matModelView)
        {
            // Use the shader program.
            shader.Bind(gl);

            // Set the matrices.
            mat4 invProj = glm.inverse(matProj);
            mat4 invModelView = glm.inverse(matModelView);

            shader.SetUniformMatrix4(gl, "matProj", matProj.to_array());
            shader.SetUniformMatrix4(gl, "matModelView", matModelView.to_array());
            shader.SetUniformMatrix4(gl, "invProj", invProj.to_array());
            shader.SetUniformMatrix4(gl, "invModelView", invModelView.to_array());

            vec4 tmin = view.param.THRESHOLD_INTENSITY_MIN;
            vec4 tmax = view.param.THRESHOLD_INTENSITY_MAX;
            if (view.param.BAND_VISIBLE.x == 0) { tmin.x = 0; tmax.x = 0; }
            if (view.param.BAND_VISIBLE.y == 0) { tmin.y = 0; tmax.y = 0; }
            if (view.param.BAND_VISIBLE.z == 0) { tmin.z = 0; tmax.z = 0; }
            if (view.param.BAND_VISIBLE.w == 0) { tmin.w = 0; tmax.w = 0; }

            gl.Uniform4(shader.GetUniformLocation(gl, "THRESHOLD_INTENSITY_MIN"), tmin.x, tmin.y, tmin.z, tmin.w); // uniform4
            gl.Uniform4(shader.GetUniformLocation(gl, "THRESHOLD_INTENSITY_MAX"), tmax.x, tmax.y, tmax.z, tmax.w); // uniform4
            gl.Uniform4(shader.GetUniformLocation(gl, "ALPHA_WEIGHT"), view.param.ALPHA_WEIGHT.x, view.param.ALPHA_WEIGHT.y, view.param.ALPHA_WEIGHT.z, view.param.ALPHA_WEIGHT.w); // uniform4
            gl.Uniform4(shader.GetUniformLocation(gl, "BAND_ORDER"), view.param.BAND_ORDER.x, view.param.BAND_ORDER.y, view.param.BAND_ORDER.z, view.param.BAND_ORDER.w); // uniform4
            gl.Uniform4(shader.GetUniformLocation(gl, "BAND_VISIBLE"), view.param.BAND_VISIBLE.x, view.param.BAND_VISIBLE.y, view.param.BAND_VISIBLE.z, view.param.BAND_VISIBLE.w); // uniform4
            shader.SetUniform3(gl, "BG_COLOR", view.param.BG_COLOR.x, view.param.BG_COLOR.y, view.param.BG_COLOR.z);
            shader.SetUniform1(gl, "PER_PIXEl_ITERATION", view.param.PER_PIXEl_ITERATION);
            shader.SetUniform1(gl, "BOX_HEIGHT", view.param.BOX_HEIGHT);
            shader.SetUniform1(gl, "RENDER_MODE", view.param.RENDER_MODE);
            shader.SetUniform1(gl, "IS_COLOCALIZATION", view.param.IS_COLOCALIZATION);

            view.scene.tex3D.Bind(gl);
            
            vertArray.Bind(gl);
            gl.DrawArrays(OpenGL.GL_QUADS, 0, vertCount);

            view.scene.tex3D.Unbind(gl);

            // Unbind the shader.
            shader.Unbind(gl);
        }
    }
}
