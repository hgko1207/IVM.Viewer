namespace ivm
{
    public static class VertexAttributes1
    {
        public const uint Position = 0;
        public const uint Normal = 1;
        public const uint TexCoord = 2;
    }

    public static class VertexAttributes2
    {
        public const uint Position = 0;
        public const uint TexCoord = 1;
    }

    public static class ViewRenderMode
    {
        public const uint BLEND = 0;
        public const uint ADDED = 1;
        public const uint OBLIQUE = 2;
        public const uint SLICE = 3;
    }

    public static class ViewAxisDirection
    {
        public const uint X = 0;
        public const uint Y = 1;
        public const uint Z1 = 2;
        public const uint Z2 = 3;
    }

    public static class ViewConst
    {
        public static string[] IN_IMG_EXTS = new string[] { ".tif", ".png" };
    }
}
