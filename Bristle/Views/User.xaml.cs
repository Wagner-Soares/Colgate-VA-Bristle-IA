using APIVision;
using APIVision.Controllers;
using APIVision.Controllers.DataBaseControllers;
using APIVision.DataModels;
using Bristle.UseCases;
using Database;
using System;
using System.Security.Cryptography;
using System.Threading;
using System.Windows;
using System.Windows.Input;

namespace Bristle.Views
{
    /// <summary>
    /// Interação lógica para MainWindow.xam
    /// </summary>
    public partial class User : Window
    {
        private bool maximized = true;
        private readonly BusinessSystem businessSystem;
        private readonly bool maximized__ = false;

        private readonly UserSystemController _userSystemController;
        readonly ColgateSkeltaEntities _colgateSkeltaEntities;

        public User(bool maximized_, BusinessSystem businessSystem_, ColgateSkeltaEntities colgateSkeltaEntities)
        {
            _colgateSkeltaEntities = colgateSkeltaEntities;

            businessSystem = businessSystem_;
            maximized__ = maximized_;

            _userSystemController = new UserSystemController(colgateSkeltaEntities);

            InitializeComponent();           

            Loaded += User_Loaded;           
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void User_Loaded(object sender, RoutedEventArgs e)
        {
            comboBoxType.Items.Add("operator");
            comboBoxType.Items.Add("administrator");
            comboBoxType.SelectedIndex = 1;

            gridUserRegistration.ItemsSource = _userSystemController.ListUserSystemModel();

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
            if (ScreenNavigationUseCases.OpenMainScreen(ScreenNavigationUseCases.GetGeneralLocalSettings(), businessSystem, _colgateSkeltaEntities, maximized))
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
            if (ScreenNavigationUseCases.OpenPasswordScreen(businessSystem.UserSystemCurrent, businessSystem.NetworkUserModel, ScreenNavigationUseCases.GetGeneralLocalSettings(), businessSystem, _colgateSkeltaEntities, maximized))
            {
                this.Close();
            }
            else
            {
                MessageBox.Show("Necessary administrative rights!");
            }
        }

        private void User_Click(object sender, RoutedEventArgs e)
        {
            if (ScreenNavigationUseCases.OpenUserScreen(businessSystem.UserSystemCurrent, businessSystem.NetworkUserModel, ScreenNavigationUseCases.GetGeneralLocalSettings(), businessSystem, _colgateSkeltaEntities, maximized))
            {
                this.Close();
            }
            else
            {
                MessageBox.Show("Necessary administrative rights!");
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            switch (comboBoxType.SelectedIndex)
            {
                case 0:
                    businessSystem.UserSystemModel_.Type = "operator";
                    break;
                case 1:
                    businessSystem.UserSystemModel_.Type = "administrator";
                    break;
            }

            if (userValue.Text != "")
            {
                businessSystem.UserSystemModel_.Name = userValue.Text;

                if (password_.Text != "")
                {
                    string passwordView = password_.Text.ToString();

                    Encryptor(passwordView);
                    _userSystemController.UpdateUserSystemModel(businessSystem.UserSystemModel_,  null);
                }
                else
                {
                    password_.Text = "cannot be empty!";
                }         
            }
            else
            {
                userValue.Text = "cannot be empty!";
            }

            gridUserRegistration.ItemsSource = _userSystemController.ListUserSystemModel();
        }       

        private void Encryptor(string password)
        {
            byte[] salt, key;
            // specify that we want to randomly generate a 20-byte salt
            using (var deriveBytes = new Rfc2898DeriveBytes(password, 20))
            {
                salt = deriveBytes.Salt;
                key = deriveBytes.GetBytes(20);  // derive a 20-byte key

                // save salt and key to database
            }
            businessSystem.UserSystemModel_.Salt = Convert.ToBase64String(salt);
            businessSystem.UserSystemModel_.Key = Convert.ToBase64String(key);
        }

        private new void MouseDown(object sender, MouseButtonEventArgs e)
        {
            password_.Text = "";
            userValue.Text = "";
        }

        private void Action_Click(object sender, RoutedEventArgs e)
        {
            var cell = gridUserRegistration.SelectedItem;
            UserSystemModel cell_ = (UserSystemModel)cell;

            if(cell_.Name != "admin")
            {
                _userSystemController.UpdateUserSystemModel(null,  cell_);
                gridUserRegistration.ItemsSource = _userSystemController.ListUserSystemModel();
            }
            else
            {
                MessageBox.Show("You do not remove the administrator user!");
            }
        }

        private new void MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Views.Help help = new Views.Help();
            help.Show();
        }
    }
}
