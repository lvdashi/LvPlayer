using Microsoft.WindowsAPICodePack.Dialogs;
using Microsoft.WindowsAPICodePack.Shell.PropertySystem;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LvPlayer.Utils
{
    /// <summary>
    /// 选择文件工具
    /// </summary>
    public class FileSelectUtils
    {
        public static string selectMp4()
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                Title = "选择MP4视频",
                Filter = "视频文件(*.mp4;*.TS;)|*.mp4;*.TS;",
                RestoreDirectory = true,
            };
            if (dialog.ShowDialog() != DialogResult.OK)
            {
                return "";
            }
            
            string filename = dialog.FileName;
            Trace.WriteLine("选择了视频-> "+filename);
            return filename;
        }
        public static string selectVideo()
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                Title = "选择视频",
                Filter = "视频文件(*.mp4;*.lvp)|*.mp4;*.lvp",
                RestoreDirectory = true,
            };
            if (dialog.ShowDialog() != DialogResult.OK)
            {
                return "";
            }

            string filename = dialog.FileName;
            Trace.WriteLine("选择了视频-> " + filename);
            return filename;
        }
        public static IList<FileInfo> selectMp4PathAndGetFiles() {
            string folderName = selectMp4Path();
            IList<FileInfo> files = GetFiles(folderName);
            return files;
           
        }

        public static string selectMp4Path() {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog
            {
                IsFolderPicker = true
            };

            if (dialog.ShowDialog() != CommonFileDialogResult.Ok)
            {
                return "";
            }
            string folderName = dialog.FileName;
            Trace.WriteLine("选择了视频文件夹-> " + folderName);
            return folderName;
        }

        /// <summary>
        /// 遍历当前目录及子目录
        /// </summary>
        /// <param name="strPath">文件路径</param>
        /// <returns>所有文件</returns>
        public static IList<FileInfo> GetFiles(string strPath)
        {
            List<FileInfo> lstFiles = new List<FileInfo>();
            List<string> lstDirect = new List<string>();
            lstDirect.Add(strPath);
            DirectoryInfo diFliles = null;
            GetDirectorys(strPath, ref lstDirect);
            foreach (string str in lstDirect)
            {
                try
                {
                    diFliles = new DirectoryInfo(str);
                    lstFiles.AddRange(diFliles.GetFiles());
                }
                catch
                {
                    continue;
                }
            }
            return lstFiles;
        }
        private static void GetDirectorys(string strPath, ref List<string> lstDirect)
        {
            if (strPath=="") {
                return;
            }
            DirectoryInfo diFliles = new DirectoryInfo(strPath);
            DirectoryInfo[] diArr = diFliles.GetDirectories();

            foreach (DirectoryInfo di in diArr)
            {
                try
                {
                    lstDirect.Add(di.FullName);
                    GetDirectorys(di.FullName, ref lstDirect);
                }
                catch
                {
                    continue;
                }
            }
        }


        public static string selectImg() {
            OpenFileDialog dialog = new OpenFileDialog
            {
                Title = "选择图片",
                Filter = "图像文件(*.jpg;*.png;*.bmp)|*.jpg;*.png;*.bmp",
                RestoreDirectory = true,
            };
            if (dialog.ShowDialog()!=DialogResult.OK)
            {
                return "";
            }
            string filename = dialog.FileName;
            return filename;
        }
    }
}
