using System.Drawing;
using GDIDrawing = System.Drawing;

namespace IVM.Studio.Services
{
    public class ImageService
    {
        public Bitmap LoadImage(string Path)
        {
            using (Bitmap bitmap = new Bitmap(Path))
            {
                return bitmap.Clone(new Rectangle(0, 0, bitmap.Width, bitmap.Height), GDIDrawing.Imaging.PixelFormat.Format32bppArgb);
            }
        }
    }
}
