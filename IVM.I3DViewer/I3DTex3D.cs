using SharpGL;
using SharpGL.Textures;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace IVM.Studio.I3D
{
    public class I3DTex3D
    {
        I3DViewer view;

        List<Texture3D> textures;
        string imagePath = "";

        int currentTexIdx = 0;
        bool loading = false;
        public bool Loading
        {
            get => loading;
        }

        DateTime lastTick = DateTime.Now;

        public I3DTex3D(I3DViewer v)
        {
            view = v;

            textures = new List<Texture3D>();
        }

        private async Task<Texture3D> LoadTexture(OpenGL gl, string imgPath, int lower, int upper, bool reverse)
        {
            Texture3D tex = new Texture3D();
            Bitmap3D b = await Task.Run(() => tex.LoadBitmapFromDisk(imgPath, lower, upper, reverse));

            if (b == null)
                return null;

            tex.Create(gl);
            tex.Bind(gl);
            tex.SetParameter(gl, OpenGL.GL_TEXTURE_MIN_FILTER, OpenGL.GL_LINEAR);
            tex.SetParameter(gl, OpenGL.GL_TEXTURE_MAG_FILTER, OpenGL.GL_LINEAR);
            tex.SetParameter(gl, OpenGL.GL_TEXTURE_WRAP_S, OpenGL.GL_CLAMP_TO_EDGE);
            tex.SetParameter(gl, OpenGL.GL_TEXTURE_WRAP_T, OpenGL.GL_CLAMP_TO_EDGE);
            tex.SetParameter(gl, OpenGL.GL_TEXTURE_WRAP_R, OpenGL.GL_CLAMP_TO_EDGE);
            tex.SetImage(gl, b);
            tex.Unbind(gl);

            return tex;
        }

        private bool IsValidTexturePath(string imgPath)
        {
            string[] files = Directory.GetFiles(imgPath).OrderBy(f => f).ToArray();
            foreach (string imgpath in files)
            {
                string ext = Path.GetExtension(imgpath).ToLower();
                if (I3DConst.IN_IMG_EXTS.Contains(ext))
                    return true;
            }

            return false;
        }

        private void Init()
        {
            imagePath = "";

            currentTexIdx = 0;

            foreach (Texture3D tex in textures)
                tex.Delete(view.gl);

            textures.Clear();
        }

        public async Task<bool> Load(OpenGL gl, string imgPath, int lower, int upper, bool reverse)
        {
            if (!Directory.Exists(imgPath))
                return false;

            loading = true;

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

                        Texture3D tex = await LoadTexture(gl, dir, lower, upper, reverse);
                        if (tex != null)
                            textures.Add(tex);
                    }
                }
            }
            else
            {
                imagePath = imgPath;

                Texture3D tex = await LoadTexture(gl, imgPath, lower, upper, reverse);
                if (tex != null)
                    textures.Add(tex);
            }

            loading = false;

            bool loaded = (textures.Count > 0);

            return loaded;
        }

        public void Bind(OpenGL gl)
        {
            if (loading)
                return;

            if (textures.Count <= 0)
                return;

            textures[currentTexIdx].Bind(gl);
        }

        public void Unbind(OpenGL gl)
        {
            if (loading)
                return;

            if (textures.Count <= 0)
                return;

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
            if (loading)
                return 0;

            if (textures.Count <= 0)
                return 0;

            return textures[0].Width;
        }

        public uint GetHeight()
        {
            if (loading)
                return 0;

            if (textures.Count <= 0)
                return 0;

            return textures[0].Height;
        }

        public uint GetDepth()
        {
            if (loading)
                return 0;

            if (textures.Count <= 0)
                return 0;

            return textures[0].Depth;
        }

        public string GetImagePath()
        {
            if (loading)
                return "";

            return imagePath;
        }

    }
}
