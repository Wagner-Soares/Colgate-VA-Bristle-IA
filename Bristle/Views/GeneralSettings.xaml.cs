using APIVision;
using APIVision.Controllers;
using APIVision.Controllers.DataBaseControllers;
using APIVision.DataModels;
using Bristle.UseCases;
using Bristle.utils;
using Database;
using System;
using System.IO;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Bristle.Views
{
    /// <summary>
    /// Lógica interna para EvaluateSample.xaml
    /// </summary>
    public partial class GeneralSettings : Window
    {
        private bool maximized = true;
        private readonly BusinessSystem businessSystem;
        private readonly bool maximized__ = false;
        private Thread threadLoadingWait;
        private Thread threadLoadingWait_;
        private LoadingAnimation loading;

        private readonly ColgateSkeltaEntities _colgateSkeltaEntities;

        private readonly GeneralLocalSettings _generalSettings;

        public GeneralSettings(bool maximized_, BusinessSystem businessSystem_, ColgateSkeltaEntities colgateSkeltaEntities)
        {
            businessSystem = businessSystem_;
            maximized__ = maximized_;

            _colgateSkeltaEntities = colgateSkeltaEntities;

            _generalSettings = ScreenNavigationUseCases.GetGeneralLocalSettings();

            InitializeComponent();

            this.Loaded += GeneralSettings_Loaded;          

            Init();
        }

        private void StartLoadingWait(bool start)
        {
            if (start)
            {
                threadLoadingWait = new Thread(() => ThreadMethod())
                {
                    Name = "threadLoadingWait"
                };
                threadLoadingWait.SetApartmentState(ApartmentState.STA);
                threadLoadingWait.Start();
            }
            else
            {
                threadLoadingWait_ = new Thread(() => ThreadMethod());
                threadLoadingWait_.SetApartmentState(ApartmentState.STA);
                threadLoadingWait_.Start();
            }
        }
        private void LoadingWait_()
        {
            loading = new Views.LoadingAnimation(ConfigurationConstants.GeneralConfigurationName)
            {
                Topmost = true
            };
            loading.Activate();
            loading.ShowDialog();
        }

        private void ThreadMethod()
        {
            if (loading == null)
            {
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
        private void Init()
        {
            try
            {
                IpPrediction.Text = _generalSettings.IpPrediction;
                PortPrediction.Text = _generalSettings.PortPrediction.ToString();
                IpTrain.Text = _generalSettings.IpTrain;
                PortTrain.Text = _generalSettings.PortTrain.ToString();
                PortInterface.Text = _generalSettings.PortInterface.ToString();

                txbEndroudTestName.Text = _generalSettings.EndroundTestName;
                txbtuftTBristleCountTestName.Text = _generalSettings.TuftTBristleCountTestName;
                txbtuftM1BristleCountTestName.Text = _generalSettings.TuftM1BristleCountTestName;
                txbtuftM2BristleCountTestName.Text = _generalSettings.TuftM2BristleCountTestName;
                txbtuftM3BristleCountTestName.Text = _generalSettings.TuftM3BristleCountTestName;
                txbtuftNBristleCountTestName.Text = _generalSettings.TuftNBristleCountTestName;

                txbAdminsADGroup.Text = _generalSettings.AD_Admin;
                txbQualityADGroup.Text = _generalSettings.AD_Quality;
                txbOperatorsADGroup.Text = _generalSettings.AD_Operator;

                prefix.Text = _generalSettings.NamePrefix;

                for (int i = 0; i < CameraObject.DinoLiteSDK.GetVideoDeviceCount(); i++)
                {
                    cameraSelect.Items.Add(CameraObject.DinoLiteSDK.GetVideoDeviceName(i));
                }
                cameraSelect.SelectedIndex = _generalSettings.CameraSelect;

                selectResolution.Items.Add("Width = 640, Height = 480");
                selectResolution.Items.Add("Width = 1280, Height = 960");
                selectResolution.Items.Add("Width = 1600, Height = 1200");
                selectResolution.Items.Add("Width = 2048, Height = 1536");
                selectResolution.Items.Add("Width = 2592, Height = 1944");

                switch(_generalSettings.VideoFormatHeight.ToString())
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
                //tryCatch to avoid crash
            }
        }

        public void ButtonPopUpLogout_Click(object sender, RoutedEventArgs e)
        {
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

        private void ButtonGeneralReport_Click(object sender, RoutedEventArgs e)
        {
            Views.GeneralReport generalReport = new GeneralReport(maximized, businessSystem, _colgateSkeltaEntities);
            generalReport.Show();
            this.Close();
        }

        private void ButtonHome_Click(object sender, RoutedEventArgs e)
        {
            Views.MainWindow mainWindow = new Views.MainWindow(maximized, businessSystem, _colgateSkeltaEntities);
            mainWindow.Show();
            this.Close();
        }

        private void ButtonAutomaticBristleClassification_Click(object sender, RoutedEventArgs e)
        {
            ScreenNavigationUseCases.SaveGeneralLocalSettings(_generalSettings);
            Views.AutomaticBristleClassification automaticBristleClassification = new Views.AutomaticBristleClassification(maximized, businessSystem, _colgateSkeltaEntities);
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

        private void ButtonWindowMinimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void ButtonNeuralNetworkRetraining_Click(object sender, RoutedEventArgs e)
        {
            if (businessSystem.UserSystemCurrent.Type == "administrator")
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

        private void ButtonBristleRegister_Click_1(object sender, RoutedEventArgs e)
        {
            Views.GeneralSettings generalSettings = new Views.GeneralSettings(maximized, businessSystem, _colgateSkeltaEntities);
            generalSettings.Show();
            this.Close();
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
                Views.User userView = new User(maximized, businessSystem, _colgateSkeltaEntities);
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            StartLoadingWait(true);

            _generalSettings.IpPrediction = IpPrediction.Text.Replace(',', '.');
            _generalSettings.PortPrediction = int.Parse(PortPrediction.Text);
            _generalSettings.IpTrain = IpTrain.Text.Replace(',', '.');
            _generalSettings.PortTrain = int.Parse(PortTrain.Text);
            _generalSettings.PortInterface = int.Parse(PortInterface.Text);
            _generalSettings.NamePrefix = prefix.Text;

            _generalSettings.EndroundTestName = txbEndroudTestName.Text;
            _generalSettings.TuftTBristleCountTestName = txbtuftTBristleCountTestName.Text;
            _generalSettings.TuftM1BristleCountTestName = txbtuftM1BristleCountTestName.Text;
            _generalSettings.TuftM2BristleCountTestName = txbtuftM2BristleCountTestName.Text;
            _generalSettings.TuftM3BristleCountTestName = txbtuftM3BristleCountTestName.Text;
            _generalSettings.TuftNBristleCountTestName = txbtuftNBristleCountTestName.Text;

            _generalSettings.AD_Admin = txbAdminsADGroup.Text;
            _generalSettings.AD_Quality = txbQualityADGroup.Text;
            _generalSettings.AD_Operator = txbOperatorsADGroup.Text;

            DataHandlerUseCases.SaveJsonIntoSettings(_generalSettings, @ConfigurationConstants.GeneralConfigurationName);

            CameraObject.DinoLiteSDK.Preview = false;
            CameraObject.DinoLiteSDK.Connected = false;
            CameraObject.DinoLiteSDK.PreviewScale = false;
            CameraObject.DinoLiteSDK.VideoDeviceIndex = CameraObject.VideoDeviceIndex;
            CameraObject.DinoLiteSDK.UseVideoFilter = DNVideoXLib.vcxUseVideoFilterEnum.vcxBoth;
            CameraObject.DinoLiteSDK.Connected = true;
            CameraObject.DinoLiteSDK.EnableMicroTouch(true);
            CameraObject.DinoLiteSDK.SetVideoFormat(CameraObject.ResolutionWidth, CameraObject.ResolutionHeight);
            CameraObject.DinoLiteSDK.PreviewScale = true;
            CameraObject.DinoLiteSDK.ColorFormat = 15;
            CameraObject.DinoLiteSDK.Preview = true;
            CameraObject.DinoLiteSDK.SetFLCLevel(0, 15);
            Thread.Sleep(280);
            CameraObject.DinoLiteSDK.Preview = false;

            StartLoadingWait(false);
        }

        private void ComboBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch(selectResolution.SelectedIndex)
            {
                case 0:
                    CameraObject.ResolutionWidth = 640;
                    CameraObject.ResolutionHeight = 480;
                    break;
                case 1:
                    CameraObject.ResolutionWidth = 1280;
                    CameraObject.ResolutionHeight = 960;
                    break;          
                case 2:
                    CameraObject.ResolutionWidth = 1600;
                    CameraObject.ResolutionHeight = 1200;
                    break;
                case 3:
                    CameraObject.ResolutionWidth = 2048;
                    CameraObject.ResolutionHeight = 1536;
                    break;
                case 4:
                    CameraObject.ResolutionWidth = 2592;
                    CameraObject.ResolutionHeight = 1944;
                    break;
            }
            _generalSettings.VideoFormatHeight = CameraObject.ResolutionWidth;
            _generalSettings.VideoFormatWidth = CameraObject.ResolutionHeight;
            DataHandlerUseCases.SaveJsonIntoSettings(_generalSettings, @ConfigurationConstants.GeneralConfigurationName);
        }

        private new void MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void ComboBox0_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CameraObject.VideoDeviceIndex = cameraSelect.SelectedIndex;
            _generalSettings.CameraSelect = CameraObject.VideoDeviceIndex;
            DataHandlerUseCases.SaveJsonIntoSettings(_generalSettings, @ConfigurationConstants.GeneralConfigurationName);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Help help = new Help();
            help.Show();
        }

        private void TestIpPredictionConnection(object sender, RoutedEventArgs e)
        {
            if (NetworkTestUseCases.TestIpPingConnection(IpPrediction.Text))
            {
                btnIPPrediction.Background = Brushes.Green;
            }
            else
            {
                btnIPPrediction.Background = Brushes.DarkRed;
            }
        }

        private void TestIpTrainConnection(object sender, RoutedEventArgs e)
        {
            if (NetworkTestUseCases.TestIpPingConnection(IpTrain.Text))
            {
                btnIpTrain.Background = Brushes.Green;
            }
            else
            {
                btnIpTrain.Background = Brushes.DarkRed;
            }
        }

        private void BtnPortPrediction_Click(object sender, RoutedEventArgs e)
        {
            if (NetworkTestUseCases.TestIpPortConnection(IpPrediction.Text, Convert.ToInt16(PortPrediction.Text)))
            {
                btnPortPredition.Background = Brushes.Green;
            }
            else
            {
                btnPortPredition.Background = Brushes.DarkRed;
            }
        }

        private void BtnPortTrain_Click(object sender, RoutedEventArgs e)
        {
            if (NetworkTestUseCases.TestIpPortConnection(IpTrain.Text, Convert.ToInt16(PortTrain.Text)))
            {
                btnPortTrain.Background = Brushes.Green;
            }
            else
            {
                btnPortTrain.Background = Brushes.DarkRed;
            }
        }
    }
}
