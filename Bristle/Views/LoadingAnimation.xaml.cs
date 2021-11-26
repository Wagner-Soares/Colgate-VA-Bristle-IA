using Bristle.utils;
using System;
using System.Windows;

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

            Init(type);
        }

        private void Init(string type)
        {
            switch(type)
            {
                case ConfigurationConstants.GeneralConfigurationName:
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
    }
}
