using APIVision;
using APIVision.Controllers;
using APIVision.DataModels;
using Database;
using System;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using Bristle.UseCases;

namespace Bristle.Views
{
    /// <summary>
    /// Interação lógica para MainWindow.xam
    /// </summary>
    public partial class Password : Window
    {
        private bool maximized = true;
        private readonly BusinessSystem businessSystem;
        private readonly bool maximized__ = false;

        private readonly ColgateSkeltaEntities _colgateSkeltaEntities;

        public Password(bool maximized_, BusinessSystem businessSystem_, ColgateSkeltaEntities colgateSkeltaEntities)
        {
            businessSystem = businessSystem_;
            maximized__ = maximized_;

            _colgateSkeltaEntities = colgateSkeltaEntities;

            InitializeComponent();
            Loaded += Password_Loaded;            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Password_Loaded(object sender, RoutedEventArgs e)
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

        public void ButtonPopUpLogout_Click(object sender, RoutedEventArgs e)
        {
            CameraObject.DinoLiteSDK.MicroTouchPressed -= V_MicroTouchPress;
            CameraObject.DinoLiteSDK.Connected = false;
            Thread.Sleep(280);
            Views.UserControl userControl = new UserControl();
            userControl.Show();
            this.Close();
        }

        private void V_MicroTouchPress(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void ButtonOpenMenu_Click(object sender, RoutedEventArgs e)
        {
            ButtonOpenMenu.Visibility = Visibility.Collapsed;
            ButtonCloseMenu.Visibility = Visibility.Visible;
        }

        private void ButtonCloseMenu_Click(object sender, RoutedEventArgs e)
        {
            ButtonOpenMenu.Visibility = Visibility.Visible;
            ButtonCloseMenu.Visibility = Visibility.Collapsed;
        }

        private void ButtonHome_Click(object sender, RoutedEventArgs e)
        {
            Views.MainWindow mainWindow = new Views.MainWindow(maximized, businessSystem, _colgateSkeltaEntities);
            mainWindow.Show();
            this.Close();
        }

        private void ButtonAutomaticBristleClassification_Click(object sender, RoutedEventArgs e)
        {
            if (ScreenNavigationUseCases.OpenAutomaticBristleClassificationScreen(ScreenNavigationUseCases.GetGeneralLocalSettings(), businessSystem, _colgateSkeltaEntities, maximized))
                this.Close();
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

        private void ButtonWindowMinimize_Click(object sender, RoutedEventArgs e)
        {
                this.WindowState = WindowState.Minimized;
        }

        private void ButtonNeuralNetworkRetraining_Click(object sender, RoutedEventArgs e)
        {
            if (ScreenNavigationUseCases.OpenNeuralNetworkRetrainingScreen(businessSystem.UserSystemCurrent, businessSystem.NetworkUserModel, ScreenNavigationUseCases.GetGeneralLocalSettings(), businessSystem, _colgateSkeltaEntities, maximized))
            {
                this.Close();
            }
            else
            {
                MessageBox.Show("Necessary administrative rights!");
            }
        }

        private void ButtonGeneralReport_Click(object sender, RoutedEventArgs e)
        {
            if (ScreenNavigationUseCases.OpenGeneralReportScreen(businessSystem.UserSystemCurrent, businessSystem.NetworkUserModel, ScreenNavigationUseCases.GetGeneralLocalSettings(), businessSystem, _colgateSkeltaEntities, maximized))
            {
                this.Close();
            }
            else
            {
                MessageBox.Show("Necessary administrative rights!");
            }
        }

        private void ButtonBristleRegister_Click_1(object sender, RoutedEventArgs e)
        {
            if (ScreenNavigationUseCases.OpenGeneralSettingsScreen(businessSystem.UserSystemCurrent, businessSystem.NetworkUserModel, ScreenNavigationUseCases.GetGeneralLocalSettings(), businessSystem, _colgateSkeltaEntities, maximized))
            {
                this.Close();
            }
            else
            {
                MessageBox.Show("Necessary administrative rights!");
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
                Views.Password passwordView = new Views.Password(maximized, businessSystem, _colgateSkeltaEntities);
                passwordView.Show();
                this.Close();
            }
            else
            {
                if (ScreenNavigationUseCases.ValidateUserAdministratorPermission(businessSystem.NetworkUserModel) || ScreenNavigationUseCases.ValidateUserQualityPermission(businessSystem.NetworkUserModel))
                {
                    Views.Password passwordView = new Views.Password(maximized, businessSystem, _colgateSkeltaEntities);
                        passwordView.Show();
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
                Views.User userView = new Views.User(maximized, businessSystem, _colgateSkeltaEntities);
                userView.Show();
                this.Close();
            }
            else
            {
                if (ScreenNavigationUseCases.ValidateUserAdministratorPermission(businessSystem.NetworkUserModel) || ScreenNavigationUseCases.ValidateUserQualityPermission(businessSystem.NetworkUserModel))
                {
                    Views.User userView = new Views.User(maximized, businessSystem, _colgateSkeltaEntities);
                        userView.Show();
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Views.Help help = new Views.Help();
            help.Show();
        }
    }
}
