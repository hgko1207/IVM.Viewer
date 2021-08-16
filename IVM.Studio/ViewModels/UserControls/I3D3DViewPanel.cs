using IVM.Studio.Models;
using IVM.Studio.Models.Events;
using IVM.Studio.Mvvm;
using IVM.Studio.Services;
using IVM.Studio.Views.UserControls;
using Prism.Commands;
using Prism.Events;
using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Windows.Input;
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

        public enum SliceModeType
        {
            [Display(Name = "OBLIQUE", Order = 2)]
            OBLIQUE = 2,
            [Display(Name = "SLICE", Order = 3)]
            SLICE = 3,
        }

        private SliceModeType sliceMode = SliceModeType.OBLIQUE;
        public SliceModeType SliceMode
        {
            get => sliceMode;
            set
            {
                if (SetProperty(ref sliceMode, value))
                {
                    wcfserver.channel2.OnChangeRenderMode((int)sliceMode);
                }
            }
        }

        private float slicePosValue = 0;
        public float SlicePosValue
        {
            get => slicePosValue;
            set
            {
                if (SetProperty(ref slicePosValue, value))
                {
                    wcfserver.channel2.OnChangeObliqueDepth(slicePosValue);
                }
            }
        }

        private float slicePosFreq = 0.1f;
        public float SlicePosFreq
        {
            get => slicePosFreq;
            set => SetProperty(ref slicePosFreq, value);
        }

        private int slicePosStep = 1;
        public int SlicePosStep
        {
            get => slicePosStep;
            set => SetProperty(ref slicePosStep, value);
        }

        private string sliceUmPix = "";
        public string SliceUmPix
        {
            get => sliceUmPix;
            set => SetProperty(ref sliceUmPix, value);
        }

        public ICommand CameraCoronalCommand { get; private set; }
        public ICommand CameraSagittalCommand { get; private set; }
        public ICommand CameraAxialCommand { get; private set; }
        
        public ICommand SlicePrevStepCommand { get; private set; }
        public ICommand SliceNextStepCommand { get; private set; }

        bool subscribing = false;

        int width = 0;
        int height = 0;
        float umWidth = 0;
        float umHeight = 0;

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="container"></param>
        public I3D3DViewPanelViewModel(IContainerExtension container) : base(container)
        {
            wcfserver = container.Resolve<I3DWcfServer>();

            EventAggregator.GetEvent<I3DCameraUpdateEvent>().Subscribe(UpdateCamera);
            EventAggregator.GetEvent<I3DMetaLoadedEvent>().Subscribe(OnMetaLoaded);

            CameraCoronalCommand = new DelegateCommand(CameraCoronal);
            CameraSagittalCommand = new DelegateCommand(CameraSagittal);
            CameraAxialCommand = new DelegateCommand(CameraAxial);

            SlicePrevStepCommand = new DelegateCommand(SlicePrevStep);
            SliceNextStepCommand = new DelegateCommand(SliceNextStep);
        }

        private void SliceNextStep()
        {
            float pos = slicePosValue + (float)slicePosStep * slicePosFreq;
            if (pos > 1.0f)
                pos = 1.0f;

            SlicePosValue = pos;
        }

        private void SlicePrevStep()
        {
            float pos = slicePosValue - (float)slicePosStep * slicePosFreq;
            if (pos < -1.0f)
                pos = -1.0f;

            SlicePosValue = pos;
        }

        private void OnMetaLoaded(I3DMetaLoadedParam p)
        {
            width = p.width;
            height = p.height;
            umWidth = p.umWidth;
            umHeight = p.umHeight;

            SlicePosFreq = 1.0f / width;
            SliceUmPix = string.Format("{0:0.00} um", (float)umWidth / (float)width);
        }

        private void CameraAxial()
        {
            RotXValue = 90;
            RotYValue = 0;
            RotZValue = 0;
        }

        private void CameraSagittal()
        {
            RotXValue = 0;
            RotYValue = 90;
            RotZValue = 0;
        }

        private void CameraCoronal()
        {
            RotXValue = 0;
            RotYValue = 0;
            RotZValue = 0;
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
