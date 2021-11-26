using APIVision;
using APIVision.Controllers;
using APIVision.Models;
using Microsoft.Expression.Encoder.Devices;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
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
    public partial class User : Window
    {
        private static bool maximized = true;
        private BusinessSystem businessSystem;
        private bool maximized__ = false;
        private Views.AutomaticBristleClassification automaticBristleClassification;

        public User(bool maximized_, BusinessSystem businessSystem_, Views.AutomaticBristleClassification automaticBristleClassification_)
        {
            businessSystem = businessSystem_;
            maximized__ = maximized_;
            automaticBristleClassification = automaticBristleClassification_;

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

            gridUserRegistration.ItemsSource = businessSystem.dataBaseController.listUserSystemModel();

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
            ProductionObject.dinoLiteSDK.MicroTouchPressed -= v_MicroTouchPress;
            ProductionObject.dinoLiteSDK.Connected = false;
            Thread.Sleep(280);
            Views.UserControl userControl = new UserControl();
            userControl.Show();
            this.Close();
        }

        private void v_MicroTouchPress(object sender, EventArgs e)
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
            Views.MainWindow mainWindow = new Views.MainWindow(maximized, businessSystem);
            mainWindow.Show();
            this.Close();
        }

        private void ButtonBristleRegister_Click(object sender, RoutedEventHandler e)
        {
            Views.GeneralSettings generalSettings = new Views.GeneralSettings(maximized, businessSystem, null);
            generalSettings.Show();
            this.Close();
        }

        private void ButtonAutomaticBristleClassification_Click(object sender, RoutedEventArgs e)
        {            
            if (automaticBristleClassification == null)
            {
                automaticBristleClassification = new Views.AutomaticBristleClassification(maximized, businessSystem);
            }
            automaticBristleClassification.live = true;
            automaticBristleClassification.Show();
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

        private void ButtonPopUpLogout_Click()
        {
            this.Close();
        }

        private void ButtonWindowMinimize_Click(object sender, RoutedEventArgs e)
        {
                this.WindowState = WindowState.Minimized;
        }

        private void ButtonNeuralNetworkRetraining_Click(object sender, RoutedEventArgs e)
        {
            if (businessSystem.userSystemCurrent.Type == "administrator")
            {
                Views.NeuralNetworkRetraining neuralNetworkRetraining = new NeuralNetworkRetraining(maximized, businessSystem, automaticBristleClassification);
                neuralNetworkRetraining.Show();
                this.Close();
            }
            else
            {
                //Validate the group that can access this part 
                foreach (var group in businessSystem.networkUserModel.NetworkUserGroup)
                {
                    if (group == businessSystem.AD_Admin || group == businessSystem.AD_Quality)
                    {
                        Views.NeuralNetworkRetraining neuralNetworkRetraining = new NeuralNetworkRetraining(maximized, businessSystem, automaticBristleClassification);
                        neuralNetworkRetraining.Show();
                        this.Close();
                    }
                }
                MessageBox.Show("Necessary administrative rights!");
            }
        }

        private void ButtonGeneralReport_Click(object sender, RoutedEventArgs e)
        {
            Views.GeneralReport generalReport = new GeneralReport(maximized, businessSystem, automaticBristleClassification);
            generalReport.Show();
            this.Close();
        }

        private void ButtonBristleRegister_Click_1(object sender, RoutedEventArgs e)
        {           
            if (businessSystem.userSystemCurrent.Type == "administrator")
            {
                Views.GeneralSettings generalSettings = new Views.GeneralSettings(maximized, businessSystem, automaticBristleClassification);
                generalSettings.Show();
                this.Close();
            }
            else
            {
                //Validate the group that can access this part 
                foreach (var group in businessSystem.networkUserModel.NetworkUserGroup)
                {
                    if (group == businessSystem.AD_Admin || group == businessSystem.AD_Quality)
                    {
                        Views.GeneralSettings generalSettings = new Views.GeneralSettings(maximized, businessSystem, automaticBristleClassification);
                        generalSettings.Show();
                        this.Close();
                    }
                }
                MessageBox.Show("Necessary administrative rights!");
            }
        }
        private void CameraCalibration_Click(object sender, RoutedEventArgs e)
        {
            Views.CameraCalibration cameraCalibration = new Views.CameraCalibration(maximized, businessSystem);
            cameraCalibration.Show();
            this.Close();
        }

        private void password_Click(object sender, RoutedEventArgs e)
        {
            if (businessSystem.userSystemCurrent.Type == "administrator")
            {
                Views.Password password = new Views.Password(maximized, businessSystem, automaticBristleClassification);
                password.Show();
                this.Close();
            }
            else
            {
                //Validate the group that can access this part 
                foreach (var group in businessSystem.networkUserModel.NetworkUserGroup)
                {
                    if (group == businessSystem.AD_Admin || group == businessSystem.AD_Quality)
                    {
                        Views.Password password = new Views.Password(maximized, businessSystem, automaticBristleClassification);
                        password.Show();
                        this.Close();
                    }
                }
                MessageBox.Show("Necessary administrative rights!");
            }
        }

        private void user_Click(object sender, RoutedEventArgs e)
        {           
            if (businessSystem.userSystemCurrent.Type == "administrator")
            {
                Views.User user = new Views.User(maximized, businessSystem, automaticBristleClassification);
                user.Show();
                this.Close();
            }
            else
            {
                //Validate the group that can access this part 
                foreach (var group in businessSystem.networkUserModel.NetworkUserGroup)
                {
                    if (group == businessSystem.AD_Admin || group == businessSystem.AD_Quality)
                    {
                        Views.User user = new Views.User(maximized, businessSystem, automaticBristleClassification);
                        user.Show();
                        this.Close();
                    }
                }
                MessageBox.Show("Necessary administrative rights!");
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            switch (comboBoxType.SelectedIndex)
            {
                case 0:
                    businessSystem.userSystemModel_.Type = "operator";
                    break;
                case 1:
                    businessSystem.userSystemModel_.Type = "administrator";
                    break;
            }

            if (userValue.Text != "")
            {
                businessSystem.userSystemModel_.Name = userValue.Text;

                if (password_.Text != "")
                {
                    string password = password_.Text.ToString();

                    encryptor(password); ////decrypt(password, salt_, key_);
                    businessSystem.dataBaseController.updateUserSystemModel(businessSystem.userSystemModel_, null, null);
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

            gridUserRegistration.ItemsSource = businessSystem.dataBaseController.listUserSystemModel();
        }       

        private void encryptor(string password)
        {
            byte[] salt, key;
            // specify that we want to randomly generate a 20-byte salt
            using (var deriveBytes = new Rfc2898DeriveBytes(password, 20))
            {
                salt = deriveBytes.Salt;
                key = deriveBytes.GetBytes(20);  // derive a 20-byte key

                // save salt and key to database
            }
            businessSystem.userSystemModel_.Salt = Convert.ToBase64String(salt);
            businessSystem.userSystemModel_.Key = Convert.ToBase64String(key);
        }

        private void mouseDown(object sender, MouseButtonEventArgs e)
        {
            password_.Text = "";
            userValue.Text = "";
        }

        private void action_Click(object sender, RoutedEventArgs e)
        {
            var cell = gridUserRegistration.SelectedItem;
            UserSystemModel cell_ = (UserSystemModel)cell;

            if(cell_.Name != "admin")
            {
                businessSystem.dataBaseController.updateUserSystemModel(null, null, cell_);
                gridUserRegistration.ItemsSource = businessSystem.dataBaseController.listUserSystemModel();
            }
            else
            {
                MessageBox.Show("You do not remove the administrator user!");
            }
        }

        private void MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Views.Help help = new Views.Help(maximized, businessSystem, automaticBristleClassification);
            help.Show();
        }
    }
}
