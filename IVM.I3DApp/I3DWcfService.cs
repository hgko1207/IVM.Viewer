using IVM.Studio.Services;
using System.ServiceModel;

namespace IVM.Studio.I3D
{
    public class I3DClientService : I3DClientContract
    {
        public static MainWindow w;

        public void OnOpen(string path)
        {
            w.vw.Open(path);
        }
    }
}
