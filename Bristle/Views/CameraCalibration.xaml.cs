using APIVision;
using APIVision.Controllers;
using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using Database;
using APIVision.DataModels;
using Bristle.UseCases;

namespace Bristle.Views
{
    /// <summary>
    /// Interação lógica para BriterResister.xam
    /// </summary>
    public partial class CameraCalibration : Window
    {
        private bool maximized = true;
        private readonly BusinessSystem businessSystem;               
        private int focusValue = 0;
        private readonly bool maximized__ = false;
        readonly ColgateSkeltaEntities _colgateSkeltaEntities;

        public CameraCalibration(bool maximized_, BusinessSystem businessSystem_, ColgateSkeltaEntities colgateSkeltaEntities)
        {
            businessSystem = businessSystem_;
            maximized__ = maximized_;

            _colgateSkeltaEntities = colgateSkeltaEntities;

            InitializeComponent();
          
            Loaded += CameraCalibration_Loaded;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void CameraCalibration_Loaded(object sender, RoutedEventArgs e)
        {
            //SDK
            System.Windows.Forms.Integration.WindowsFormsHost host =
            new System.Windows.Forms.Integration.WindowsFormsHost
            {
                Child = CameraObject.DinoLiteSDK
            };
            // Add the interop host control to the Grid
            // control's collection of child controls.
            this.viewSDK.Children.Add(host);

            double w = (648 * SystemParameters.PrimaryScreenWidth) / 934.5;
            double h = (486 * SystemParameters.PrimaryScreenHeight) / 610.5;
            viewSDK.Width = w;
            viewSDK.Height = h;
            
            CameraObject.DinoLiteSDK.Preview = true;

            for (int i = 0; i < CameraObject.DinoLiteSDK.GetVideoDeviceCount(); i++)
            {
                comboBox1.Items.Add(CameraObject.DinoLiteSDK.GetVideoDeviceName(i));
            }

            CheckBoxStatus();

            comboBox1.SelectedIndex = 1;
            
            comboBox1_LEDs.Items.Add("LEDs-Shitch-On-1");
            comboBox1_LEDs.Items.Add("LEDs-Shitch-On-2");
            comboBox1_LEDs.Items.Add("LEDs-Shitch-On-1-2");
            comboBox1_LEDs.Items.Add("LEDs-Shitch-On-3");
            comboBox1_LEDs.Items.Add("LEDs-Shitch-On-1-3");
            comboBox1_LEDs.Items.Add("LEDs-Shitch-On-2-3");
            comboBox1_LEDs.Items.Add("LEDs-Shitch-On-1-2-3");
            comboBox1_LEDs.Items.Add("LEDs-Shitch-On-4");
            comboBox1_LEDs.Items.Add("LEDs-Shitch-On-1-4");
            comboBox1_LEDs.Items.Add("LEDs-Shitch-On-2-4");
            comboBox1_LEDs.Items.Add("LEDs-Shitch-On-1-2-4");
            comboBox1_LEDs.Items.Add("LEDs-Shitch-On-3-4");
            comboBox1_LEDs.Items.Add("LEDs-Shitch-On-1-3-4");
            comboBox1_LEDs.Items.Add("LEDs-Shitch-On-2-3-4");
            comboBox1_LEDs.Items.Add("LEDs-Shitch-On-1-2-3-4");
            comboBox1_LEDs.Items.Add("Turn-Off-All-LEDs");

            comboBox1_setFLCLevel.Items.Add("Level 1");
            comboBox1_setFLCLevel.Items.Add("Level 2");
            comboBox1_setFLCLevel.Items.Add("Level 3");
            comboBox1_setFLCLevel.Items.Add("Level 4");
            comboBox1_setFLCLevel.Items.Add("Level 5");
            comboBox1_setFLCLevel.Items.Add("Level 6");

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
        private void V_MicroTouchPress(object sender, EventArgs e)
        {
            CameraObject.DinoLiteSDK.SaveFrame("c:\\55.png");
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
            Views.GeneralReport generalReport = new GeneralReport(maximized, businessSystem, _colgateSkeltaEntities);
            generalReport.Show();
            this.Close();
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

        private void Button2_Click(object sender, RoutedEventArgs e)
        {
            CameraObject.DinoLiteSDK.SetLEDState(CameraObject.VideoDeviceIndex, 0);            
        }

        private void Button4_Click(object sender, RoutedEventArgs e)
        {
            CameraObject.DinoLiteSDK.Connected = false;
            CameraObject.DinoLiteSDK.Preview = false;            
        }

        private void ComboBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!CameraObject.DinoLiteSDK.Connected)
            {
                try
                {
                    CameraObject.DinoLiteSDK.VideoDeviceIndex = comboBox1.SelectedIndex - 1;
                    CameraObject.DinoLiteSDK.Connected = true;
                    CameraObject.DinoLiteSDK.Preview = true;
                    CameraObject.DinoLiteSDK.EnableMicroTouch(true);
                    CameraObject.DinoLiteSDK.MicroTouchPressed += V_MicroTouchPress;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("SDK: {0}", ex.Message);
                }
            }               
        }

        private void CheckBoxStatus()
        {
            if (checkBoxExpoure.IsChecked == true)
            {
                try
                {
                    sliderExpoure.IsEnabled = true;
                    CameraObject.DinoLiteSDK.SetAutoExposure(CameraObject.VideoDeviceIndex, 1);   
                }
                catch 
                {
                    sliderExpoure.IsEnabled = true;
                }        
            }
            else
            {
                try
                {
                    sliderExpoure.IsEnabled = false;
                    CameraObject.DinoLiteSDK.SetAutoExposure(CameraObject.VideoDeviceIndex, 0);                   
                }
                catch 
                {
                    sliderExpoure.IsEnabled = false;
                }

            }
        }

        private void CheckBoxExpoure_Checked(object sender, RoutedEventArgs e)
        {
            CheckBoxStatus();
        }

        private void SliderExpoure_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (sliderExpoure.IsEnabled)
            {
                try
                {                  
                    CameraObject.DinoLiteSDK.SetAutoExposure(CameraObject.VideoDeviceIndex, 1);
                }
                catch 
                {
                    //tryCatch to avoid crash
                }

            }
            else
            {
                try
                {                    
                    CameraObject.DinoLiteSDK.SetAutoExposure(CameraObject.VideoDeviceIndex, 0);
                }
                catch
                {
                    //tryCatch to avoid crash
                }
            }

            try
            {
                CameraObject.DinoLiteSDK.SetExposure(CameraObject.VideoDeviceIndex, (int)sliderExpoure.Value);
                expoureText.Text = CameraObject.DinoLiteSDK.GetExposure(CameraObject.VideoDeviceIndex).ToString();
            }
            catch 
            {
                //tryCatch to avoid crash
            }
            Thread.Sleep(100);            
        }

