using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LvPlayer.Model
{
    public class AppConfig : INotifyPropertyChanged
    {
        public string AesKey { get; set; }
        public string Iv { get; set; }
        public string Pwd { get; set; }
        public string CheckSelect { get; set; }
        public string CheckSet { get; set; }
        public string TaskNum { get; set; }
        public string Device { get; set; }
        public string UseGpu { get; set; }
        /// <summary>
        /// 输出目录
        /// </summary>
        public string OutPath { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
