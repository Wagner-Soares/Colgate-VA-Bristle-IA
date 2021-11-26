using APIVision;
using APIVision.Controllers;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Controls;

namespace Bristle.Views
{
    /// <summary>
    /// Interação lógica para BriterResister.xam
    /// </summary>
    public partial class CameraCalibration : Window
    {
        private static bool maximized = true;
        private BusinessSystem businessSystem;               
        private static int focusValue = 0;
        private bool maximized__ = false;

        public CameraCalibration(bool maximized_, BusinessSystem businessSystem_)
        {
            businessSystem = businessSystem_;
            maximized__ = maximized_;

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
            new System.Windows.Forms.Integration.WindowsFormsHost();
            /*
             * Create the ActiveX control.
             * WmpAxLib.AxWindowsMediaPlayer axWmp = new WmpAxLib.AxWindowsMediaPlayer();
             * Assign the ActiveX control as the host control's child.
             */
            host.Child = ProductionObject.dinoLiteSDK;
            // Add the interop host control to the Grid
            // control's collection of child controls.
            this.viewSDK.Children.Add(host);

            double w = (648 * SystemParameters.PrimaryScreenWidth) / 934.5;
            double h = (486 * SystemParameters.PrimaryScreenHeight) / 610.5;
            viewSDK.Width = w;
            viewSDK.Height = h;
            
            ProductionObject.dinoLiteSDK.Preview = true;

            // Play a .wav file with the ActiveX control.
            // axWmp.URL = @"C:\Windows\Media\tada.wav";

            //label1.Text = "";

            //dinoLiteSDK.MicroTouchPressed();
            for (int i = 0; i < ProductionObject.dinoLiteSDK.GetVideoDeviceCount(); i++)
            {
                comboBox1.Items.Add(ProductionObject.dinoLiteSDK.GetVideoDeviceName(i));
            }

            checkBoxStatus();

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
        private void v_MicroTouchPress(object sender, EventArgs e)
        {
            // label1.Content = "Press";
            ProductionObject.dinoLiteSDK.SaveFrame("c:\\55.png");// .bmp");
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

        private void ButtonAutomaticBristleClassification_Click(object sender, RoutedEventArgs e)
        {
            //fthis.Close();
            Views.AutomaticBristleClassification automaticBristleClassification = new Views.AutomaticBristleClassification(maximized, businessSystem);
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
                Views.NeuralNetworkRetraining neuralNetworkRetraining = new NeuralNetworkRetraining(maximized, businessSystem, null);
                neuralNetworkRetraining.Show();
                this.Close();
            }
            else
            {
                //Validate the group that can access this part 
                foreach (var group in businessSystem.networkUserModel.NetworkUserGroup)
                {
                    if (group == "type1" || group == "type2")
                    {
                        Views.NeuralNetworkRetraining neuralNetworkRetraining = new NeuralNetworkRetraining(maximized, businessSystem, null);
                        neuralNetworkRetraining.Show();
                        this.Close();
                    }
                }
                MessageBox.Show("Necessary administrative rights!");
            }
        }

        private void ButtonGeneralReport_Click(object sender, RoutedEventArgs e)
        {
            Views.GeneralReport generalReport = new GeneralReport(maximized, businessSystem, null);
            generalReport.Show();
            this.Close();
        }

        private void ButtonBristleRegister_Click_1(object sender, RoutedEventArgs e)
        {
            Views.GeneralSettings generalSettings = new Views.GeneralSettings(maximized, businessSystem, null);
            generalSettings.Show();
            this.Close();
        }

        private void S100_Click(object sender, RoutedEventArgs e)
        {
            //businessSystem.SendCommand("S100", null);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //businessSystem.userSystemModel.Name = "Dionata2";
            //businessSystem.userSystemModel.Password = 1234;
            //businessSystem.testDB();
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            ProductionObject.dinoLiteSDK.SetLEDState(ProductionObject.VideoDeviceIndex, 0);            
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            //label1.Text = ProductionObject.dinoLiteSDK.GetSerialNum();
        }

        private void button4_Click(object sender, RoutedEventArgs e)
        {
            ProductionObject.dinoLiteSDK.Connected = false;
            ProductionObject.dinoLiteSDK.Preview = false;            
        }

        private void comboBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ProductionObject.dinoLiteSDK.Connected == false)
            {
                try
                {
                    ProductionObject.dinoLiteSDK.VideoDeviceIndex = comboBox1.SelectedIndex - 1;
                    ProductionObject.dinoLiteSDK.Connected = true;
                    ProductionObject.dinoLiteSDK.Preview = true;
                    ProductionObject.dinoLiteSDK.EnableMicroTouch(true);
                    ProductionObject.dinoLiteSDK.MicroTouchPressed += v_MicroTouchPress;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("SDK: {0}", ex.Message);
                }
            }               
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void checkBoxStatus()
        {
            if (checkBoxExpoure.IsChecked == true)
            {
                try
                {
                    sliderExpoure.IsEnabled = true;
                    ProductionObject.dinoLiteSDK.SetAutoExposure(ProductionObject.VideoDeviceIndex, 1);   
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
                    ProductionObject.dinoLiteSDK.SetAutoExposure(ProductionObject.VideoDeviceIndex, 0);                   
                }
                catch 
                {
                    sliderExpoure.IsEnabled = false;
                }

            }
        }

