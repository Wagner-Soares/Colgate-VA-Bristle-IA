using APIVision;
using APIVision.Controllers;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Diagnostics;
using Application = System.Windows.Application;
using MessageBox = System.Windows.Forms.MessageBox;
using Bristle.Log;

namespace Bristle.Views
{
    /// <summary>
    /// Playful interaction for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static bool maximized = true;
        private BusinessSystem businessSystem;
        private static Thread startServeTCP_;
        private string data = String.Empty;
        private int exposure = 0;
        private System.Drawing.Image img;
        private bool cameraStart = false;
        private bool maximized__ = false;
        private System.Windows.Forms.Integration.WindowsFormsHost host;
        private Thread threadLoadingWait;
        private LoadingAnimation loading;
        private System.Windows.Forms.Timer myTimer = new System.Windows.Forms.Timer();
        private bool releaseMenu_ = false;
        private Log.Log log = new Log.Log(); 

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="maximized_"></param>
        /// <param name="businessSystem_"></param>
        public MainWindow(bool maximized_, BusinessSystem businessSystem_)
        {       
            businessSystem = businessSystem_;
            maximized__ = maximized_;

            InitializeComponent();

            this.Loaded += MainWindow_Loaded;
            this.Closed += new EventHandler(MainWindow_Closed);
        
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            //Bitmap bitmap = (Bitmap)ProductionObject.dinoLiteSDK.GrabFrame();
            //IntPtr bh = bitmap.GetHbitmap();
            //DeleteObject(bh);
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
                host = new System.Windows.Forms.Integration.WindowsFormsHost();
                host.Child = ProductionObject.dinoLiteSDK;
                this.viewSDK.Children.Add(host);
            }
            catch (Exception ex)
            {
                log.LogMessage(ex.Message);
            }
            
            StartLoadingWait(true);

            double w = (648 * SystemParameters.PrimaryScreenWidth) / 934.5;
            double h = (486 * SystemParameters.PrimaryScreenHeight) / 610.5;
            viewSDK.Width = w;
            viewSDK.Height = h;

            viewSDKImage.Width = viewSDK.Width;
            viewSDKImage.Height = viewSDK.Height;
            //viewSDKImage.Stretch = Stretch.Fill;

            frameHolder.Width = viewSDK.Width;
            frameHolder.Height = viewSDK.Height;
            frameHolder.Stretch = Stretch.Fill;

            Thread Camera = new Thread(() => loadCamera());
            Camera.Name = "Camera";
            Camera.IsBackground = true;
            Camera.Start();

            myTimer.Tick += new EventHandler(releaseMenu);
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

