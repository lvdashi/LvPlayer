using Ganss.Xss;
using HandyControl.Tools;
using HandyControl.Tools.Converter;
using LvPlayer.Model;
using LvPlayer.Utils;
using LvPlayer.ViewModel;
using Masuit.Tools;
using Masuit.Tools.Files;
using Masuit.Tools.Logging;
using Mpv.NET.API;
using Mpv.NET.Player;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Xabe.FFmpeg;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

namespace LvPlayer.UserControl
{
    /// <summary>
    /// Player.xaml 的交互逻辑
    /// </summary>
    public partial class Player {

        private PlayerViewModel vm;

        public static Player playerInstance;

        public MpvPlayer? player;
        private bool useVideo = false;
        private bool playStatus = false;
        private double allSeconds = 0.0d;//视频时长 秒
        private int percent = 0;//播放进度
        public string videoPath = "";//视频路径

        private KeyboardHookLib _keyboardHook = null;

        public Player()
        {

            InitializeComponent();

            playerInstance = this;
        }
        public Player(string path)
        {
            videoPath = path;

            InitializeComponent();

        }
        public void initMpv() {
            var libPath = AppDomain.CurrentDomain.BaseDirectory;

            Trace.WriteLine("初始化播放器");
            player = new MpvPlayer(PlayerHost.Handle, libPath + "\\lib\\mpv-1.dll")
            {
                Loop = true,
                Volume = 50,
                AutoPlay = true
            };

            player.MediaStartedBuffering += Player_MediaStartedBuffering;
            player.MediaLoaded += Player_MediaLoaded;
            player.MediaError += Player_MediaError;
            player.MediaUnloaded += Player_MediaUnloaded;
            player.PositionChanged += Player_PositionChanged;
            Trace.WriteLine("播放器注册事件");


            player.API.SetOptionString("demuxer-lavf-o", "decryption_key=" + App.ini.IniReadValue("aes", "decryption_key"));
            Trace.WriteLine("播放器设置解密参数");

        }


        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            Trace.WriteLine("初始化播放器开始------------------------------");
            initMpv();
            Trace.WriteLine("初始化播放器结束------------------------------");

            vm = new PlayerViewModel();

            DataContext = vm;
            this.Dispatcher.Invoke(new Action(delegate
            {
                vm.VideoTitle = "播放器就绪...";
            }));

            if (videoPath!="") {
                playVideo(videoPath);
                //页面加载完毕
                this.Dispatcher.Invoke(new Action(delegate
                {
                    vm.VideoTitle = "视频载入中...";
                }));
                Trace.WriteLine("播放视频------------------------------");
            }
            

        }
        

        [Obsolete]
        public async void playVideo(string path) {
            //    //安装键盘勾子
            _keyboardHook = new KeyboardHookLib();
            _keyboardHook.InstallHook(this.KeyPress);

            var libPath = AppDomain.CurrentDomain.BaseDirectory;
            FFmpeg.SetExecutablesPath(libPath + "\\lib");


            //获取视频信息
            try {
                IMediaInfo mediaInfo = await FFmpeg.GetMediaInfo(path);
                Trace.WriteLine("播放 -> " + path + " 时长 -> " + mediaInfo.Duration.TotalSeconds);
                allSeconds = mediaInfo.Duration.TotalSeconds;
                this.Dispatcher.Invoke(new Action(delegate
                {
                    vm.VideoTime = mediaInfo.Duration.ToString().Substring(0, 8);
                }));
            }
            catch(Exception ex)
            {
                Trace.WriteLine("获取视频信息异常");
            }
           
           

           

            try
            {
                FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
                {
                    Trace.WriteLine("播放器加载视频资源 -》" + path + "  -->" + player);
                    vm.IsPlay = true;
                    player.Load(path, false);
                }
                
            }
            catch (Exception e)
            {
                Trace.WriteLine("播放器加载视频资源出错-》" + e.ToString());
            }
            //Thread.Sleep(1000);
            //if (player.IsMediaLoaded)
            //{
            //    player.Resume();
            //}


            playStatus = true; useVideo = true;


        }