        private void sliderExpoure_Scroll(object sender, System.Windows.Controls.Primitives.ScrollEventArgs e)
        {

        }

        private void trackBarScroll(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {

        }

        private void checkBoxExpoure_Checked(object sender, RoutedEventArgs e)
        {
            checkBoxStatus();
        }

        private void sliderExpoure_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (sliderExpoure.IsEnabled == true)
            {
                try
                {                  
                    ProductionObject.dinoLiteSDK.SetAutoExposure(ProductionObject.VideoDeviceIndex, 1);
                }
                catch 
                {
                }
                
            }
            else
            {
                try
                {                    
                    ProductionObject.dinoLiteSDK.SetAutoExposure(ProductionObject.VideoDeviceIndex, 0);
                }
                catch
                {
                }
            }

            try
            {
                ProductionObject.dinoLiteSDK.SetExposure(ProductionObject.VideoDeviceIndex, (int)sliderExpoure.Value);
                expoureText.Text = ProductionObject.dinoLiteSDK.GetExposure(ProductionObject.VideoDeviceIndex).ToString();
            }
            catch 
            {
            }
            Thread.Sleep(100);            
        }

        private void autoFocus_Click(object sender, RoutedEventArgs e)
        {
            ProductionObject.dinoLiteSDK.AutoFocus(ProductionObject.VideoDeviceIndex);
            focusValue = 0;
            manualFocusValue.Text = focusValue.ToString();
        }