        private void releaseMenu(Object myObject, EventArgs myEventArgs)
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
                threadLoadingWait = new Thread(() => ThreadMethod());
                threadLoadingWait.Name = "threadLoadingWait";
                threadLoadingWait.SetApartmentState(ApartmentState.STA); //Set the thread to STA
                //threadLoadingWait.IsBackground = true;
                threadLoadingWait.Start();
            }
            else
            {
                Thread wait__ = new Thread(() => wait_());
                wait__.Name = "wait_";
                wait__.SetApartmentState(ApartmentState.STA); //Set the thread to STA                
                wait__.Start();
                wait__.Join();                
            }
        }

        private void sizeChanged(object sender, SizeChangedEventArgs e)
        {
            double w = (648 * this.ActualWidth) / 934.5;
            double h = (486 * this.ActualHeight) / 610.5;
            viewSDK.Width = w;
            viewSDK.Height = h;

            viewSDKImage.Width = viewSDK.Width;
            viewSDKImage.Height = viewSDK.Height;
            //viewSDKImage.Stretch = Stretch.Fill;

            frameHolder.Width = viewSDK.Width;
            frameHolder.Height = viewSDK.Height;
            frameHolder.Stretch = Stretch.Fill;

            viewSDK.UpdateLayout();
            viewSDKImage.UpdateLayout();
            frameHolder.UpdateLayout();
        }

        private void wait_()
        {
            if(businessSystem.generalSettings.MissingCamera)
            {
                Thread.Sleep(2000);
            }           
            ThreadMethod();
            releaseMenu_ = true;
        }

        private void LoadingWait_()
        {
            loading = new Views.LoadingAnimation("other");
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

        /// <summary>
        /// Initialize general system settings and camera configuration parameters. 
        /// Create the directories, if they do not exist: generalSettings.json and 
        /// cameraSettings.json and populate with default values.
        /// </summary>
        private void loadCamera()
        {           
            if(!cameraStart)
            {
                log.LogMessage("Initializating camera");

                string pathAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                businessSystem.localRepository = pathAppData + @"\Colgate Vision\Interface";
                if (!Directory.Exists(businessSystem.localRepository))
                {
                    log.LogMessage("Trying to create Interface dir");
                    Directory.CreateDirectory(businessSystem.localRepository);
                }

                if (!File.Exists(businessSystem.localRepository + @"\generalSettings.json"))
                {
                    log.LogMessage("Creating generalSettings json");
                    //businessSystem.generalSettings.NamePrefix = @"Datalog\";////@"\\UO631M4067452\Dataset-Colgate\";
                    businessSystem.generalSettings.IpPrediction = "10.140.1.9";
                    businessSystem.generalSettings.PortPrediction = 8888;
                    businessSystem.generalSettings.IpTrain = "10.140.1.9";
                    businessSystem.generalSettings.PortTrain = 8888;
                    businessSystem.generalSettings.IpInterface = "127.0.0.1";
                    businessSystem.generalSettings.PortInterface = 5050;
                    businessSystem.generalSettings.VideoFormatHeight = 2592;
                    businessSystem.generalSettings.VideoFormatWidth = 1944;
                    businessSystem.generalSettings.CameraSelect = 0;
                    businessSystem.generalSettings.CurrentCameraConfiguration = "Default";
                    businessSystem.generalController.SaveJson(businessSystem.generalSettings, businessSystem.localRepository + @"\generalSettings.json");
                }
                else
                {
                    log.LogMessage("Reading generalSettings json");
                    businessSystem.generalSettings = businessSystem.generalController.ReadJson3(businessSystem.localRepository + @"\generalSettings.json");
                }            

                try
                {
                    if (startServeTCP_ != null)
                    {
                        startServeTCP_.Abort();
                        //startServeTCP_ = new Thread(startServeTCP);
                        startServeTCP_.IsBackground = true;
                        startServeTCP_.Start();
                    }
                    else
                    {
                        startServeTCP_ = new Thread(startServeTCP);
                        //startServeTCP_.IsBackground = true;
                        startServeTCP_.Start();
                    }
                }
                catch (Exception ex)
                {
                    log.LogMessage("Error during initialization: " + ex.Message);
                }

                if (!File.Exists(businessSystem.localRepository + @"\commandS300.json"))
                {
                    log.LogMessage("Creating command300 json");
                    businessSystem.commandS300Model.NewModelId = 1;
                    businessSystem.commandS300Model.OldModelId = 0;
                    businessSystem.generalController.SaveJson(businessSystem.commandS300Model, businessSystem.localRepository + @"\commandS300.json");

                    object obj = businessSystem.generalController.ReadJson(businessSystem.localRepository + @"\commandS300.json");
                    businessSystem.generalSettings.Model = businessSystem.generalController.SaveJson(obj);

                    //businessSystem.SendCommand("S100", "");
                    //businessSystem.data = "";
                    //businessSystem.SendCommand("S300", data);
                    //businessSystem.data = "";      
                }
                else
                {
                    object obj = businessSystem.generalController.ReadJson(businessSystem.localRepository + @"\commandS300.json");
                    businessSystem.commandS300Model.NewModelId = businessSystem.generalController.ReadJson2(businessSystem.localRepository + @"\commandS300.json");
                    businessSystem.generalSettings.Model = businessSystem.generalController.SaveJson(obj);
                    businessSystem.commandS300Model.OldModelId = 0;

                    //businessSystem.SendCommand("S100", "");
                    //businessSystem.data = "";
                    //businessSystem.SendCommand("S300", data);
                    //businessSystem.data = "";
                }

                businessSystem.generalSettingsModels = businessSystem.dataBaseController.listGeneralSettingsModel();
                try
                {
                    if (businessSystem.generalSettingsModels.Count > 0)
                    {
                        businessSystem.generalSettings.NamePrefix = businessSystem.generalSettingsModels[0].Prefix;
                    }
                    else
                    {
                        businessSystem.generalSettingsModel.Prefix = @"\\BRVIPE06VZ51\Colgate Storage\Images\";
                        //businessSystem.generalSettingsModel.Prefix = @"\\UO631M4067452\Dataset-Colgate\";
                        businessSystem.dataBaseController.updateGeneralSettingsModel(businessSystem.generalSettingsModel, null, null);
                    }
                }
                catch (Exception ex)
                {
                    log.LogMessage("Error during initialization: " + ex.Message);
                }
                

                ProductionObject.ResolutionWidth = businessSystem.generalSettings.VideoFormatHeight;
                ProductionObject.ResolutionHeight = businessSystem.generalSettings.VideoFormatWidth;
                cameraStart = true;
            }

            try
            {
                try
                {
                    for (int i = 0; i < ProductionObject.dinoLiteSDK.GetVideoDeviceCount(); i++)
                    {
                        businessSystem.generalSettings.MissingCamera = true;
                    }
                    
                    if(businessSystem.generalSettings.MissingCamera)
                    {
                        ProductionObject.dinoLiteSDK.Connected = true;
                        ProductionObject.dinoLiteSDK.Preview = false;
                        ProductionObject.dinoLiteSDK.Connected = false;
                        ProductionObject.dinoLiteSDK.PreviewScale = false;
                        Thread.Sleep(50);
                        ProductionObject.dinoLiteSDK.VideoDeviceIndex = ProductionObject.VideoDeviceIndex;
                        ProductionObject.dinoLiteSDK.UseVideoFilter = DNVideoXLib.vcxUseVideoFilterEnum.vcxBoth;
                        ProductionObject.dinoLiteSDK.Connected = true;
                        Thread.Sleep(50);
                        ProductionObject.dinoLiteSDK.EnableMicroTouch(true);
                        ProductionObject.dinoLiteSDK.MicroTouchPressed += v_MicroTouchPress;
                        ProductionObject.dinoLiteSDK.SetVideoFormat(ProductionObject.ResolutionWidth, ProductionObject.ResolutionHeight);
                        Thread.Sleep(50);
                        ProductionObject.dinoLiteSDK.PreviewScale = true;
                        Thread.Sleep(50);
                        ProductionObject.dinoLiteSDK.ColorFormat = 15;
                        Thread.Sleep(50);
                        //ProductionObject.dinoLiteSDK.EnableNewFrameEvent = true;
                        ProductionObject.dinoLiteSDK.Preview = true;
                        ProductionObject.dinoLiteSDK.SetFLCLevel(0, 15);
                        Thread.Sleep(280);//led on need to wait it for reading         
                    }                                   
                }
                catch (Exception ex)
                {
                    log.LogMessage("Error during camera initialization: " + ex.Message);
                }
            }
            catch (Exception ex)
            {
                log.LogMessage("Error during camera initialization: " + ex.Message);
            }


            if (!File.Exists(businessSystem.localRepository + @"\cameraSettings.json"))
            {
                log.LogMessage("Creating cameraSettings json");
                businessSystem.cameraSettingsModel.Name = "Default";
                businessSystem.cameraSettingsModel.LedsOn = 15;
                businessSystem.cameraSettingsModel.LedBrightness = 5;
                businessSystem.cameraSettingsModel.Exposure = 500;
                businessSystem.cameraSettingsModel.Brightness = 30;
                businessSystem.cameraSettingsModel.Contrast = 25;
                businessSystem.cameraSettingsModel.Hue = 0;
                businessSystem.cameraSettingsModel.WhiteBalance = 0;
                businessSystem.cameraSettingsModel.AWBR = 0;
                businessSystem.cameraSettingsModel.AWBG = 0;
                businessSystem.cameraSettingsModel.AWBB = 0;
                businessSystem.cameraSettingsModel.Saturation = 16;
                businessSystem.cameraSettingsModel.Sharpness = 1;
                businessSystem.cameraSettingsModel.Gamma = 8;
                businessSystem.cameraSettingsModel.AutomaticExposure = 0;
                businessSystem.cameraSettingsModel.Focus = 0;//Math.Round(ProductionObject.dinoLiteSDK.GetAMRwithLensPos(ProductionObject.VideoDeviceIndex), 2);
                businessSystem.cameraSettingsModel.Mirror = 0;
                businessSystem.cameraSettingsModel.Negative = 0;
                businessSystem.cameraSettingsModel.Monochrome = 0;
                businessSystem.cameraSettingsModel.ColorFormat = 15;
                businessSystem.cameraSettingsModel.SKU = "";
                businessSystem.cameraSettingsModel.Equipment = "";
                businessSystem.generalController.SaveJson(businessSystem.cameraSettingsModel, businessSystem.localRepository + @"\cameraSettings.json", true);
            }

            try
            {
                businessSystem.generalController.CMD("./StopSocketAI.vbs");
                Thread.Sleep(1000);
            }
            catch(Exception ex)
            {
                log.LogMessage("Error during Stop Socket AI Command: " + ex.Message);
            }

            try
            {
                if (File.Exists(@"C:\ProgramData\Colgate Vision\AI\nn_controller.pid"))
                {
                    businessSystem.generalController.CMD("./DelSocketAI.vbs");
                }
            }
            catch (Exception ex)
            {
                log.LogMessage("Error during Del Socket AI Command: " + ex.Message);
            }

            try
            {
                businessSystem.generalController.CMD("./StartSocketAI.vbs");
                Thread.Sleep(1000);
            }
            catch(Exception ex)
            {
                log.LogMessage("Error during Start Socket AI Command: " + ex.Message);
            }
            
            StartLoadingWait(false);
        }

        private void v_MicroTouchPress(object sender, EventArgs e)
        {
            // label1.Content = "Press";
            ProductionObject.dinoLiteSDK.SaveFrame("c:\\55.png");
        }

        /// <summary>
        /// Initialize TCP communication. At this moment, a socket is opened to listen on the network.
        /// </summary>
        private void startServeTCP()
        {
            log.LogMessage(businessSystem
                .StartComunication(businessSystem.generalSettings.IpInterface, businessSystem.generalSettings.PortInterface));//("0.0.0.0", 5050); //"10.167.1.212"           
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
            //Views.MainWindow mainWindow = new Views.MainWindow(maximized, businessSystem);
            //mainWindow.Show();
            //this.Close();
        }

        /// <summary>
        /// AutomaticBristleClassification class call
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonAutomaticBristleClassification_Click(object sender, RoutedEventArgs e)
        {
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
            ProductionObject.dinoLiteSDK.MicroTouchPressed -= v_MicroTouchPress;
            ProductionObject.dinoLiteSDK.Connected = false;
            Views.UserControl userControl = new UserControl();
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
                if (businessSystem.networkUserModel.NetworkUserGroup != null)
                {
                    //Validate the group that can access this part 
                    foreach (var group in businessSystem.networkUserModel.NetworkUserGroup)
                    {
                        if (group == businessSystem.AD_Admin || group == businessSystem.AD_Quality)
                        {
                            Views.NeuralNetworkRetraining neuralNetworkRetraining = new NeuralNetworkRetraining(maximized, businessSystem, null);
                            neuralNetworkRetraining.Show();
                            this.Close();
                        }
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
            if (businessSystem.userSystemCurrent.Type == "administrator")
            {
                Views.GeneralSettings generalSettings = new Views.GeneralSettings(maximized, businessSystem, null);
                generalSettings.Show();
                this.Close();
            }
            else
            {
                if (businessSystem.networkUserModel.NetworkUserGroup != null)
                {
                    //Validate the group that can access this part 
                    foreach (var group in businessSystem.networkUserModel.NetworkUserGroup)
                    {
                        if (group == businessSystem.AD_Admin || group == businessSystem.AD_Quality)
                        {
                            Views.GeneralSettings generalSettings = new Views.GeneralSettings(maximized, businessSystem, null);
                            generalSettings.Show();
                            this.Close();
                        }
                    }
                }

                MessageBox.Show("Necessary administrative rights!");
            }
        }

        private void ButtonPoC_Click_1(object sender, RoutedEventArgs e)
        {
     
        }

        private void ButtonReport_Click_1(object sender, RoutedEventArgs e)
        {           
            if (businessSystem.userSystemCurrent.Type == "administrator")
            {
                Views.Report report = new Report(maximized, businessSystem, null);
                report.Show();
                this.Close();
            }
            else
            {
                if (businessSystem.networkUserModel.NetworkUserGroup != null)
                {
                    //Validate the group that can access this part 
                    foreach (var group in businessSystem.networkUserModel.NetworkUserGroup)
                    {
                        if (group == businessSystem.AD_Admin || group == businessSystem.AD_Quality)
                        {
                            Views.Report report = new Report(maximized, businessSystem, null);
                            report.Show();
                            this.Close();
                        }
                    }
                }

                MessageBox.Show("Necessary administrative rights!");
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
        private void Window_MouseLeftButtonDown(object sender )
        {
            // aqui movemos usand dragMove
            this.DragMove();
        }

        private void ButtonGeneralReport_Click_1(object sender, RoutedEventArgs e)
        {          
            if (businessSystem.userSystemCurrent.Type == "administrator")
            {
                Views.GeneralReport generalReport = new GeneralReport(maximized, businessSystem, null);
                generalReport.Show();
                this.Close();
            }
            else
            {
                if (businessSystem.networkUserModel.NetworkUserGroup != null)
                {
                    //Validate the group that can access this part 
                    foreach (var group in businessSystem.networkUserModel.NetworkUserGroup)
                    {
                        if (group == businessSystem.AD_Admin || group == businessSystem.AD_Quality)
                        {
                            Views.GeneralReport generalReport = new GeneralReport(maximized, businessSystem, null);
                            generalReport.Show();
                            this.Close();
                        }
                    }
                }

                MessageBox.Show("Necessary administrative rights!");
            }
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
                if(businessSystem.networkUserModel.NetworkUserGroup != null)
                {
                    //Validate the group that can access this part 
                    foreach (var group in businessSystem.networkUserModel.NetworkUserGroup)
                    {
                        if (group == businessSystem.AD_Admin || group == businessSystem.AD_Quality)
                        {
                            Views.Password password = new Views.Password(maximized, businessSystem, null);
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
                Views.User user = new Views.User(maximized, businessSystem, null);
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
                            Views.User user = new Views.User(maximized, businessSystem, null);
                            user.Show();
                            this.Close();
                        }
                    }
                }
              
                MessageBox.Show("Necessary administrative rights!");
            }

            //add user
            //Views.User user = new Views.User(maximized, businessSystem);
            //user.Show();
            //this.Close();

        }

        private void ListViewItem_Selected(object sender, RoutedEventArgs e)
        {

        }

        private void MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();                      
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Views.Help help = new Views.Help(maximized, businessSystem, null);
            help.Show();       
        }
    }
}
