using HandyControl.Controls;
using LvPlayer.Model;
using LvPlayer.Utils;
using LvPlayer.ViewModel;
using Masuit.Tools.Strings;
using Masuit.Tools.Systems;
using Mpv.NET.API;
using MS.WindowsAPICodePack.Internal;
using Newtonsoft.Json.Linq;
using SixLabors.ImageSharp.ColorSpaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Xabe.FFmpeg;

namespace LvPlayer.UserControl
{
    /// <summary>
    /// TaskControl.xaml 的交互逻辑
    /// </summary>
    public partial class TaskControl 
    {
        private TaskViewModel vm;
        private static List<int> taskIndexList = new List<int>();
        public static TaskControl taskControlInstance;

        private System.Windows.Threading.DispatcherTimer dt = new DispatcherTimer();
        Thread taskThread;

        public TaskControl()
        {
            InitializeComponent();

            vm = new TaskViewModel();

            DataContext = vm;

            taskControlInstance = this;

            taskThread = new Thread(timeTaskRun);
            taskThread.Start();
        }
        async void timeTaskRun()
        {
            dt.Interval = TimeSpan.FromMilliseconds(1000);
            dt.Tick += new EventHandler(timeTask);
            dt.Start();

        }

        /// <summary>
        /// 定时任务，获取未开始加密的任务执行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void timeTask(object sender, EventArgs e)
        {
            //Trace.WriteLine("定时任务");
            object lockThis = new object();

            lock (lockThis)

            {
                int runNum = 0;
                int canAddNum = 0;
                for (int i = 0; i < vm.RunDataList.Count; i++)
                {
                    if (vm.RunDataList[i].IsRun == true)
                    {
                        runNum += 1;
                    }
                }
                //Trace.WriteLine("运行中数量：" + runNum);
                canAddNum = Convert.ToInt32(App.ini.IniReadValue("aes", "task_num")) - runNum;
                //Trace.WriteLine("可以开始执行的数量："+canAddNum);
                int index = 0;
                for (int i = 0; i < vm.RunDataList.Count; i++)
                {
                    if (vm.RunDataList[i].IsFinish == false && vm.RunDataList[i].IsRun == false)
                    {
                        
                        //Trace.WriteLine("剩余未执行清单变化：" + index);
                        if (index<canAddNum) {
                            //Trace.WriteLine("开始执行任务：" + i);
                            index += 1;
                            runConvert(vm.RunDataList[i].InoutFilePath, vm.RunDataList[i].OutFilePath, i + 1);
                        }
                    }
                }
                


            }
            
        }
        /// <summary>
        /// 添加加密任务
        /// </summary>
        /// <param name="inputFile"></param>
        /// <param name="fileSize"></param>
        /// <param name="fileName"></param>
        /// <param name="dirName"></param>
        /// <returns></returns>
        public async Task convertTask(string inputFile,string fileSize,string fileName,string dirName)
        {
            //输出目录
            string outDir = App.ini.IniReadValue("out", "path");

            var libPath = AppDomain.CurrentDomain.BaseDirectory;
            FFmpeg.SetExecutablesPath(libPath+"\\lib");
            taskIndexList.Add(taskIndexList.Count+1);


            string outputPath = "";
            if (App.ini.IniReadValue("out", "select_path")=="True")
            {
                outputPath = inputFile.Replace(".mp4", ".lvp.mp4");
            }
            else {
                outputPath = outDir +"\\"+ fileName.Replace(".mp4", ".lvp.mp4");
            }
            vm.RunDataList.Add(new CommonDataModel { Index = taskIndexList.Count, ImgPath = "pack://application:,,,/Resource/Image/videoLogo.png", Name = fileName, InoutFilePath = inputFile, IsSelected = false, Type = CommonType.Type1,Percent=0,FileSize=fileSize,NameAes= outputPath.Replace(".lvp.mp4", ".lvp"),IsRun=false,IsFinish=false,OutFilePath=outputPath, TaskStatus="等待加密",OutFileDir= dirName });


            
        }
        /// <summary>
        /// 进行视频加密
        /// </summary>
        /// <param name="inputFile"></param>
        /// <param name="outputPath"></param>
        /// <param name="taskIndex"></param>
        public async void runConvert(string inputFile,string outputPath, int taskIndex) {
            vm.RunDataList[taskIndex - 1].IsRun = true;
            vm.RunDataList[taskIndex - 1].TaskStatus = "加密中";

            //如果目标文件已存在，则跳过
            string finishName = outputPath.Replace(".lvp.mp4", ".lvp");
            if (System.IO.File.Exists(finishName)) {
                Trace.WriteLine("已经加密过，跳过");
                vm.RunDataList[taskIndex - 1].TaskTime = "00:00:00";
                vm.RunDataList[taskIndex - 1].IsFinish = true;
                vm.RunDataList[taskIndex - 1].TaskStatus = "加密完成";
                vm.FinishDataList.Add(vm.RunDataList[taskIndex - 1]);
                vm.RunDataList[taskIndex - 1].Percent = 100;
                vm.RunDataList[taskIndex - 1].IsRun = false;
                return;
            }

            var argTemplate = new Template("{{useGpu}} {{deviceFast}} -i {{inputFile}} -vcodec copy -acodec copy -encryption_scheme cenc-aes-ctr -encryption_key {{key}} -encryption_kid {{iv}} {{outputFile}}");
            
            if (App.ini.IniReadValue("out", "gpu")=="True") { //启用硬件加速
                argTemplate.Set("useGpu", "-hwaccel");
                argTemplate.Set("deviceFast", App.ini.IniReadValue("out", "device"));//设置硬件加速
            }
            else
            {
                argTemplate.Set("useGpu", "");
                argTemplate.Set("deviceFast", "");
            }
            string outDir=App.ini.IniReadValue("out", "path");
            //如果输出目录不存在了，自动创建
            if (!Directory.Exists(outDir))
            {
                Directory.CreateDirectory(outDir);
                Trace.WriteLine("自动创建不存在的输出文件夹："+outDir);
            }


            argTemplate.Set("inputFile", "\"" + inputFile + "\"");
            argTemplate.Set("outputFile", "\"" + outputPath + "\"");
            argTemplate.Set("key", App.ini.IniReadValue("aes", "decryption_key"));
            argTemplate.Set("iv", App.ini.IniReadValue("aes", "iv"));
            string arguments = argTemplate.Render();

            Trace.WriteLine("arguments：" + arguments);

            IConversion conversion = FFmpeg.Conversions.New();


            conversion.OnProgress += (sender, args) =>
            {
                var percent = (int)(Math.Round(args.Duration.TotalSeconds / args.TotalLength.TotalSeconds, 2) * 100);

                Application.Current.Dispatcher.Invoke((Action)(() =>
                {
                    //修改进度条
                    vm.RunDataList[taskIndex - 1].Percent = percent;
                }));
            };

            try
            {
                IConversionResult result = await conversion.Start(arguments);
                //Trace.WriteLine("转换耗时：" + result.Duration.ToString().Substring(0, 8));
                vm.RunDataList[taskIndex - 1].TaskTime = result.Duration.ToString().Substring(0, 8);
                vm.RunDataList[taskIndex - 1].IsFinish = true;
                vm.RunDataList[taskIndex - 1].TaskStatus = "加密完成";
                vm.RunDataList[taskIndex - 1].IsRun = false;
                vm.FinishDataList.Add(vm.RunDataList[taskIndex - 1]);
                //修改文件后缀
                if (System.IO.File.Exists(outputPath))
                {
                    try
                    {
                        System.IO.File.Move(outputPath, outputPath.Replace(".lvp.mp4", ".lvp"));
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine(ex.ToString());
                    }

                }
            }
            catch(Exception err)
            {
                Trace.WriteLine("加密异常",err.ToString());
                vm.RunDataList[taskIndex - 1].TaskStatus = "加密异常";
                vm.RunDataList[taskIndex - 1].IsFinish = true;
                vm.RunDataList[taskIndex - 1].IsRun = false;
                return;
            }
            
            //TODO 系统提示，加密完成

            //Trace.WriteLine("转换结果：" + result.StartTime +" - "+result.EndTime +" 持续时间："+result.Duration +" 参数-》"+result.Arguments);

        }

        /// <summary>
        /// 打开输出目录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var item = btn.DataContext as CommonDataModel;
            //Trace.WriteLine(item.OutFileDir);
            FileUtils.OpenFolder(item.OutFileDir);
        }
    }
}