        private void focusAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                focusValue++;
                manualFocusValue.Text = focusValue.ToString();
                ProductionObject.dinoLiteSDK.SetLensPos(ProductionObject.VideoDeviceIndex, focusValue);               
                Thread.Sleep(1000);
            }
            catch 
            {
            }
        }

        private void focusDecrease_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                focusValue--;
                manualFocusValue.Text = focusValue.ToString();
                ProductionObject.dinoLiteSDK.SetLensPos(ProductionObject.VideoDeviceIndex, focusValue);                
                Thread.Sleep(1000);               
            }
            catch 
            {
            }        
        }

        private void Off_Click(object sender, RoutedEventArgs e)
        {
            ProductionObject.dinoLiteSDK.MicroTouchPressed -= v_MicroTouchPress;
            ProductionObject.dinoLiteSDK.Connected = false;
        }

        private void On_Click(object sender, RoutedEventArgs e)
        {
            ProductionObject.dinoLiteSDK.MicroTouchPressed -= v_MicroTouchPress;
            ProductionObject.dinoLiteSDK.Connected = false;

            ProductionObject.dinoLiteSDK.VideoDeviceIndex = ProductionObject.VideoDeviceIndex;
            ProductionObject.dinoLiteSDK.Connected = true;
            ProductionObject.dinoLiteSDK.Preview = true;
            ProductionObject.dinoLiteSDK.EnableMicroTouch(true);
            ProductionObject.dinoLiteSDK.MicroTouchPressed += v_MicroTouchPress;
        }

        private void CameraCalibration_Click(object sender, RoutedEventArgs e)
        {
            //Views.CameraCalibration cameraCalibration = new Views.CameraCalibration(maximized, businessSystem);
            //cameraCalibration.Show();
            //this.Close();
        }

        private void password_Click(object sender, RoutedEventArgs e)
        {
            if (businessSystem.userSystemCurrent.Type == "administrator")
            {
                Views.Password password = new Views.Password(maximized, businessSystem, null);
                password.Show();
                this.Close();
            }
            else
            {
                //Validate the group that can access this part 
                foreach (var group in businessSystem.networkUserModel.NetworkUserGroup)
                {
                    if (group == "type1" || group == "type2")
                    {
                        Views.NeuralNetworkRetraining neuralNetworkRetraining = new NeuralNetworkRetraining(maximized, businessSystem, null);
                        neuralNetworkRetraining.Show();
                        this.Close();
                    }
                }
                MessageBox.Show("Necessary administrative rights!");
            }
        }

        private void user_Click(object sender, RoutedEventArgs e)
        {
            if (businessSystem.userSystemCurrent.Type == "adminstrator")
            {
                Views.User user = new Views.User(maximized, businessSystem, null);
                user.Show();
                this.Close();
            }
            else
            {
                //Validate the group that can access this part 
                foreach (var group in businessSystem.networkUserModel.NetworkUserGroup)
                {
                    if (group == "type1" || group == "type2")
                    {
                        Views.NeuralNetworkRetraining neuralNetworkRetraining = new NeuralNetworkRetraining(maximized, businessSystem, null);
                        neuralNetworkRetraining.Show();
                        this.Close();
                    }
                }
                MessageBox.Show("Necessary administrative rights!");
            }
        }

        private void comboBox1_LEDs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch(comboBox1_LEDs.SelectedIndex)
            {
                case 1:
                    ProductionObject.dinoLiteSDK.LightOn(1);
                    ProductionObject.dinoLiteSDK.SetFLCSwitch(ProductionObject.VideoDeviceIndex, 1);
                    break;
                case 2:
                    ProductionObject.dinoLiteSDK.LightOn(1);
                    ProductionObject.dinoLiteSDK.SetFLCSwitch(ProductionObject.VideoDeviceIndex, 2);
                    break;
                case 3:
                    ProductionObject.dinoLiteSDK.LightOn(1);
                    ProductionObject.dinoLiteSDK.SetFLCSwitch(ProductionObject.VideoDeviceIndex, 3);
                    break;
                case 4:
                    ProductionObject.dinoLiteSDK.LightOn(1);
                    ProductionObject.dinoLiteSDK.SetFLCSwitch(ProductionObject.VideoDeviceIndex, 4);
                    break;
                case 5:
                    ProductionObject.dinoLiteSDK.LightOn(1);
                    ProductionObject.dinoLiteSDK.SetFLCSwitch(ProductionObject.VideoDeviceIndex, 5);
                    break;
                case 6:
                    ProductionObject.dinoLiteSDK.LightOn(1);
                    ProductionObject.dinoLiteSDK.SetFLCSwitch(ProductionObject.VideoDeviceIndex, 6);
                    break;
                case 7:
                    ProductionObject.dinoLiteSDK.LightOn(1);
                    ProductionObject.dinoLiteSDK.SetFLCSwitch(ProductionObject.VideoDeviceIndex, 7);
                    break;
                case 8:
                    ProductionObject.dinoLiteSDK.LightOn(1);
                    ProductionObject.dinoLiteSDK.SetFLCSwitch(ProductionObject.VideoDeviceIndex, 8);
                    break;
                case 9:
                    ProductionObject.dinoLiteSDK.LightOn(1);
                    ProductionObject.dinoLiteSDK.SetFLCSwitch(ProductionObject.VideoDeviceIndex, 9);
                    break;
                case 10:
                    ProductionObject.dinoLiteSDK.LightOn(1);
                    ProductionObject.dinoLiteSDK.SetFLCSwitch(ProductionObject.VideoDeviceIndex, 10);
                    break;
                case 11:
                    ProductionObject.dinoLiteSDK.LightOn(1);
                    ProductionObject.dinoLiteSDK.SetFLCSwitch(ProductionObject.VideoDeviceIndex, 11);
                    break;
                case 12:
                    ProductionObject.dinoLiteSDK.LightOn(1);
                    ProductionObject.dinoLiteSDK.SetFLCSwitch(ProductionObject.VideoDeviceIndex, 12);
                    break;
                case 13:
                    ProductionObject.dinoLiteSDK.LightOn(1);
                    ProductionObject.dinoLiteSDK.SetFLCSwitch(ProductionObject.VideoDeviceIndex, 13);
                    break;
                case 14:
                    ProductionObject.dinoLiteSDK.LightOn(1);
                    ProductionObject.dinoLiteSDK.SetFLCSwitch(ProductionObject.VideoDeviceIndex, 14);
                    break;
                case 15:
                    ProductionObject.dinoLiteSDK.LightOn(1);
                    ProductionObject.dinoLiteSDK.SetFLCSwitch(ProductionObject.VideoDeviceIndex, 15);
                    break;
                case 16:
                    ProductionObject.dinoLiteSDK.LightOn(1);
                    ProductionObject.dinoLiteSDK.SetFLCSwitch(ProductionObject.VideoDeviceIndex, 16);
                    break;
            }
        }

        private void comboBox1_setFLCLevel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (comboBox1_setFLCLevel.SelectedIndex)
            {
                case 1:
                    ProductionObject.dinoLiteSDK.SetFLCLevel(ProductionObject.VideoDeviceIndex, 1);
                    break;
                case 2:
                    ProductionObject.dinoLiteSDK.SetFLCLevel(ProductionObject.VideoDeviceIndex, 2);
                    break;
                case 3:
                    ProductionObject.dinoLiteSDK.SetFLCLevel(ProductionObject.VideoDeviceIndex, 3);
                    break;
                case 4:
                    ProductionObject.dinoLiteSDK.SetFLCLevel(ProductionObject.VideoDeviceIndex, 4);
                    break;
                case 5:
                    ProductionObject.dinoLiteSDK.SetFLCLevel(ProductionObject.VideoDeviceIndex, 5);
                    break;
                case 6:
                    ProductionObject.dinoLiteSDK.SetFLCLevel(ProductionObject.VideoDeviceIndex, 6);
                    break;
            }
        }

        private void Grid_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}
