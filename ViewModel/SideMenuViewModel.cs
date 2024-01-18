using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using HandyControl.Controls;
using HandyControl.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LvPlayer.ViewModel
{
    public class SideMenuViewModel : ViewModelBase
    {
        public RelayCommand<FunctionEventArgs<object>> SwitchItemCmd => new Lazy<RelayCommand<FunctionEventArgs<object>>>(() =>
            new RelayCommand<FunctionEventArgs<object>>(SwitchItem)).Value;

       

        private void SwitchItem(FunctionEventArgs<object> info) {
            string header = (info.Info as SideMenuItem)?.Header.ToString();
            MainWindow.mainWindowItem.mainContentChange(header);
        }

      

        public RelayCommand<string> SelectCmd => new Lazy<RelayCommand<string>>(() =>
            new RelayCommand<string>(Select)).Value;

        private void Select(string header) => Growl.Success(header, "InfoMessage");
    }
}
