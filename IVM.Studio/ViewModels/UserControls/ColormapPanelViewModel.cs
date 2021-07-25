using IVM.Studio.Models;
using IVM.Studio.Models.Events;
using IVM.Studio.Mvvm;
using IVM.Studio.Services;
using IVM.Studio.Views.UserControls;
using Ookii.Dialogs.Wpf;
using Prism.Commands;
using Prism.Events;
using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using GDIDrawing = System.Drawing;

/**
 * @Class Name : ColormapPanelViewModel.cs
 * @Description : 컬러 맵 화면 뷰 모델
 * @
 * @ 수정일         수정자              수정내용
 * @ ----------   ---------   -------------------------------
 * @ 2021.05.10     고형균              최초생성
 *
 * @author 고형균
 * @since 2021.05.10
 * @version 1.0
 */
namespace IVM.Studio.ViewModels.UserControls
{
    public class ColormapPanelViewModel : ViewModelBase, IViewLoadedAndUnloadedAware<ColormapPanel>
    {
        public IEnumerable<ColorMap> ColorMaps
        {
            get
            {
                return new[] { ColorMap.Hot, ColorMap.Cool, ColorMap.Inferno, ColorMap.Bone, ColorMap.Jet,
                    ColorMap.Rainbow, ColorMap.Autumn, ColorMap.Ocean, ColorMap.TwilightShifted,
                    ColorMap.Winter, ColorMap.Summer, ColorMap.Spring, ColorMap.Hsv, ColorMap.Pink,
                    ColorMap.Parula, ColorMap.Magma, ColorMap.Plasma, ColorMap.Viridis, ColorMap.Cividis, ColorMap.Twilight
                };
            }
        }

        private ColorMap selectedDAPIColorMap;
        public ColorMap SelectedDAPIColorMap
        {
            get => selectedDAPIColorMap;
            set
            {
                if (SetProperty(ref selectedDAPIColorMap, value))
                    colorChannelInfoMap[ChannelType.DAPI].ColorMap = value;
            }
        }

        private ColorMap selectedGFPColorMap;
        public ColorMap SelectedGFPColorMap
        {
            get => selectedGFPColorMap;
            set
            {
                if (SetProperty(ref selectedGFPColorMap, value))
                    colorChannelInfoMap[ChannelType.GFP].ColorMap = value;
            }
        }

        private ColorMap selectedRFPColorMap;
        public ColorMap SelectedRFPColorMap
        {
            get => selectedRFPColorMap;
            set
            {
                if (SetProperty(ref selectedRFPColorMap, value))
                    colorChannelInfoMap[ChannelType.RFP].ColorMap = value;
            }
        }

        private ColorMap selectedNIRColorMap;
        public ColorMap SelectedNIRColorMap
        {
            get => selectedNIRColorMap;
            set
            {
                if (SetProperty(ref selectedNIRColorMap, value))
                    colorChannelInfoMap[ChannelType.NIR].ColorMap = value;
            }
        }

        private bool _DAPIColorMapEnabled;
        public bool DAPIColorMapEnabled
        {
            get => _DAPIColorMapEnabled;
            set
            {
                if (SetProperty(ref _DAPIColorMapEnabled, value))
                    colorChannelInfoMap[ChannelType.DAPI].ColorMapEnabled = value;
            }
        }

        private bool _GFPColorMapEnabled;
        public bool GFPColorMapEnabled
        {
            get => _GFPColorMapEnabled;
            set
            {
                if (SetProperty(ref _GFPColorMapEnabled, value))
                    colorChannelInfoMap[ChannelType.GFP].ColorMapEnabled = value;
            }
        }

        private bool _RFPColorMapEnabled;
        public bool RFPColorMapEnabled
        {
            get => _RFPColorMapEnabled;
            set
            {
                if (SetProperty(ref _RFPColorMapEnabled, value))
                    colorChannelInfoMap[ChannelType.RFP].ColorMapEnabled = value;
            }
        }

