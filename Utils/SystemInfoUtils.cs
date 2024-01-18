using Masuit.Tools.Hardware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace LvPlayer.Utils
{
    public class SystemInfoUtils
    {
        public static void getSystemInfo() {
            float load = SystemInfo.CpuLoad;// 获取CPU占用率
            long physicalMemory = SystemInfo.PhysicalMemory;// 获取物理内存总数
            long memoryAvailable = SystemInfo.MemoryAvailable;// 获取物理内存可用率
            double freePhysicalMemory = SystemInfo.GetFreePhysicalMemory();// 获取可用物理内存
            double temperature = SystemInfo.GetCPUTemperature();// 获取CPU温度
            int cpuCount = SystemInfo.GetCpuCount();// 获取CPU核心数
            List<UnicastIPAddressInformation> ipAddress = SystemInfo.GetLocalIPs();// 获取本机所有IP地址
            IList<string> macAddress = SystemInfo.GetMacAddress();// 获取本机所有网卡mac地址
            IPAddress localUsedIp = SystemInfo.GetLocalUsedIP();// 获取本机当前正在使用的IP地址
            //string osVersion = SystemInfo.GetOsVersion();// 获取操作系统版本
            
            RamInfo ramInfo = SystemInfo.GetRamInfo();// 获取内存信息
            var cpuSN = SystemInfo.GetCpuInfo()[0].SerialNumber; // CPU序列号
            var driveSN = SystemInfo.GetDiskInfo()[0].SerialNumber; // 硬盘序列号

            // 快速方法
            var cpuInfos = CpuInfo.Locals; // 快速获取CPU的信息
            var ramInfoFast = RamInfo.Local; // 快速获取内存的信息
            var diskInfos = DiskInfo.Locals; // 快速获取硬盘的信息
            var biosInfo = BiosInfo.Local; // 快速获取主板的信息
        }
    }
}
