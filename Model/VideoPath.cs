using SixLabors.ImageSharp.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LvPlayer.Model
{
    public class VideoPath
    {
       

        //视频文件夹路径
        public string path { get; set; }

        //路径标签
        public string tag { get; set; }

        public bool useFlag { get; set; }
        //public VideoPath(string path, string tag)
        //{
        //    this.path = path;
        //    this.tag = tag;
        //}
    }
}
