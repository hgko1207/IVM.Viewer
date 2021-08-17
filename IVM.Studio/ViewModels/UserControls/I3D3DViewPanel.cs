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

        private string _DAPIColor = "Red";
        public string DAPIColor
        {
            get => _DAPIColor;
            set => SetProperty(ref _DAPIColor, value);
        }

        private string _GFPColor = "Green";
        public string GFPColor
        {
            get => _GFPColor;
            set => SetProperty(ref _GFPColor, value);
        }

        private string _RFPColor = "Blue";
        public string RFPColor
        {
            get => _RFPColor;
            set => SetProperty(ref _RFPColor, value);
        }

        private string _NIRColor = "None";
        public string NIRColor
        {
            get => _NIRColor;
            set => SetProperty(ref _NIRColor, value);
        }

        private bool _DAPIVisible = true;
        public bool DAPIVisible
        {
            get => _DAPIVisible;
            set
            {
                if (SetProperty(ref _DAPIVisible, value))
                {
                    wcfserver.channel1.OnChangeBandVisible(_DAPIVisible, _GFPVisible, _RFPVisible, _NIRVisible);
                    wcfserver.channel2.OnChangeBandVisible(_DAPIVisible, _GFPVisible, _RFPVisible, _NIRVisible);
                }
            }
        }

        private bool _GFPVisible = true;
        public bool GFPVisible
        {
            get => _GFPVisible;
            set
            {
                if (SetProperty(ref _GFPVisible, value))
                {
                    wcfserver.channel1.OnChangeBandVisible(_DAPIVisible, _GFPVisible, _RFPVisible, _NIRVisible);
                    wcfserver.channel2.OnChangeBandVisible(_DAPIVisible, _GFPVisible, _RFPVisible, _NIRVisible);
                }
            }
        }

        private bool _RFPVisible = true;
        public bool RFPVisible
        {
            get => _RFPVisible;
            set
            {
                if (SetProperty(ref _RFPVisible, value))
                {
                    wcfserver.channel1.OnChangeBandVisible(_DAPIVisible, _GFPVisible, _RFPVisible, _NIRVisible);
                    wcfserver.channel2.OnChangeBandVisible(_DAPIVisible, _GFPVisible, _RFPVisible, _NIRVisible);
                }
            }
        }

        private bool _NIRVisible = false;
        public bool NIRVisible
        {
            get => _NIRVisible;
            set
            {
                if (SetProperty(ref _NIRVisible, value))
                {
                    wcfserver.channel1.OnChangeBandVisible(_DAPIVisible, _GFPVisible, _RFPVisible, _NIRVisible);
                    wcfserver.channel2.OnChangeBandVisible(_DAPIVisible, _GFPVisible, _RFPVisible, _NIRVisible);
                }
            }
        }

        private int _DAPILevelLower = 0;
        public int DAPILevelLower
        {
            get => _DAPILevelLower;
            set
            {
                if (SetProperty(ref _DAPILevelLower, value))
                {
                    wcfserver.channel1.OnChangeIntensityThreshold(_DAPILevelLower, _DAPILevelUpper, _GFPLevelLower, _GFPLevelUpper, _RFPLevelLower, _RFPLevelUpper, _NIRLevelLower, _NIRLevelUpper);
                }
            }
        }

        private int _DAPILevelUpper = 255;
        public int DAPILevelUpper
        {
            get => _DAPILevelUpper;
            set
            {
                if (SetProperty(ref _DAPILevelUpper, value))
                {
                    wcfserver.channel1.OnChangeIntensityThreshold(_DAPILevelLower, _DAPILevelUpper, _GFPLevelLower, _GFPLevelUpper, _RFPLevelLower, _RFPLevelUpper, _NIRLevelLower, _NIRLevelUpper);
                }
            }
        }

        private int _GFPLevelLower = 0;
        public int GFPLevelLower
        {
            get => _GFPLevelLower;
            set
            {
                if (SetProperty(ref _GFPLevelLower, value))
                {
                    wcfserver.channel1.OnChangeIntensityThreshold(_DAPILevelLower, _DAPILevelUpper, _GFPLevelLower, _GFPLevelUpper, _RFPLevelLower, _RFPLevelUpper, _NIRLevelLower, _NIRLevelUpper);
                }
            }
        }

        private int _GFPLevelUpper = 255;
        public int GFPLevelUpper
        {
            get => _GFPLevelUpper;
            set
            {
                if (SetProperty(ref _GFPLevelUpper, value))
                {
                    wcfserver.channel1.OnChangeIntensityThreshold(_DAPILevelLower, _DAPILevelUpper, _GFPLevelLower, _GFPLevelUpper, _RFPLevelLower, _RFPLevelUpper, _NIRLevelLower, _NIRLevelUpper);
                }
            }
        }

        private int _RFPLevelLower = 0;
        public int RFPLevelLower
        {
            get => _RFPLevelLower;
            set
            {
                if (SetProperty(ref _RFPLevelLower, value))
                {
                    wcfserver.channel1.OnChangeIntensityThreshold(_DAPILevelLower, _DAPILevelUpper, _GFPLevelLower, _GFPLevelUpper, _RFPLevelLower, _RFPLevelUpper, _NIRLevelLower, _NIRLevelUpper);
                }
            }
        }

        private int _RFPLevelUpper = 255;
        public int RFPLevelUpper
        {
            get => _RFPLevelUpper;
            set
            {
                if (SetProperty(ref _RFPLevelUpper, value))
                {
                    wcfserver.channel1.OnChangeIntensityThreshold(_DAPILevelLower, _DAPILevelUpper, _GFPLevelLower, _GFPLevelUpper, _RFPLevelLower, _RFPLevelUpper, _NIRLevelLower, _NIRLevelUpper);
                }
            }
        }

        private int _NIRLevelLower = 0;
        public int NIRLevelLower
        {
            get => _NIRLevelLower;
            set
            {
                if (SetProperty(ref _NIRLevelLower, value))
                {
                    wcfserver.channel1.OnChangeIntensityThreshold(_DAPILevelLower, _DAPILevelUpper, _GFPLevelLower, _GFPLevelUpper, _RFPLevelLower, _RFPLevelUpper, _NIRLevelLower, _NIRLevelUpper);
                }
            }
        }

        private int _NIRLevelUpper = 255;
        public int NIRLevelUpper
        {
            get => _NIRLevelUpper;
            set
            {
                if (SetProperty(ref _NIRLevelUpper, value))
                {
                    wcfserver.channel1.OnChangeIntensityThreshold(_DAPILevelLower, _DAPILevelUpper, _GFPLevelLower, _GFPLevelUpper, _RFPLevelLower, _RFPLevelUpper, _NIRLevelLower, _NIRLevelUpper);
                }
            }
        }

        private float _DAPIAlphaValue = 10.0f;
        public float DAPIAlphaValue
        {
            get => _DAPIAlphaValue;
            set
            {
                if (SetProperty(ref _DAPIAlphaValue, value))
                {
                    wcfserver.channel1.OnChangeAlphaWeight(_DAPIAlphaValue, _GFPAlphaValue, _RFPAlphaValue, _NIRAlphaValue);
                }
            }
        }

        private float _GFPAlphaValue = 10.0f;
        public float GFPAlphaValue
        {
            get => _GFPAlphaValue;
            set
            {
                if (SetProperty(ref _GFPAlphaValue, value))
                {
                    wcfserver.channel1.OnChangeAlphaWeight(_DAPIAlphaValue, _GFPAlphaValue, _RFPAlphaValue, _NIRAlphaValue);
                }
            }
        }

        private float _RFPAlphaValue = 10.0f;
        public float RFPAlphaValue
        {
            get => _RFPAlphaValue;
            set
            {
                if (SetProperty(ref _RFPAlphaValue, value))
                {
                    wcfserver.channel1.OnChangeAlphaWeight(_DAPIAlphaValue, _GFPAlphaValue, _RFPAlphaValue, _NIRAlphaValue);
                }
            }
        }

        private float _NIRAlphaValue = 10.0f;
        public float NIRAlphaValue
        {
            get => _NIRAlphaValue;
            set
            {
                if (SetProperty(ref _NIRAlphaValue, value))
                {
                    wcfserver.channel1.OnChangeAlphaWeight(_DAPIAlphaValue, _GFPAlphaValue, _RFPAlphaValue, _NIRAlphaValue);
                }
            }
        }

        public ICommand CameraCoronalCommand { get; private set; }
        public ICommand CameraSagittalCommand { get; private set; }
        public ICommand CameraAxialCommand { get; private set; }
        
        public ICommand SlicePrevStepCommand { get; private set; }
        public ICommand SliceNextStepCommand { get; private set; }

        public ICommand DAPIColorChangedCommand { get; private set; }
        public ICommand GFPColorChangedCommand { get; private set; }
        public ICommand RFPColorChangedCommand { get; private set; }
        public ICommand NIRColorChangedCommand { get; private set; }

        bool subscribing = false;

        int width = 0;
        int height = 0;
        float umWidth = 0;
        float umHeight = 0;

        int StrToBandIdx(string col)
        {
            switch (col)
            {
                case "Red":
                    return 0;
                case "Green":
                    return 1;
                case "Blue":
                    return 2;
            }
            return 3;
        }

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

            DAPIColorChangedCommand = new DelegateCommand<string>(DAPIColorChanged);
            GFPColorChangedCommand = new DelegateCommand<string>(GFPColorChanged);
            RFPColorChangedCommand = new DelegateCommand<string>(RFPColorChanged);
            NIRColorChangedCommand = new DelegateCommand<string>(NIRColorChanged);
        }

        private void DAPIColorChanged(string col)
        {
            DAPIColor = col;

            wcfserver.channel1.OnChangeBandOrder(StrToBandIdx(DAPIColor), StrToBandIdx(GFPColor), StrToBandIdx(RFPColor), StrToBandIdx(NIRColor));
            wcfserver.channel2.OnChangeBandOrder(StrToBandIdx(DAPIColor), StrToBandIdx(GFPColor), StrToBandIdx(RFPColor), StrToBandIdx(NIRColor));
        }
        private void GFPColorChanged(string col)
        {
            GFPColor = col;

            wcfserver.channel1.OnChangeBandOrder(StrToBandIdx(DAPIColor), StrToBandIdx(GFPColor), StrToBandIdx(RFPColor), StrToBandIdx(NIRColor));
            wcfserver.channel2.OnChangeBandOrder(StrToBandIdx(DAPIColor), StrToBandIdx(GFPColor), StrToBandIdx(RFPColor), StrToBandIdx(NIRColor));
        }

        private void RFPColorChanged(string col)
        {
            RFPColor = col;

            wcfserver.channel1.OnChangeBandOrder(StrToBandIdx(DAPIColor), StrToBandIdx(GFPColor), StrToBandIdx(RFPColor), StrToBandIdx(NIRColor));
            wcfserver.channel2.OnChangeBandOrder(StrToBandIdx(DAPIColor), StrToBandIdx(GFPColor), StrToBandIdx(RFPColor), StrToBandIdx(NIRColor));
        }
        private void NIRColorChanged(string col)
        {
            NIRColor = col;

            wcfserver.channel1.OnChangeBandOrder(StrToBandIdx(DAPIColor), StrToBandIdx(GFPColor), StrToBandIdx(RFPColor), StrToBandIdx(NIRColor));
            wcfserver.channel2.OnChangeBandOrder(StrToBandIdx(DAPIColor), StrToBandIdx(GFPColor), StrToBandIdx(RFPColor), StrToBandIdx(NIRColor));
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
