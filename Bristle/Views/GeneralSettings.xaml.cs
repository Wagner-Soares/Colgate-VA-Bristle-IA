using APIVision;
using APIVision.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Bristle.Views
{
    /// <summary>
    /// Lógica interna para EvaluateSample.xaml
    /// </summary>
    public partial class GeneralSettings : Window
    {
        private static bool maximized = true;
        private BusinessSystem businessSystem;
        private bool maximized__ = false;
        private Views.AutomaticBristleClassification automaticBristleClassification;
        private Thread threadLoadingWait;
        private Thread threadLoadingWait_;
        private LoadingAnimation loading;
        public GeneralSettings(bool maximized_, BusinessSystem businessSystem_, Views.AutomaticBristleClassification automaticBristleClassification_ )
        {
            businessSystem = businessSystem_;
            maximized__ = maximized_;
            automaticBristleClassification = automaticBristleClassification_;

            InitializeComponent();

            this.Loaded += GeneralSettings_Loaded;          

            init();
        }

        private void StartLoadingWait(bool start)
        {
            if (start)
            {
                threadLoadingWait = new Thread(() => ThreadMethod());
                threadLoadingWait.Name = "threadLoadingWait";
                threadLoadingWait.SetApartmentState(ApartmentState.STA); //Set the thread to STA
                //threadLoadingWait.IsBackground = true;
                threadLoadingWait.Start();
            }
            else
            {
                threadLoadingWait_ = new Thread(() => ThreadMethod());
                threadLoadingWait_.SetApartmentState(ApartmentState.STA); //Set the thread to STA
                //threadLoadingWait_.IsBackground = true;
                threadLoadingWait_.Start();
            }
        }
        private void LoadingWait_()
        {
            loading = new Views.LoadingAnimation("generalSettings");
            loading.Topmost = true;
            loading.Activate();
            loading.ShowDialog();
        }

        private void ThreadMethod()
        {
            if (loading == null)
            {
                //Dispatcher.BeginInvoke((Action)(() => { LoadingWait_(); }));
                LoadingWait_();
            }
            else
            {
                loading.CloseMe();
                loading = null;
            }
        }
        private void GeneralSettings_Loaded(object sender, RoutedEventArgs e)
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

        /// <summary>
        /// 
        /// </summary>
        private void init()
        {
            try
            {
                businessSystem.generalSettings = businessSystem.generalController.ReadJson3(businessSystem.localRepository + @"\generalSettings.json");

                IpPrediction.Text = businessSystem.generalSettings.IpPrediction;
                PortPrediction.Text = businessSystem.generalSettings.PortPrediction.ToString();
                IpTrain.Text = businessSystem.generalSettings.IpTrain;
                PortTrain.Text = businessSystem.generalSettings.PortTrain.ToString();
                //IpInterface.Text = businessSystem.generalSettings.IpInterface;
                PortInterface.Text = businessSystem.generalSettings.PortInterface.ToString();

                businessSystem.generalSettingsModels = businessSystem.dataBaseController.listGeneralSettingsModel();
                prefix.Text = @businessSystem.generalSettingsModels[0].Prefix;

                for (int i = 0; i < ProductionObject.dinoLiteSDK.GetVideoDeviceCount(); i++)
                {
                    cameraSelect.Items.Add(ProductionObject.dinoLiteSDK.GetVideoDeviceName(i));
                }
                cameraSelect.SelectedIndex = businessSystem.generalSettings.CameraSelect;

                selectResolution.Items.Add("Width = 640, Height = 480");
                selectResolution.Items.Add("Width = 1280, Height = 960");
                selectResolution.Items.Add("Width = 1600, Height = 1200");
                selectResolution.Items.Add("Width = 2048, Height = 1536");
                selectResolution.Items.Add("Width = 2592, Height = 1944");

                switch(businessSystem.generalSettings.VideoFormatHeight.ToString())
                {
                    case "640":
                        selectResolution.SelectedIndex = 0;
                        break;
                    case "1280":
                        selectResolution.SelectedIndex = 1;
                        break;
                    case "1600":
                        selectResolution.SelectedIndex = 2;
                        break;
                    case "2048":
                        selectResolution.SelectedIndex = 3;
                        break;
                    case "2592":
                        selectResolution.SelectedIndex = 4;
                        break;
                }                
            }
            catch 
            {

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

        private void ButtonGeneralReport_Click(object sender, RoutedEventArgs e)
        {
            Views.GeneralReport generalReport = new GeneralReport(maximized, businessSystem, automaticBristleClassification);
            generalReport.Show();
            this.Close();
        }

        private void ButtonHome_Click(object sender, RoutedEventArgs e)
        {
            Views.MainWindow mainWindow = new Views.MainWindow(maximized, businessSystem);
            mainWindow.Show();
            this.Close();
        }

        private void ButtonBristleRegister_Click(object sender, RoutedEventHandler e)
        {

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
                if(businessSystem.networkUserModel.NetworkUserGroup != null)
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
                }
               
                MessageBox.Show("Necessary administrative rights!");
            }
        }

        private void ButtonBristleRegister_Click_1(object sender, RoutedEventArgs e)
        {
            Views.GeneralSettings generalSettings = new Views.GeneralSettings(maximized, businessSystem, null);
            generalSettings.Show();
            this.Close();
        }

        private void ButtonWindowMaximize__Click(object sender, RoutedEventArgs e)
        {

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
                if(businessSystem.networkUserModel.NetworkUserGroup != null)
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
                if(businessSystem.networkUserModel.NetworkUserGroup != null)
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
                }
              
                MessageBox.Show("Necessary administrative rights!");
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            StartLoadingWait(true);

            businessSystem.generalSettings.IpPrediction = IpPrediction.Text.Replace(',', '.');
            businessSystem.generalSettings.PortPrediction = int.Parse(PortPrediction.Text);
            businessSystem.generalSettings.IpTrain = IpTrain.Text.Replace(',', '.');
            businessSystem.generalSettings.PortTrain = int.Parse(PortTrain.Text);
            //businessSystem.generalSettings.IpInterface = IpInterface.Text.Replace(',', '.');
            businessSystem.generalSettings.PortInterface = int.Parse(PortInterface.Text);
            //businessSystem.generalSettings.NamePrefix = prefix.Text;// @"Datalog\";////@"\\UO631M4067452\Dataset-Colgate\";
            businessSystem.generalController.SaveJson(businessSystem.generalSettings, businessSystem.localRepository + @"\generalSettings.json");

            businessSystem.generalSettingsModel.Prefix = prefix.Text;
            businessSystem.generalSettings.NamePrefix = prefix.Text;
            businessSystem.generalSettingsModel.Id = 1;
            businessSystem.dataBaseController.updateGeneralSettingsModel(null, businessSystem.generalSettingsModel, null);

            ProductionObject.dinoLiteSDK.Preview = false;
            ProductionObject.dinoLiteSDK.Connected = false;
            ProductionObject.dinoLiteSDK.PreviewScale = false;
            ProductionObject.dinoLiteSDK.VideoDeviceIndex = ProductionObject.VideoDeviceIndex;
            ProductionObject.dinoLiteSDK.UseVideoFilter = DNVideoXLib.vcxUseVideoFilterEnum.vcxBoth;
            ProductionObject.dinoLiteSDK.Connected = true;
            ProductionObject.dinoLiteSDK.EnableMicroTouch(true);
            ProductionObject.dinoLiteSDK.MicroTouchPressed += v_MicroTouchPress;
            ProductionObject.dinoLiteSDK.SetVideoFormat(ProductionObject.ResolutionWidth, ProductionObject.ResolutionHeight);
            ProductionObject.dinoLiteSDK.PreviewScale = true;
            ProductionObject.dinoLiteSDK.ColorFormat = 15;
            ProductionObject.dinoLiteSDK.Preview = true;
            ProductionObject.dinoLiteSDK.SetFLCLevel(0, 15);
            Thread.Sleep(280);
            ProductionObject.dinoLiteSDK.Preview = false;

            StartLoadingWait(false);
        }

        private void comboBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch(selectResolution.SelectedIndex)
            {
                case 0:
                    ProductionObject.ResolutionWidth = 640;
                    ProductionObject.ResolutionHeight = 480;
                    break;
                case 1:
                    ProductionObject.ResolutionWidth = 1280;
                    ProductionObject.ResolutionHeight = 960;
                    break;          
                case 2:
                    ProductionObject.ResolutionWidth = 1600;
                    ProductionObject.ResolutionHeight = 1200;
                    break;
                case 3:
                    ProductionObject.ResolutionWidth = 2048;
                    ProductionObject.ResolutionHeight = 1536;
                    break;
                case 4:
                    ProductionObject.ResolutionWidth = 2592;
                    ProductionObject.ResolutionHeight = 1944;
                    break;
            }
            businessSystem.generalSettings.VideoFormatHeight = ProductionObject.ResolutionWidth;
            businessSystem.generalSettings.VideoFormatWidth = ProductionObject.ResolutionHeight;
            businessSystem.generalController.SaveJson(businessSystem.generalSettings, businessSystem.localRepository + @"\generalSettings.json");
        }
        private void v_MicroTouchPress(object sender, EventArgs e)
        {
            // label1.Content = "Press";
            //ProductionObjectdinoLiteSDK.SaveFrame("c:\\55.png");// .bmp");
        }

        private void MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void comboBox0_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ProductionObject.VideoDeviceIndex = cameraSelect.SelectedIndex;
            businessSystem.generalSettings.CameraSelect = ProductionObject.VideoDeviceIndex;
            businessSystem.generalController.SaveJson(businessSystem.generalSettings, businessSystem.localRepository + @"\generalSettings.json");
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Views.Help help = new Views.Help(maximized, businessSystem, automaticBristleClassification);
            help.Show();
        }
    }
}
