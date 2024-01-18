using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LvPlayer.Model
{
    public class CommonDataModel : ViewModelBase
    {
        public int Index { get; set; }//序号

        private string name;
        public string Name//视频名称
        {
            get => name;
            set => Set(ref name, value);
        }

        public bool IsRun { get; set; }//是否已运行
        public bool IsFinish { get; set; }//是否已完成
        public string TaskStatus { get; set; }//任务状态
        public string InoutFilePath { get; set; }//输入文件完整路径
        public string OutFilePath { get; set; }//输出文件完整名称
        public string OutFileDir { get; set; }//输出目录
        public string NameAes { get; set; }//加密后视频
        public int Percent { get; set; }//加密百分比
        public string FileSize { get; set; }//视频大小
        public string TaskTime { get; set; }//加密耗时





        public bool IsSelected { get; set; }

        public string Remark { get; set; }

        public CommonType Type { get; set; }

        public string Tag { get; set; }

        public string VideoType { get; set; }

        public string ImgPath { get; set; }

        public ObservableCollection<CommonDataModel> DataList { get; set; }

        // Card
        public string Header { get; set; }

        public string Content { get; set; }

        public string Footer { get; set; }

        public string Path { get; set; }

        // Avatar
        public string DisplayName { get; set; }

        public string Link { get; set; }

        public string AvatarUri { get; set; }

       


    }
}
