using APIVision;
using APIVision.Controllers;
using APIVision.Controllers.DataBaseControllers;
using System;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using MessageBox = System.Windows.Forms.MessageBox;
using Database;
using Bristle.UseCases;
using System.Configuration;
using APIVision.DataModels;
using Bristle.utils;
using System.Threading.Tasks;

namespace Bristle.Views
{
    /// <summary>
    /// Playful interaction for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private bool maximized = true;
        private readonly BusinessSystem businessSystem;
        private Thread startServeTCP_;
        private bool cameraStart = false;
        private readonly bool maximized__ = false;
        private System.Windows.Forms.Integration.WindowsFormsHost host;
        private Thread threadLoadingWait;
        private LoadingAnimation loading;
        private readonly System.Windows.Forms.Timer myTimer = new System.Windows.Forms.Timer();
        private bool releaseMenu_ = false;

        private readonly ColgateSkeltaEntities _colgateSkeltaEntities;

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        private static extern bool DeleteObject(IntPtr hObject);

        private GeneralLocalSettings _generalSettings;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="maximized_"></param>
        /// <param name="businessSystem_"></param>
        public MainWindow(bool maximized_, BusinessSystem businessSystem_, ColgateSkeltaEntities colgateSkeltaEntities)
        {       
            businessSystem = businessSystem_;
            maximized__ = maximized_;

            _colgateSkeltaEntities = colgateSkeltaEntities;

            _generalSettings = ScreenNavigationUseCases.GetGeneralLocalSettings();

            InitializeComponent();

            this.Loaded += MainWindow_Loaded;
                    
        }

        /// <summary>
        /// Initialization of the MainWindow class
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            GridMenu.IsEnabled = false;

            //*
            //* SDK Dino-lite
            //*/
            try
            {
                host = new System.Windows.Forms.Integration.WindowsFormsHost
                {
                    Child = CameraObject.DinoLiteSDK
                };
                this.viewSDK.Children.Add(host);
            }
            catch (Exception ex)
            {
                Log.CustomLog.LogMessage(ex.Message);
            }
            
            StartLoadingWait(true);

            double w = (648 * SystemParameters.PrimaryScreenWidth) / 934.5;
            double h = (486 * SystemParameters.PrimaryScreenHeight) / 610.5;
            viewSDK.Width = w;
            viewSDK.Height = h;

            viewSDKImage.Width = viewSDK.Width;
            viewSDKImage.Height = viewSDK.Height;

            frameHolder.Width = viewSDK.Width;
            frameHolder.Height = viewSDK.Height;
            frameHolder.Stretch = Stretch.Fill;

            Thread Camera = new Thread(() => LoadCamera())
            {
                Name = "Camera",
                IsBackground = true
            };
            Camera.Start();

            myTimer.Tick += new EventHandler(ReleaseMenu);
            myTimer.Interval = 300;
            myTimer.Start();

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

        private void ReleaseMenu(Object myObject, EventArgs myEventArgs)
        {
            if (releaseMenu_)
            {
                GridMenu.IsEnabled = true;
                myTimer.Stop();
                myTimer.Enabled = false;
            }
            else
            {
                myTimer.Stop();
                myTimer.Enabled = true;
            }    
        }

