namespace IVM.Studio.Models
{
    public class ColorMapModel
    {
        public ColorMap ColorMap { get; set; }

        public string Image { get; set; }
    }

    public enum ColorMap
    {
        Autumn = 0,
        Bone = 1,
        Jet = 2,
        Winter = 3,
        Rainbow = 4,
        Ocean = 5,
        Summer = 6,
        Spring = 7,
        Cool = 8,
        Hsv = 9,
        Pink = 10,
        Hot = 11,
        Parula = 12,
        Magma = 13,
        Inferno = 14,
        Plasma = 15,
        Viridis = 16,
        Cividis = 17,
        Twilight = 18,
        TwilightShifted = 19
    }

    public enum Colors
    {
        Red = 0, Green = 1, Blue = 2, Alpha = 3, None = -1
    }
}
