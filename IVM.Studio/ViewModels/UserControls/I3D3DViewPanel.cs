using IVM.Studio.Models;
using IVM.Studio.Models.Events;
using IVM.Studio.Mvvm;
using IVM.Studio.Services;
using IVM.Studio.Views.UserControls;
using Prism.Events;
using Prism.Ioc;
using System;
using System.Collections.Generic;
using static IVM.Studio.Models.Common;

namespace IVM.Studio.ViewModels.UserControls
{
    public class I3D3DViewPanelViewModel : ViewModelBase
    {
        I3DWcfServer wcfserver;

        private float rotXValue = 0;
        public float RotXValue
        {
            get => rotXValue;
            set
            {
                if (SetProperty(ref rotXValue, value))
                {
                    wcfserver.channel1.OnUpdateCamera(0, 0, -5.0f, 0, 0, rotXValue * (float)(Math.PI / 180.0), 0.8f);
                    wcfserver.channel2.OnUpdateCamera(0, 0, -5.0f, 0, 0, rotXValue * (float)(Math.PI / 180.0), 0.8f);
                }
            }

            // {
            //    if (SetProperty(ref selectedSlideInfo, value) && value != null)
            //    {
            //        EventAggregator.GetEvent<StopSlideShowEvent>().Publish();
            //        EventAggregator.GetEvent<EnableImageSlidersEvent>().Publish(new SlidersParam() { CurrentSlidesPath = currentSlidesPath, SlideName = value.Name });
            //        DisplaySlide(true);
            //        dataManager.SelectedSlideInfo = value;
            //    }
            //}
        }

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="container"></param>
        public I3D3DViewPanelViewModel(IContainerExtension container) : base(container)
        {
            wcfserver = container.Resolve<I3DWcfServer>();
        }
    }
}
