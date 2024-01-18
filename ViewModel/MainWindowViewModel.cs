using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using LvPlayer.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace LvPlayer.ViewModel
{
    class MainWindowViewModel : ViewModelBase
    {
        public RelayCommand<string> ButtonCommand { get; set; }

        public MainWindowViewModel()
        {
            ButtonCommand = new RelayCommand<string>(buttonClick);
        }

        

        //绑定按钮点击事件
        public void buttonClick(string type)
        {

            var mainWindow = MainWindow.mainWindowItem;
            switch (type)
            {
                case "最小化":
                    mainWindow.ResizeMode = System.Windows.ResizeMode.NoResize;
                    mainWindow.WindowState = System.Windows.WindowState.Minimized;
                    mainWindow.Hide();
                    mainWindow.Show();
                    return;
                case "恢复默认":
                    mainWindow.ResizeMode = System.Windows.ResizeMode.CanResize;
                    mainWindow.WindowState = System.Windows.WindowState.Normal;
                    mainWindow.Hide();
                    mainWindow.Show();
                    mainWindow.middleButton.IsEnabled = false;
                    mainWindow.bigButton.IsEnabled = true;
                    return;
                case "最大化":
                    mainWindow.ResizeMode = System.Windows.ResizeMode.NoResize;
                    mainWindow.WindowState = System.Windows.WindowState.Maximized;
                    mainWindow.Hide();
                    mainWindow.Show();
                    mainWindow.middleButton.IsEnabled = true;
                    mainWindow.bigButton.IsEnabled = false;
                    return;
                case "退出程序":
                    MessageBoxResult res = HandyControl.Controls.MessageBox.Show("确定退出程序?");
                    if (res == MessageBoxResult.OK)
                    {
                        //    //终止所有线程
                        Environment.Exit(Environment.ExitCode);
                    }
                    return;
                default:
                    return;
            }
        }

    }
}