        private bool _NIRColorMapEnabled;
        public bool NIRColorMapEnabled
        {
            get => _NIRColorMapEnabled;
            set
            {
                if (SetProperty(ref _NIRColorMapEnabled, value))
                    colorChannelInfoMap[ChannelType.NIR].ColorMapEnabled = value;
            }
        }

        private bool isLock;
        public bool IsLock
        {
            get => isLock;
            set => SetProperty(ref isLock, value);
        }

        public ICommand ExportLabelCommand { get; private set; }

        private Dictionary<ChannelType, ColorChannelModel> colorChannelInfoMap;

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="container"></param>
        public ColormapPanelViewModel(IContainerExtension container) : base(container)
        {
            ExportLabelCommand = new DelegateCommand<string>(ExportLabel);

            EventAggregator.GetEvent<RefreshMetadataEvent>().Subscribe(RefreshMetadata, ThreadOption.UIThread);

            colorChannelInfoMap = Container.Resolve<DataManager>().ColorChannelInfoMap;

            SelectedDAPIColorMap = colorChannelInfoMap[ChannelType.DAPI].ColorMap;
            SelectedGFPColorMap = colorChannelInfoMap[ChannelType.GFP].ColorMap;
            SelectedRFPColorMap = colorChannelInfoMap[ChannelType.RFP].ColorMap;
            SelectedNIRColorMap = colorChannelInfoMap[ChannelType.NIR].ColorMap;

            IsLock = true;
        }

        /// <summary>
        /// OnLoaded
        /// </summary>
        /// <param name="view"></param>
        public void OnLoaded(ColormapPanel view)
        {
        }

        /// <summary>
        /// OnUnloaded
        /// </summary>
        /// <param name="view"></param>
        public void OnUnloaded(ColormapPanel view)
        {
        }

        /// <summary>
        /// 메타데이터 변경 시
        /// </summary>
        /// <param name="param"></param>
        private void RefreshMetadata(DisplayParam param)
        {
            if (!IsLock)
                Reset();
        }

        /// <summary>
        /// Reset
        /// </summary>
        private void Reset()
        {
            DAPIColorMapEnabled = false;
            GFPColorMapEnabled = false;
            RFPColorMapEnabled = false;
            NIRColorMapEnabled = false;
        }

        /// <summary>
        /// 현재 선택된 범례 이미지 다운로드
        /// </summary>
        /// <param name="type"></param>
        private void ExportLabel(string type)
        {
            string fileName = "ColorMap_";

            switch (type)
            {
                case "DAPI":
                    fileName += SelectedDAPIColorMap;
                    break;
                case "GFP":
                    fileName += SelectedGFPColorMap;
                    break;
                case "RFP":
                    fileName += SelectedRFPColorMap;
                    break;
                case "NIR":
                    fileName += SelectedNIRColorMap;
                    break;
            }

            fileName += ".jpg";

            VistaSaveFileDialog dialog = new VistaSaveFileDialog
            {
                DefaultExt = ".jpg",
                Filter = "JPG image file(*.jpg)|*.jpg",
                FileName = fileName
            };

            if (dialog.ShowDialog().GetValueOrDefault())
            {
                Uri uri = new Uri($"pack://application:,,,/Resources/Images/" + fileName, UriKind.RelativeOrAbsolute);

                using (Bitmap bitmap = BitmapImage2Bitmap(new BitmapImage(uri)))
                {
                    bitmap.Save(dialog.FileName, GDIDrawing.Imaging.ImageFormat.Jpeg);
                }
            }
        }

        /// <summary>
        /// BitmapImage To Bitmap
        /// </summary>
        /// <param name="bitmapImage"></param>
        /// <returns></returns>
        private Bitmap BitmapImage2Bitmap(BitmapImage bitmapImage)
        {
            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder encoder = new BmpBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmapImage));
                encoder.Save(outStream);
                Bitmap bitmap = new Bitmap(outStream);

                return new Bitmap(bitmap);
            }
        }
    }
}
