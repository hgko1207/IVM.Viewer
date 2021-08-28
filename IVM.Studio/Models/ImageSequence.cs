using IVM.Studio.Utils;
using System.ComponentModel;

/**
 * @Class Name : ImageSequence.cs
 * @Description : 이미지 시퀀스 모델
 * @
 * @ 수정일         수정자              수정내용
 * @ ----------   ---------   -------------------------------
 * @ 2021.03.31     고형균              최초생성
 *
 * @author 고형균
 * @since 2021.03.31
 * @version 1.0
 */
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

        public ImageSequence(int sequence)
        {
            this.Sequence = sequence;
            Mode = false;
        }

        public ImageSequence(int timeLapseNumbering, int multiPositionNumbering, int mosaicNumbering, int zStackNumbering)
        {
            this.TimeLapseNumbering = timeLapseNumbering;
            this.MultiPositionNumbering = multiPositionNumbering;
            this.MosaicNumbering = mosaicNumbering;
            this.ZStackNumbering = zStackNumbering;
            Mode = true;
        }
    }
}
