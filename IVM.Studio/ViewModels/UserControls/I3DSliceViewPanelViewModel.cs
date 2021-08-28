using IVM.Studio.Models;
using IVM.Studio.Models.Events;
using IVM.Studio.Mvvm;
using IVM.Studio.Services;
using Prism.Commands;
using Prism.Ioc;
using System.Windows.Input;

namespace IVM.Studio.ViewModels.UserControls
{
    public class I3DSliceViewPanelViewModel : ViewModelBase
    {
        I3DWcfServer wcfserver;

        private readonly DataManager datamanager;

        public I3DChannelInfo I3DChannelInfo { get; set; }
        public I3DBackgroundInfo I3DBackgroundInfo { get; set; }

        private float sliceDepthX = 0;
        public float SliceDepthX
        {
            get => sliceDepthX;
            set
            {
                if (SetProperty(ref sliceDepthX, value))
                {
                    wcfserver.channel2.OnChangeSliceDepth(sliceDepthX / 100.0f, sliceDepthY / 100.0f, sliceDepthZ / 100.0f);

                    SliceDepthValX = string.Format("{0:0.0} / {1:0.0}", (sliceDepthX / 100.0f / 2.0f + 0.5f) * umHeight, umHeight);
                }
            }
        }

        private float sliceDepthY = 0;
        public float SliceDepthY
        {
            get => sliceDepthY;
            set
            {
                if (SetProperty(ref sliceDepthY, value))
                {
                    wcfserver.channel2.OnChangeSliceDepth(sliceDepthX / 100.0f, sliceDepthY / 100.0f, sliceDepthZ / 100.0f);

                    SliceDepthValY = string.Format("{0:0.0} / {1:0.0}", (sliceDepthY / 100.0f / 2.0f + 0.5f) * umWidth, umWidth);
                }
            }
        }

        private float sliceDepthZ = 0;
        public float SliceDepthZ
        {
            get => sliceDepthZ;
            set
            {
                if (SetProperty(ref sliceDepthZ, value))
                {
                    wcfserver.channel2.OnChangeSliceDepth(sliceDepthX / 100.0f, sliceDepthY / 100.0f, sliceDepthZ / 100.0f);

                    SliceDepthValZ = string.Format("{0:0.0} / {1:0.0}", (sliceDepthZ / 100.0f / 2.0f + 0.5f) * depth * umPerPixelZ, depth * umPerPixelZ);
                }
            }
        }

        private string sliceDepthValX = "";
        public string SliceDepthValX
        {
            get => sliceDepthValX;
            set => SetProperty(ref sliceDepthValX, value);
        }

        private string sliceDepthValY = "";
        public string SliceDepthValY
        {
            get => sliceDepthValY;
            set => SetProperty(ref sliceDepthValY, value);
        }

        private string sliceDepthValZ = "";
        public string SliceDepthValZ
        {
            get => sliceDepthValZ;
            set => SetProperty(ref sliceDepthValZ, value);
        }

        public ICommand SliceDepthResetCommand { get; private set; }

        int width = 0;
        int height = 0;
        int depth = 0;
        float umWidth = 0;
        float umHeight = 0;
        float umPerPixelZ = 0;

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="container"></param>
        public I3DSliceViewPanelViewModel(IContainerExtension container) : base(container)
        {
            wcfserver = container.Resolve<I3DWcfServer>();
            datamanager = container.Resolve<DataManager>();

            I3DChannelInfo = datamanager.I3DChannelInfo2;
            I3DBackgroundInfo = datamanager.I3DBackgroundInfo2;

            EventAggregator.GetEvent<I3DMetaLoadedEvent>().Subscribe(OnMetaLoaded);

            SliceDepthResetCommand = new DelegateCommand(ResetSliceDepth);
        }

        private void ResetSliceDepth()
        {
            SliceDepthX = 0;
            SliceDepthY = 0;
            SliceDepthZ = 0;
        }

        private void OnMetaLoaded(I3DMetaLoadedParam p)
        {
            if (p.Width > 0 && p.Height > 0 && p.Depth > 0)
            {
                width = p.Width;
                height = p.Height;
                depth = p.Depth;
            }

            if (p.UmWidth > 0 && p.UmHeight > 0 && p.UmPerPixelZ > 0)
            {
                umWidth = p.UmWidth;
                umHeight = p.UmHeight;
                umPerPixelZ = p.UmPerPixelZ;
            }

            SliceDepthX = -1;
            SliceDepthY = -1;
            SliceDepthZ = -1;

            ResetSliceDepth();
        }
    }
}