        private void KeyPress(KeyboardHookLib.HookStruct param, out bool handle)
        {
            handle = false; //预设不拦截任何键
            try
            {
                Trace.WriteLine("键盘按下了->" + param.vkCode + " -> " + param.flags);
                if (param.vkCode == 32 && param.flags == 0)//空格
                {
                    if (playStatus)
                    {
                        player.Pause();
                        playStatus = false;
                        vm.IsPlay = false;
                    }
                    else
                    {
                        player.Resume();
                        playStatus = true;
                        vm.IsPlay = true;
                    }

                }
                else if (param.vkCode == 39 && param.flags == 1)//方向右
                {
                    ChangeProgress(true);
                }
                else if (param.vkCode == 37 && param.flags == 1)//方向左
                {
                    ChangeProgress(false);
                }
                else if (param.vkCode == 38 && param.flags == 1)//方向上
                {
                    if (vm.SoundVal <= 95)
                    {
                        vm.SoundVal += 5;
                    }

                }
                else if (param.vkCode == 40 && param.flags == 1)//方向下
                {
                    if (vm.SoundVal >= 5)
                    {
                        vm.SoundVal -= 5;
                    }
                }
            }
            catch(Exception ex)
            {
                Trace.WriteLine("键盘控制异常");
            }
            
        }
        //修改视频进度
        public void ChangeProgress(bool isAdd) {
            if (player != null)
            {
                try
                {
                    TimeSpan time;
                    if (isAdd) {
                        time = new TimeSpan(player.Position.Hours, player.Position.Minutes, player.Position.Seconds) + TimeSpan.FromSeconds(15);
                    }
                    else
                    {
                        time = new TimeSpan(player.Position.Hours, player.Position.Minutes, player.Position.Seconds) - TimeSpan.FromSeconds(15);
                    }
                    player.Position = time;
                }
                catch (Exception ex)
                {
                    Trace.WriteLine("超过视频范围，不修改");
                }
            }
        }

        private void Player_MediaUnloaded(object? sender, EventArgs e)
        {
            Trace.WriteLine("视频播放完，取消加载视频资源 .....");
        }

        private void Player_MediaError(object? sender, EventArgs e)
        {
            Trace.WriteLine("MediaError .....");
            this.Dispatcher.Invoke(new Action(delegate
            {
                vm.VideoTitle = "视频加载出错...";
            }));
        }

       
        private void Player_MediaLoaded(object? sender, EventArgs e)
        {
            Trace.WriteLine("视频加载完成，开始播放 .....");
            this.Dispatcher.Invoke(new Action(delegate
            {
                vm.VideoTitle = "当前播放：" + player.MediaTitle;
            }));

            
        }

        private void Player_MediaStartedBuffering(object? sender, EventArgs e)
        {
            Trace.WriteLine("MediaStartedBuffering .....");
        }

        private void Player_PositionChanged(object? sender, MpvPlayerPositionChangedEventArgs e)
        {
            try
            {
                //Trace.WriteLine("播放进度更新");
                int hour = player.Position.Hours;
                int minutes = player.Position.Minutes;
                int second = player.Position.Seconds;
                this.Dispatcher.Invoke(new Action(delegate
                {
                    vm.PlayTime = hour.ToString().PadLeft(2, '0') + ":" + minutes.ToString().PadLeft(2, '0') + ":" + second.ToString().PadLeft(2, '0');
                    percent = GetResult(player.Position.Duration().TotalSeconds * 100, allSeconds);
                    //更新进度条
                    vm.RateVal = percent;
                }));
            }
            catch (Exception ex)
            {
                //播放错误提示，停止播放
                MessageBoxResult res = HandyControl.Controls.MessageBox.Show("视频播放出错了");
                if (res == MessageBoxResult.OK)
                {
                    stopPlay();
                }
               
            }
            
           
        }

        public int GetResult(double a, double b)
        {
            double temp = System.Math.IEEERemainder(a, b);
            if (Math.Abs(temp) < 2E-10)
            {
                return (int)Math.Ceiling(a / b);
            }
            else
            {
                return (int)(a / b);
            }
        }

        private void playButton_Click(object sender, RoutedEventArgs e)
        {
            this.Dispatcher.Invoke(new Action(delegate
            {
                if (player==null || !useVideo)
                {
                    return;
                }
                if (playStatus)
                {
                    player.Pause();
                    playStatus = false;
                    vm.IsPlay = false;
                }
                else
                {
                    player.Resume();
                    playStatus = true;
                    vm.IsPlay = true;
                }
            }));
            

        }

        /// <summary>
        /// 停止播放视频
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void stopButton_Click(object sender, RoutedEventArgs e)
        {

            stopPlay();
        }

        public void stopPlay() {
            Trace.WriteLine("点击了停止播放视频");
            this.Dispatcher.Invoke(new Action(delegate
            {
                player.Stop();
                vm.PlayTime = "00:00:00";
                vm.VideoTime = "00:00:00";
                vm.VideoTitle = "";
                vm.SoundVal = 50;
                vm.RateVal = 0.0;
                vm.IsPlay = false;
            }));
        }

        private void fullScreanButton_Click(object sender, RoutedEventArgs e)
        {
            
                
        }

        private void SoundProgress_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            this.Dispatcher.Invoke(new Action(delegate
            {
                double sound = SoundProgress.Value;
                if (player != null) {
                    player.Volume = Convert.ToInt32(sound);
                }
            }));
           
        }


       

       
    }
}
