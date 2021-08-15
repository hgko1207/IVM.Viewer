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
                if (SetProperty(ref rotXValue, value) && !subscribing)
                {
                    wcfserver.channel1.OnUpdateCamera(0, 0, -5.0f, rotXValue * (float)(Math.PI / 180.0), rotYValue * (float)(Math.PI / 180.0), rotZValue * (float)(Math.PI / 180.0), 0.8f);
                    wcfserver.channel2.OnUpdateCamera(0, 0, -5.0f, rotXValue * (float)(Math.PI / 180.0), rotYValue * (float)(Math.PI / 180.0), rotZValue * (float)(Math.PI / 180.0), 0.8f);
                }
            }
        }

        private float rotYValue = 0;
        public float RotYValue
        {
            get => rotYValue;
            set
            {
                if (SetProperty(ref rotYValue, value) && !subscribing)
                {
                    wcfserver.channel1.OnUpdateCamera(0, 0, -5.0f, rotXValue * (float)(Math.PI / 180.0), rotYValue * (float)(Math.PI / 180.0), rotZValue * (float)(Math.PI / 180.0), 0.8f);
                    wcfserver.channel2.OnUpdateCamera(0, 0, -5.0f, rotXValue * (float)(Math.PI / 180.0), rotYValue * (float)(Math.PI / 180.0), rotZValue * (float)(Math.PI / 180.0), 0.8f);
                }
            }
        }

        private float rotZValue = 0;
        public float RotZValue
        {
            get => rotZValue;
            set
            {
                if (SetProperty(ref rotZValue, value) && !subscribing)
                {
                    wcfserver.channel1.OnUpdateCamera(0, 0, -5.0f, rotXValue * (float)(Math.PI / 180.0), rotYValue * (float)(Math.PI / 180.0), rotZValue * (float)(Math.PI / 180.0), 0.8f);
                    wcfserver.channel2.OnUpdateCamera(0, 0, -5.0f, rotXValue * (float)(Math.PI / 180.0), rotYValue * (float)(Math.PI / 180.0), rotZValue * (float)(Math.PI / 180.0), 0.8f);
                }
            }
        }

        bool subscribing = false;

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="container"></param>
        public I3D3DViewPanelViewModel(IContainerExtension container) : base(container)
        {
            wcfserver = container.Resolve<I3DWcfServer>();

            EventAggregator.GetEvent<I3DCameraUpdateEvent>().Subscribe(UpdateCamera);
        }

        private void UpdateCamera(I3DCameraUpdateParam p)
        {
            subscribing = true;

            RotXValue = p.ax / (float)(Math.PI / 180.0);
            RotYValue = p.ay / (float)(Math.PI / 180.0);
            RotZValue = p.az / (float)(Math.PI / 180.0);

            subscribing = false;
        }
    }
}
