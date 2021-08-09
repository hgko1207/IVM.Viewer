namespace ivm
{
    public static class I3DVertexAttributes1
    {
        public const uint Position = 0;
        public const uint Normal = 1;
        public const uint TexCoord = 2;
    }

    public static class I3DVertexAttributes2
    {
        public const uint Position = 0;
        public const uint TexCoord = 1;
    }

    public static class I3DRenderMode
    {
        public const uint BLEND = 0;
        public const uint ADDED = 1;
        public const uint OBLIQUE = 2;
        public const uint SLICE = 3;
    }

    public static class I3DAxisDirection
    {
        public const uint X = 0;
        public const uint Y = 1;
        public const uint Z1 = 2;
        public const uint Z2 = 3;
    }

    public static class I3DConst
    {
        public static string[] IN_IMG_EXTS = new string[] { ".tif", ".png" };
    }
}
