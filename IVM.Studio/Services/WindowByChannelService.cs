using IVM.Studio.Models;
using IVM.Studio.ViewModels;
using IVM.Studio.Views;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Windows;
using System.Windows.Media;

/**
 * @Class Name : WindowByChannelService.cs
 * @Description : 채널별 윈도우 관리 서비스
 * @author 고형균
 * @since 2021.04.04
 * @version 1.0
 */
namespace IVM.Studio.Services
{
    public class WindowByChannelService
    {
        private ChannelViewerWindow[] channelViewerWindows;

        /// <summary>
        /// 생성자
        /// </summary>
        public WindowByChannelService()
        {
            channelViewerWindows = new ChannelViewerWindow[4] {
                null, null, null, null
            };
        }

        /// <summary>
        /// Display 창 열기
        /// </summary>
        /// <param name="index"></param>
        /// <param name="channelType"></param>
        /// <param name="alwaysTop"></param>
        public void ShowDisplay(int index, ChannelType channelType, bool alwaysTop)
        {
            if (index < 0 || index >= 4)
                return;

            if (channelViewerWindows[index] == null)
            {
                channelViewerWindows[index] = new ChannelViewerWindow();
                ChangeOwner(index, alwaysTop);
            }

            // 저장된 위치 불러오기
            string pos = ConfigurationManager.AppSettings.Get($"Channel{index}Position");
            if (pos != null)
            {
                List<double> parsedPosition = pos.Split(';').Select(s => Convert.ToDouble(s)).ToList();
                channelViewerWindows[index].Top = parsedPosition[0];
                channelViewerWindows[index].Left = parsedPosition[1];
                channelViewerWindows[index].Width = parsedPosition[2];
                channelViewerWindows[index].Height = parsedPosition[3];
            }

            // 띄우기
            channelViewerWindows[index].Show();
            if (channelViewerWindows[index].DataContext is ChannelViewerWindowViewModel vm)
            {
                vm.Title = $"Display #{index + 1}";
                vm.Channel = index;
                vm.ChannelType = channelType;
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

            if (channelViewerWindows[channel] != null)
            {
                // 위치 저장
                string key = $"Channel{channel}Position";
                string value = $"{channelViewerWindows[channel].Top};{channelViewerWindows[channel].Left};{channelViewerWindows[channel].Width};{channelViewerWindows[channel].Height}";

                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                KeyValueConfigurationElement element = config.AppSettings.Settings[key];
                if (element != null)
                    element.Value = value;
                else 
                    config.AppSettings.Settings.Add(key, value);

                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");

                // 닫기
                channelViewerWindows[channel].Close();
                channelViewerWindows[channel] = null;
            }
        }

        /// <summary>
        /// DisplayImage
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="image"></param>
        public void DisplayImage(int channel, ImageSource image)
        {
            if (channel < 0 || channel >= 4 || channelViewerWindows[channel] == null)
                return;

            channelViewerWindows[channel].Dispatcher.Invoke(() => {
                if (channelViewerWindows[channel].DataContext is ChannelViewerWindowViewModel vm)
                {
                    vm.DisplayImage = image;
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
            if (channel < 0 || channel >= 4 || channelViewerWindows[channel] == null)
                return;

            if (enableAlwaysTop)
            {
                channelViewerWindows[channel].Owner = Application.Current.MainWindow;
                channelViewerWindows[channel].Activate();
            }
            else
            {
                channelViewerWindows[channel].Owner = null;
                Application.Current.MainWindow.Activate();
            }
        }
    }
}
