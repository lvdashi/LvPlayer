using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using LvPlayer.UserControl;
using LvPlayer.Utils;
using Masuit.Tools.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace LvPlayer.ViewModel
{
    class PlayerViewModel : ViewModelBase
    {

        private string playTime;
        private string videoTime;
        private string videoTitle;
        private int soundVal;
        private double rateVal;
        private bool isPlay;
        private string showNoSound;
        private string showSound;

        public RelayCommand<string> ButtonCommand { get; set; }//按钮点击

        public bool IsPlay
        {
            get { return isPlay; }
            set
            {
                isPlay = value;
                RaisePropertyChanged("IsPlay");
            }
        }
        public string ShowSound
        {
            get { return showSound; }
            set
            {
                showSound = value;
                RaisePropertyChanged("ShowSound");
            }
        }
        public string ShowNoSound
        {
            get { return showNoSound; }
            set
            {
                showNoSound = value;
                RaisePropertyChanged("ShowNoSound");
            }
        }
        public string PlayTime {
            get { return playTime; }
            set
            {
                playTime = value;
                RaisePropertyChanged("PlayTime");
            }
        }

        public string VideoTime
        {
            get { return videoTime; }
            set
            {
                videoTime = value;
                RaisePropertyChanged("VideoTime");
            }
        }

        public string VideoTitle
        {
            get { return videoTitle; }
            set
            {
                videoTitle = value;
                RaisePropertyChanged("VideoTitle");
            }
        }

        public int SoundVal
        {
            get { return soundVal; }
            set
            {
                soundVal = value;
                RaisePropertyChanged("SoundVal");
            }
        }
        public double RateVal
        {
            get { return rateVal; }
            set
            {
                rateVal = value;
                RaisePropertyChanged("RateVal");
            }
        }

        public PlayerViewModel() {
            PlayTime = "00:00:00";
            VideoTime = "00:00:00";
            VideoTitle = "";
            SoundVal = 50;
            RateVal = 0.0;
            IsPlay= false;
            ShowNoSound = "Collapsed";
            ShowSound = "Visible";

            ButtonCommand = new RelayCommand<string>(buttonClick);
        }

        //绑定按钮点击事件
        public void buttonClick(string type) {
            var player = Player.playerInstance.player;
            switch (type)
            {
                case "打开本地视频":
                    string video = FileSelectUtils.selectVideo();
                    if (video != null)
                    {
                        Player.playerInstance.stopPlay();
                        Player.playerInstance.videoPath = video;
                        Player.playerInstance.playVideo(video);
                    }
                    return;
                case "恢复默认":
                   
                    return;
                case "最大化":
                   
                    return;
                case "退出程序":
                  
                    return;
                case "启用声音":
                    ShowNoSound = "Visible";
                    ShowSound = "Collapsed";
                    SoundVal = 0;

                    return;
                case "静音模式":
                    ShowNoSound = "Collapsed";
                    ShowSound = "Visible";
                    SoundVal = 50;

                    return;
                case "快进":
                    Player.playerInstance.ChangeProgress(true);
                    return;
                case "后退":
                    Player.playerInstance.ChangeProgress(false);
                    return;
                default:
                    return;
            }
        }
    }
}
