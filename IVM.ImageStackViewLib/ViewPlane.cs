namespace ivm
{
    public class Plane
    {
        public float a;
        public float b;
        public float c;
        public float d;

        public Plane()
        {
            a = 0.0f;
            b = 0.0f;
            c = 0.0f;
            d = 0.0f;
        }

        public Plane(float a_, float b_, float c_, float d_)
        {
            a = a_;
            b = b_;
            c = c_;
            d = d_;
        }
    }
}