        /// <summary>
        /// StartLoadingWait
        /// </summary>
        /// <param name="start"></param>
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
                Thread wait__ = new Thread(() => Wait_())
                {
                    Name = "wait_"
                };
                wait__.SetApartmentState(ApartmentState.STA); //Set the thread to STA                
                wait__.Start();
                wait__.Join();                
            }
        }

        private new void SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double w = (648 * this.ActualWidth) / 934.5;
            double h = (486 * this.ActualHeight) / 610.5;
            viewSDK.Width = w;
            viewSDK.Height = h;

            viewSDKImage.Width = viewSDK.Width;
            viewSDKImage.Height = viewSDK.Height;

            frameHolder.Width = viewSDK.Width;
            frameHolder.Height = viewSDK.Height;
            frameHolder.Stretch = Stretch.Fill;

            viewSDK.UpdateLayout();
            viewSDKImage.UpdateLayout();
            frameHolder.UpdateLayout();
        }

        private void Wait_()
        {
            if(_generalSettings.MissingCamera)
            {
                Thread.Sleep(2000);
            }           
            ThreadMethod();
            releaseMenu_ = true;
        }

        private void LoadingWait_()
        {
            loading = new Views.LoadingAnimation("other")
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

        /// <summary>
        /// Initialize general system settings and camera configuration parameters. 
        /// Create the directories, if they do not exist: generalSettings.json and 
        /// cameraSettings.json and populate with default values.
        /// </summary>
        private void LoadCamera()
        {           
            if(!cameraStart)
            {
                _generalSettings = ScreenNavigationUseCases.GetGeneralLocalSettings();

                Log.CustomLog.LogMessage("Initializating camera");

                string pathAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                businessSystem.LocalRepository = pathAppData + @"\Colgate Vision\Interface";
                if (!Directory.Exists(businessSystem.LocalRepository))
                {
                    Log.CustomLog.LogMessage("Trying to create Interface dir");
                    Directory.CreateDirectory(businessSystem.LocalRepository);
                }

                if (!DataHandlerUseCases.SettingExists(ConfigurationConstants.GeneralConfigurationName))
                {
                    Log.CustomLog.LogMessage("Creating generalSettings json");
                    _generalSettings = FirstTimeConfigurationUseCases.GetGeneralConfigurationDefaults();
                    DataHandlerUseCases.SaveJsonIntoSettings(_generalSettings, ConfigurationConstants.GeneralConfigurationName);
                }
                else
                {
                    Log.CustomLog.LogMessage("Reading generalSettings json");
                    _generalSettings = ScreenNavigationUseCases.GetGeneralLocalSettings();
                }            

                try
                {
                    if (startServeTCP_ != null)
                    {
                        startServeTCP_.Abort();
                        startServeTCP_.IsBackground = true;
                        startServeTCP_.Start();
                    }
                    else
                    {
                        startServeTCP_ = new Thread(StartServeTCP);
                        startServeTCP_.Start();
                    }
                }
                catch (Exception ex)
                {
                    Log.CustomLog.LogMessage("Error during initialization: " + ex.Message);
                }

                if (!DataHandlerUseCases.SettingExists(@"commandS300"))
                {
                    Log.CustomLog.LogMessage("Creating command300 json");
                    businessSystem.CommandS300Model.NewModelId = 1;
                    businessSystem.CommandS300Model.OldModelId = 0;
                    DataHandlerUseCases.SaveJsonIntoSettings(businessSystem.CommandS300Model, @"commandS300");

                    object obj = DataHandlerUseCases.ReadJsonFromSettingsKey(@"commandS300");
                    _generalSettings.Model = DataHandlerUseCases.ConvertObjectToJson(obj);

                }
                else
                {
                    object obj = DataHandlerUseCases.ReadJsonFromSettingsKey(@"commandS300");
                    businessSystem.CommandS300Model.NewModelId = DataHandlerUseCases.ReadJsonCommand300(@"commandS300");
                    _generalSettings.Model = DataHandlerUseCases.ConvertObjectToJson(obj);
                    businessSystem.CommandS300Model.OldModelId = 0;

                }               

                CameraObject.ResolutionWidth = _generalSettings.VideoFormatHeight;
                CameraObject.ResolutionHeight = _generalSettings.VideoFormatWidth;
                cameraStart = true;
            }

            try
            {
                try
                {
                    for (int i = 0; i < CameraObject.DinoLiteSDK.GetVideoDeviceCount(); i++)
                    {
                        _generalSettings.MissingCamera = true;
                    }
                    
                    if(_generalSettings.MissingCamera)
                    {
                        CameraObject.DinoLiteSDK.Connected = true;
                        CameraObject.DinoLiteSDK.Preview = false;
                        CameraObject.DinoLiteSDK.Connected = false;
                        CameraObject.DinoLiteSDK.PreviewScale = false;
                        Thread.Sleep(50);
                        CameraObject.DinoLiteSDK.VideoDeviceIndex = CameraObject.VideoDeviceIndex;
                        CameraObject.DinoLiteSDK.UseVideoFilter = DNVideoXLib.vcxUseVideoFilterEnum.vcxBoth;
                        CameraObject.DinoLiteSDK.Connected = true;
                        Thread.Sleep(50);
                        CameraObject.DinoLiteSDK.EnableMicroTouch(true);
                        CameraObject.DinoLiteSDK.MicroTouchPressed += V_MicroTouchPress;
                        CameraObject.DinoLiteSDK.SetVideoFormat(CameraObject.ResolutionWidth, CameraObject.ResolutionHeight);
                        Thread.Sleep(50);
                        CameraObject.DinoLiteSDK.PreviewScale = true;
                        Thread.Sleep(50);
                        CameraObject.DinoLiteSDK.ColorFormat = 15;
                        Thread.Sleep(50);
                        CameraObject.DinoLiteSDK.Preview = true;
                        CameraObject.DinoLiteSDK.SetFLCLevel(0, 15);
                        Thread.Sleep(280);//led on need to wait it for reading         
                    }                                   
                }
                catch (Exception ex)
                {
                    Log.CustomLog.LogMessage("Error during camera initialization: " + ex.Message);
                }
            }
            catch (Exception ex)
            {
                Log.CustomLog.LogMessage("Error during camera initialization: " + ex.Message);
            }


            if (!DataHandlerUseCases.SettingExists(ConfigurationConstants.CameraConfigurationName))
            {
                Log.CustomLog.LogMessage("Creating cameraSettings json");
                businessSystem.CameraSettingsModel = FirstTimeConfigurationUseCases.GetCameraConfigurationDefaults();
                DataHandlerUseCases.SaveOrAppendToSettings(businessSystem.CameraSettingsModel, ConfigurationConstants.CameraConfigurationName);
            }

            try
            {
                DataHandlerUseCases.CMD("./StopSocketAI.vbs");
                Thread.Sleep(1000);
            }
            catch(Exception ex)
            {
                Log.CustomLog.LogMessage("Error during Stop Socket AI Command: " + ex.Message);
            }

            try
            {
                DataHandlerUseCases.CMD("./DelSocketAI.vbs");
            }
            catch (Exception ex)
            {
                Log.CustomLog.LogMessage("Error during Del Socket AI Command: " + ex.Message);
            }

            try
            {
                DataHandlerUseCases.CMD("./StartSocketAI.vbs");
                Thread.Sleep(1000);
            }
            catch(Exception ex)
            {
                Log.CustomLog.LogMessage("Error during Start Socket AI Command: " + ex.Message);
            }
            
            StartLoadingWait(false);
        }

        private void V_MicroTouchPress(object sender, EventArgs e)
        {
            CameraObject.DinoLiteSDK.SaveFrame("c:\\55.png");
        }

        /// <summary>
        /// Initialize TCP communication. At this moment, a socket is opened to listen on the network.
        /// </summary>
        private void StartServeTCP()
        {
            Log.CustomLog.LogMessage(businessSystem
                .StartComunication(_generalSettings.PortInterface));//("0.0.0.0", 5050) //"10.167.1.212"           
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

        /// <summary>
        /// AutomaticBristleClassification class call
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                ScreenNavigationUseCases.SaveGeneralLocalSettings(_generalSettings);
                Views.NeuralNetworkRetraining neuralNetworkRetraining = new NeuralNetworkRetraining(maximized, businessSystem, _colgateSkeltaEntities);
                neuralNetworkRetraining.Show();
                this.Close();
            }
            else
            {
                if (ScreenNavigationUseCases.ValidateUserAdministratorPermission(businessSystem.NetworkUserModel) || ScreenNavigationUseCases.ValidateUserQualityPermission(businessSystem.NetworkUserModel))
                {
                    ScreenNavigationUseCases.SaveGeneralLocalSettings(_generalSettings);
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
            if (businessSystem.UserSystemCurrent.Type == "administrator")
            {
                ScreenNavigationUseCases.SaveGeneralLocalSettings(_generalSettings);
                Views.GeneralSettings generalSettings = new Views.GeneralSettings(maximized, businessSystem, _colgateSkeltaEntities);
                generalSettings.Show();
                this.Close();
            }
            else
            {
                if (ScreenNavigationUseCases.ValidateUserAdministratorPermission(businessSystem.NetworkUserModel) || ScreenNavigationUseCases.ValidateUserQualityPermission(businessSystem.NetworkUserModel))
                {
                    ScreenNavigationUseCases.SaveGeneralLocalSettings(_generalSettings);
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

        private void ButtonGeneralReport_Click_1(object sender, RoutedEventArgs e)
        {          
            if (businessSystem.UserSystemCurrent.Type == "administrator")
            {
                ScreenNavigationUseCases.SaveGeneralLocalSettings(_generalSettings);
                Views.GeneralReport generalReport = new GeneralReport(maximized, businessSystem, _colgateSkeltaEntities);
                generalReport.Show();
                this.Close();
            }
            else
            {
                if (ScreenNavigationUseCases.ValidateUserAdministratorPermission(businessSystem.NetworkUserModel) || ScreenNavigationUseCases.ValidateUserQualityPermission(businessSystem.NetworkUserModel))
                {
                    ScreenNavigationUseCases.SaveGeneralLocalSettings(_generalSettings);
                    Views.GeneralReport generalReport = new GeneralReport(maximized, businessSystem, _colgateSkeltaEntities);
                            generalReport.Show();
                            this.Close();
                }
                else
                {
                    MessageBox.Show("Necessary administrative rights!");
                }
            }
        }

        private void Password_Click(object sender, RoutedEventArgs e)
        {
            if (businessSystem.UserSystemCurrent.Type == "administrator")
            {
                ScreenNavigationUseCases.SaveGeneralLocalSettings(_generalSettings);
                Views.Password passwordView = new Views.Password(maximized, businessSystem, _colgateSkeltaEntities);
                passwordView.Show();
                this.Close();
            }
            else
            {
                if (ScreenNavigationUseCases.ValidateUserAdministratorPermission(businessSystem.NetworkUserModel) || ScreenNavigationUseCases.ValidateUserQualityPermission(businessSystem.NetworkUserModel))
                {
                    ScreenNavigationUseCases.SaveGeneralLocalSettings(_generalSettings);
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
                ScreenNavigationUseCases.SaveGeneralLocalSettings(_generalSettings);
                Views.User userView = new Views.User(maximized, businessSystem, _colgateSkeltaEntities);
                userView.Show();
                this.Close();
            }
            else
            {
                if (ScreenNavigationUseCases.ValidateUserAdministratorPermission(businessSystem.NetworkUserModel) || ScreenNavigationUseCases.ValidateUserQualityPermission(businessSystem.NetworkUserModel))
                {
                    ScreenNavigationUseCases.SaveGeneralLocalSettings(_generalSettings);
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
