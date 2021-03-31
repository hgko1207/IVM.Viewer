using IVM.Studio.Utils;
using System;

namespace IVM.Studio.Models
{
    public class Metadata
    {
        [CSVSerializable(0, "General")]
        public ImageSequence Sequence { get; set; }

        [CSVSerializable(1, "General")]
        public string FileName { get; set; }

        [CSVSerializable(2, "General")]
        public string ChA { get; set; }

        [CSVSerializable(3, "General")]
        public string ChB { get; set; }

        [CSVSerializable(4, "General")]
        public string ChC { get; set; }

        [CSVSerializable(5, "General")]
        public string ChD { get; set; }

        [CSVSerializable(6, "General")]
        public double GainA { get; set; }

        [CSVSerializable(7, "General")]
        public double GainB { get; set; }

        [CSVSerializable(8, "General")]
        public double GainC { get; set; }

        [CSVSerializable(9, "General")]
        public double GainD { get; set; }

        [CSVSerializable(10, "General")]
        public int StageX { get; set; }

        [CSVSerializable(11, "General")]
        public int StageY { get; set; }

        [CSVSerializable(12, "General")]
        public int StageZ { get; set; }

        [CSVSerializable(13, "General")]
        public int Averaging { get; set; }

        [CSVSerializable(14, "General")]
        public int FPS { get; set; }

        [CSVSerializable(15, "Confocal")]
        public double LaserA { get; set; }

        [CSVSerializable(16, "Confocal")]
        public double LaserB { get; set; }

        [CSVSerializable(17, "Confocal")]
        public double LaserC { get; set; }

        [CSVSerializable(18, "Confocal")]
        public double LaserD { get; set; }

        [CSVSerializable(19, "Confocal")]
        public string PhA { get; set; }

        [CSVSerializable(20, "Confocal")]
        public string PhB { get; set; }

        [CSVSerializable(21, "Confocal")]
        public string PhC { get; set; }

        [CSVSerializable(22, "Confocal")]
        public string PhD { get; set; }

        [CSVSerializable(23, "TPM")]
        public int LaserPower { get; set; }

        [CSVSerializable(24, "TPM")]
        public string LaserWL { get; set; }

        [CSVSerializable(25, "TPM")]
        public int GDD { get; set; }

        [CSVSerializable(26, "General")]
        public string Objective { get; set; }

        [CSVSerializable(27, "General")]
        public int FovX { get; set; }

        [CSVSerializable(28, "General")]
        public int FovY { get; set; }

        [CSVSerializable(29, "General")]
        public int Xpixel { get; set; }

        [CSVSerializable(30, "General")]
        public int Ypixel { get; set; }

        [CSVSerializable(31, "General")]
        public string ZaxisAF { get; set; }

        [CSVSerializable(32, "General")]
        public string ROItracking { get; set; }

        [CSVSerializable(33, "General")]
        public string Sequential { get; set; }

        [CSVSerializable(34, "General")]
        public string SetFolder { get; set; }

        [CSVSerializable(35, "General")]
        public string Registration { get; set; }

        [CSVSerializable(36, "General")]
        public DateTime Time { get; set; }
    }
}
