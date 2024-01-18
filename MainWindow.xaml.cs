using HandyControl.Controls;
using LvPlayer.Model;
using LvPlayer.Properties;
using LvPlayer.UserControl;
using LvPlayer.Utils;
using LvPlayer.ViewModel;
using Masuit.Tools;
using Masuit.Tools.Files;
using Masuit.Tools.Logging;
using Masuit.Tools.Security;
using Microsoft.Extensions.Configuration;
using Mpv.NET.API;
using Mpv.NET.Player;
using SixLabors.ImageSharp.Drawing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Resources;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ToastNotifications;
using Xabe.FFmpeg;
using static System.Collections.Specialized.BitVector32;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using ToastNotifications.Messages;

namespace LvPlayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private MainWindowViewModel vm;
        public static MainWindow? mainWindowItem;

        Player player = new Player();

        public MainWindow()
        {
            mainWinowsInit();

        }
        public MainWindow(string path)
        {
            mainWinowsInit();
            Player player = new Player(path);
            App.Current.Dispatcher.Invoke((Action)(() =>
            {
                mainContent.Children.Add(player);
            }));
        }

        public void mainWinowsInit()
        {
            InitializeComponent();

            mainWindowItem = this;

            windowSet();

            vm = new MainWindowViewModel();
            DataContext = vm;



            ResourceDictionary theme = new ResourceDictionary
            {
                Source = new Uri("pack://application:,,,/Resource/Style/Theme/BaseDark.xaml")
            };
            Resources.MergedDictionaries.Add(theme);
            LogManager.Info("程序启动..." + App.ini.IniReadValue("aes", "iv"));

            MenuContent.Children.Add(new MenuControl());

            //string resourceStr = "pack://application:,,,/Resource/Style/Theme/BaseLight.xaml";
            //UpdataResourceDictionary(resourceStr, 0);
        }


        /// <summary>
        /// 更新主题颜色
        /// </summary>
        /// <param name="resourceStr"></param>
        /// <param name="pos"></param>
        private void UpdataResourceDictionary(string resourceStr, int pos)
        {
            if (pos < 0 || pos > 2)
            {
                return;
            }
            ResourceDictionary resource = new ResourceDictionary
            {
                Source = new Uri(resourceStr)
            };
            int count=Resources.MergedDictionaries.Count;
            //Trace.WriteLine("count=>"+count);
            if (count>=0) {
                Resources.MergedDictionaries.RemoveAt(pos);
                Resources.MergedDictionaries.Insert(pos, resource);
            }
           
        }

        //窗体设置
        public void windowSet()
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;// 窗体居中
            //this.Topmost = true;
            //标题栏设置
            this.WindowStyle = System.Windows.WindowStyle.None;
            middleButton.IsEnabled = false;


        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            // 获取鼠标相对标题栏位置  
            Point position = e.GetPosition(titleBar);
            // 如果鼠标位置在标题栏内，允许拖动  
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (position.X >= 0 && position.X < titleBar.ActualWidth && position.Y >= 0 && position.Y < titleBar.ActualHeight)
                {
                    this.DragMove();
                }
            }

        }
       

        public void mainContentChange(string titleName) {
            mainContent.Children.Clear();
            if (titleName != "播放本地视频") {
                player.stopPlay();

            }
            if (titleName == "播放本地视频")
            {
                
                App.Current.Dispatcher.Invoke((Action)(() =>
                {
                    mainContent.Children.Add(player);
                }));

            }
            else if (titleName == "加密任务")
            {
                mainContent.Children.Add(App.taskControl);
            }
            else if (titleName == "基础设置") { 
                ConfigControl configControl= new ConfigControl();
                App.Current.Dispatcher.Invoke((Action)(() =>
                {
                    mainContent.Children.Add(configControl);
                }));
            }
            else if (titleName == "软件信息")
            {
                mainContent.Children.Add(App.appInfoControl);
            }
        }
       

    }
}