        private void AutoFocus_Click(object sender, RoutedEventArgs e)
        {
            CameraObject.DinoLiteSDK.AutoFocus(CameraObject.VideoDeviceIndex);
            focusValue = 0;
            manualFocusValue.Text = focusValue.ToString();
        }

        private void FocusAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                focusValue++;
                manualFocusValue.Text = focusValue.ToString();
                CameraObject.DinoLiteSDK.SetLensPos(CameraObject.VideoDeviceIndex, focusValue);               
                Thread.Sleep(1000);
            }
            catch
            {
                //tryCatch to avoid crash
            }
        }

        private void FocusDecrease_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                focusValue--;
                manualFocusValue.Text = focusValue.ToString();
                CameraObject.DinoLiteSDK.SetLensPos(CameraObject.VideoDeviceIndex, focusValue);                
                Thread.Sleep(1000);               
            }
            catch 
            {
                //tryCatch to avoid crash
            }
        }

        private void Off_Click(object sender, RoutedEventArgs e)
        {
            CameraObject.DinoLiteSDK.MicroTouchPressed -= V_MicroTouchPress;
            CameraObject.DinoLiteSDK.Connected = false;
        }

        private void On_Click(object sender, RoutedEventArgs e)
        {
            CameraObject.DinoLiteSDK.MicroTouchPressed -= V_MicroTouchPress;
            CameraObject.DinoLiteSDK.Connected = false;

            CameraObject.DinoLiteSDK.VideoDeviceIndex = CameraObject.VideoDeviceIndex;
            CameraObject.DinoLiteSDK.Connected = true;
            CameraObject.DinoLiteSDK.Preview = true;
            CameraObject.DinoLiteSDK.EnableMicroTouch(true);
            CameraObject.DinoLiteSDK.MicroTouchPressed += V_MicroTouchPress;
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

        private void ComboBox1_LEDs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch(comboBox1_LEDs.SelectedIndex)
            {
                case 1:
                    CameraObject.DinoLiteSDK.LightOn(1);
                    CameraObject.DinoLiteSDK.SetFLCSwitch(CameraObject.VideoDeviceIndex, 1);
                    break;
                case 2:
                    CameraObject.DinoLiteSDK.LightOn(1);
                    CameraObject.DinoLiteSDK.SetFLCSwitch(CameraObject.VideoDeviceIndex, 2);
                    break;
                case 3:
                    CameraObject.DinoLiteSDK.LightOn(1);
                    CameraObject.DinoLiteSDK.SetFLCSwitch(CameraObject.VideoDeviceIndex, 3);
                    break;
                case 4:
                    CameraObject.DinoLiteSDK.LightOn(1);
                    CameraObject.DinoLiteSDK.SetFLCSwitch(CameraObject.VideoDeviceIndex, 4);
                    break;
                case 5:
                    CameraObject.DinoLiteSDK.LightOn(1);
                    CameraObject.DinoLiteSDK.SetFLCSwitch(CameraObject.VideoDeviceIndex, 5);
                    break;
                case 6:
                    CameraObject.DinoLiteSDK.LightOn(1);
                    CameraObject.DinoLiteSDK.SetFLCSwitch(CameraObject.VideoDeviceIndex, 6);
                    break;
                case 7:
                    CameraObject.DinoLiteSDK.LightOn(1);
                    CameraObject.DinoLiteSDK.SetFLCSwitch(CameraObject.VideoDeviceIndex, 7);
                    break;
                case 8:
                    CameraObject.DinoLiteSDK.LightOn(1);
                    CameraObject.DinoLiteSDK.SetFLCSwitch(CameraObject.VideoDeviceIndex, 8);
                    break;
                case 9:
                    CameraObject.DinoLiteSDK.LightOn(1);
                    CameraObject.DinoLiteSDK.SetFLCSwitch(CameraObject.VideoDeviceIndex, 9);
                    break;
                case 10:
                    CameraObject.DinoLiteSDK.LightOn(1);
                    CameraObject.DinoLiteSDK.SetFLCSwitch(CameraObject.VideoDeviceIndex, 10);
                    break;
                case 11:
                    CameraObject.DinoLiteSDK.LightOn(1);
                    CameraObject.DinoLiteSDK.SetFLCSwitch(CameraObject.VideoDeviceIndex, 11);
                    break;
                case 12:
                    CameraObject.DinoLiteSDK.LightOn(1);
                    CameraObject.DinoLiteSDK.SetFLCSwitch(CameraObject.VideoDeviceIndex, 12);
                    break;
                case 13:
                    CameraObject.DinoLiteSDK.LightOn(1);
                    CameraObject.DinoLiteSDK.SetFLCSwitch(CameraObject.VideoDeviceIndex, 13);
                    break;
                case 14:
                    CameraObject.DinoLiteSDK.LightOn(1);
                    CameraObject.DinoLiteSDK.SetFLCSwitch(CameraObject.VideoDeviceIndex, 14);
                    break;
                case 15:
                    CameraObject.DinoLiteSDK.LightOn(1);
                    CameraObject.DinoLiteSDK.SetFLCSwitch(CameraObject.VideoDeviceIndex, 15);
                    break;
                case 16:
                    CameraObject.DinoLiteSDK.LightOn(1);
                    CameraObject.DinoLiteSDK.SetFLCSwitch(CameraObject.VideoDeviceIndex, 16);
                    break;
            }
        }

        private void ComboBox1_setFLCLevel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (comboBox1_setFLCLevel.SelectedIndex)
            {
                case 1:
                    CameraObject.DinoLiteSDK.SetFLCLevel(CameraObject.VideoDeviceIndex, 1);
                    break;
                case 2:
                    CameraObject.DinoLiteSDK.SetFLCLevel(CameraObject.VideoDeviceIndex, 2);
                    break;
                case 3:
                    CameraObject.DinoLiteSDK.SetFLCLevel(CameraObject.VideoDeviceIndex, 3);
                    break;
                case 4:
                    CameraObject.DinoLiteSDK.SetFLCLevel(CameraObject.VideoDeviceIndex, 4);
                    break;
                case 5:
                    CameraObject.DinoLiteSDK.SetFLCLevel(CameraObject.VideoDeviceIndex, 5);
                    break;
                case 6:
                    CameraObject.DinoLiteSDK.SetFLCLevel(CameraObject.VideoDeviceIndex, 6);
                    break;
            }
        }

        private void Grid_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}
