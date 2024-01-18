using LvPlayer.UserControl;
using LvPlayer.Utils;
using Masuit.Tools.Files;
using Masuit.Tools.Logging;
using Mpv.NET.API;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using ToastNotifications;
using ToastNotifications.Lifetime;
using ToastNotifications.Messages;
using ToastNotifications.Position;

namespace LvPlayer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        //Toast提示
        public static Notifier notifier;
        //配置文件
        public static INIFile? ini;

        public static TaskControl taskControl;
        public static AppInfoControl appInfoControl;
        /// <summary>
        /// 程序启动监听器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Application_Startup(object sender, StartupEventArgs e)
        {
            LogManager.Info("initLogConfig...");
            initLogConfig();
            LogManager.Info("initConfigFile...");
            initConfigFile();
           
            LogManager.Info("SetFileExtend...");
            FileUtils.SetFileExtend();

            bool FileLoadApp = false;//是否文件启动本程序

            String[] arguments = Environment.GetCommandLineArgs();

            if (arguments.GetLength(0) > 1)
            {
                if (arguments[1].EndsWith(".lvp"))
                {
                    string filePathFormMainArgs = arguments[1];//视频文件路径

                    LogManager.Info("有参数，直接打开播放");
                    FileLoadApp = true;
                    MainWindow window= new MainWindow(filePathFormMainArgs);
                    window.Show();
                }
                else
                {
                    LogManager.Info("没有参数，打开默认页面");
                    MainWindow window = new MainWindow();
                    window.Show();
                }
            }
            else
            {
                LogManager.Info("没有参数，打开默认页面");
                MainWindow window = new MainWindow();
                window.Show();
            }
            if (!FileLoadApp)
            {
                LogManager.Info("initDb...");
                initDb();
            }

            notifier = new Notifier(cfg =>
            {
                cfg.PositionProvider = new WindowPositionProvider(
                    parentWindow: Application.Current.MainWindow,
                    corner: Corner.TopRight,
                    offsetX: 10,
                    offsetY: 10);

                cfg.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(
                    notificationLifetime: TimeSpan.FromSeconds(3),
                    maximumNotificationCount: MaximumNotificationCount.FromCount(5));

                cfg.Dispatcher = Application.Current.Dispatcher;
            });

            taskControl = new UserControl.TaskControl();
            appInfoControl = new UserControl.AppInfoControl();
        }
        //如果没有配置，初始化默认配置
        public void initConfig() {
            Trace.WriteLine("初始化配置："+ ini.IniReadValue("aes", "decryption_key"));
            if(ini.IniReadValue("aes", "decryption_key")=="")
            {
                ini.IniWriteValue("aes", "decryption_key", "206f9fc69ca44de0a391730e473ea3e7");
            }
            if (ini.IniReadValue("aes", "iv") == "")
            {
                ini.IniWriteValue("aes", "iv", "e591d3eb7ab04f1fae9b93d11e44b267");
            }
            if (ini.IniReadValue("out", "select_path") == "")
            {
                ini.IniWriteValue("out", "select_path", "True");
            }
            if (ini.IniReadValue("aes", "task_num") == "")
            {
                ini.IniWriteValue("aes", "task_num", "2");
            }
            if (ini.IniReadValue("out", "device") == "")
            {
                ini.IniWriteValue("out", "device", "cuda");
            }
            if (ini.IniReadValue("out", "gpu") == "")
            {
                ini.IniWriteValue("out", "gpu", "True");
            }
            
        }

        /// <summary>
        /// 初始化数据库
        /// </summary>
        public void initDb()
        {
            var path = AppDomain.CurrentDomain.BaseDirectory;
            path = System.IO.Path.Combine(path, "app.db");
            if (!File.Exists(path))
            { //说明没有初始化过数据库
                SqliteUtil.connectDb();
                Trace.WriteLine("没有使用过db,初始化表");
                SqliteUtil.initTable();
            }
            else
            {
                SqliteUtil.connectDb();
            }
        }

        /// <summary>
        /// 日志初始化
        /// </summary>
        public void initLogConfig() {
            //注册日志组件
            LogManager.LogDirectory = AppDomain.CurrentDomain.BaseDirectory + "/logs";
        }
        /// <summary>
        ///初始化配置文件
        /// </summary>
        /// <returns></returns>
        public string initConfigFile()
        {
            var configPath = AppDomain.CurrentDomain.BaseDirectory + "config";
            if (!Directory.Exists(configPath))
            {
                Directory.CreateDirectory(configPath);
            }
            var configFile = AppDomain.CurrentDomain.BaseDirectory + "config\\LvpConfig.ini";
            if (!File.Exists(configFile))
            {
                File.Create(configFile);
            }
            Thread.Sleep(200);
            ini = new INIFile(configFile);
            Thread.Sleep(200);
            initConfig();
            return configFile;
        }
    }
}
