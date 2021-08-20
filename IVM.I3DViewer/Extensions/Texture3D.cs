using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpGL.Textures
{
    public class Bitmap3D
    {
        public int width;
        public int height;
        public int depth;
        public Bitmap bitmap;
    }

    /// <summary>
    /// TODO: Move into the core in version 3.0 when we reference Drawing.
    /// </summary>
    public class Texture3D
    {
        public void Create(OpenGL gl)
        {
            //  Generate the texture object array.
            uint[] ids = new uint[1];
            gl.GenTextures(1, ids);
            textureObject = ids[0];
        }

        public void Delete(OpenGL gl)
        {
            gl.DeleteTextures(1, new[] { textureObject });
            textureObject = 0;
        }

        public void Bind(OpenGL gl)
        {
            gl.BindTexture(OpenGL.GL_TEXTURE_3D, textureObject);
        }

        public void Unbind(OpenGL gl)
        {
            gl.BindTexture(OpenGL.GL_TEXTURE_3D, 0);
        }

        public void SetParameter(OpenGL gl, uint parameterName, uint parameterValue)
        {
            gl.TexParameter(OpenGL.GL_TEXTURE_3D, parameterName, parameterValue);
        }

        public async Task<Bitmap3D> LoadBitmapFromDisk(string imgPath, int lower, int upper, bool reverse) // read all images into memory
        {
            string[] files = Directory.GetFiles(imgPath).OrderBy(f => f).ToArray();
            Array.Sort(files);

            if (reverse)
                Array.Reverse(files);

            List<Bitmap> images = new List<Bitmap>();

            Bitmap3D r = new Bitmap3D();

            int idx = 0;

            foreach (string imgpath in files)
            {
                string ext = Path.GetExtension(imgpath).ToLower();
                if (!(new string[] { ".tif", ".png" }).Contains(ext))
                    continue;

                if (lower >= 0 && idx < lower)
                {
                    idx++;
                    continue;
                }

                if (upper >= 0 && idx > upper)
                {
                    idx++;
                    continue;
                }

                // create a Bitmap from the file and add it to the list
                Bitmap bitmap = new Bitmap(imgpath);

                // update the size of the final bitmap
                r.width = bitmap.Width;
                r.height = bitmap.Height;

                images.Add(bitmap);

                idx++;
            }

            if (images.Count <= 0)
                return null;

            r.depth = images.Count;

            // create a bitmap to hold the combined image
            r.bitmap = new Bitmap(r.width, r.height * r.depth);

            // get a graphics object from the image so we can draw on it
            using (Graphics g = Graphics.FromImage(r.bitmap))
            {
                // set background color
                g.Clear(Color.Black);

                // go through each image and draw it on the final image
                int offset = 0;
                foreach (Bitmap image in images)
                {
                    g.DrawImage(image, new Rectangle(0, offset, image.Width, image.Height));
                    offset += image.Height;
                }
            }

            return r;
        }

        /// <summary>
        /// This function creates the texture from an image.
        /// </summary>
        /// <param name="gl">The OpenGL object.</param>
        /// <param name="image">The image.</param>
        /// <returns>True if the texture was successfully loaded.</returns>
        public void SetImage(OpenGL gl, Bitmap3D r)
        {
            //	Get the maximum texture size supported by OpenGL.
            int[] textureMaxSize = { 0 };
            gl.GetInteger(OpenGL.GL_MAX_TEXTURE_SIZE, textureMaxSize);

            //	Find the target width and height sizes, which is just the highest
            //	posible power of two that'll fit into the image.
            int targetWidth = textureMaxSize[0];
            int targetHeight = textureMaxSize[0];
            
            Bitmap finalImage = r.bitmap;

            //  Lock the image bits (so that we can pass them to OGL).
            BitmapData bitmapData = finalImage.LockBits(new Rectangle(0, 0, finalImage.Width, finalImage.Height),
                ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            //	Set the width and height.
            Width = (uint)r.width;
            Height = (uint)r.height;
            Depth = (uint)r.depth;

            //	Bind our texture object (make it the current texture).
            gl.BindTexture(OpenGL.GL_TEXTURE_3D, textureObject);

            //  Set the image data.
            gl.TexImage3D(OpenGL.GL_TEXTURE_3D, 0, (int)OpenGL.GL_RGBA,
                (int)Width, (int)Height, r.depth, 0, OpenGL.GL_BGRA, OpenGL.GL_UNSIGNED_BYTE,
                bitmapData.Scan0);

            //  Unlock the image.
            finalImage.UnlockBits(bitmapData);
        }

        public uint Width { get; private set; }
        public uint Height { get; private set; }
        public uint Depth { get; private set; }

        private uint textureObject;
    }
}
