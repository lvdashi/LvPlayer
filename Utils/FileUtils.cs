using Microsoft.Win32;
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
    public class FileUtils
    {


        /// <summary>
        /// byte转换为GB/MB/KB
        /// </summary>
        /// <param name="KSize"></param>
        /// <returns></returns>
        public static Dictionary<string, double> ByteConversionGBMBKB(long KSize)
        {
            var dic = new Dictionary<string, double>();
            int GB = 1024 * 1024 * 1024;//定义GB的计算常量
            int MB = 1024 * 1024;//定义MB的计算常量
            int KB = 1024;//定义KB的计算常量

            if (KSize / GB >= 1)//如果当前Byte的值大于等于1GB
            {
                dic.Add("GB", Math.Round(KSize / (float)GB, 2)); //将其转换成GB
            }
            else if (KSize / MB >= 1)//如果当前Byte的值大于等于1MB
            {
                dic.Add("MB", Math.Round(KSize / (float)MB, 2)); //将其转换成MB
            }
            else if (KSize / KB >= 1)//如果当前Byte的值大于等于1KB
            {
                dic.Add("KB", Math.Round(KSize / (float)KB, 2)); //将其转换成KB
            }
            else
            {
                dic.Add("Byte", KSize);  //显示Byte值
            }

            return dic;
        }

        /// <summary>
        /// 因为C#提供的文件的大小是以B为单位的，所以显示文件大小的时候会出现一大串数字很不方便
        /// 所以，该函数为了方便地显示文件大小而出现
        /// 函数说明，
        ///     如果文件大小是0-1024B 以内的   显示以B为单位
        ///     如果文件大小是1KB-1024KB之间的 显示以KB为单位
        ///     如果文件大小是1M-1024M之间的   显示以M为单位
        ///     如果文件大小是1024M以上的      显示以GB为单位
        /// </summary>
        /// <param name="lengthOfDocument"> 文件的大小 单位：B 类型：long</param>
        /// <returns></returns>
        static string GetLength(long lengthOfDocument)
        {

            if (lengthOfDocument < 1024)
                return string.Format(lengthOfDocument.ToString() + 'B');
            else if (lengthOfDocument > 1024 && lengthOfDocument <= Math.Pow(1024, 2))
                return string.Format((lengthOfDocument / 1024.0).ToString() + "KB");
            else if (lengthOfDocument > Math.Pow(1024, 2) && lengthOfDocument <= Math.Pow(1024, 3))
                return string.Format((lengthOfDocument / 1024.0 / 1024.0).ToString() + "M");
            else
                return string.Format((lengthOfDocument / 1024.0 / 1024.0 / 1024.0).ToString() + "GB");
        }

        /// <summary>
        /// 获取文件MD5值
        /// </summary>
        /// <param name="file">文件绝对路径</param>
        /// <returns>MD5值</returns>
        public static string GetMD5HashFromFile(string file)
        {
            try
            {
                FileStream fileStream = new FileStream(file, FileMode.Open, FileAccess.Read);
                System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
                byte[] retVal = md5.ComputeHash(fileStream);
                fileStream.Close();
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }
                return sb.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("获取文件MD5值error:" + ex.Message);
            }
        }

        public static bool SetFileExtend()

        {

            try

            {

                string appPath = Application.ExecutablePath;

                string extendName = ".lvp"; //自定义的文件后缀名

                string extendDefaultSet = "LVP";

                string directions = "LvPlayer";



                if (Registry.ClassesRoot.OpenSubKey(extendName) != null)

                {

                    Trace.WriteLine("关联已经创建！");

                    return false;

                }



                RegistryKey key;

                //1 设置自定义文件的双击打开

                key = Registry.ClassesRoot.CreateSubKey(extendDefaultSet);

                key.SetValue("", directions);

                key = key.CreateSubKey("shell");

                key = key.CreateSubKey("open");

                key = key.CreateSubKey("command");

                key.SetValue("", string.Format("{0} %1", appPath));



                //2 设置自定义文件的默认图标  --这点测试时有点问题，还有待研究

                key = Registry.ClassesRoot.OpenSubKey(extendDefaultSet, true);

                key = key.CreateSubKey("DefaultIcon");

                key.SetValue("", string.Format("{0},0", appPath));



                //3 新增的扩展名和设置关联

                key = Registry.ClassesRoot.CreateSubKey(extendName);

                key.SetValue("", extendDefaultSet);





                Trace.WriteLine("关联创建成功！");

                return true;

            }

            catch (Exception ex)

            {

                Trace.WriteLine(ex.Message);

                return false;

            }

        }
        /// <summary>
        /// 打开文件夹
        /// </summary>
        /// <param name="folderPath"></param>
        public static void OpenFolder(string folderPath)
        {
            if (string.IsNullOrEmpty(folderPath)) return;

            Process process = new Process();
            ProcessStartInfo psi = new ProcessStartInfo("Explorer.exe");
            psi.Arguments = folderPath;
            process.StartInfo = psi;

            try
            {
                process.Start();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                process?.Close();

            }

        }


    }
}
