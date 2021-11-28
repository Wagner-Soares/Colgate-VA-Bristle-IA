using APIVision;
using APIVision.Controllers;
using APIVision.DataModels;
using Bristle.UseCases;
using Database;
using Microsoft.Expression.Encoder.Devices;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
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
    /// Interação lógica para MainWindow.xam
    /// </summary>
    public partial class About : Window
    {
        private bool maximized = true;
        private readonly BusinessSystem businessSystem;
        private readonly bool maximized__ = false;
        readonly ColgateSkeltaEntities _colgateSkeltaEntities;


        public About(bool maximized_, BusinessSystem businessSystem_, ColgateSkeltaEntities colgateSkeltaEntities)
        {
            businessSystem = businessSystem_;
            maximized__ = maximized_;

            _colgateSkeltaEntities = colgateSkeltaEntities;

            InitializeComponent();
            Loaded += About_Loaded;         
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void About_Loaded(object sender, RoutedEventArgs e)
        {
            if (maximized__)
            {
                this.WindowState = WindowState.Maximized;
                maximized = true;
            }
            else
            {
                this.WindowState = WindowState.Normal;
                maximized = false;
            }
        }

        private void ButtonWindowMaximize_Click(object sender, RoutedEventArgs e)
        {
            if (!maximized)
            {               
                this.WindowState = WindowState.Maximized;
                maximized = true;
            }
            else
            {                
                this.WindowState = WindowState.Normal;
                maximized = false;
            }
        }

        private void V_MicroTouchPress(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void ButtonWindowMinimize_Click(object sender, RoutedEventArgs e)
        {
                this.WindowState = WindowState.Minimized;
        }

        private void ButtonNeuralNetworkRetraining_Click(object sender, RoutedEventArgs e)
        {
            if(businessSystem.UserSystemCurrent.Type == "administrator")
            {
                Views.NeuralNetworkRetraining neuralNetworkRetraining = new NeuralNetworkRetraining(maximized, businessSystem, _colgateSkeltaEntities);
                neuralNetworkRetraining.Show();
                this.Close();
            }          
            else
            {
                if (ScreenNavigationUseCases.ValidateUserAdministratorPermission(businessSystem.NetworkUserModel) || ScreenNavigationUseCases.ValidateUserQualityPermission(businessSystem.NetworkUserModel))
                {
                    Views.NeuralNetworkRetraining neuralNetworkRetraining = new NeuralNetworkRetraining(maximized, businessSystem, _colgateSkeltaEntities);
                        neuralNetworkRetraining.Show();
                        this.Close();
                }
                else
                {
                    MessageBox.Show("Necessary administrative rights!");
                }
            }
        }

        private void ButtonGeneralReport_Click(object sender, RoutedEventArgs e)
        {
            Views.GeneralReport generalReport = new GeneralReport(maximized, businessSystem, _colgateSkeltaEntities);
            generalReport.Show();
            this.Close();
        }

        private void ButtonBristleRegister_Click_1(object sender, RoutedEventArgs e)
        {
            if (businessSystem.UserSystemCurrent.Type == "administrator")
            {
                Views.GeneralSettings generalSettings = new Views.GeneralSettings(maximized, businessSystem, _colgateSkeltaEntities);
                generalSettings.Show();
                this.Close();
            }
            else
            {
                if (ScreenNavigationUseCases.ValidateUserAdministratorPermission(businessSystem.NetworkUserModel) || ScreenNavigationUseCases.ValidateUserQualityPermission(businessSystem.NetworkUserModel))
                {
                    Views.GeneralSettings generalSettings = new Views.GeneralSettings(maximized, businessSystem, _colgateSkeltaEntities);
                        generalSettings.Show();
                        this.Close();
                }
                else
                {
                    MessageBox.Show("Necessary administrative rights!");
                }
            }          
        }

        private void CameraCalibration_Click(object sender, RoutedEventArgs e)
        {
            Views.CameraCalibration cameraCalibration = new Views.CameraCalibration(maximized, businessSystem, _colgateSkeltaEntities);
            cameraCalibration.Show();
            this.Close();
        }

        private void Password_Click(object sender, RoutedEventArgs e)
        {
            if (businessSystem.UserSystemCurrent.Type == "administrator")
            {
                Views.Password password = new Views.Password(maximized, businessSystem, _colgateSkeltaEntities);
                password.Show();
                this.Close();
            }
            else
            {
                if (ScreenNavigationUseCases.ValidateUserAdministratorPermission(businessSystem.NetworkUserModel) || ScreenNavigationUseCases.ValidateUserQualityPermission(businessSystem.NetworkUserModel))
                {
                    Views.Password password = new Views.Password(maximized, businessSystem, _colgateSkeltaEntities);
                        password.Show();
                        this.Close();
                }
                else
                {
                    MessageBox.Show("Necessary administrative rights!");
                }
            }           
        }

        private void User_Click(object sender, RoutedEventArgs e)
        {
            if (businessSystem.UserSystemCurrent.Type == "administrator")
            {
                Views.User user = new Views.User(maximized, businessSystem, _colgateSkeltaEntities);
                user.Show();
                this.Close();
            }
            else
            {
                if (ScreenNavigationUseCases.ValidateUserAdministratorPermission(businessSystem.NetworkUserModel) || ScreenNavigationUseCases.ValidateUserQualityPermission(businessSystem.NetworkUserModel))
                {
                    Views.User user = new Views.User(maximized, businessSystem, _colgateSkeltaEntities);
                        user.Show();
                        this.Close();
                }
                else
                {
                    MessageBox.Show("Necessary administrative rights!");
                }
            }           
        }

        private new void MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();

        }

        private void ButtonWindowClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
