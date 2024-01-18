using AngleSharp.Css.Dom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace LvPlayer.UserControl
{
    /// <summary>
    /// LoadWindow.xaml 的交互逻辑
    /// </summary>
    public partial class LoadWindow : Window
    {
        Action handler;

        public LoadWindow(string title, Action handler)
        {
            InitializeComponent();

            this.txtTitle.Text = title;
            this.handler = handler;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            handler?.BeginInvoke(OnComplate, null);
        }

        private void OnComplate(IAsyncResult ar)
        {
            this.Dispatcher.Invoke(new Action(() => { this.Close(); }));
        }

        public static void ShowLoading(string title, Action handler, Window owner)
        {
            var loading = new LoadWindow(title, handler) { Owner = owner };

            loading.ShowDialog();
        }
    }
}
