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
    public class I3DSliceViewPanelViewModel : ViewModelBase
    {
        I3DWcfServer wcfserver;

        DataManager datamanager;

        public I3DChannelInfo I3DChannelInfo { get; set; }

        private float sliceDepthX = 0;
        public float SliceDepthX
        {
            get => sliceDepthX;
            set
            {
                if (SetProperty(ref sliceDepthX, value))
                {
                    wcfserver.channel2.OnChangeSliceDepth(sliceDepthX / 100.0f, sliceDepthY / 100.0f, sliceDepthZ / 100.0f);

                    SliceDepthValX = string.Format("{0} / {1}", (sliceDepthX / 100.0f / 2.0f + 0.5f) * umHeight, umHeight);
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

                    SliceDepthValY = string.Format("{0} / {1}", (sliceDepthY / 100.0f / 2.0f + 0.5f) * umWidth, umWidth);
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

                    SliceDepthValZ = string.Format("{0} / {1}", (sliceDepthZ / 100.0f / 2.0f + 0.5f) * depth * umPerPixelZ, depth * umPerPixelZ);
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

        private bool scaleVisible = true;
        public bool ScaleVisible
        {
            get => scaleVisible;
            set
            {
                if (SetProperty(ref scaleVisible, value))
                {
                    wcfserver.channel2.OnChangeSliceScaleParam(scaleVisible, scaleFontSize);
                }
            }
        }

        private int scaleFontSize = 12;
        public int ScaleFontSize
        {
            get => scaleFontSize;
            set
            {
                if (SetProperty(ref scaleFontSize, value))
                {
                    wcfserver.channel2.OnChangeSliceScaleParam(scaleVisible, scaleFontSize);
                }
            }
        }

        public ICommand DAPIColorChangedCommand { get; private set; }
        public ICommand GFPColorChangedCommand { get; private set; }
        public ICommand RFPColorChangedCommand { get; private set; }
        public ICommand NIRColorChangedCommand { get; private set; }
        public ICommand SliceDepthResetCommand { get; private set; }

        int width = 0;
        int height = 0;
        int depth = 0;
        float umWidth = 0;
        float umHeight = 0;
        float umPerPixelZ = 0;

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
        public I3DSliceViewPanelViewModel(IContainerExtension container) : base(container)
        {
            wcfserver = container.Resolve<I3DWcfServer>();
            datamanager = container.Resolve<DataManager>();

            I3DChannelInfo = datamanager.I3DChannelInfo;

            EventAggregator.GetEvent<I3DMetaLoadedEvent>().Subscribe(OnMetaLoaded);

            DAPIColorChangedCommand = new DelegateCommand<string>(DAPIColorChanged);
            GFPColorChangedCommand = new DelegateCommand<string>(GFPColorChanged);
            RFPColorChangedCommand = new DelegateCommand<string>(RFPColorChanged);
            NIRColorChangedCommand = new DelegateCommand<string>(NIRColorChanged);
            SliceDepthResetCommand = new DelegateCommand(ResetSliceDepth);
        }

        private void ResetSliceDepth()
        {
            SliceDepthX = 0;
            SliceDepthY = 0;
            SliceDepthZ = 0;
        }

        private void DAPIColorChanged(string col)
        {
            I3DChannelInfo c = datamanager.I3DChannelInfo;
            c.DAPIColor = col;

            wcfserver.channel1.OnChangeBandOrder(StrToBandIdx(c.DAPIColor), StrToBandIdx(c.GFPColor), StrToBandIdx(c.RFPColor), StrToBandIdx(c.NIRColor));
            wcfserver.channel2.OnChangeBandOrder(StrToBandIdx(c.DAPIColor), StrToBandIdx(c.GFPColor), StrToBandIdx(c.RFPColor), StrToBandIdx(c.NIRColor));
        }

        private void GFPColorChanged(string col)
        {
            I3DChannelInfo c = datamanager.I3DChannelInfo;
            c.GFPColor = col;

            wcfserver.channel1.OnChangeBandOrder(StrToBandIdx(c.DAPIColor), StrToBandIdx(c.GFPColor), StrToBandIdx(c.RFPColor), StrToBandIdx(c.NIRColor));
            wcfserver.channel2.OnChangeBandOrder(StrToBandIdx(c.DAPIColor), StrToBandIdx(c.GFPColor), StrToBandIdx(c.RFPColor), StrToBandIdx(c.NIRColor));
        }

        private void RFPColorChanged(string col)
        {
            I3DChannelInfo c = datamanager.I3DChannelInfo;
            c.RFPColor = col;

            wcfserver.channel1.OnChangeBandOrder(StrToBandIdx(c.DAPIColor), StrToBandIdx(c.GFPColor), StrToBandIdx(c.RFPColor), StrToBandIdx(c.NIRColor));
            wcfserver.channel2.OnChangeBandOrder(StrToBandIdx(c.DAPIColor), StrToBandIdx(c.GFPColor), StrToBandIdx(c.RFPColor), StrToBandIdx(c.NIRColor));
        }

        private void NIRColorChanged(string col)
        {
            I3DChannelInfo c = datamanager.I3DChannelInfo;
            c.NIRColor = col;

            wcfserver.channel1.OnChangeBandOrder(StrToBandIdx(c.DAPIColor), StrToBandIdx(c.GFPColor), StrToBandIdx(c.RFPColor), StrToBandIdx(c.NIRColor));
            wcfserver.channel2.OnChangeBandOrder(StrToBandIdx(c.DAPIColor), StrToBandIdx(c.GFPColor), StrToBandIdx(c.RFPColor), StrToBandIdx(c.NIRColor));
        }

        private void OnMetaLoaded(I3DMetaLoadedParam p)
        {
            width = p.width;
            height = p.height;
            depth = p.depth;
            umWidth = p.umWidth;
            umHeight = p.umHeight;
            umPerPixelZ = p.umPerPixelZ;

            SliceDepthX = -1;
            SliceDepthY = -1;
            SliceDepthZ = -1;

            ResetSliceDepth();
        }
    }
}
