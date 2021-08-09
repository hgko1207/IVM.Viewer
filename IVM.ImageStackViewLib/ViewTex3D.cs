using SharpGL;
using SharpGL.Textures;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ivm
{
    public class ViewTex3D
    {
        ImageStackView view;
        OpenGL ogl;

        List<Texture3D> textures;
        string imagePath = "";
        
        int currentTexIdx = 0;
        DateTime lastTick = DateTime.Now;

        public ViewTex3D(ImageStackView v)
        {
            view = v;

            textures = new List<Texture3D>();
        }

        private Texture3D LoadTexture(OpenGL gl, string imgPath)
        {
            Texture3D tex = new Texture3D();
            tex.Create(gl);
            tex.Bind(gl);
            tex.SetParameter(gl, OpenGL.GL_TEXTURE_MIN_FILTER, OpenGL.GL_LINEAR);
            tex.SetParameter(gl, OpenGL.GL_TEXTURE_MAG_FILTER, OpenGL.GL_LINEAR);
            tex.SetParameter(gl, OpenGL.GL_TEXTURE_WRAP_S, OpenGL.GL_CLAMP_TO_EDGE);
            tex.SetParameter(gl, OpenGL.GL_TEXTURE_WRAP_T, OpenGL.GL_CLAMP_TO_EDGE);
            tex.SetParameter(gl, OpenGL.GL_TEXTURE_WRAP_R, OpenGL.GL_CLAMP_TO_EDGE);
            tex.SetImage(gl, imgPath);
            tex.Unbind(gl);

            return tex;
        }

        private bool IsValidTexturePath(string imgPath)
        {
            string[] files = Directory.GetFiles(imgPath).OrderBy(f => f).ToArray();
            foreach (string imgpath in files)
            {
                string ext = Path.GetExtension(imgpath).ToLower();
                if (ViewConst.IN_IMG_EXTS.Contains(ext))
                    return true;
            }

            return false;
        }

        private void Init()
        {
            foreach (Texture3D tex in textures)
                tex.Delete(ogl);

            textures.Clear();
        }

        public bool Load(OpenGL gl, string imgPath)
        {
            ogl = gl;

            if (!Directory.Exists(imgPath))
                return false;

            Init();

            // try load 4D
            if (!IsValidTexturePath(imgPath))
            {
                string[] dirs = Directory.GetDirectories(imgPath).OrderBy(f => f).ToArray();
                foreach (string dir in dirs)
                {
                    if (IsValidTexturePath(dir))
                    {
                        imagePath = imgPath;

                        Texture3D tex = LoadTexture(gl, dir);
                        textures.Add(tex);
                    }
                }
            }
            else
            {
                imagePath = imgPath;

                Texture3D tex = LoadTexture(gl, imgPath);
                textures.Add(tex);
            }

            return true;
        }

        public void Bind(OpenGL gl)
        {
            textures[currentTexIdx].Bind(gl);
        }

        public void Unbind(OpenGL gl)
        {
            textures[currentTexIdx].Unbind(gl);

            if ((DateTime.Now - lastTick).TotalMilliseconds > view.param.TIMELAPSE_TEXTURE_DELAY)
            {
                lastTick = DateTime.Now;

                currentTexIdx++;
                if (currentTexIdx >= textures.Count)
                    currentTexIdx = 0;
            }
        }

        public uint GetWidth()
        {
            return textures[0].Width;
        }

        public uint GetHeight()
        {
            return textures[0].Height;
        }

        public uint GetDepth()
        {
            return textures[0].Depth;
        }

        public string GetImagePath()
        {
            return imagePath;
        }

    }
}
