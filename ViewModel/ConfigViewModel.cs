using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Ganss.Xss;
using LvPlayer.Model;
using LvPlayer.Utils;
using Masuit.Tools.Security;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LvPlayer.ViewModel
{
    public class ConfigViewModel : ViewModelBase
    {
        private ObservableCollection<string> dataList;
        private ObservableCollection<string> deviceList;
        public RelayCommand<string> ButtonCommand { get; set; }

        private AppConfig config;

        public AppConfig Config
        {
            get { return config; }
            set
            {
                config = value;
                RaisePropertyChanged("Config");
            }
        }


        public ObservableCollection<string> DataList
        {
            get => dataList;
            set => Set(ref dataList, value);
        }
        public ObservableCollection<string> DeviceList
        {
            get => deviceList;
            set => Set(ref deviceList, value);
        }
        public ConfigViewModel()
        {
            DataList = GetDataList();
            DeviceList=GetDeviceDataList();

            Config = new AppConfig {
                AesKey = App.ini.IniReadValue("aes", "decryption_key"), 
                Iv = App.ini.IniReadValue("aes", "iv"),
                Pwd= App.ini.IniReadValue("aes", "pwd"),
                TaskNum= App.ini.IniReadValue("aes", "task_num"),
                OutPath= App.ini.IniReadValue("out", "path"),
                Device= App.ini.IniReadValue("out", "device"),
                UseGpu= App.ini.IniReadValue("out", "gpu")
            };
            if (App.ini.IniReadValue("out", "select_path") == "True")
            {
                Config.CheckSelect = "True";
                Config.CheckSet = "False";
            }
            else
            {
                Config.CheckSelect = "False";
                Config.CheckSet = "True";
            }

            ButtonCommand = new RelayCommand<string>(buttonClick);
        }

        //绑定按钮点击事件
        public void buttonClick(string type) {
            Trace.WriteLine("按钮点击了 -> " + type);
            switch (type)
            {
                case "重新计算加密要素":
                    var key = Config.Pwd.MDString3("lvp");// MD5三次加盐
                    var iv = Config.Pwd.MDString2("lvp");// MD5两次加盐
                    Config.AesKey = key;
                    Config.Iv= iv;

                    return;
                case "保存设置":
                    App.ini.IniWriteValue("aes", "decryption_key", Config.AesKey);
                    App.ini.IniWriteValue("aes", "iv", Config.Iv);
                    App.ini.IniWriteValue("aes", "pwd", Config.Pwd);
                    App.ini.IniWriteValue("aes", "task_num", Config.TaskNum);
                    App.ini.IniWriteValue("out", "path", Config.OutPath);
                    App.ini.IniWriteValue("out", "device", Config.Device);
                    App.ini.IniWriteValue("out", "gpu",Config.UseGpu);
                    App.ini.IniWriteValue("out", "select_path", Config.CheckSelect);

                    return;
                case "选择目录":
                    string path=FileSelectUtils.selectMp4Path();
                    Config.OutPath= path;

                    return;
                default:
                    return;
            }
        }

        private ObservableCollection<string> GetDataList()
        {
            return new ObservableCollection<string>
            {
                "1",
                "2",
                "3",
                "4",
                "5",
            };
        }

        private ObservableCollection<string> GetDeviceDataList()
        {
            return new ObservableCollection<string>
            {
                "cuda",
                "dxva2",
                "qsv",
                "d3d11va",
                "qsv",
                "cuvid",
            };
        }
    }
}
