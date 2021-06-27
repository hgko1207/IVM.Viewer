using IVM.Studio.ViewModels;
using IVM.Studio.Views;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace IVM.Studio.Services
{
    public class WindowByHistogramService
    {
        private ChannelHistogramWindow[] channelHistogramWindows;

        /// <summary>
        /// 생성자
        /// </summary>
        public WindowByHistogramService()
        {
            channelHistogramWindows = new ChannelHistogramWindow[4] {
                null, null, null, null
            };
        }

        /// <summary>
        /// Histogram 창 열기
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="alwaysTop"></param>
        public void ShowDisplay(int channel, bool alwaysTop)
        {
            if (channel < 0 || channel >= 4)
                return;

            if (channelHistogramWindows[channel] == null)
            {
                channelHistogramWindows[channel] = new ChannelHistogramWindow();
                ChangeOwner(channel, alwaysTop);
            }

            // 저장된 위치 불러오기
            string pos = ConfigurationManager.AppSettings.Get($"Histogram{channel}Position");
            if (pos != null)
            {
                List<double> parsedPosition = pos.Split(';').Select(s => Convert.ToDouble(s)).ToList();
                channelHistogramWindows[channel].Top = parsedPosition[0];
                channelHistogramWindows[channel].Left = parsedPosition[1];
                channelHistogramWindows[channel].Width = parsedPosition[2];
                channelHistogramWindows[channel].Height = parsedPosition[3];
            }

            // 띄우기
            channelHistogramWindows[channel].Show();
            if (channelHistogramWindows[channel].DataContext is ChannelHistogramWindowViewModel vm)
            {
                vm.Title = $"Histogram #{channel + 1}";
                vm.Channel = channel;
            }
        }

        /// <summary>
        /// Display 창 닫기
        /// </summary>
        /// <param name="channel"></param>
        public void CloseDisplay(int channel)
        {
            if (channel < 0 || channel >= 4)
                return;

            if (channelHistogramWindows[channel] != null)
            {
                // 위치 저장
                string key = $"Histogram{channel}Position";
                string value = $"{channelHistogramWindows[channel].Top};{channelHistogramWindows[channel].Left};{channelHistogramWindows[channel].Width};{channelHistogramWindows[channel].Height}";

                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                KeyValueConfigurationElement element = config.AppSettings.Settings[key];
                if (element != null)
                    element.Value = value;
                else
                    config.AppSettings.Settings.Add(key, value);

                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");

                // 닫기
                channelHistogramWindows[channel].Close();
                channelHistogramWindows[channel] = null;
            }
        }

        /// <summary>
        /// DisplayHistogram
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="image"></param>
        public void DisplayHistogram(int channel, ImageSource histogram)
        {
            if (channel < 0 || channel >= 4 || channelHistogramWindows[channel] == null)
                return;

            channelHistogramWindows[channel].Dispatcher.Invoke(() => {
                if (channelHistogramWindows[channel].DataContext is ChannelHistogramWindowViewModel vm)
                {
                    vm.DisplayHistogram = histogram;
                }
            });
        }

        /// <summary>
        /// Owner 전환
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="enableAlwaysTop"></param>
        public void ChangeOwner(int channel, bool enableAlwaysTop)
        {
            if (channel < 0 || channel >= 4 || channelHistogramWindows[channel] == null)
                return;

            if (enableAlwaysTop)
            {
                channelHistogramWindows[channel].Owner = Application.Current.MainWindow;
                channelHistogramWindows[channel].Activate();
            }
            else
            {
                channelHistogramWindows[channel].Owner = null;
                Application.Current.MainWindow.Activate();
            }
        }
    }
}
