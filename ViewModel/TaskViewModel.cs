using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using LvPlayer.Model;
using LvPlayer.UserControl;
using LvPlayer.Utils;
using Microsoft.VisualBasic.Logging;
using Newtonsoft.Json.Linq;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ToastNotifications;
using ToastNotifications.Messages;

namespace LvPlayer.ViewModel
{
    class TaskViewModel : ViewModelBase, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;


        private ObservableCollection<CommonDataModel> runDataList;
        private ObservableCollection<CommonDataModel> finishDataList;

        public RelayCommand<string> ButtonCommand { get; set; }

        Thread thread;

        public ObservableCollection<CommonDataModel> RunDataList
        {
            get => runDataList;
            set
            {
                this.runDataList = value;
                RaisePropertyChanged("RunDataList");
            }
        }

        public ObservableCollection<CommonDataModel> FinishDataList
        {
            get => finishDataList;
            set
            {
                this.finishDataList = value;
                RaisePropertyChanged("FinishDataList");
            }
        }





        public TaskViewModel()
        {
            RunDataList = GetRunDataList();
            FinishDataList = GetFinishDataList();

            ButtonCommand = new RelayCommand<string>(buttonClick);
        }


        
        private ObservableCollection<CommonDataModel> GetRunDataList()
        {
            return new ObservableCollection<CommonDataModel> {

            };
        }

        private ObservableCollection<CommonDataModel> GetFinishDataList()
        {
            return new ObservableCollection<CommonDataModel> { };
        }
        //绑定按钮点击事件
        public void buttonClick(string type)
        {
            switch (type)
            {
                case "新建任务":
                    string mp4 = FileSelectUtils.selectMp4();
                    if (mp4!="") {
                        FileInfo file = new FileInfo(mp4);
                        Dictionary<string, double> fileSize = FileUtils.ByteConversionGBMBKB((long)file.Length);

                        Task task = TaskControl.taskControlInstance.convertTask(mp4, fileSize.Values.First() + " " + fileSize.Keys.First(), file.Name, file.DirectoryName);

                    }

                    return;
                case "批量添加任务":
                    addBatchTask();
                    return;
                case "清空所有":
                    for (int i = RunDataList.Count-1; i >=0; i--)
                    {
                        try
                        {
                            FinishDataList.RemoveAt(i);
                        }
                        catch (Exception ex)
                        {
                            Trace.WriteLine(ex);
                        }
                        RunDataList.RemoveAt(i);
                    }
                    return;
                case "清除文件夹内原视频":
                    floderClear();
                    return;
                default:
                    return;
            }
        }

        public async void floderClear() {
            IList<FileInfo> files = FileSelectUtils.selectMp4PathAndGetFiles();

            foreach (FileInfo file in files)
            {
                Dictionary<string, double> fileSize = FileUtils.ByteConversionGBMBKB((long)file.Length);
                //Trace.WriteLine("文件-> " + file.FullName + "  ," + fileSize.Values.First() +" "+fileSize.Keys.First());

                //文件重命名
                if (file.FullName.EndsWith(".lvp.mp4"))
                {
                    System.IO.File.Move(file.FullName, file.FullName.Replace(".lvp.mp4", ".lvp"));
                }
            }
            foreach (FileInfo file in files)
            {
                Dictionary<string, double> fileSize = FileUtils.ByteConversionGBMBKB((long)file.Length);
                //Trace.WriteLine("文件-> " + file.FullName + "  ," + fileSize.Values.First() +" "+fileSize.Keys.First());

                if (file.FullName.EndsWith(".mp4"))
                {
                    System.IO.File.Delete(file.FullName);
                    //Trace.WriteLine("删除文件-> " + file.FullName);
                }
            }
            App.notifier.ShowInformation("处理完成");
        }

        public async void addBatchTask() {
            IList<FileInfo> files = FileSelectUtils.selectMp4PathAndGetFiles();

            foreach (FileInfo file in files)
            {
                Dictionary<string, double> fileSize = FileUtils.ByteConversionGBMBKB((long)file.Length);
                //Trace.WriteLine("文件-> " + file.FullName + "  ," + fileSize.Values.First() +" "+fileSize.Keys.First());

                //TS文件重命名
                if (file.FullName.EndsWith(".TS"))
                {
                    System.IO.File.Move(file.FullName, file.FullName.Replace(".TS", ".mp4"));
                }

                if (file.FullName.EndsWith(".mp4"))
                {
                    TaskControl.taskControlInstance.convertTask(file.FullName, fileSize.Values.First() + " " + fileSize.Keys.First(), file.Name, file.DirectoryName);
                }
            }
        }

    }
}
