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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Bristle.Views
{
    /// <summary>
    /// Interação lógica para LoadingAnimation.xam
    /// </summary>
    public partial class LoadingAnimation : Window
    {
        public LoadingAnimation(string type)
        {
            InitializeComponent();

            init(type);
        }

        private void init(string type)
        {
            switch(type)
            {
                case "generalSettings":
                    tex.Content = "saving ...";
                    break;
                case "other":
                    tex.Content = "loading ...";
                    break;
            }

        }

        public void CloseMe()
        {

            Dispatcher.BeginInvoke((Action)(() => { Close(); }));
        }
        private void t()
        {

        }
    }
}
