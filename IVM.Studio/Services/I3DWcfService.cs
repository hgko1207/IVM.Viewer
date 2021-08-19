using IVM.Studio.Models.Events;
using Prism.Events;

namespace IVM.Studio.Services
{
    public class I3DServerService : I3DServerContract
    {
        public static IEventAggregator EventAggregator;

        public void OnWindowLoaded(int viewtype)
        {
            EventAggregator.GetEvent<I3DWindowLoadedEvent>().Publish(viewtype);
        }
        public void OnMetaLoaded(int width, int height, int depth, float umWidth, float umHeight, float umPerPixelZ)
        {
            I3DMetaLoadedParam p = new I3DMetaLoadedParam();
            p.width = width;
            p.height = height;
            p.depth = depth;
            p.umWidth = umWidth;
            p.umHeight = umHeight;
            p.umPerPixelZ = umPerPixelZ;

            EventAggregator.GetEvent<I3DMetaLoadedEvent>().Publish(p);
        }

        public void OnUpdateCamera(int viewtype, float px, float py, float pz, float ax, float ay, float az, float s)
        {
            I3DCameraUpdateParam p = new I3DCameraUpdateParam();
            p.viewtype = viewtype;
            p.px = px;
            p.py = py;
            p.pz = pz;
            p.ax = ax;
            p.ay = ay;
            p.az = az;
            p.s = s;

            EventAggregator.GetEvent<I3DCameraUpdateEvent>().Publish(p);
        }

        public void OnFirstRender(int viewtype)
        {
            EventAggregator.GetEvent<I3DFirstRenderEvent>().Publish(viewtype);
        }
    }
}

