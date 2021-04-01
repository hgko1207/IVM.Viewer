using IVM.Studio.Utils;
using System.ComponentModel;

namespace IVM.Studio.Models
{
    [TypeConverter(typeof(ImageSequenceConverter))]
    public class ImageSequence
    {
        public bool Mode { get; set; }

        public int TimeLapseNumbering { get; set; }

        public int MultiPositionNumbering { get; set; }

        public int MosaicNumbering { get; set; }

        public int ZStackNumbering { get; set; }

        public int Sequence { get; set; }

        public ImageSequence(int Sequence)
        {
            this.Sequence = Sequence;
            Mode = false;
        }

        public ImageSequence(int TimeLapseNumbering, int MultiPositionNumbering, int MosaicNumbering, int ZStackNumbering)
        {
            this.TimeLapseNumbering = TimeLapseNumbering;
            this.MultiPositionNumbering = MultiPositionNumbering;
            this.MosaicNumbering = MosaicNumbering;
            this.ZStackNumbering = ZStackNumbering;
            Mode = true;
        }
    }
}
