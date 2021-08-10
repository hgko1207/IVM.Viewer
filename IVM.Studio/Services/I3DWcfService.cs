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
    }
}

