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

        public void OnUpdateCamera(int viewtype, float px, float py, float pz, float ax, float ay, float s)
        {
            CameraUpdateParam p = new CameraUpdateParam();
            p.viewtype = viewtype;
            p.px = px;
            p.py = py;
            p.pz = pz;
            p.ax = ax;
            p.ay = ay;
            p.s = s;

            EventAggregator.GetEvent<I3DCameraUpdateEvent>().Publish(p);
        }
    }
}

