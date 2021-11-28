using APIVision;
using APIVision.Controllers;
using APIVision.Controllers.DataBaseControllers;
using APIVision.DataModels;
using Bristle.UseCases;
using Bristle.utils;
using Database;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Bristle.Views
{
    /// <summary>
    /// Internal logic for AutomaticBristleClassification.xaml
    /// </summary>
    public partial class AutomaticBristleClassification : Window
    {
        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        private static extern bool DeleteObject(IntPtr hObject);

        public readonly BusinessSystem businessSystem;
        private readonly BackgroundWorker bkwAnalyzeStatus = new BackgroundWorker();

        private MemoryStream ms;
        private BitmapImage bi;
        private int X;
        private int Y;
        private int size_ = ConfigurationConstants.DefaultManualNewBoxSize;
        private bool LED1On = true;
        private bool LED2On = true;
        private bool LED3On = true;
        private bool LED4On = true;
        private readonly System.Windows.Forms.Timer myTimer = new System.Windows.Forms.Timer();
        private readonly System.Windows.Forms.Timer myTimer2 = new System.Windows.Forms.Timer();
        private readonly System.Windows.Forms.Timer myTimer3 = new System.Windows.Forms.Timer();
        private readonly System.Windows.Forms.Timer myTimer5 = new System.Windows.Forms.Timer();
        private readonly System.Windows.Forms.Timer timerConnectionStatus = new System.Windows.Forms.Timer();
        private readonly System.Windows.Forms.Timer timerConnectionStatusUpdateLayout = new System.Windows.Forms.Timer();
        private System.Drawing.Image img2;
        private System.Drawing.Image photo_;
        private int focus = 0;
        private int currentFocus = 0;
        private bool sqlConnStat = false;
        private bool visionConnStat = false;
        private bool startNotConfigureCameraInit = false;
        private readonly bool maximized__ = false;
        private System.Windows.Forms.Integration.WindowsFormsHost host;
        private Thread threadLoadingWait;
        private LoadingAnimation loading;

        private readonly AnalyzeSetController _analyzeSetController;
        private readonly TestController _testController;
        private readonly SkuController _sKUController;
        private readonly TuffAnalysisResultSetController _tuffAnalysisResultSetController;
        private readonly BristleAnalysisResultSetController _bristleAnalysisResultSetController;
        public readonly TuftTempSetSetController _tuftTempSetSetController;
        public readonly RegistrationWaitingController _registrationWaitingController;
        public readonly ImageTempSetSetController _imageTempSetSetController;
        private readonly BrushAnalysisResultSetController _brushAnalysisResultSetController;
        private readonly EquipmentController _equipmentController;
        private readonly QmSpecController _qM_SpecController;
        private AutomaticBristleClassificationPredictionLayer automaticBristleClassificationPredictionLayer;

        public bool Live { get; set; } = false;
        public bool StartNotConfigureCamera { get; set; }
        public List<string> Defect { get; } = new List<string>();
        public List<SocketModel> BoundingBox { get; } = new List<SocketModel>();

        public bool Maximized { get; set; } = true;
        public bool CaptureImage_ { get; set; } = true;
        public int CountBox { get; set; } = 0;
        public bool RegisterBox { get; set; } = true;
        public int Cont { get; set; } = 0;
        public int SelectAnalysis { get; set; } = 0;
        public string PositionResultManual { get; set; } = "T";
        public string Position { get; set; } = "T";
        public int BoundingBoxSelect { get; set; } = 0;
        public bool BoundingBoxEdit { get; set; } = false;
        public double W { get; set; } = 0;
        public double H { get; set; } = 0;
        public bool StartAnalyzing { get; set; } = false;
        public int SelectedCanvasBoundingBox { get; set; } = -1;
        public bool EditBoundingBoxLeft { get; set; } = false;
        public bool EditBoundingBoxRight { get; set; } = false;
        public bool EditBoundingBoxUp { get; set; } = false;
        public bool EditBoundingBoxDown { get; set; } = false;
        public string BoundingBoxSelectType { get; set; } = "undefined";
        public bool FrameHolderMouseMove { get; set; } = true;
        public bool StartCameraParam { get; set; } = false;
        public bool StartAutoFocus { get; set; } = true;
        public bool StopAutoFocus { get; set; } = true;
        public bool ManualFocusChanged { get; set; } = false;
        public float Adjustment { get; set; } = 0.093F;
        public int ImageCount { get; set; } = 0;
        public bool PhotoResult { get; set; } = false;
        public int Error1 { get; set; } = 0;
        public int Error2 { get; set; } = 0;
        public int Error3 { get; set; } = 0;
        public int Discard { get; set; } = 0;
        public int ToolboxOperation { get; set; } = 0;
        public bool LiveOn_ { get; set; } = true;
        public List<SocketModel> SocketModels { get; set; } = new List<SocketModel>();
        public System.Windows.Point MaskPointStart { get; set; } = new System.Windows.Point();
        public System.Windows.Point MaskPointStartMemory { get; set; } = new System.Windows.Point();
        public System.Windows.Point MaskPointStopMemory { get; set; } = new System.Windows.Point();
        public bool MaskDelete { get; set; } = false;
        public int NumberMask { get; set; } = -1;
        public bool VerySmallMask { get; set; } = false;

        public ColgateSkeltaEntities ColgateSkeltaEntities { get; set; }
        public GeneralLocalSettings GeneralSettings { get; set; }
        public List<CameraSettingsModel> CameraSettingsModels { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="maximized_"></param>
        /// <param name="businessSystem_"></param>
        public AutomaticBristleClassification(bool maximized_, BusinessSystem businessSystem_, ColgateSkeltaEntities colgateSkeltaEntities)
        {
            ColgateSkeltaEntities = colgateSkeltaEntities;

            _analyzeSetController = new AnalyzeSetController(colgateSkeltaEntities);
            _testController = new TestController(colgateSkeltaEntities);
            _sKUController = new SkuController(colgateSkeltaEntities);
            _tuffAnalysisResultSetController = new TuffAnalysisResultSetController(colgateSkeltaEntities);
            _bristleAnalysisResultSetController = new BristleAnalysisResultSetController(colgateSkeltaEntities);
            _tuftTempSetSetController = new TuftTempSetSetController(colgateSkeltaEntities);
            _registrationWaitingController = new RegistrationWaitingController(colgateSkeltaEntities);
            _imageTempSetSetController = new ImageTempSetSetController(colgateSkeltaEntities);
            _brushAnalysisResultSetController = new BrushAnalysisResultSetController(colgateSkeltaEntities);
            _equipmentController = new EquipmentController(colgateSkeltaEntities);
            _qM_SpecController = new QmSpecController(colgateSkeltaEntities);

            GeneralSettings = ScreenNavigationUseCases.GetGeneralLocalSettings();

            businessSystem = businessSystem_;
            maximized__ = maximized_;

            InitializeComponent();

            this.Loaded += AutomaticBristleClassification_Loaded;
            this.Closed += new EventHandler(AutomaticBristleClassification_Closed);

            StartNotConfigureCamera = false;

            CameraObject.DinoLiteSDK.MicroTouchPressed += V_MicroTouchPress;
            bkwAnalyzeStatus.DoWork += new DoWorkEventHandler(SQLAndVisionConnectionStatus);
            bkwAnalyzeStatus.RunWorkerCompleted += BkwAnalyzeStatus_RunWorkerCompleted;
        }

        private void BkwAnalyzeStatus_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            timerConnectionStatus.Enabled = true;
        }

        private void AutomaticBristleClassification_Loaded(object sender, RoutedEventArgs e)
        {
            automaticBristleClassificationPredictionLayer = new AutomaticBristleClassificationPredictionLayer(this)
            {
                Owner = this
            };

            StartLoadingWait(true);
            /*
             * SDK Dino-lite
             */
            host = new System.Windows.Forms.Integration.WindowsFormsHost
            {
                Child = CameraObject.DinoLiteSDK
            };

            this.viewSDK.Children.Add(host);

            if (Directory.Exists("img"))
            {
                Directory.Delete("img", true);
            }

            WindowSize();
            //Updates some camera parameters
            myTimer2.Tick += new EventHandler(CameraParam);
            myTimer2.Interval = 50;
            myTimer2.Start();

            //Monitors if there is boundingbox to draw
            myTimer.Tick += new EventHandler(IA);
            myTimer.Interval = 10;
            myTimer.Start();

            timerConnectionStatus.Tick += new EventHandler(TimerStatus_Tick);
            timerConnectionStatus.Interval = 10000;
            timerConnectionStatus.Start();

            timerConnectionStatusUpdateLayout.Tick += new EventHandler(TimerStatusLayout_Tick);
            timerConnectionStatusUpdateLayout.Interval = 5000;
            timerConnectionStatusUpdateLayout.Start();

            automaticBristleClassificationPredictionLayer.warningText1.Visibility = Visibility.Visible;
            automaticBristleClassificationPredictionLayer.warningButton1.Visibility = Visibility.Visible;
            automaticBristleClassificationPredictionLayer.modalL.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 188, 64, 49));
            automaticBristleClassificationPredictionLayer.modalR.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 188, 64, 49));

            if (maximized__)
            {
                this.WindowState = WindowState.Maximized;
                Maximized = true;
            }
            else
            {
                this.WindowState = WindowState.Normal;
                Maximized = false;
            }
        }

        private void TimerStatusLayout_Tick(object sender, EventArgs e)
        {
            if (visionConnStat)
            {
                PythonConnectionStatus.Background = System.Windows.Media.Brushes.Green;
            }
            else
            {
                PythonConnectionStatus.Background = System.Windows.Media.Brushes.DarkRed;
            }

            if (sqlConnStat)
            {
                btnSQLStatus.Background = System.Windows.Media.Brushes.Green;
            }
            else
            {
                btnSQLStatus.Background = System.Windows.Media.Brushes.DarkRed;
            }

            timerConnectionStatusUpdateLayout.Stop();
            timerConnectionStatusUpdateLayout.Enabled = true;
        }

        private void TimerStatus_Tick(object sender, EventArgs e)
        {
            if(!bkwAnalyzeStatus.IsBusy)
                bkwAnalyzeStatus.RunWorkerAsync();

            timerConnectionStatus.Stop();
        }

        /// <summary>
        /// StartLoadingWait
        /// </summary>
        /// <param name="start"></param>
        public void StartLoadingWait(bool start)
        {
            if (start)
            {
                threadLoadingWait = new Thread(() => ThreadMethod());
                threadLoadingWait.SetApartmentState(ApartmentState.STA); //Set the thread to STA
                threadLoadingWait.Start();
            }
            else
            {
                Thread wait__ = new Thread(() => Wait_());
                wait__.SetApartmentState(ApartmentState.STA); //Set the thread to STA
                wait__.Start();
            }
        }

        private void Wait_()
        {
            ThreadMethod();
        }

        private void LoadingWait_()
        {
            try
            {
                loading = new Views.LoadingAnimation("other")
                {
                    Topmost = true
                };
                loading.Activate();
                loading.ShowDialog();
            }
            catch
            {
                //Preven
            }
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

        private void V_MicroTouchPress(object sender, EventArgs e)
        {
            CameraObject.DinoLiteSDK.AutoFocus(CameraObject.VideoDeviceIndex);
            StartAutoFocus = false;
        }

        private void CameraParam(Object myObject, EventArgs myEventArgs)
        {
            try
            {
                if (!StartCameraParam && GeneralSettings.MissingCamera)
                {
                    Init();
                    StartCameraParam = true;
                    CameraConfigure();
                    Live = true;
                    KickStart();

                    StartLoadingWait(false);
                }
            }
            catch(Exception e)
            {
                Log.CustomLog.LogMessage("An error occured while configuring camera: " + e.Message);
            }

            Thread updateMnualFocusValue_ = new Thread(() => UpdateMnualFocusValue())
            {
                IsBackground = true
            };
            updateMnualFocusValue_.Start();

            try
            {
                if (ManualFocusChanged)
                {
                    focusValue.Text = Math.Round(businessSystem.CameraSettingsModel.Focus, 2).ToString();
                    ManualFocusChanged = false;
                }
            }
            catch(Exception e)
            {
                Log.CustomLog.LogMessage("An error occured while sending S100 command: " + e.Message);
            }


            if (!StartAutoFocus)
            {
                focusValue.Text = "WAIT";
                Thread autoFocusUpdateWait_ = new Thread(() => AutoFocusUpdateWait())
                {
                    IsBackground = true
                };
                autoFocusUpdateWait_.Start();
                StartAutoFocus = true;
            }

            if (!StopAutoFocus)
            {
                try
                {
                    double tmp = CameraObject.DinoLiteSDK.GetAMRwithLensPos(CameraObject.VideoDeviceIndex);
                    focusValue.Text = Math.Round(tmp, 2).ToString();
                    currentFocus = (int)(((tmp - businessSystem.CameraSettingsModel.Focus) * 1) / Adjustment);
                    businessSystem.CameraSettingsModel.Focus = tmp;
                    focus -= currentFocus;
                    StopAutoFocus = true;
                }
                catch(Exception e)
                {
                    Log.CustomLog.LogMessage("An error occured while stopping autoFocus: " + e.Message);
                }
            }

            myTimer2.Stop();
            myTimer2.Enabled = true;
        }

        private void FocusUpdate()
        {
            try
            {
                double tmp = CameraObject.DinoLiteSDK.GetAMRwithLensPos(CameraObject.VideoDeviceIndex);
                if (tmp > 10)
                {
                    Thread.Sleep(50);
                    currentFocus = (int)(((tmp - businessSystem.CameraSettingsModel.Focus) * 1) / Adjustment);
                    focus += currentFocus;
                    CameraObject.DinoLiteSDK.SetLensPos(CameraObject.VideoDeviceIndex, focus);
                    Thread.Sleep(50);
                    focusValue.Text = Math.Round(CameraObject.DinoLiteSDK.GetAMRwithLensPos(CameraObject.VideoDeviceIndex), 2).ToString();
                }
            }
            catch(Exception e)
            {
                Log.CustomLog.LogMessage("An error occured while updating focus: " + e.Message);
            }
        }

        private void AutoFocusUpdateWait()
        {
            Thread.Sleep(8000);
            StopAutoFocus = false;
        }

        private void UpdateMnualFocusValue()
        {
            try
            {
                double tmpFocus = CameraObject.DinoLiteSDK.GetAMRwithLensPos(CameraObject.VideoDeviceIndex);
                if (businessSystem.CameraSettingsModel.Focus != tmpFocus)
                {
                    focus = 0;
                    CameraObject.DinoLiteSDK.SetLensPos(CameraObject.VideoDeviceIndex, focus);
                    businessSystem.CameraSettingsModel.Focus = tmpFocus;
                    Thread.Sleep(25);
                    ManualFocusChanged = true;
                }
            }
            catch (Exception e)
            {
                businessSystem.CameraSettingsModel.Focus = CameraObject.DinoLiteSDK.GetAMR(CameraObject.VideoDeviceIndex);
                ManualFocusChanged = true;
                Log.CustomLog.LogMessage("An error occured while manually updating focus: " + e.Message);
            }
        }

        /// <summary>
        /// Resizing images
        /// </summary>
        private void WindowSize()
        {
            if (Maximized)
            {
                W = (648 * SystemParameters.PrimaryScreenWidth) / 934.5;
                H = (486 * SystemParameters.PrimaryScreenHeight) / 610.5;

                viewSDK_.Width = W;
                viewSDK_.Height = H;

                automaticBristleClassificationPredictionLayer.Width = W;
                automaticBristleClassificationPredictionLayer.Height = H;

                automaticBristleClassificationPredictionLayer.viewSDKImage.Width = W;
                automaticBristleClassificationPredictionLayer.viewSDKImage.Height = H;

                automaticBristleClassificationPredictionLayer.canvas.Width = viewSDK.Width;
                automaticBristleClassificationPredictionLayer.canvas.Height = viewSDK.Height;

                automaticBristleClassificationPredictionLayer.canvasMask.Width = viewSDK.Width;
                automaticBristleClassificationPredictionLayer.canvasMask.Height = viewSDK.Height;

                System.Windows.Point locationFromScreen = this.viewSDK.PointToScreen(new System.Windows.Point(0, 0));

                // Transform screen point to WPF device independent point

                PresentationSource source = PresentationSource.FromVisual(this);

                System.Windows.Point targetPoints = source.CompositionTarget.TransformFromDevice.Transform(locationFromScreen);

                // Set coordinates

                automaticBristleClassificationPredictionLayer.Top = targetPoints.Y;

                automaticBristleClassificationPredictionLayer.Left = targetPoints.X;

                automaticBristleClassificationPredictionLayer.Show();
            }
        }

        /// <summary>
        /// Initializes the selection inputs by the user
        /// </summary>
        private void Init()
        {
            skuSelect.Items.Add("-");
            testSelect.Items.Add("-");
            areaSelect.Items.Add("-");
            equipmentSelect.Items.Add("-");

            skuSelect.SelectedIndex = 0;
            testSelect.SelectedIndex = 0;
            areaSelect.SelectedIndex = 0;
            equipmentSelect.SelectedIndex = 0;

            var sKUModelList = _sKUController.ListSKUsModel();
            var testModels = _testController.ListTestModelBySKUId(-1);

            List<int> idSKUTest = new List<int>();
            foreach (var itemTest in testModels)
            {
                if (itemTest.SDescription == "Arredondamento")
                {
                    idSKUTest.Add((int)itemTest.ISKU);
                }
            }

            foreach (var item in sKUModelList)
            {
                foreach (var id in idSKUTest)
                {
                    if (id == item.IID)
                    {
                        skuSelect.Items.Add(item.SSKU);
                    }
                }
            }

            try
            {
                CameraSettingsModels = DataHandlerUseCases.ReadJsonCameraSettings(ConfigurationConstants.CameraConfigurationName);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            foreach (var item in CameraSettingsModels)
            {
                cameraConfigurationSelection.Items.Add(item.Name);
            }
            cameraConfigurationSelection.SelectedItem = GeneralSettings.CurrentCameraConfiguration;

            GridMenuRight.Visibility = Visibility.Collapsed;

            ledBrightness.Items.Add(1);
            ledBrightness.Items.Add(2);
            ledBrightness.Items.Add(3);
            ledBrightness.Items.Add(4);
            ledBrightness.Items.Add(5);
            ledBrightness.Items.Add(6);
            ledBrightness.SelectedIndex = businessSystem.CameraSettingsModel.LedBrightness - 1;

            businessSystem.BoundingBoxDiscards.Clear();
            businessSystem.BrushAnalysisResultModel.TotalBristles = 0;
            businessSystem.BrushAnalysisResultModel.TotalBristlesAnalyzed = 0;
            businessSystem.BrushAnalysisResultModel.TotalGoodBristles = 0;
            BoundingBox.Clear();
            businessSystem.TuffAnalysisResultModel.SelectedManual = false;
        }

        /// <summary>
        /// Sends an S100 command to the artificial intelligence module to start predictions
        /// </summary>
        private void KickStart()
        {
            AutomaticBristleClassification automaticBristleClassification = this;
            lock (automaticBristleClassification)
            {
                StartAnalyzing = true;
                businessSystem.SendCommand("S300", GeneralSettings.Model, GeneralSettings.IpPrediction, GeneralSettings.PortPrediction);
                businessSystem.Data = "";
                Thread.Sleep(1000);
                businessSystem.SendCommand("S100", (Bitmap)CameraObject.DinoLiteSDK.GrabFrame().Clone(), GeneralSettings.IpPrediction, GeneralSettings.PortPrediction);
                CameraObject.DinoLiteSDK.GrabFrame().Dispose();
                businessSystem.Data = "";
            }
        }

        public void ButtonPopUpLogout_Click(object sender, RoutedEventArgs e)
        {
            Live = false;

            CameraObject.DinoLiteSDK.MicroTouchPressed -= V_MicroTouchPress;
            CameraObject.DinoLiteSDK.Connected = false;
            Thread.Sleep(280);
            myTimer.Dispose();
            myTimer2.Dispose();
            myTimer3.Dispose();
            myTimer5.Dispose();
            Views.UserControl userControl = new UserControl();
            LiveOn_ = true;
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

        private void ButtonOpenMenuRight_Click(object sender, RoutedEventArgs e)
        {
            ButtonOpenMenuRight.Visibility = Visibility.Collapsed;
            ButtonCloseMenuRight.Visibility = Visibility.Visible;
        }

        private void ButtonCloseMenuRight_Click(object sender, RoutedEventArgs e)
        {
            ButtonOpenMenuRight.Visibility = Visibility.Visible;
            ButtonCloseMenuRight.Visibility = Visibility.Collapsed;
        }

        private void ButtonHome_Click(object sender, RoutedEventArgs e)
        {
            Live = false;

            if (ScreenNavigationUseCases.OpenMainScreen(GeneralSettings, businessSystem, ColgateSkeltaEntities, Maximized))
                this.Close();
        }

        private void ButtonWindowMaximize_Click(object sender, RoutedEventArgs e)
        {
            if (!Maximized)
            {
                this.WindowState = WindowState.Maximized;
                Maximized = true;
                WindowSize();
            }
            else
            {
                this.WindowState = WindowState.Normal;
                Maximized = false;
                WindowSize();
            }
        }

        private void ButtonWindowMinimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        /// <summary>
        /// Check if the user has administrative rights to access this class
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonNeuralNetworkRetraining_Click(object sender, RoutedEventArgs e)
        {
            if (ScreenNavigationUseCases.OpenNeuralNetworkRetrainingScreen(businessSystem.UserSystemCurrent, businessSystem.NetworkUserModel, GeneralSettings, businessSystem, ColgateSkeltaEntities, Maximized))
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
            if (ScreenNavigationUseCases.OpenGeneralSettingsScreen(businessSystem.UserSystemCurrent, businessSystem.NetworkUserModel, GeneralSettings, businessSystem, ColgateSkeltaEntities, Maximized))
            {
                this.Close();
            }
            else
            {
                MessageBox.Show("Necessary administrative rights!");
            }
        }

        private void ButtonGeneralReport_Click_1(object sender, RoutedEventArgs e)
        {
            Live = false;

            if (ScreenNavigationUseCases.OpenGeneralReportScreen(businessSystem.UserSystemCurrent, businessSystem.NetworkUserModel, GeneralSettings, businessSystem, ColgateSkeltaEntities, Maximized))
            {
                this.Close();
            }
            else
            {
                MessageBox.Show("Necessary administrative rights!");
            }
        }

        /// <summary>
        /// Apply the conditions to be able to run the live.
        /// </summary>
        /// <param name="next_"></param>
        private void LiveConditions(bool next_)
        {
            Live = true;
            BoundingBoxSelect = 0;
            AutomaticBristleClassification automaticBristleClassification = this;
            lock (automaticBristleClassification)
            {
                StartAnalyzing = true;
                businessSystem.SendCommand("S100", (Bitmap)CameraObject.DinoLiteSDK.GrabFrame().Clone(), GeneralSettings.IpPrediction, GeneralSettings.PortPrediction);
                CameraObject.DinoLiteSDK.GrabFrame().Dispose();
                businessSystem.Data = "";
            }

            GridMenuRight.Visibility = Visibility.Collapsed;
            next.IsEnabled = false;

            if (!next_)
            {
                ImageCount++;

                try
                {
                    ///Prepares to generate auxiliary images for the user. If there is no 
                    ///boudingBox and send only images of the frame.

                    if (automaticBristleClassificationPredictionLayer.canvas.Children.Count > 0)
                    {
                        Bitmap bitmap2;
                        AutomaticBristleClassification automaticBristleClassification1 = this;
                        lock (automaticBristleClassification1)
                        {
                            img2 = (Bitmap)CameraObject.DinoLiteSDK.GrabFrame().Clone();
                            CameraObject.DinoLiteSDK.GrabFrame().Dispose();
                            bitmap2 = DataHandlerUseCases.ResizeImage(img2, img2.Width, img2.Height);

                        }

                        RenderTargetBitmap rtb = new RenderTargetBitmap((int)automaticBristleClassificationPredictionLayer.canvas.RenderSize.Width,
                        (int)automaticBristleClassificationPredictionLayer.canvas.RenderSize.Height, 96d, 96d, System.Windows.Media.PixelFormats.Default);
                        rtb.Render(automaticBristleClassificationPredictionLayer.canvas);

                        BitmapEncoder pngEncoder = new PngBitmapEncoder();
                        pngEncoder.Frames.Add(BitmapFrame.Create(rtb));

                        using (var fs = System.IO.File.OpenWrite("imageAux.png"))
                        {
                            pngEncoder.Save(fs);
                        }

                        System.Drawing.Image image2;
                        AutomaticBristleClassification automaticBristleClassification2 = this;
                        lock (automaticBristleClassification2)
                        {
                            image2 = System.Drawing.Image.FromFile("imageAux.png");
                        }

                        System.Drawing.Bitmap bitmap = DataHandlerUseCases.ResizeImage(image2, img2.Width, img2.Height);


                        Thread t = new Thread(() => NewImageShow(bitmap2, bitmap, ImageCount))
                        {
                            Name = "t"
                        };
                        t.SetApartmentState(ApartmentState.STA); //Set the thread to STA
                        t.IsBackground = true;
                        t.Start();

                    }
                    else
                    {
                        img2 = (Bitmap)CameraObject.DinoLiteSDK.GrabFrame().Clone();
                        CameraObject.DinoLiteSDK.GrabFrame().Dispose();
                        System.Drawing.Bitmap bitmap2 = DataHandlerUseCases.ResizeImage(img2, img2.Width, img2.Height);
                        System.Drawing.Bitmap bitmap = null;
                        Thread t = new Thread(() => NewImageShow(bitmap2, bitmap, ImageCount))
                        {
                            Name = "t"
                        };
                        t.SetApartmentState(ApartmentState.STA); //Set the thread to STA
                        t.IsBackground = true;
                        t.Start();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        /// <summary>
        /// Auxiliary images presentation
        /// </summary>
        /// <param name="image">Frame</param>
        /// <param name="boundingBox">BoundingBox</param>
        /// <param name="imageCount">Image number</param>
        private void NewImageShow(Bitmap image, Bitmap boundingBox, int imageCount)
        {
            try
            {
                Directory.CreateDirectory("img");
                string path = Directory.GetCurrentDirectory() + "\\img\\auxImage" + imageCount;
                image.Save(path);
                List<Bitmap> images = new List<Bitmap>
                {
                    image,
                    boundingBox
                };

                System.Drawing.Bitmap imageWithBoundingBox;
                AutomaticBristleClassification automaticBristleClassification = this;
                lock (automaticBristleClassification)
                {
                    imageWithBoundingBox = new System.Drawing.Bitmap(image.Width, image.Height);
                    using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(imageWithBoundingBox))
                    {
                        int offset = 0;
                        foreach (System.Drawing.Bitmap image__ in images)
                        {
                            g.DrawImage(image__,
                              new System.Drawing.Rectangle(offset, 0, image.Width, image.Height));
                        }
                    }
                }

                path = Directory.GetCurrentDirectory() + "\\img\\auxBoundingBox" + imageCount;
                imageWithBoundingBox.Save(path);

                image.Dispose();
                boundingBox.Dispose();
                imageWithBoundingBox.Dispose();

                Views.ImageView imageView = new Views.ImageView(imageCount);
                imageView.ShowDialog();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        /// <summary>
        /// Applications for live condition is disabled
        /// </summary>
        private void ConditionsNotAlive()
        {
            Live = false;
            IAPhoto();
            next.IsEnabled = true;
            GridMenuRight.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// warning
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StartAnalysis_Click(object sender, RoutedEventArgs e)
        {
            if (skuSelect.SelectedIndex != 0 && equipmentSelect.SelectedIndex != 0 && areaSelect.SelectedIndex != 0 && batchLoteT.Text != string.Empty)
            {

                automaticBristleClassificationPredictionLayer.warningText1.Visibility = Visibility.Collapsed;
                automaticBristleClassificationPredictionLayer.warningButton1.Visibility = Visibility.Collapsed;
                automaticBristleClassificationPredictionLayer.warningText2.Visibility = Visibility.Visible;
                automaticBristleClassificationPredictionLayer.warningButton2.Visibility = Visibility.Visible;
                automaticBristleClassificationPredictionLayer.modalL.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 188, 64, 49));
                automaticBristleClassificationPredictionLayer.modalR.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 188, 64, 49));

                Analysis_();
            }
            else
            {
                automaticBristleClassificationPredictionLayer.warning.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// Applications for analysis of the bristles and everything that will be checked.
        /// </summary>
        private void Analysis_()
        {
            switch (SelectAnalysis)
            {
                case 0:
                    businessSystem.AnalyzeModel.Name = skuSelect.Items[skuSelect.SelectedIndex].ToString();
                    businessSystem.AnalyzeModel.Timestamp = DateTime.UtcNow;
                    businessSystem.AnalyzeModel.Equipament = equipmentSelect.Items[equipmentSelect.SelectedIndex].ToString();
                    SKU_select_view.Text = businessSystem.AnalyzeModel.Name;
                    GeneralSettings.Test = testSelect.Items[testSelect.SelectedIndex].ToString();
                    test_select_view.Text = GeneralSettings.Test;
                    GeneralSettings.Area = areaSelect.Items[areaSelect.SelectedIndex].ToString();
                    area_select_view.Text = GeneralSettings.Area;
                    equipament_select_view.Text = businessSystem.AnalyzeModel.Equipament;
                    GeneralSettings.BatchLote = batchLoteT.Text;
                    batchLote_select_view.Text = GeneralSettings.BatchLote;
                    camera_configuration_selection.Text = cameraConfigurationSelection.Text;
                    sku.Text = businessSystem.AnalyzeModel.Name;
                    automaticBristleClassificationPredictionLayer.AnalyzedId = _analyzeSetController.UpdateAnalyzeModel(businessSystem.AnalyzeModel);
                    GetImageFromCameraToClassification();
                    ConditionsNotAlive();
                    result_view.Visibility = Visibility.Visible;
                    CaptureImage_ = false;
                    startAnalyse_view.Visibility = Visibility.Collapsed;
                    cameraStartAnalysis.Kind = MaterialDesignThemes.Wpf.PackIconKind.TelevisionPause;
                    break;
                case 1:
                    businessSystem.AnalyzeModel.Timestamp = DateTime.UtcNow;
                    result_view.Visibility = Visibility.Visible;
                    CaptureImage_ = false;
                    GetImageFromCameraToClassification();
                    ConditionsNotAlive();
                    cameraStartAnalysis.Kind = MaterialDesignThemes.Wpf.PackIconKind.TelevisionPause;
                    break;
                default:
                    result_view.Visibility = Visibility.Visible;
                    CaptureImage_ = false;
                    GetImageFromCameraToClassification();
                    ConditionsNotAlive();
                    cameraStartAnalysis.Kind = MaterialDesignThemes.Wpf.PackIconKind.TelevisionPause;
                    break;
            }
        }

        /// <summary>
        /// Sends 1 frame for prediction via S100 command.
        /// </summary>
        private void IAPhoto()
        {
            try
            {
                AutomaticBristleClassification automaticBristleClassification = this;
                lock (automaticBristleClassification)
                {
                    ms = new MemoryStream();
                    photo_.Save(ms, ImageFormat.Png);
                    ms.Seek(0, SeekOrigin.Begin);
                    bi = new BitmapImage();
                    bi.BeginInit();
                    bi.StreamSource = ms;
                    bi.EndInit();

                    StartAnalyzing = true;
                    businessSystem.SendCommand("S100", (Bitmap)CameraObject.DinoLiteSDK.GrabFrame().Clone(), GeneralSettings.IpPrediction, GeneralSettings.PortPrediction);
                    CameraObject.DinoLiteSDK.GrabFrame().Dispose();
                    businessSystem.Data = "";

                    PhotoResult = true;
                }
            }
            catch(Exception e)
            {
                Log.CustomLog.LogMessage("An error occured while sending S100 command in AIPhoto method: " + e.Message);
            }


            myTimer3.Tick += new EventHandler(PhotoResult_);
            myTimer3.Interval = 10;
            myTimer3.Enabled = true;
            myTimer3.Start();
        }

        /// <summary>
        /// Monitors the result of the prediction of the sending done in the IAPhoto () method.
        /// </summary>
        /// <param name="myObject"></param>
        /// <param name="myEventArgs"></param>
        private void PhotoResult_(Object myObject, EventArgs myEventArgs)
        {
            if (PhotoResult)
            {
                if (businessSystem.Data != null && businessSystem.Data.Length > 0)
                {
                    if (businessSystem.Data != "*")
                    {
                        Cont = 0;
                        AutomaticBristleClassification automaticBristleClassification = this;
                        lock (automaticBristleClassification)
                        {
                            SocketModels = DataHandlerUseCases.GetSocketModelsReturned(businessSystem.Data, businessSystem);
                            businessSystem.Data = "";
                            DrawBoundingBox();
                            PhotoResult = false;
                        }
                    }
                }
                else
                {
                    Cont++;
                    if (Cont > 1000)
                    {
                        AutomaticBristleClassification automaticBristleClassification = this;
                        lock (automaticBristleClassification)
                        {
                            businessSystem.SendCommand("S100", (Bitmap)CameraObject.DinoLiteSDK.GrabFrame().Clone(), GeneralSettings.IpPrediction, GeneralSettings.PortPrediction);
                            CameraObject.DinoLiteSDK.GrabFrame().Dispose();
                            businessSystem.Data = "";
                        }
                        Cont = 0;
                    }
                }
            }
            myTimer3.Stop();
            myTimer3.Enabled = true;
        }

        /// <summary>
        /// Monitors AI returns to draw bounding boxes. Used for live.
        /// </summary>
        public void IA(Object myObject, EventArgs myEventArgs)
        {
            if (Live && StartAnalyzing)
            {
                if (businessSystem.Data != null && businessSystem.Data.Length > 0 && businessSystem.Data != "*")
                {
                    Cont = 0;
                    AutomaticBristleClassification automaticBristleClassification = this;
                    lock (automaticBristleClassification)
                    {
                        SocketModels = DataHandlerUseCases.GetSocketModelsReturned(businessSystem.Data, businessSystem);
                        businessSystem.Data = "";
                        DrawBoundingBox();
                    }
                    try
                    {
                        AutomaticBristleClassification automaticBristleClassification1 = this;
                        lock (automaticBristleClassification1)
                        {
                            businessSystem.SendCommand("S100", (Bitmap)CameraObject.DinoLiteSDK.GrabFrame().Clone(), GeneralSettings.IpPrediction, GeneralSettings.PortPrediction);

                            CameraObject.DinoLiteSDK.GrabFrame().Dispose();
                            businessSystem.Data = "";
                        }
                    }
                    catch (Exception e)
                    {
                        Log.CustomLog.LogMessage("An error occured while sending S100 command: " + e.Message);
                    }
                }
                else
                {
                    Cont++;
                    if (Cont > 10)
                    {
                        AutomaticBristleClassification automaticBristleClassification = this;
                        lock (automaticBristleClassification)
                        {
                            RecoverCommunicationWithAI();
                        }
                        Cont = 0;
                    }
                }
            }

            myTimer.Stop();
            myTimer.Enabled = true;
        }

        private void GetImageFromCameraToClassification()
        {
            automaticBristleClassificationPredictionLayer.photo.Visibility = Visibility.Visible;

            Bitmap frame = (Bitmap)CameraObject.DinoLiteSDK.GrabFrame().Clone();
            CameraObject.DinoLiteSDK.GrabFrame().Dispose();
            if (frame != null)
            {
                photo_ = (Bitmap)frame.Clone();
                MemoryStream memoryStream = new MemoryStream();
                frame.Save(memoryStream, ImageFormat.Jpeg);
                memoryStream.Seek(0, SeekOrigin.Begin);
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memoryStream;
                bitmapImage.EndInit();
                bitmapImage.Freeze();
                Dispatcher.BeginInvoke(new ThreadStart(delegate
                {
                    automaticBristleClassificationPredictionLayer.photo.Source = bitmapImage;
                }));

                WindowSize();

                frame.Dispose();
            }
        }

        /// <summary>
        /// recoverCommunicationWithAI
        /// </summary>
        private void RecoverCommunicationWithAI()
        {
            try
            {
                businessSystem.SendCommand("S300", GeneralSettings.Model, GeneralSettings.IpPrediction, GeneralSettings.PortPrediction);
                businessSystem.Data = "";
                businessSystem.SendCommand("S100", (Bitmap)CameraObject.DinoLiteSDK.GrabFrame().Clone(), GeneralSettings.IpPrediction, GeneralSettings.PortPrediction);
                CameraObject.DinoLiteSDK.GrabFrame().Dispose();
                businessSystem.Data = "";

                DataHandlerUseCases.CMD("./StartSocketAI.vbs");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        /// <summary>
        /// Draw the bounding box
        /// </summary>
        public void DrawBoundingBox()
        {
            try
            {
                automaticBristleClassificationPredictionLayer.canvas.Children.Clear();
                BoundingBox.Clear();

                if (Live)
                {
                    DrawRawBoxesWhilePositioning();
                }
                else
                {
                    DrawBoxesAfterAnalyzeRequest();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            FrameHolderMouseMove = true;
            RefreshBarGaugeOfBristles();
        }

        private void DrawBoxesAfterAnalyzeRequest()
        {
            int contDefect = 0;

            Error1 = 0;
            Error2 = 0;
            Error3 = 0;
            Discard = 0;
            foreach (var socketModelFromList in SocketModels)
            {
                double WidthR = (socketModelFromList.Width * W) / CameraObject.ResolutionWidth;
                double HeightR = (socketModelFromList.Height * H) / CameraObject.ResolutionHeight;
                double xR = (socketModelFromList.X * W) / CameraObject.ResolutionWidth;
                double yR = (socketModelFromList.Y * H) / CameraObject.ResolutionHeight;

                if (socketModelFromList.Obj_class == "none")
                {
                    System.Windows.Shapes.Rectangle rect = new System.Windows.Shapes.Rectangle
                    {
                        Stroke = System.Windows.Media.Brushes.Green,
                        Opacity = 100,
                        Width = WidthR,
                        Height = HeightR
                    };
                    Canvas.SetLeft(rect, xR);
                    Canvas.SetTop(rect, yR);
                    automaticBristleClassificationPredictionLayer.canvas.Children.Add(rect);
                }                
                else if (socketModelFromList.Obj_class == "discard")
                {
                    System.Windows.Shapes.Rectangle rect = new System.Windows.Shapes.Rectangle
                    {
                        Stroke = System.Windows.Media.Brushes.Yellow,
                        Opacity = 100,
                        Width = WidthR,
                        Height = HeightR
                    };
                    Canvas.SetLeft(rect, xR);
                    Canvas.SetTop(rect, yR);
                    automaticBristleClassificationPredictionLayer.canvas.Children.Add(rect);
                }
                else
                {
                    System.Windows.Shapes.Rectangle rect = new System.Windows.Shapes.Rectangle
                    {
                        Stroke = System.Windows.Media.Brushes.Red,
                        Opacity = 100,
                        Width = WidthR,
                        Height = HeightR
                    };
                    Canvas.SetLeft(rect, xR);
                    Canvas.SetTop(rect, yR);
                    automaticBristleClassificationPredictionLayer.canvas.Children.Add(rect);
                }

                TextBlock textBlock = new TextBlock
                {
                    FontSize = 18
                };

                if (socketModelFromList.Obj_class == "none")
                {
                    textBlock.Foreground = new SolidColorBrush(Colors.DarkGreen);
                    textBlock.Text = "Ok";
                }

                if (socketModelFromList.Obj_class != "none")
                {
                    switch (PositionResultManual)
                    {
                        case "T":
                            switch(socketModelFromList.Obj_class)
                            {
                                case "type1":
                                    Error1++;
                                    contDefect++;
                                    totalNumberBristlesTtype1.Text = Error1.ToString();
                                    break;
                                case "type2":
                                    Error2++;
                                    contDefect++;
                                    totalNumberBristlesTtype2.Text = Error2.ToString();
                                    break;
                                case "type3":
                                case "reb":
                                    Error3++;
                                    contDefect++;
                                    totalNumberBristlesTtype3.Text = Error3.ToString();
                                    break;
                                case "discard":
                                    Discard++;
                                    totalNumberBristlesTdiscard.Text = Discard.ToString();
                                    break;
                            }
                            break;
                        case "M1":
                            switch (socketModelFromList.Obj_class)
                            {
                                case "type1":
                                    Error1++;
                                    contDefect++;
                                    totalNumberBristlesM1type1.Text = Error1.ToString();
                                    break;
                                case "type2":
                                    Error2++;
                                    contDefect++;
                                    totalNumberBristlesM1type2.Text = Error2.ToString();
                                    break;
                                case "type3":
                                case "reb":
                                    Error3++;
                                    contDefect++;
                                    totalNumberBristlesM1type3.Text = Error3.ToString();
                                    break;
                                case "discard":
                                    Discard++;
                                    totalNumberBristlesM1discard.Text = Discard.ToString();
                                    break;
                            }
                            break;
                        case "M2":
                            switch (socketModelFromList.Obj_class)
                            {
                                case "type1":
                                    Error1++;
                                    contDefect++;
                                    totalNumberBristlesM2type1.Text = Error1.ToString();
                                    break;
                                case "type2":
                                    Error2++;
                                    contDefect++;
                                    totalNumberBristlesM2type2.Text = Error2.ToString();
                                    break;
                                case "type3":
                                case "reb":
                                    Error3++;
                                    contDefect++;
                                    totalNumberBristlesM2type3.Text = Error3.ToString();
                                    break;
                                case "discard":
                                    Discard++;
                                    totalNumberBristlesM2discard.Text = Discard.ToString();
                                    break;
                            }
                            break;
                        case "M3":
                            switch (socketModelFromList.Obj_class)
                            {
                                case "type1":
                                    Error1++;
                                    contDefect++;
                                    totalNumberBristlesM3type1.Text = Error1.ToString();
                                    break;
                                case "type2":
                                    Error2++;
                                    contDefect++;
                                    totalNumberBristlesM3type2.Text = Error2.ToString();
                                    break;
                                case "type3":
                                case "reb":
                                    Error3++;
                                    contDefect++;
                                    totalNumberBristlesM3type3.Text = Error3.ToString();
                                    break;
                                case "discard":
                                    Discard++;
                                    totalNumberBristlesM3discard.Text = Discard.ToString();
                                    break;
                            }
                            break;
                        case "N":
                            switch (socketModelFromList.Obj_class)
                            {
                                case "type1":
                                    Error1++;
                                    contDefect++;
                                    totalNumberBristlesNtype1.Text = Error1.ToString();
                                    break;
                                case "type2":
                                    Error2++;
                                    contDefect++;
                                    totalNumberBristlesNtype2.Text = Error2.ToString();
                                    break;
                                case "type3":
                                case "reb":
                                    Error3++;
                                    contDefect++;
                                    totalNumberBristlesNtype3.Text = Error3.ToString();
                                    break;
                                case "discard":
                                    Discard++;
                                    totalNumberBristlesNdiscard.Text = Discard.ToString();
                                    break;
                            }
                            break;
                    }

                    textBlock.Foreground = new SolidColorBrush(Colors.DarkRed);

                    if (socketModelFromList.Obj_class == "discard")
                    {
                        textBlock.Foreground = new SolidColorBrush(Colors.Yellow);
                        textBlock.Text = "Undefined";
                    }

                    if (socketModelFromList.Obj_class != "discard")
                    {
                        textBlock.Text = DataHandlerUseCases.ConvertDefaultToIA(socketModelFromList.Obj_class);
                    }
                }

                Canvas.SetLeft(textBlock, xR);
                Canvas.SetTop(textBlock, yR);
                automaticBristleClassificationPredictionLayer.canvas.Children.Add(textBlock);

                SocketModel box = new SocketModel
                {
                    Width = socketModelFromList.Width,
                    Height = socketModelFromList.Height,
                    X = socketModelFromList.X,
                    Y = socketModelFromList.Y
                };
                BoundingBox.Add(box);
                //Para contabilizar a porcentagem adicionada para o canvas 
                //                  
                SocketModel box2 = new SocketModel
                {
                    Width = 0,
                    Height = 0,
                    X = 0,
                    Y = 0
                };
                BoundingBox.Add(box2);

                RefreshRightMenuAmountsTexts(contDefect);

                CountBox = automaticBristleClassificationPredictionLayer.canvas.Children.Count;
                totalBristlesFound.Text = SocketModels.Count.ToString();
                totalDefectiveBristles.Text = contDefect.ToString();

                automaticBristleClassificationPredictionLayer.canvas.UpdateLayout();
                automaticBristleClassificationPredictionLayer.canvas.UpdateDefaultStyle();
                automaticBristleClassificationPredictionLayer.viewSDKImage.UpdateLayout();
            }

            switch (PositionResultManual)
            {
                case "T":
                    if (Error1 == 0) totalNumberBristlesTtype1.Text = "0";
                    if (Error2 == 0) totalNumberBristlesTtype2.Text = "0";
                    if (Error3 == 0) totalNumberBristlesTtype3.Text = "0";
                    if (Discard == 0) totalNumberBristlesTdiscard.Text = "0";
                    if ((SocketModels.Count - contDefect) <= 0) totalNumberBristlesT.Text = "0";
                    if (contDefect == 0) totalNumberBristlesTnok.Text = "0";
                    break;
                case "M1":
                    if (Error1 == 0) totalNumberBristlesM1type1.Text = "0";
                    if (Error2 == 0) totalNumberBristlesM1type2.Text = "0";
                    if (Error3 == 0) totalNumberBristlesM1type3.Text = "0";
                    if (Discard == 0) totalNumberBristlesM1discard.Text = "0";
                    if ((SocketModels.Count - contDefect) <= 0) totalNumberBristlesM1.Text = "0";
                    if (contDefect == 0) totalNumberBristlesM1nok.Text = "0";
                    break;
                case "M2":
                    if (Error1 == 0) totalNumberBristlesM2type1.Text = "0";
                    if (Error2 == 0) totalNumberBristlesM2type2.Text = "0";
                    if (Error3 == 0) totalNumberBristlesM2type3.Text = "0";
                    if (Discard == 0) totalNumberBristlesM2discard.Text = "0";
                    if ((SocketModels.Count - contDefect) <= 0) totalNumberBristlesM2.Text = "0";
                    if (contDefect == 0) totalNumberBristlesM2nok.Text = "0";
                    break;
                case "M3":
                    if (Error1 == 0) totalNumberBristlesM3type1.Text = "0";
                    if (Error2 == 0) totalNumberBristlesM3type2.Text = "0";
                    if (Error3 == 0) totalNumberBristlesM3type3.Text = "0";
                    if (Discard == 0) totalNumberBristlesM3discard.Text = "0";
                    if ((SocketModels.Count - contDefect) <= 0) totalNumberBristlesM3.Text = "0";
                    if (contDefect == 0) totalNumberBristlesM3nok.Text = "0";
                    break;
                case "N":
                    if (Error1 == 0) totalNumberBristlesNtype1.Text = "0";
                    if (Error2 == 0) totalNumberBristlesNtype2.Text = "0";
                    if (Error3 == 0) totalNumberBristlesNtype3.Text = "0";
                    if (Discard == 0) totalNumberBristlesNdiscard.Text = "0";
                    if ((SocketModels.Count - contDefect) <= 0) totalNumberBristlesN.Text = "0";
                    if (contDefect == 0) totalNumberBristlesNnok.Text = "0";
                    break;
            }
            if (SocketModels.Count <= 0)
            {
                totalBristlesFound.Text = "0";
            }
            if (contDefect == 0)
            {
                totalDefectiveBristles.Text = "0";
            }
        }

        private void DrawRawBoxesWhilePositioning()
        {
            int contDefect = 0;

            foreach (var d in SocketModels)
            {
                double WidthR = (d.Width * W) / CameraObject.ResolutionWidth;
                double HeightR = (d.Height * H) / CameraObject.ResolutionHeight;
                double xR = (d.X * W) / CameraObject.ResolutionWidth;
                double yR = (d.Y * H) / CameraObject.ResolutionHeight;

                System.Windows.Shapes.Rectangle rect = new System.Windows.Shapes.Rectangle
                {
                    Stroke = System.Windows.Media.Brushes.Yellow,
                    Opacity = 100,
                    Width = WidthR,
                    Height = HeightR
                };
                Canvas.SetLeft(rect, xR);
                Canvas.SetTop(rect, yR);
                automaticBristleClassificationPredictionLayer.canvas.Children.Add(rect);
            }
            CountBox = automaticBristleClassificationPredictionLayer.canvas.Children.Count;
            totalBristlesFound.Text = SocketModels.Count.ToString();
            totalDefectiveBristles.Text = contDefect.ToString();

            automaticBristleClassificationPredictionLayer.canvas.UpdateLayout();
            automaticBristleClassificationPredictionLayer.canvas.UpdateDefaultStyle();
            automaticBristleClassificationPredictionLayer.viewSDKImage.UpdateLayout();
        }

        /// <summary>
        /// Design user manual bounding boxes
        /// </summary>
        public void WriteBoundingBox()
        {
            if (SelectedCanvasBoundingBox == -1)
            {
                int contDefectManual = 0;

                try
                {
                    FrameHolderMouseMove = false;

                    if (automaticBristleClassificationPredictionLayer.canvas.Children.Count > CountBox)
                    {
                        automaticBristleClassificationPredictionLayer.canvas.Children.RemoveAt(CountBox);
                        FrameHolderMouseMove = true;
                    }

                    X = automaticBristleClassificationPredictionLayer.XCurrent - (size_ / 2);
                    Y = automaticBristleClassificationPredictionLayer.YCurrent - (size_ / 2);

                    if (BoundingBoxSelectType == "undefined")
                    {
                        System.Windows.Shapes.Rectangle rect = new System.Windows.Shapes.Rectangle
                        {
                            Stroke = System.Windows.Media.Brushes.Yellow,
                            Opacity = 100,
                            Width = size_,
                            Height = size_
                        };

                        Canvas.SetLeft(rect, X);
                        Canvas.SetTop(rect, Y);
                        automaticBristleClassificationPredictionLayer.canvas.Children.Add(rect);
                    }
                    
                    if (BoundingBoxSelectType == "Ok")
                    {
                        System.Windows.Shapes.Rectangle rect = new System.Windows.Shapes.Rectangle
                        {
                            Stroke = System.Windows.Media.Brushes.Green,
                            Opacity = 100,
                            Width = size_,
                            Height = size_
                        };
                        SocketModel box = new SocketModel
                        {
                            Width = size_,
                            Height = size_,
                            X = X,
                            Y = Y
                        };
                        Canvas.SetLeft(rect, X);
                        Canvas.SetTop(rect, Y);
                        automaticBristleClassificationPredictionLayer.canvas.Children.Add(rect);

                        //scale conversion
                        box = AutomaticBristleClassificationUseCases.SocketModelScaleConversion(box, W, H, BoundingBoxSelectType);
                        TextBlock textBlock = new TextBlock
                        {
                            FontSize = 18,
                            Foreground = new SolidColorBrush(Colors.DarkGreen),
                            Text = ("Ok")
                        };
                        Canvas.SetLeft(textBlock, X);
                        Canvas.SetTop(textBlock, Y);
                        automaticBristleClassificationPredictionLayer.canvas.Children.Add(textBlock);

                        SocketModels.Add(box);
                        BoundingBox.Add(box);

                        totalBristlesFound.Text = SocketModels.Count.ToString();
                        contDefectManual += Error1 + Error2 + Error3;

                        RefreshRightMenuAmountsTexts(contDefectManual);

                        FrameHolderMouseMove = true;
                    }
                    else
                    {
                        SocketModel box = new SocketModel();
                        if (BoundingBoxSelectType != "discard")
                        {
                            System.Windows.Shapes.Rectangle rect = new System.Windows.Shapes.Rectangle
                            {
                                Stroke = System.Windows.Media.Brushes.Red,
                                Opacity = 100,
                                Width = size_,
                                Height = size_
                            };

                            box.Width = size_;
                            box.Height = size_;
                            box.X = X;
                            box.Y = Y;
                            box.Probability = -1;
                            Canvas.SetLeft(rect, X);
                            Canvas.SetTop(rect, Y);

                            automaticBristleClassificationPredictionLayer.canvas.Children.Add(rect);
                        }
                        
                        if(BoundingBoxSelectType == "discard")
                        {
                            System.Windows.Shapes.Rectangle rect = new System.Windows.Shapes.Rectangle
                            {
                                Stroke = System.Windows.Media.Brushes.Yellow,
                                Opacity = 100,
                                Width = size_,
                                Height = size_
                            };
                            box.Width = size_;
                            box.Height = size_;
                            box.X = X;
                            box.Y = Y;
                            box.Probability = -1;
                            Canvas.SetLeft(rect, X);
                            Canvas.SetTop(rect, Y);

                            automaticBristleClassificationPredictionLayer.canvas.Children.Add(rect);
                        }

                        switch (PositionResultManual)
                        {
                            case "T":
                                switch (BoundingBoxSelectType)
                                {
                                    case "Error1":
                                        Error1++;
                                        totalNumberBristlesTtype1.Text = Error1.ToString();
                                        break;

                                    case "Error2":
                                        Error2++;
                                        totalNumberBristlesTtype2.Text = Error2.ToString();
                                        break;

                                    case "Error3":
                                    case "reb":
                                        Error3++;
                                        totalNumberBristlesTtype3.Text = Error3.ToString();
                                        break;

                                    case "discard":
                                        Discard++;
                                        totalNumberBristlesTdiscard.Text = Discard.ToString();
                                        break;
                                }
                                break;
                            case "M1":
                                switch (BoundingBoxSelectType)
                                {
                                    case "Error1":
                                        Error1++;
                                        totalNumberBristlesM1type1.Text = Error1.ToString();
                                        break;

                                    case "Error2":
                                        Error2++;
                                        totalNumberBristlesM1type2.Text = Error2.ToString();
                                        break;

                                    case "Error3":
                                    case "reb":
                                        Error3++;
                                        totalNumberBristlesM1type3.Text = Error3.ToString();
                                        break;

                                    case "discard":
                                        Discard++;
                                        totalNumberBristlesM1discard.Text = Discard.ToString();
                                        break;
                                }
                                break;
                            case "M2":
                                switch (BoundingBoxSelectType)
                                {
                                    case "Error1":
                                        Error1++;
                                        totalNumberBristlesM2type1.Text = Error1.ToString();
                                        break;

                                    case "Error2":
                                        Error2++;
                                        totalNumberBristlesM2type2.Text = Error2.ToString();
                                        break;

                                    case "Error3":
                                    case "reb":
                                        Error3++;
                                        totalNumberBristlesM2type3.Text = Error3.ToString();
                                        break;

                                    case "discard":
                                        Discard++;
                                        totalNumberBristlesM2discard.Text = Discard.ToString();
                                        break;
                                }
                                break;
                            case "M3":
                                switch (BoundingBoxSelectType)
                                {
                                    case "Error1":
                                        Error1++;
                                        totalNumberBristlesM3type1.Text = Error1.ToString();
                                        break;

                                    case "Error2":
                                        Error2++;
                                        totalNumberBristlesM3type2.Text = Error2.ToString();
                                        break;

                                    case "Error3":
                                    case "reb":
                                        Error3++;
                                        totalNumberBristlesM3type3.Text = Error3.ToString();
                                        break;

                                    case "discard":
                                        Discard++;
                                        totalNumberBristlesM3discard.Text = Discard.ToString();
                                        break;
                                }
                                break;
                            case "N":
                                switch (BoundingBoxSelectType)
                                {
                                    case "Error1":
                                        Error1++;
                                        totalNumberBristlesNtype1.Text = Error1.ToString();
                                        break;

                                    case "Error2":
                                        Error2++;
                                        totalNumberBristlesNtype2.Text = Error2.ToString();
                                        break;

                                    case "Error3":
                                    case "reb":
                                        Error3++;
                                        totalNumberBristlesNtype3.Text = Error3.ToString();
                                        break;

                                    case "discard":
                                        Discard++;
                                        totalNumberBristlesNdiscard.Text = Discard.ToString();
                                        break;
                                }
                                break;
                        }

                        contDefectManual += Error1 + Error2 + Error3;

                        switch (PositionResultManual)
                        {
                            case "T":
                                totalNumberBristlesTnok.Text = contDefectManual.ToString();
                                break;
                            case "M1":
                                totalNumberBristlesM1nok.Text = contDefectManual.ToString();
                                break;
                            case "M2":
                                totalNumberBristlesM2nok.Text = contDefectManual.ToString();
                                break;
                            case "M3":
                                totalNumberBristlesM3nok.Text = contDefectManual.ToString();
                                break;
                            case "N":
                                totalNumberBristlesNnok.Text = contDefectManual.ToString();
                                break;
                        }

                        //scale conversion
                        box = AutomaticBristleClassificationUseCases.SocketModelScaleConversion(box, W, H, BoundingBoxSelectType);

                        if (BoundingBoxSelectType != "discard")
                        {
                            TextBlock textBlock = new TextBlock
                            {
                                FontSize = 18,
                                Foreground = new SolidColorBrush(Colors.DarkRed),
                                Text = BoundingBoxSelectType
                            };
                            Canvas.SetLeft(textBlock, X);
                            Canvas.SetTop(textBlock, Y);
                            automaticBristleClassificationPredictionLayer.canvas.Children.Add(textBlock);
                        }
                        
                        if(BoundingBoxSelectType == "discard")
                        {
                            TextBlock textBlock = new TextBlock
                            {
                                FontSize = 18,
                                Foreground = new SolidColorBrush(Colors.Yellow),
                                Text = "Undefined"
                            };
                            Canvas.SetLeft(textBlock, X);
                            Canvas.SetTop(textBlock, Y);
                            automaticBristleClassificationPredictionLayer.canvas.Children.Add(textBlock);
                        }

                        SocketModels.Add(box);
                        BoundingBox.Add(box);

                        totalBristlesFound.Text = SocketModels.Count.ToString();
                        totalDefectiveBristles.Text = (int.Parse(totalDefectiveBristles.Text) + 1).ToString();

                        FrameHolderMouseMove = true;
                    }

                    BoundingBoxSelectType = "undefined";
                }
                catch(Exception e)
                {
                    Log.CustomLog.LogMessage("An error occured while Writting Bouding Boxes: " + e.Message);
                }
            }

            FrameHolderMouseMove = true;
            RefreshBarGaugeOfBristles();
        }

        private void RefreshRightMenuAmountsTexts(int contDefectManual)
        {
            switch (PositionResultManual)
            {
                case "T":
                    totalNumberBristlesT.Text = "0";

                    if ((int.Parse(totalBristlesFound.Text) - contDefectManual) > 0)
                    {
                        totalNumberBristlesT.Text = (int.Parse(totalBristlesFound.Text) - contDefectManual - Discard).ToString();
                    }

                    totalNumberBristlesTnok.Text = contDefectManual.ToString();
                    break;
                case "M1":
                    totalNumberBristlesM1.Text = "0";

                    if ((int.Parse(totalBristlesFound.Text) - contDefectManual) > 0)
                    {
                        totalNumberBristlesM1.Text = (int.Parse(totalBristlesFound.Text) - contDefectManual - Discard).ToString();
                    }

                    totalNumberBristlesM1nok.Text = contDefectManual.ToString();
                    break;
                case "M2":
                    totalNumberBristlesM2.Text = "0";

                    if ((int.Parse(totalBristlesFound.Text) - contDefectManual) > 0)
                    {
                        totalNumberBristlesM2.Text = (int.Parse(totalBristlesFound.Text) - contDefectManual - Discard).ToString();
                    }

                    totalNumberBristlesM2nok.Text = contDefectManual.ToString();
                    break;
                case "M3":
                    totalNumberBristlesM3.Text = "0";

                    if ((int.Parse(totalBristlesFound.Text) - contDefectManual) > 0)
                    {
                        totalNumberBristlesM3.Text = (int.Parse(totalBristlesFound.Text) - contDefectManual - Discard).ToString();
                    }

                    totalNumberBristlesM3nok.Text = contDefectManual.ToString();
                    break;
                case "N":
                    totalNumberBristlesN.Text = "0";

                    if ((int.Parse(totalBristlesFound.Text) - contDefectManual) > 0)
                    {
                        totalNumberBristlesN.Text = (int.Parse(totalBristlesFound.Text) - contDefectManual - Discard).ToString();
                    }

                    totalNumberBristlesNnok.Text = contDefectManual.ToString();
                    break;
            }
        }

        private void Cancelar_Click(object sender, RoutedEventArgs e)
        {
            WithdrawalOfBoundingBoxMarking();
        }

        private void WithdrawalOfBoundingBoxMarking()
        {
            try
            {
                if (SelectedCanvasBoundingBox == -1 && automaticBristleClassificationPredictionLayer.canvas.Children.Count > CountBox)
                {
                    FrameHolderMouseMove = false;
                    automaticBristleClassificationPredictionLayer.canvas.Children.RemoveAt(CountBox);
                    BoundingBox.RemoveAt(automaticBristleClassificationPredictionLayer.canvas.Children.Count - 1);
                }

                FrameHolderMouseMove = true;
            }
            catch(Exception e)
            {
                Log.CustomLog.LogMessage("An error occured in WithdrawalOfBoundingBoxMarking method: " + e.Message);
            }
        }

        private void Size1_Click(object sender, RoutedEventArgs e)
        {
            size_ = 85;
        }

        private void Size2_Click(object sender, RoutedEventArgs e)
        {
            size_ = 80;
        }

        private void Size3_Click(object sender, RoutedEventArgs e)
        {
            size_ = 75;
        }

        private void Size4_Click(object sender, RoutedEventArgs e)
        {
            size_ = 70;
        }

        private void Size5_Click(object sender, RoutedEventArgs e)
        {
            size_ = 65;
        }

        /// <summary>
        /// Save the analysis results to the database.
        /// </summary>
        private void SaveResultDatabase()
        {
            var bristleAnalysisResultModels = new List<BristleAnalysisResultModel>();
            businessSystem.TuffAnalysisResultModel.TotalBristlesFoundNN = 0;
            businessSystem.TuffAnalysisResultModel.TotalBristleFoundManual = 0;

            for (int i = 0; i < SocketModels.Count; i++)
            {
                businessSystem.BristleAnalysisResultModel = new BristleAnalysisResultModel();
                if (SocketModels[i].Obj_class == "discard")
                {
                    businessSystem.BristleAnalysisResultModel.DefectClassification = "discard";
                }
                else if (SocketModels[i].Obj_class == "none")
                {
                    businessSystem.BristleAnalysisResultModel.DefectClassification = "Ok";
                }
                else
                {
                    //converter names
                    if (SocketModels[i].Obj_class == "type1")
                    {
                        businessSystem.BristleAnalysisResultModel.DefectClassification = "Error1";
                    }
                    else if (SocketModels[i].Obj_class == "type2")
                    {
                        businessSystem.BristleAnalysisResultModel.DefectClassification = "Error2";
                    }
                    else if (SocketModels[i].Obj_class == "type3" || SocketModels[i].Obj_class == "reb")
                    {
                        businessSystem.BristleAnalysisResultModel.DefectClassification = "Error3";
                    }
                }
                if (businessSystem.BristleAnalysisResultModel.DefectClassification != "Ok")
                {
                    businessSystem.BristleAnalysisResultModel.DefectIdentified = "yes";
                }
                else
                {
                    businessSystem.BristleAnalysisResultModel.DefectIdentified = "No";
                    businessSystem.BrushAnalysisResultModel.TotalGoodBristles++;
                }
                businessSystem.BrushAnalysisResultModel.TotalBristles++;

                businessSystem.BristleAnalysisResultModel.X = SocketModels[i].X;
                businessSystem.BristleAnalysisResultModel.Y = SocketModels[i].Y;
                businessSystem.BristleAnalysisResultModel.Width = SocketModels[i].Height;
                businessSystem.BristleAnalysisResultModel.Height = SocketModels[i].Width;

                if (SocketModels[i].Probability == -1)
                {
                    businessSystem.BristleAnalysisResultModel.SelectedManual = true;
                    businessSystem.TuffAnalysisResultModel.SelectedManual = true;
                    businessSystem.TuffAnalysisResultModel.TotalBristleFoundManual++;
                }
                else
                {
                    businessSystem.BristleAnalysisResultModel.SelectedManual = false;
                    businessSystem.TuffAnalysisResultModel.TotalBristlesFoundNN++;
                }
                businessSystem.BristleAnalysisResultModel.Position = Position;
                bristleAnalysisResultModels.Add(businessSystem.BristleAnalysisResultModel);
            }
            businessSystem.TuffAnalysisResultModel.Position = Position;
            businessSystem.TuffAnalysisResultModel.Probability = "95";
            _tuffAnalysisResultSetController.UpdateTuffAnalysisResultModel(businessSystem.TuffAnalysisResultModel);
            _bristleAnalysisResultSetController.UpdateBristleAnalysisResultModel(bristleAnalysisResultModels);
        }

        /// <summary>
        /// Configures the system for the next tuft
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NextAnalysis_Click(object sender, RoutedEventArgs e)
        {
            if (LiveOn_)
            {
                if ((int.Parse(totalNumberBristlesTdiscard.Text) + int.Parse(totalNumberBristlesM1discard.Text) +
                int.Parse(totalNumberBristlesM2discard.Text) + int.Parse(totalNumberBristlesM3discard.Text) +
                int.Parse(totalNumberBristlesNdiscard.Text)) != 0)
                {
                    automaticBristleClassificationPredictionLayer.warningText2.Visibility = Visibility.Collapsed;
                    automaticBristleClassificationPredictionLayer.warningButton2.Visibility = Visibility.Collapsed;
                    automaticBristleClassificationPredictionLayer.warningText3.Visibility = Visibility.Visible;
                    automaticBristleClassificationPredictionLayer.warningButton3.Visibility = Visibility.Visible;
                    automaticBristleClassificationPredictionLayer.modalL.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 218, 140, 16));
                    automaticBristleClassificationPredictionLayer.modalR.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 218, 140, 16));
                    automaticBristleClassificationPredictionLayer.warning.Visibility = Visibility.Visible;
                }
                else
                {
                    automaticBristleClassificationPredictionLayer.warningText3.Visibility = Visibility.Collapsed;
                    automaticBristleClassificationPredictionLayer.warningButton3.Visibility = Visibility.Collapsed;
                    automaticBristleClassificationPredictionLayer.warningText2.Visibility = Visibility.Visible;
                    automaticBristleClassificationPredictionLayer.warningButton2.Visibility = Visibility.Visible;
                    automaticBristleClassificationPredictionLayer.modalL.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 188, 64, 49));
                    automaticBristleClassificationPredictionLayer.modalR.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 188, 64, 49));

                    GridMenuRight.Visibility = Visibility.Collapsed;
                    ImageCount = 0;

                    SelectAnalysis++;

                    switch (SelectAnalysis)
                    {
                        case 1:
                            RewriteNBristlesSpecification(GeneralSettings.TuftM1BristleCountSpecTest);
                            automaticBristleClassificationPredictionLayer.warning.Visibility = Visibility.Visible;
                            businessSystem.TuffAnalysisResultModel.SelectedManual = false;
                            result_view.Visibility = Visibility.Collapsed;
                            startAnalyse2_view.Visibility = Visibility.Visible;
                            CaptureImage_ = false;
                            LiveConditions(true);
                            cameraStartAnalysis.Kind = MaterialDesignThemes.Wpf.PackIconKind.Camera;
                            Position = "T";
                            PositionResultManual = "M1";
                            SaveResultDatabase();
                            SaveResult("T");
                            SocketModels.Clear();
                            automaticBristleClassificationPredictionLayer.canvas.Children.Clear();
                            BoundingBox.Clear();
                            totalBristlesFound.Text = "0";
                            totalDefectiveBristles.Text = "0";

                            arrowPositionM1.Visibility = Visibility.Visible;
                            arrowPositionM2.Visibility = Visibility.Collapsed;
                            arrowPositionM3.Visibility = Visibility.Collapsed;
                            arrowPositionN.Visibility = Visibility.Collapsed;
                            startAnalyse2_view.UpdateLayout();

                            break;
                        case 2:
                            RewriteNBristlesSpecification(GeneralSettings.TuftM2BristleCountSpecTest);
                            automaticBristleClassificationPredictionLayer.warning.Visibility = Visibility.Visible;
                            businessSystem.TuffAnalysisResultModel.SelectedManual = false;
                            automaticBristleClassificationPredictionLayer.canvas.Children.Clear();
                            result_view.Visibility = Visibility.Collapsed;
                            CaptureImage_ = false;
                            LiveConditions(true);
                            Position = "M1";
                            PositionResultManual = "M2";
                            SaveResultDatabase();
                            SaveResult("M1");
                            SocketModels.Clear();
                            automaticBristleClassificationPredictionLayer.canvas.Children.Clear();
                            BoundingBox.Clear();
                            totalBristlesFound.Text = "0";
                            totalDefectiveBristles.Text = "0";

                            arrowPositionM1.Visibility = Visibility.Collapsed;
                            arrowPositionM2.Visibility = Visibility.Visible;
                            arrowPositionM3.Visibility = Visibility.Collapsed;
                            arrowPositionN.Visibility = Visibility.Collapsed;
                            startAnalyse2_view.UpdateLayout();

                            break;
                        case 3:
                            RewriteNBristlesSpecification(GeneralSettings.TuftM3BristleCountSpecTest);
                            automaticBristleClassificationPredictionLayer.warning.Visibility = Visibility.Visible;
                            businessSystem.TuffAnalysisResultModel.SelectedManual = false;
                            automaticBristleClassificationPredictionLayer.canvas.Children.Clear();
                            result_view.Visibility = Visibility.Collapsed;
                            CaptureImage_ = false;
                            LiveConditions(true);
                            cameraStartAnalysis.Kind = MaterialDesignThemes.Wpf.PackIconKind.Camera;
                            Position = "M2";
                            PositionResultManual = "M3";
                            SaveResultDatabase();
                            SaveResult("M2");
                            SocketModels.Clear();
                            automaticBristleClassificationPredictionLayer.canvas.Children.Clear();
                            BoundingBox.Clear();
                            totalBristlesFound.Text = "0";
                            totalDefectiveBristles.Text = "0";

                            arrowPositionM1.Visibility = Visibility.Collapsed;
                            arrowPositionM2.Visibility = Visibility.Collapsed;
                            arrowPositionM3.Visibility = Visibility.Visible;
                            arrowPositionN.Visibility = Visibility.Collapsed;
                            startAnalyse2_view.UpdateLayout();

                            break;
                        case 4:
                            RewriteNBristlesSpecification(GeneralSettings.TuftNBristleCountSpecTest);
                            automaticBristleClassificationPredictionLayer.warning.Visibility = Visibility.Visible;
                            businessSystem.TuffAnalysisResultModel.SelectedManual = false;
                            automaticBristleClassificationPredictionLayer.canvas.Children.Clear();
                            result_view.Visibility = Visibility.Collapsed;
                            CaptureImage_ = false;
                            LiveConditions(true);
                            cameraStartAnalysis.Kind = MaterialDesignThemes.Wpf.PackIconKind.Camera;
                            Position = "M3";
                            PositionResultManual = "N";
                            SaveResultDatabase();
                            SaveResult("M3");
                            SocketModels.Clear();
                            automaticBristleClassificationPredictionLayer.canvas.Children.Clear();
                            BoundingBox.Clear();
                            totalBristlesFound.Text = "0";
                            totalDefectiveBristles.Text = "0";

                            arrowPositionM1.Visibility = Visibility.Collapsed;
                            arrowPositionM2.Visibility = Visibility.Collapsed;
                            arrowPositionM3.Visibility = Visibility.Collapsed;
                            arrowPositionN.Visibility = Visibility.Visible;
                            startAnalyse2_view.UpdateLayout();

                            break;
                        case 5:
                            divTestMetrics.Visibility = Visibility.Collapsed;
                            automaticBristleClassificationPredictionLayer.warning.Visibility = Visibility.Visible;
                            businessSystem.TuffAnalysisResultModel.SelectedManual = false;
                            automaticBristleClassificationPredictionLayer.canvas.Children.Clear();
                            next.Content = "Result";
                            SelectAnalysis = 0;
                            CaptureImage_ = false;
                            LiveConditions(true);
                            cameraStartAnalysis.Kind = MaterialDesignThemes.Wpf.PackIconKind.Camera;
                            Position = "N";
                            SaveResultDatabase();
                            SaveResult("N");
                            SocketModels.Clear();
                            automaticBristleClassificationPredictionLayer.canvas.Children.Clear();
                            BoundingBox.Clear();
                            totalBristlesFound.Text = "0";
                            totalDefectiveBristles.Text = "0";

                            arrowPositionM1.Visibility = Visibility.Visible;
                            arrowPositionM2.Visibility = Visibility.Collapsed;
                            arrowPositionM3.Visibility = Visibility.Collapsed;
                            arrowPositionN.Visibility = Visibility.Collapsed;
                            startAnalyse2_view.UpdateLayout();

                            break;
                    }
                }
                automaticBristleClassificationPredictionLayer.photo.Visibility = Visibility.Collapsed;
            }
            else
            {
                MessageBox.Show("Active live to proceed!");
            }
        }

        /// <summary>
        /// Generates the results of everything checked
        /// </summary>
        /// <param name="position">tuff checked</param>
        private void SaveResult(string position)
        {
            if (position == "N")
            {
                if (int.Parse(totalNumberBristlesT.Text) != 0 || int.Parse(totalNumberBristlesM1.Text) != 0 || int.Parse(totalNumberBristlesM2.Text) != 0
               || int.Parse(totalNumberBristlesM3.Text) != 0 || int.Parse(totalNumberBristlesN.Text) != 0)
                {
                    businessSystem.BrushAnalysisResultModel.Hybrid = true;
                }
                else
                {
                    businessSystem.BrushAnalysisResultModel.Hybrid = false;
                }

                int total = (int.Parse(totalNumberBristlesT.Text) +
                    int.Parse(totalNumberBristlesM1.Text) +
                    int.Parse(totalNumberBristlesM2.Text) +
                    int.Parse(totalNumberBristlesM3.Text) +
                    int.Parse(totalNumberBristlesN.Text) +
                    int.Parse(totalNumberBristlesTnok.Text) +
                    int.Parse(totalNumberBristlesM1nok.Text) +
                    int.Parse(totalNumberBristlesM2nok.Text) +
                    int.Parse(totalNumberBristlesM3nok.Text) +
                    int.Parse(totalNumberBristlesNnok.Text));

                if (total == 0)
                {
                    total = 1;
                }

                int result = (businessSystem.BrushAnalysisResultModel.TotalGoodBristles * 100) / total;

                if (result >= GeneralSettings.EndroundSpecTest.TestTarget)
                {
                    businessSystem.BrushAnalysisResultModel.AnalysisResult = "APPROVED";
                    businessSystem.BrushAnalysisResultModel.Signaling_Id = 1;
                }
                else if (result >= GeneralSettings.EndroundSpecTest.TestSpecLowerLimit && result < GeneralSettings.EndroundSpecTest.TestTarget)
                {
                    businessSystem.BrushAnalysisResultModel.AnalysisResult = "ALERT";
                    businessSystem.BrushAnalysisResultModel.Signaling_Id = 2;
                }
                else
                {
                    businessSystem.BrushAnalysisResultModel.AnalysisResult = "NOT APPROVED";
                    businessSystem.BrushAnalysisResultModel.Signaling_Id = 3;
                }

                _brushAnalysisResultSetController.UpdateBrushAnalysisResultModel(businessSystem.BrushAnalysisResultModel);
            }
        }

        private void SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var listSkus = _sKUController.ListSKUsModel();
            var chosenSKU = listSkus
                            .FirstOrDefault(sku => sku.SSKU == skuSelect.SelectedItem.ToString());

            testSelect.Items.Clear();
            testSelect.Items.Add(GeneralSettings.EndroundTestName);

            var testModels = _testController.ListTestModelBySKUId(chosenSKU == null ? -1 : chosenSKU.IID);

            foreach (var item in testModels)
            {
                if (item.SDescription == GeneralSettings.EndroundTestName)
                {
                    GeneralSettings.EndroundSpecTest = _qM_SpecController
                                                                                    .GetQM_SpecByTestId(item.Id);
                }
                else if (item.SDescription == GeneralSettings.TuftTBristleCountTestName)
                {
                    GeneralSettings.TuftTBristleCountSpecTest = _qM_SpecController
                                                                                    .GetQM_SpecByTestId(item.Id);
                }
                else if (item.SDescription == GeneralSettings.TuftM1BristleCountTestName)
                {
                    GeneralSettings.TuftM1BristleCountSpecTest = _qM_SpecController
                                                                                    .GetQM_SpecByTestId(item.Id);
                }
                else if (item.SDescription == GeneralSettings.TuftM2BristleCountTestName)
                {
                    GeneralSettings.TuftM2BristleCountSpecTest = _qM_SpecController
                                                                                    .GetQM_SpecByTestId(item.Id);
                }
                else if (item.SDescription == GeneralSettings.TuftM3BristleCountTestName)
                {
                    GeneralSettings.TuftM3BristleCountSpecTest = _qM_SpecController
                                                                                  .GetQM_SpecByTestId(item.Id);
                }
                else if (item.SDescription == GeneralSettings.TuftNBristleCountTestName)
                {
                    GeneralSettings.TuftNBristleCountSpecTest = _qM_SpecController
                                                                                .GetQM_SpecByTestId(item.Id);
                }
            }

            testSelect.SelectedIndex = 0;

            areaSelect.Items.Clear();
            areaSelect.Items.Add("-");

            if (chosenSKU != null)
                areaSelect.Items.Add(chosenSKU.IArea_id);

            if (areaSelect.Items.Count > 1)
            {
                areaSelect.SelectedIndex = 2;
            }

            equipmentSelect.Items.Clear();
            equipmentSelect.Items.Add("-");

            if (chosenSKU != null)
            {
                foreach (var item in _equipmentController.ListEquipmentModel(chosenSKU.IArea_id))
                {
                    equipmentSelect.Items.Add(item.IEquipment_id);
                }

                if (chosenSKU.IID > 0)
                {
                    divTestMetrics.Visibility = Visibility.Visible;

                    CameraObject.NominalBristle = (int)GeneralSettings.TuftM1BristleCountSpecTest.TestTarget;

                    RewriteNBristlesSpecification(GeneralSettings.TuftTBristleCountSpecTest);
                }
            }
        }

        public void ChangeClassInPlace(int selectedSocketModel)
        {
            if (selectedSocketModel > -1)
            {
                try
                {
                    SocketModels[selectedSocketModel].Obj_class = DataHandlerUseCases.ConvertIAToDefault(BoundingBoxSelectType);
                    SocketModels[selectedSocketModel].Probability = -1;
                    BoundingBoxSelect = 0;
                    SelectedCanvasBoundingBox = -1;
                    BoundingBoxSelectType = "undefined";
                }
                catch(Exception e)
                {
                    Log.CustomLog.LogMessage("An error occured in ChangeClassInPlace method: " + e.Message);
                }
            }
        }

        private void AnalysisFinish_Click(object sender, RoutedEventArgs e)
        {
            if ((int.Parse(totalNumberBristlesTdiscard.Text) + int.Parse(totalNumberBristlesM1discard.Text) +
              int.Parse(totalNumberBristlesM2discard.Text) + int.Parse(totalNumberBristlesM3discard.Text) +
              int.Parse(totalNumberBristlesNdiscard.Text)) != 0)
            {
                automaticBristleClassificationPredictionLayer.warningText2.Visibility = Visibility.Collapsed;
                automaticBristleClassificationPredictionLayer.warningButton2.Visibility = Visibility.Collapsed;
                automaticBristleClassificationPredictionLayer.warningText3.Visibility = Visibility.Visible;
                automaticBristleClassificationPredictionLayer.warningButton3.Visibility = Visibility.Visible;
                automaticBristleClassificationPredictionLayer.modalL.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 218, 140, 16));
                automaticBristleClassificationPredictionLayer.modalR.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 218, 140, 16));
                automaticBristleClassificationPredictionLayer.warning.Visibility = Visibility.Visible;
            }
            else
            {
                automaticBristleClassificationPredictionLayer.warningText3.Visibility = Visibility.Collapsed;
                automaticBristleClassificationPredictionLayer.warningButton3.Visibility = Visibility.Collapsed;
                automaticBristleClassificationPredictionLayer.warningText2.Visibility = Visibility.Visible;
                automaticBristleClassificationPredictionLayer.warningButton2.Visibility = Visibility.Visible;
                automaticBristleClassificationPredictionLayer.modalL.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 188, 64, 49));
                automaticBristleClassificationPredictionLayer.modalR.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 188, 64, 49));
            }
        }

        private void Password_Click(object sender, RoutedEventArgs e)
        {
            if (ScreenNavigationUseCases.OpenPasswordScreen(businessSystem.UserSystemCurrent, businessSystem.NetworkUserModel, GeneralSettings, businessSystem, ColgateSkeltaEntities, Maximized))
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
            if (ScreenNavigationUseCases.OpenUserScreen(businessSystem.UserSystemCurrent, businessSystem.NetworkUserModel, GeneralSettings, businessSystem, ColgateSkeltaEntities, Maximized))
            {
                this.Close();
            }
            else
            {
                MessageBox.Show("Necessary administrative rights!");
            }
        }

        private void ButtonCameraConfiguration1_Click(object sender, RoutedEventArgs e)
        {
            CameraCalibrationClose();
            cameraConfiguration1.Visibility = Visibility.Visible;
        }

        private void ButtonCameraConfiguration2_Click(object sender, RoutedEventArgs e)
        {
            CameraCalibrationClose();
            cameraConfiguration2.Visibility = Visibility.Visible;
        }

        private void ButtonCameraConfiguration3_Click(object sender, RoutedEventArgs e)
        {
            CameraCalibrationClose();
            cameraConfiguration3.Visibility = Visibility.Visible;
        }

        private void ButtonCameraConfiguration4_Click(object sender, RoutedEventArgs e)
        {
            CameraCalibrationClose();
            cameraConfiguration4.Visibility = Visibility.Visible;
        }

        private void ButtonCameraConfiguration5_Click(object sender, RoutedEventArgs e)
        {
            CameraCalibrationClose();

            if (!LiveOn_) viewSDK.Visibility = Visibility.Collapsed;
            automaticBristleClassificationPredictionLayer.cameraConfigure_.Visibility = Visibility.Visible;
        }
        private void ButtonBack_Click(object sender, RoutedEventArgs e)
        {
            CameraCalibrationClose();
        }

        private void CameraCalibrationClose()
        {
            cameraConfiguration1.Visibility = Visibility.Collapsed;
            cameraConfiguration2.Visibility = Visibility.Collapsed;
            cameraConfiguration3.Visibility = Visibility.Collapsed;
            cameraConfiguration4.Visibility = Visibility.Collapsed;
            cameraConfiguration5.Visibility = Visibility.Collapsed;
        }

        private void Grid_MouseDown_LED1(object sender, MouseButtonEventArgs e)
        {
            if (LED1On)
            {
                LED1.Background = System.Windows.Media.Brushes.Red;
            }
            else
            {
                LED1.Background = System.Windows.Media.Brushes.Green;
            }

            EnableLEDs(1);
        }

        /// <summary>
        /// Control LEDs
        /// </summary>
        /// <param name="LED"></param>
        private void EnableLEDs(int LED)
        {
            AutomaticBristleClassification automaticBristleClassification = this;
            lock (automaticBristleClassification)
            {
                switch (LED)
                {
                    case 1:
                        if (!LED1On)
                        {
                            if (LED2On && !LED3On && !LED4On)
                            {
                                CameraObject.DinoLiteSDK.SetFLCSwitch(CameraObject.VideoDeviceIndex, 3);
                                Thread.Sleep(280);
                                businessSystem.CameraSettingsModel.LedsOn = 3;
                            }
                            else if (!LED2On && LED3On && !LED4On)
                            {
                                CameraObject.DinoLiteSDK.SetFLCSwitch(CameraObject.VideoDeviceIndex, 5);
                                Thread.Sleep(280);
                                businessSystem.CameraSettingsModel.LedsOn = 5;
                            }
                            else if (!LED2On && !LED3On && LED4On)
                            {
                                CameraObject.DinoLiteSDK.SetFLCSwitch(CameraObject.VideoDeviceIndex, 9);
                                Thread.Sleep(280);
                                businessSystem.CameraSettingsModel.LedsOn = 9;
                            }
                            else if (LED2On && LED3On && !LED4On)
                            {
                                CameraObject.DinoLiteSDK.SetFLCSwitch(CameraObject.VideoDeviceIndex, 7);
                                Thread.Sleep(280);
                                businessSystem.CameraSettingsModel.LedsOn = 7;
                            }
                            else if (LED3On && LED4On && !LED2On)
                            {
                                CameraObject.DinoLiteSDK.SetFLCSwitch(CameraObject.VideoDeviceIndex, 13);
                                Thread.Sleep(280);
                                businessSystem.CameraSettingsModel.LedsOn = 13;
                            }
                            else if (LED2On && LED4On && !LED3On)
                            {
                                CameraObject.DinoLiteSDK.SetFLCSwitch(CameraObject.VideoDeviceIndex, 11);
                                Thread.Sleep(50);
                                businessSystem.CameraSettingsModel.LedsOn = 11;
                            }
                            else if (!LED2On && !LED4On && !LED3On)
                            {
                                CameraObject.DinoLiteSDK.SetFLCSwitch(CameraObject.VideoDeviceIndex, 1);
                                Thread.Sleep(280);
                                businessSystem.CameraSettingsModel.LedsOn = 1;
                            }
                            else if (LED2On && LED4On && LED3On)
                            {
                                CameraObject.DinoLiteSDK.SetFLCSwitch(CameraObject.VideoDeviceIndex, 15);
                                Thread.Sleep(280);
                                businessSystem.CameraSettingsModel.LedsOn = 15;
                            }
                            LED1On = true;
                        }
                        else
                        {
                            if (LED2On && !LED3On && !LED4On)
                            {
                                CameraObject.DinoLiteSDK.SetFLCSwitch(CameraObject.VideoDeviceIndex, 2);
                                Thread.Sleep(280);
                                businessSystem.CameraSettingsModel.LedsOn = 2;
                            }
                            else if (!LED2On && LED3On && !LED4On)
                            {
                                CameraObject.DinoLiteSDK.SetFLCSwitch(CameraObject.VideoDeviceIndex, 4);
                                Thread.Sleep(280);
                                businessSystem.CameraSettingsModel.LedsOn = 4;
                            }
                            else if (!LED2On && !LED3On && LED4On)
                            {
                                CameraObject.DinoLiteSDK.SetFLCSwitch(CameraObject.VideoDeviceIndex, 8);
                                Thread.Sleep(280);
                                businessSystem.CameraSettingsModel.LedsOn = 8;
                            }
                            else if (LED2On && LED3On && !LED4On)
                            {
                                CameraObject.DinoLiteSDK.SetFLCSwitch(CameraObject.VideoDeviceIndex, 6);
                                Thread.Sleep(280);
                                businessSystem.CameraSettingsModel.LedsOn = 6;
                            }
                            else if (LED3On && LED4On && !LED2On)
                            {
                                CameraObject.DinoLiteSDK.SetFLCSwitch(CameraObject.VideoDeviceIndex, 12);
                                Thread.Sleep(280);
                                businessSystem.CameraSettingsModel.LedsOn = 12;
                            }
                            else if (LED2On && LED4On && !LED3On)
                            {
                                CameraObject.DinoLiteSDK.SetFLCSwitch(CameraObject.VideoDeviceIndex, 10);
                                Thread.Sleep(280);
                                businessSystem.CameraSettingsModel.LedsOn = 10;
                            }
                            else if (!LED2On && !LED4On && !LED3On)
                            {
                                CameraObject.DinoLiteSDK.SetFLCSwitch(CameraObject.VideoDeviceIndex, 16);
                                Thread.Sleep(280);
                                businessSystem.CameraSettingsModel.LedsOn = 16;
                            }
                            else if (LED2On && LED4On && LED3On)
                            {
                                CameraObject.DinoLiteSDK.SetFLCSwitch(CameraObject.VideoDeviceIndex, 14);
                                Thread.Sleep(280);
                                businessSystem.CameraSettingsModel.LedsOn = 14;
                            }
                            LED1On = false;
                        }
                        break;
                    case 2:
                        if (!LED2On)
                        {
                            if (LED1On && !LED4On && !LED3On)
                            {
                                CameraObject.DinoLiteSDK.SetFLCSwitch(CameraObject.VideoDeviceIndex, 3);
                                Thread.Sleep(280);
                                businessSystem.CameraSettingsModel.LedsOn = 3;
                            }
                            else if (LED3On && !LED1On && !LED4On)
                            {
                                CameraObject.DinoLiteSDK.SetFLCSwitch(CameraObject.VideoDeviceIndex, 6);
                                Thread.Sleep(280);
                                businessSystem.CameraSettingsModel.LedsOn = 6;
                            }
                            else if (LED4On && !LED1On && !LED3On)
                            {
                                CameraObject.DinoLiteSDK.SetFLCSwitch(CameraObject.VideoDeviceIndex, 10);
                                Thread.Sleep(280);
                                businessSystem.CameraSettingsModel.LedsOn = 10;
                            }
                            else if (LED1On && LED3On && !LED4On)
                            {
                                CameraObject.DinoLiteSDK.SetFLCSwitch(CameraObject.VideoDeviceIndex, 7);
                                Thread.Sleep(280);
                                businessSystem.CameraSettingsModel.LedsOn = 7;
                            }
                            else if (LED3On && LED4On && !LED1On)
                            {
                                CameraObject.DinoLiteSDK.SetFLCSwitch(CameraObject.VideoDeviceIndex, 14);
                                Thread.Sleep(280);
                                businessSystem.CameraSettingsModel.LedsOn = 14;
                            }
                            else if (LED1On && LED4On && !LED3On)
                            {
                                CameraObject.DinoLiteSDK.SetFLCSwitch(CameraObject.VideoDeviceIndex, 11);
                                Thread.Sleep(280);
                                businessSystem.CameraSettingsModel.LedsOn = 11;
                            }
                            else if (!LED1On && !LED4On && !LED3On)
                            {
                                CameraObject.DinoLiteSDK.SetFLCSwitch(CameraObject.VideoDeviceIndex, 2);
                                Thread.Sleep(280);
                                businessSystem.CameraSettingsModel.LedsOn = 2;
                            }
                            else if (LED1On && LED4On && LED3On)
                            {
                                CameraObject.DinoLiteSDK.SetFLCSwitch(CameraObject.VideoDeviceIndex, 15);
                                Thread.Sleep(280);
                                businessSystem.CameraSettingsModel.LedsOn = 15;
                            }
                            LED2On = true;
                        }
                        else
                        {
                            if (LED1On && !LED4On && !LED3On)
                            {
                                CameraObject.DinoLiteSDK.SetFLCSwitch(CameraObject.VideoDeviceIndex, 1);
                                Thread.Sleep(280);
                                businessSystem.CameraSettingsModel.LedsOn = 1;
                            }
                            else if (LED3On && !LED4On && !LED1On)
                            {
                                CameraObject.DinoLiteSDK.SetFLCSwitch(CameraObject.VideoDeviceIndex, 4);
                                Thread.Sleep(280);
                                businessSystem.CameraSettingsModel.LedsOn = 4;
                            }
                            else if (LED4On && !LED3On && !LED1On)
                            {
                                CameraObject.DinoLiteSDK.SetFLCSwitch(CameraObject.VideoDeviceIndex, 8);
                                Thread.Sleep(280);
                                businessSystem.CameraSettingsModel.LedsOn = 8;
                            }
                            else if (LED1On && LED3On && !LED4On)
                            {
                                CameraObject.DinoLiteSDK.SetFLCSwitch(CameraObject.VideoDeviceIndex, 5);
                                Thread.Sleep(280);
                                businessSystem.CameraSettingsModel.LedsOn = 5;
                            }
                            else if (LED3On && LED4On && !LED1On)
                            {
                                CameraObject.DinoLiteSDK.SetFLCSwitch(CameraObject.VideoDeviceIndex, 12);
                                Thread.Sleep(280);
                                businessSystem.CameraSettingsModel.LedsOn = 12;
                            }
                            else if (LED1On && LED4On && !LED3On)
                            {
                                CameraObject.DinoLiteSDK.SetFLCSwitch(CameraObject.VideoDeviceIndex, 9);
                                Thread.Sleep(280);
                                businessSystem.CameraSettingsModel.LedsOn = 9;
                            }
                            else if (!LED1On && !LED4On && !LED3On)
                            {
                                CameraObject.DinoLiteSDK.SetFLCSwitch(CameraObject.VideoDeviceIndex, 16);
                                Thread.Sleep(280);
                                businessSystem.CameraSettingsModel.LedsOn = 16;
                            }
                            else if (LED1On && LED4On && LED3On)
                            {
                                CameraObject.DinoLiteSDK.SetFLCSwitch(CameraObject.VideoDeviceIndex, 13);
                                Thread.Sleep(280);
                                businessSystem.CameraSettingsModel.LedsOn = 13;
                            }
                            LED2On = false;
                        }
                        break;
                    case 3:
                        if (!LED3On)
                        {
                            if (LED2On && !LED4On && !LED1On)
                            {
                                CameraObject.DinoLiteSDK.SetFLCSwitch(CameraObject.VideoDeviceIndex, 6);
                                Thread.Sleep(280);
                                businessSystem.CameraSettingsModel.LedsOn = 6;
                            }
                            else if (LED1On && !LED4On && !LED2On)
                            {
                                CameraObject.DinoLiteSDK.SetFLCSwitch(CameraObject.VideoDeviceIndex, 5);
                                Thread.Sleep(280);
                                businessSystem.CameraSettingsModel.LedsOn = 5;
                            }
                            else if (LED4On && !LED2On && !LED1On)
                            {
                                CameraObject.DinoLiteSDK.SetFLCSwitch(CameraObject.VideoDeviceIndex, 12);
                                Thread.Sleep(280);
                                businessSystem.CameraSettingsModel.LedsOn = 12;
                            }
                            else if (LED2On && LED1On && !LED4On)
                            {
                                CameraObject.DinoLiteSDK.SetFLCSwitch(CameraObject.VideoDeviceIndex, 7);
                                Thread.Sleep(280);
                                businessSystem.CameraSettingsModel.LedsOn = 7;
                            }
                            else if (LED1On && LED4On && !LED2On)
                            {
                                CameraObject.DinoLiteSDK.SetFLCSwitch(CameraObject.VideoDeviceIndex, 13);
                                Thread.Sleep(280);
                                businessSystem.CameraSettingsModel.LedsOn = 13;
                            }
                            else if (LED2On && LED4On && !LED1On)
                            {
                                CameraObject.DinoLiteSDK.SetFLCSwitch(CameraObject.VideoDeviceIndex, 14);
                                Thread.Sleep(280);
                                businessSystem.CameraSettingsModel.LedsOn = 14;
                            }
                            else if (!LED2On && !LED4On && !LED1On)
                            {
                                CameraObject.DinoLiteSDK.SetFLCSwitch(CameraObject.VideoDeviceIndex, 4);
                                Thread.Sleep(280);
                                businessSystem.CameraSettingsModel.LedsOn = 4;
                            }
                            else if (LED2On && LED4On && LED1On)
                            {
                                CameraObject.DinoLiteSDK.SetFLCSwitch(CameraObject.VideoDeviceIndex, 15);
                                Thread.Sleep(280);
                                businessSystem.CameraSettingsModel.LedsOn = 15;
                            }
                            LED3On = true;
                        }
                        else
                        {
                            if (LED2On && !LED4On && !LED1On)
                            {
                                CameraObject.DinoLiteSDK.SetFLCSwitch(CameraObject.VideoDeviceIndex, 2);
                                Thread.Sleep(280);
                                businessSystem.CameraSettingsModel.LedsOn = 2;
                            }
                            else if (LED1On && !LED4On && !LED2On)
                            {
                                CameraObject.DinoLiteSDK.SetFLCSwitch(CameraObject.VideoDeviceIndex, 1);
                                Thread.Sleep(280);
                                businessSystem.CameraSettingsModel.LedsOn = 1;
                            }
                            else if (LED4On && !LED4On && !LED2On)
                            {
                                CameraObject.DinoLiteSDK.SetFLCSwitch(CameraObject.VideoDeviceIndex, 8);
                                Thread.Sleep(280);
                                businessSystem.CameraSettingsModel.LedsOn = 8;
                            }
                            else if (LED2On && LED1On && !LED4On)
                            {
                                CameraObject.DinoLiteSDK.SetFLCSwitch(CameraObject.VideoDeviceIndex, 3);
                                Thread.Sleep(280);
                                businessSystem.CameraSettingsModel.LedsOn = 3;
                            }
                            else if (LED1On && LED4On && !LED2On)
                            {
                                CameraObject.DinoLiteSDK.SetFLCSwitch(CameraObject.VideoDeviceIndex, 9);
                                Thread.Sleep(280);
                                businessSystem.CameraSettingsModel.LedsOn = 9;
                            }
                            else if (LED2On && LED4On && !LED1On)
                            {
                                CameraObject.DinoLiteSDK.SetFLCSwitch(CameraObject.VideoDeviceIndex, 10);
                                Thread.Sleep(280);
                                businessSystem.CameraSettingsModel.LedsOn = 10;
                            }
                            else if (!LED2On && !LED4On && !LED1On)
                            {
                                CameraObject.DinoLiteSDK.SetFLCSwitch(CameraObject.VideoDeviceIndex, 16);
                                Thread.Sleep(280);
                                businessSystem.CameraSettingsModel.LedsOn = 16;
                            }
                            else if (LED2On && LED4On && LED1On)
                            {
                                CameraObject.DinoLiteSDK.SetFLCSwitch(CameraObject.VideoDeviceIndex, 11);
                                Thread.Sleep(280);
                                businessSystem.CameraSettingsModel.LedsOn = 11;
                            }
                            LED3On = false;
                        }
                        break;
                    case 4:
                        if (!LED4On)
                        {
                            if (LED2On && !LED3On && !LED1On)
                            {
                                CameraObject.DinoLiteSDK.SetFLCSwitch(CameraObject.VideoDeviceIndex, 10);
                                Thread.Sleep(280);
                                businessSystem.CameraSettingsModel.LedsOn = 10;
                            }
                            else if (LED3On && !LED2On && !LED1On)
                            {
                                CameraObject.DinoLiteSDK.SetFLCSwitch(CameraObject.VideoDeviceIndex, 6);
                                Thread.Sleep(280);
                                businessSystem.CameraSettingsModel.LedsOn = 6;
                            }
                            else if (LED1On && !LED3On && !LED2On)
                            {
                                CameraObject.DinoLiteSDK.SetFLCSwitch(CameraObject.VideoDeviceIndex, 3);
                                Thread.Sleep(280);
                                businessSystem.CameraSettingsModel.LedsOn = 3;
                            }
                            else if (LED2On && LED3On && !LED1On)
                            {
                                CameraObject.DinoLiteSDK.SetFLCSwitch(CameraObject.VideoDeviceIndex, 14);
                                Thread.Sleep(280);
                                businessSystem.CameraSettingsModel.LedsOn = 14;
                            }
                            else if (LED3On && LED1On && !LED2On)
                            {
                                CameraObject.DinoLiteSDK.SetFLCSwitch(CameraObject.VideoDeviceIndex, 13);
                                Thread.Sleep(280);
                                businessSystem.CameraSettingsModel.LedsOn = 13;
                            }
                            else if (LED2On && LED1On && !LED3On)
                            {
                                CameraObject.DinoLiteSDK.SetFLCSwitch(CameraObject.VideoDeviceIndex, 11);
                                Thread.Sleep(280);
                                businessSystem.CameraSettingsModel.LedsOn = 11;
                            }
                            else if (!LED2On && !LED1On && !LED3On)
                            {
                                CameraObject.DinoLiteSDK.SetFLCSwitch(CameraObject.VideoDeviceIndex, 8);
                                Thread.Sleep(280);
                                businessSystem.CameraSettingsModel.LedsOn = 8;
                            }
                            else if (LED2On && LED1On && LED3On)
                            {
                                CameraObject.DinoLiteSDK.SetFLCSwitch(CameraObject.VideoDeviceIndex, 15);
                                Thread.Sleep(280);
                                businessSystem.CameraSettingsModel.LedsOn = 15;
                            }
                            LED4On = true;
                        }
                        else
                        {
                            if ((LED2On && !LED1On && !LED3On) || (LED2On && LED1On && !LED3On))
                            {
                                CameraObject.DinoLiteSDK.SetFLCSwitch(CameraObject.VideoDeviceIndex, 2);
                                Thread.Sleep(280);
                                businessSystem.CameraSettingsModel.LedsOn = 2;
                            }
                            else if (LED3On && !LED1On && !LED2On)
                            {
                                CameraObject.DinoLiteSDK.SetFLCSwitch(CameraObject.VideoDeviceIndex, 4);
                                Thread.Sleep(280);
                                businessSystem.CameraSettingsModel.LedsOn = 4;
                            }
                            else if (LED1On && !LED2On && !LED3On)
                            {
                                CameraObject.DinoLiteSDK.SetFLCSwitch(CameraObject.VideoDeviceIndex, 1);
                                Thread.Sleep(280);
                                businessSystem.CameraSettingsModel.LedsOn = 1;
                            }
                            else if (LED2On && LED3On && !LED1On)
                            {
                                CameraObject.DinoLiteSDK.SetFLCSwitch(CameraObject.VideoDeviceIndex, 6);
                                Thread.Sleep(280);
                                businessSystem.CameraSettingsModel.LedsOn = 6;
                            }
                            else if (LED3On && LED1On && !LED2On)
                            {
                                CameraObject.DinoLiteSDK.SetFLCSwitch(CameraObject.VideoDeviceIndex, 5);
                                Thread.Sleep(280);
                                businessSystem.CameraSettingsModel.LedsOn = 5;
                            }
                            else if (!LED2On && !LED1On && !LED3On)
                            {
                                CameraObject.DinoLiteSDK.SetFLCSwitch(CameraObject.VideoDeviceIndex, 16);
                                Thread.Sleep(280);
                                businessSystem.CameraSettingsModel.LedsOn = 16;
                            }
                            else if (LED2On && LED1On && LED3On)
                            {
                                CameraObject.DinoLiteSDK.SetFLCSwitch(CameraObject.VideoDeviceIndex, 7);
                                Thread.Sleep(280);
                                businessSystem.CameraSettingsModel.LedsOn = 7;
                            }
                            LED4On = false;
                        }
                        break;
                }
            }
        }

        private void Grid_MouseDown_LED2(object sender, MouseButtonEventArgs e)
        {
            if (LED2On)
            {
                LED2.Background = System.Windows.Media.Brushes.Red;
            }
            else
            {
                LED2.Background = System.Windows.Media.Brushes.Green;
            }

            EnableLEDs(2);
        }

        private void Grid_MouseDown_LED3(object sender, MouseButtonEventArgs e)
        {
            if (LED3On)
            {
                LED3.Background = System.Windows.Media.Brushes.Red;
            }
            else
            {
                LED3.Background = System.Windows.Media.Brushes.Green;
            }

            EnableLEDs(3);
        }

        private void Grid_MouseDown_LED4(object sender, MouseButtonEventArgs e)
        {
            if (LED4On)
            {
                LED4.Background = System.Windows.Media.Brushes.Red;
            }
            else
            {
                LED4.Background = System.Windows.Media.Brushes.Green;
            }

            EnableLEDs(4);
        }

        /// <summary>
        /// LED brightness control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LedBrightness_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            if (!StartNotConfigureCamera)
            {
                int value;
                AutomaticBristleClassification automaticBristleClassification = this;
                lock (automaticBristleClassification)
                {
                    try
                    {
                        CameraObject.DinoLiteSDK.SetFLCLevel(CameraObject.VideoDeviceIndex, (int)ledBrightness.SelectedItem);
                        value = (int)ledBrightness.SelectedItem;
                        Thread.Sleep(280);

                        businessSystem.CameraSettingsModel.LedBrightness = value;
                    }
                    catch(Exception ex)
                    {
                        Log.CustomLog.LogMessage("An error occured in LedBrightness_SelectionChanged_1 method: " + ex.Message);
                    }
                }
            }
        }

        private void Brightness__ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!StartNotConfigureCamera)
            {
                AutomaticBristleClassification automaticBristleClassification = this;
                lock (automaticBristleClassification)
                {
                    CameraObject.DinoLiteSDK.set_VideoProcAmp(0, (int)brightness_.Value);
                    Thread.Sleep(50);
                    businessSystem.CameraSettingsModel.Brightness = (int)brightness_.Value;
                }
            }
        }

        /// <summary>
        /// Contrast camera control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Contrast_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!StartNotConfigureCamera)
            {
                AutomaticBristleClassification automaticBristleClassification = this;
                lock (automaticBristleClassification)
                {
                    CameraObject.DinoLiteSDK.set_VideoProcAmp(1, (int)contrast.Value);
                    Thread.Sleep(50);
                    businessSystem.CameraSettingsModel.Contrast = (int)contrast.Value;
                }
            }
        }

        /// <summary>
        /// HUE camera control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Hue_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!StartNotConfigureCamera)
            {
                AutomaticBristleClassification automaticBristleClassification = this;
                lock (automaticBristleClassification)
                {
                    CameraObject.DinoLiteSDK.set_VideoProcAmp(2, (int)hue.Value);
                    Thread.Sleep(50);
                    businessSystem.CameraSettingsModel.Hue = (int)hue.Value;
                }
            }
        }

        /// <summary>
        /// White Balance Red camera control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WhiteBalanceRed_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!StartNotConfigureCamera)
            {
                AutomaticBristleClassification automaticBristleClassification = this;
                lock (automaticBristleClassification)
                {
                    CameraObject.DinoLiteSDK.FreezeAWB(CameraObject.VideoDeviceIndex, 0);
                    CameraObject.DinoLiteSDK.SetAWBR(CameraObject.VideoDeviceIndex, (int)whiteBalanceRed.Value);
                    CameraObject.DinoLiteSDK.FreezeAWB(CameraObject.VideoDeviceIndex, 1);
                    Thread.Sleep(50);
                    businessSystem.CameraSettingsModel.AWBR = (int)whiteBalanceRed.Value;
                }
            }
        }

        /// <summary>
        /// White Balance Green camera control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WhiteBalanceGreen_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!StartNotConfigureCamera)
            {
                AutomaticBristleClassification automaticBristleClassification = this;
                lock (automaticBristleClassification)
                {
                    CameraObject.DinoLiteSDK.FreezeAWB(CameraObject.VideoDeviceIndex, 0);
                    CameraObject.DinoLiteSDK.SetAWBG(CameraObject.VideoDeviceIndex, (int)whiteBalanceGreen.Value);
                    CameraObject.DinoLiteSDK.FreezeAWB(CameraObject.VideoDeviceIndex, 1);
                    Thread.Sleep(50);
                    businessSystem.CameraSettingsModel.AWBG = (int)whiteBalanceGreen.Value;
                }
            }
        }

        /// <summary>
        /// White Balance Blue camera control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WhiteBalanceBlue_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!StartNotConfigureCamera)
            {
                AutomaticBristleClassification automaticBristleClassification = this;
                lock (automaticBristleClassification)
                {
                    CameraObject.DinoLiteSDK.FreezeAWB(CameraObject.VideoDeviceIndex, 0);
                    CameraObject.DinoLiteSDK.SetAWBB(CameraObject.VideoDeviceIndex, (int)whiteBalanceBlue.Value);
                    CameraObject.DinoLiteSDK.FreezeAWB(CameraObject.VideoDeviceIndex, 1);
                    Thread.Sleep(50);
                    businessSystem.CameraSettingsModel.AWBB = (int)whiteBalanceBlue.Value;
                }
            }
        }

        /// <summary>
        /// Saturation camera control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Saturation_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!StartNotConfigureCamera)
            {
                AutomaticBristleClassification automaticBristleClassification = this;
                lock (automaticBristleClassification)
                {
                    CameraObject.DinoLiteSDK.set_VideoProcAmp(3, (int)saturation.Value);
                    Thread.Sleep(50);
                    businessSystem.CameraSettingsModel.Saturation = (int)saturation.Value;
                }
            }
        }

        /// <summary>
        /// Sharpness camera control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Sharpness_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!StartNotConfigureCamera)
            {
                AutomaticBristleClassification automaticBristleClassification = this;
                lock (automaticBristleClassification)
                {
                    CameraObject.DinoLiteSDK.set_VideoProcAmp(4, (int)sharpness.Value);
                    Thread.Sleep(50);
                    businessSystem.CameraSettingsModel.Sharpness = (int)sharpness.Value;
                }
            }
        }

        /// <summary>
        /// Gamma camera control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Gamma_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!StartNotConfigureCamera)
            {
                AutomaticBristleClassification automaticBristleClassification = this;
                lock (automaticBristleClassification)
                {
                    CameraObject.DinoLiteSDK.set_VideoProcAmp(5, (int)gamma.Value);
                    Thread.Sleep(50);
                    businessSystem.CameraSettingsModel.Gamma = (int)gamma.Value;
                }
            }
        }

        /// <summary>
        ///  Auto Focus camera control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AutoFocus_Click(object sender, RoutedEventArgs e)
        {
            if (!StartNotConfigureCamera)
            {
                AutomaticBristleClassification automaticBristleClassification = this;
                lock (automaticBristleClassification)
                {
                    CameraObject.DinoLiteSDK.AutoFocus(CameraObject.VideoDeviceIndex);
                    Thread.Sleep(50);
                    StartAutoFocus = false;
                }
            }
        }

        /// <summary>
        /// Expoure camera control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckBoxExpoure_Checked(object sender, RoutedEventArgs e)
        {
            CheckBoxStatus();
        }

        private void CheckBoxStatus()
        {
            if (!StartNotConfigureCamera)
            {
                if (checkBoxExpoure.IsChecked == true)
                {
                    try
                    {
                        AutomaticBristleClassification automaticBristleClassification = this;
                        lock (automaticBristleClassification)
                        {
                            checkBoxExpoure.IsEnabled = true;
                            CameraObject.DinoLiteSDK.SetAutoExposure(CameraObject.VideoDeviceIndex, 1);
                            CameraObject.DinoLiteSDK.SetExposureStability(CameraObject.VideoDeviceIndex, 1);
                            Thread.Sleep(50);
                            businessSystem.CameraSettingsModel.AutomaticExposure = 1;
                        }
                    }
                    catch
                    {
                        checkBoxExpoure.IsEnabled = true;
                    }
                }
                else
                {
                    try
                    {
                        AutomaticBristleClassification automaticBristleClassification = this;
                        lock (automaticBristleClassification)
                        {
                            checkBoxExpoure.IsEnabled = false;
                            CameraObject.DinoLiteSDK.SetAutoExposure(CameraObject.VideoDeviceIndex, 0);
                            Thread.Sleep(50);
                            businessSystem.CameraSettingsModel.AutomaticExposure = 0;
                            ApplyExposureValue();
                        }
                    }
                    catch
                    {
                        checkBoxExpoure.IsEnabled = false;
                    }
                }
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (!StartNotConfigureCamera)
            {
                if (mirrorHorizontal.IsChecked == true)
                {
                    AutomaticBristleClassification automaticBristleClassification = this;
                    lock (automaticBristleClassification)
                    {
                        CameraObject.DinoLiteSDK.VideoRotateAngle = 180;
                        Thread.Sleep(280);
                        businessSystem.CameraSettingsModel.Mirror = 180;
                    }
                }
                else if (mirrorHorizontal.IsChecked == true && mirrorVertical.IsChecked == true)
                {
                    AutomaticBristleClassification automaticBristleClassification = this;
                    lock (automaticBristleClassification)
                    {
                        Thread.Sleep(280);//led on need to wait it for reading 
                        businessSystem.CameraSettingsModel.Mirror = 0;
                    }
                }
                else
                {
                    AutomaticBristleClassification automaticBristleClassification = this;
                    lock (automaticBristleClassification)
                    {
                        CameraObject.DinoLiteSDK.VideoRotateAngle = 0;
                        Thread.Sleep(280);
                        businessSystem.CameraSettingsModel.Mirror = 0;
                    }
                }
            }
        }

        /// <summary>
        /// Mirror Vertical camera control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MirrorVertical_Checked(object sender, RoutedEventArgs e)
        {
            if (!StartNotConfigureCamera)
            {
                if (mirrorVertical.IsChecked == true)
                {
                    AutomaticBristleClassification automaticBristleClassification = this;
                    lock (automaticBristleClassification)
                    {
                        Thread.Sleep(280);//led on need to wait it for reading 
                    }
                }
                else
                {
                    AutomaticBristleClassification automaticBristleClassification = this;
                    lock (automaticBristleClassification)
                    {
                        Thread.Sleep(50);
                    }
                }
            }
        }

        private void CheckBox_Checked_1(object sender, RoutedEventArgs e)
        {
            if (!StartNotConfigureCamera)
            {
                if (monochrome.IsChecked == true)
                {
                    AutomaticBristleClassification automaticBristleClassification = this;
                    lock (automaticBristleClassification)
                    {
                        CameraObject.DinoLiteSDK.SetMonochrome(CameraObject.VideoDeviceIndex, 1);
                        Thread.Sleep(50);
                        businessSystem.CameraSettingsModel.Monochrome = 1;
                    }
                }
                else
                {
                    AutomaticBristleClassification automaticBristleClassification = this;
                    lock (automaticBristleClassification)
                    {
                        CameraObject.DinoLiteSDK.SetMonochrome(CameraObject.VideoDeviceIndex, 0);
                        Thread.Sleep(50);
                        businessSystem.CameraSettingsModel.Monochrome = 0;
                    }
                }
            }
        }

        /// <summary>
        /// Negative camera control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Negative_Checked(object sender, RoutedEventArgs e)
        {
            if (!StartNotConfigureCamera)
            {
                if (negative.IsChecked == true)
                {
                    AutomaticBristleClassification automaticBristleClassification = this;
                    lock (automaticBristleClassification)
                    {
                        CameraObject.DinoLiteSDK.SetNegative(CameraObject.VideoDeviceIndex, 1);
                        Thread.Sleep(50);
                        businessSystem.CameraSettingsModel.Negative = 1;
                    }
                }
                else
                {
                    AutomaticBristleClassification automaticBristleClassification = this;
                    lock (automaticBristleClassification)
                    {
                        CameraObject.DinoLiteSDK.SetNegative(CameraObject.VideoDeviceIndex, 0);
                        Thread.Sleep(50);
                        businessSystem.CameraSettingsModel.Negative = 0;
                    }
                }
            }
        }

        private void AutomaticBristleClassification_Closed(object sender, EventArgs e)
        {
            myTimer2.Stop();
            myTimer2.Tick -= new EventHandler(CameraParam);

            myTimer3.Stop();
            myTimer3.Tick -= new EventHandler(CameraParam);

            myTimer.Stop();
            myTimer.Tick -= new EventHandler(IA);
        }

        private void WhiteBalance_Checked(object sender, RoutedEventArgs e)
        {
            if (!StartNotConfigureCamera)
            {
                if (whiteBalance.IsChecked == true)
                {
                    AutomaticBristleClassification automaticBristleClassification = this;
                    lock (automaticBristleClassification)
                    {
                        CameraObject.DinoLiteSDK.set_VideoProcAmp(7, 1);
                        Thread.Sleep(50);
                    }
                }
                else
                {
                    CameraObject.DinoLiteSDK.set_VideoProcAmp(7, 0);
                    Thread.Sleep(50);
                }
            }
        }

        private void WhiteBalance_Unchecked(object sender, RoutedEventArgs e)
        {
            if (!StartNotConfigureCamera)
            {
                if (whiteBalance.IsChecked == true)
                {
                    AutomaticBristleClassification automaticBristleClassification = this;
                    lock (automaticBristleClassification)
                    {
                        CameraObject.DinoLiteSDK.set_VideoProcAmp(7, 1);
                        Thread.Sleep(50);
                    }
                }
                else
                {
                    CameraObject.DinoLiteSDK.set_VideoProcAmp(7, 0);
                    Thread.Sleep(50);
                }
            }
        }

        /// <summary>
        /// Manual Focus camera control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ManualFocusS_Click(object sender, RoutedEventArgs e)
        {
            if (!StartNotConfigureCamera)
            {
                try
                {
                    focus += 1;
                    CameraObject.DinoLiteSDK.SetLensPos(CameraObject.VideoDeviceIndex, focus);
                    Thread.Sleep(25);
                    businessSystem.CameraSettingsModel.Focus = CameraObject.DinoLiteSDK.GetAMRwithLensPos(CameraObject.VideoDeviceIndex);
                    focusValue.Text = Math.Round(businessSystem.CameraSettingsModel.Focus, 2).ToString();
                    Thread.Sleep(25);
                }
                catch(Exception ex)
                {
                    Log.CustomLog.LogMessage("An error occured in ManualFocusS_Click method: " + ex.Message);
                }
            }
        }

        /// <summary>
        /// Manual Focus camera control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ManualFocusA_Click(object sender, RoutedEventArgs e)
        {
            if (!StartNotConfigureCamera)
            {
                try
                {
                    focus -= 1;
                    CameraObject.DinoLiteSDK.SetLensPos(CameraObject.VideoDeviceIndex, focus);
                    Thread.Sleep(25);
                    businessSystem.CameraSettingsModel.Focus = CameraObject.DinoLiteSDK.GetAMRwithLensPos(CameraObject.VideoDeviceIndex);
                    focusValue.Text = Math.Round(businessSystem.CameraSettingsModel.Focus, 2).ToString();
                    Thread.Sleep(25);
                }
                catch(Exception ex)
                {
                    Log.CustomLog.LogMessage("An error occured in ManualFocusA_Click method: " + ex.Message);
                }
            }
        }

        private void ApplyExposureValue()
        {
            if (!StartNotConfigureCamera)
            {
                businessSystem.CameraSettingsModel.Exposure = int.Parse(expureValue.Text);
                Thread.Sleep(50);
            }
        }

        /// <summary>
        /// Exposure time control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ApplyExp_Click(object sender, RoutedEventArgs e)
        {
            ApplyExp_();
        }

        private void ApplyExp_()
        {
            if (!StartNotConfigureCamera)
            {
                businessSystem.CameraSettingsModel.Exposure = double.Parse(expureValue.Text);
                CameraObject.DinoLiteSDK.SetAutoExposure(CameraObject.VideoDeviceIndex, 0);
                CameraObject.DinoLiteSDK.SetExposureValue(CameraObject.VideoDeviceIndex, (int)businessSystem.CameraSettingsModel.Exposure);
                Thread.Sleep(50);
            }
        }

        private void Accumulated_Click(object sender, RoutedEventArgs e)
        {
            if (GridMenuRight_Accumulated.Visibility == Visibility.Visible)
            {
                GridMenuRight_Accumulated.Visibility = Visibility.Collapsed;
            }
            else
            {
                GridMenuRight_Accumulated.Visibility = Visibility.Visible;
            }
        }

        private new void MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        /// <summary>
        /// Selection of camera settings
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CameraConfigurationSelection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((string)cameraConfigurationSelection.SelectedItem != null)
                GeneralSettings.CurrentCameraConfiguration = (string)cameraConfigurationSelection.SelectedItem;

            businessSystem.CameraSettingsModel = CameraSettingsModels
                                                    .FirstOrDefault(configurationPreset => configurationPreset.Name == GeneralSettings.CurrentCameraConfiguration);

            startNotConfigureCameraInit = false;
            CameraConfigure();
            DataHandlerUseCases.SaveJsonIntoSettings(GeneralSettings, ConfigurationConstants.GeneralConfigurationName);
        }

        /// <summary>
        /// Application of the settings save locally from the camera
        /// </summary>
        private void CameraConfigure()
        {
            if (!startNotConfigureCameraInit && GeneralSettings.MissingCamera)
            {
                ledBrightness.SelectedIndex = businessSystem.CameraSettingsModel.LedBrightness;
                CameraObject.DinoLiteSDK.SetFLCLevel(CameraObject.VideoDeviceIndex, businessSystem.CameraSettingsModel.LedBrightness);

                brightness_.Value = businessSystem.CameraSettingsModel.Brightness;
                CameraObject.DinoLiteSDK.set_VideoProcAmp(CameraObject.VideoDeviceIndex, businessSystem.CameraSettingsModel.Brightness);

                contrast.Value = businessSystem.CameraSettingsModel.Contrast;
                CameraObject.DinoLiteSDK.set_VideoProcAmp(1, businessSystem.CameraSettingsModel.Contrast);

                hue.Value = businessSystem.CameraSettingsModel.Hue;
                CameraObject.DinoLiteSDK.set_VideoProcAmp(2, businessSystem.CameraSettingsModel.Hue);

                saturation.Value = businessSystem.CameraSettingsModel.Saturation;
                CameraObject.DinoLiteSDK.set_VideoProcAmp(3, businessSystem.CameraSettingsModel.Saturation);

                sharpness.Value = businessSystem.CameraSettingsModel.Sharpness;
                CameraObject.DinoLiteSDK.set_VideoProcAmp(4, businessSystem.CameraSettingsModel.Sharpness);

                gamma.Value = businessSystem.CameraSettingsModel.Gamma;
                CameraObject.DinoLiteSDK.set_VideoProcAmp(5, businessSystem.CameraSettingsModel.Gamma);

                whiteBalanceRed.Value = businessSystem.CameraSettingsModel.AWBR;
                CameraObject.DinoLiteSDK.FreezeAWB(CameraObject.VideoDeviceIndex, 0);
                CameraObject.DinoLiteSDK.SetAWBR(CameraObject.VideoDeviceIndex, businessSystem.CameraSettingsModel.AWBR);
                CameraObject.DinoLiteSDK.FreezeAWB(CameraObject.VideoDeviceIndex, 1);

                whiteBalanceGreen.Value = businessSystem.CameraSettingsModel.AWBG;
                CameraObject.DinoLiteSDK.FreezeAWB(CameraObject.VideoDeviceIndex, 0);
                CameraObject.DinoLiteSDK.SetAWBG(CameraObject.VideoDeviceIndex, businessSystem.CameraSettingsModel.AWBG);
                CameraObject.DinoLiteSDK.FreezeAWB(CameraObject.VideoDeviceIndex, 1);

                whiteBalanceBlue.Value = businessSystem.CameraSettingsModel.AWBB;
                CameraObject.DinoLiteSDK.FreezeAWB(CameraObject.VideoDeviceIndex, 0);
                CameraObject.DinoLiteSDK.SetAWBB(CameraObject.VideoDeviceIndex, businessSystem.CameraSettingsModel.AWBB);
                CameraObject.DinoLiteSDK.FreezeAWB(CameraObject.VideoDeviceIndex, 1);

                switch (businessSystem.CameraSettingsModel.Mirror)
                {
                    case 0:
                        mirrorVertical.IsChecked = false;
                        mirrorHorizontal.IsChecked = false;
                        break;
                    case 1:
                        mirrorVertical.IsChecked = true;
                        mirrorHorizontal.IsChecked = false;
                        break;
                    case 2:
                        mirrorHorizontal.IsChecked = true;
                        mirrorVertical.IsChecked = false;
                        break;
                    case 3:
                        mirrorHorizontal.IsChecked = true;
                        mirrorVertical.IsChecked = true;
                        break;

                }

                FocusUpdate();

                startNotConfigureCameraInit = true;
            }
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            Views.Help help = new Views.Help();
            help.Hide();
        }

        private void RewriteNBristlesSpecification(TestSpecificationModel testSpecificationModel)
        {
            lblLowerAcceptLimit.Text = testSpecificationModel.TestSpecLowerLimit.ToString();
            lblNominalNBristles.Text = testSpecificationModel.TestTarget.ToString();
            lblUpperAcceptLimit.Text = testSpecificationModel.TestSpecUpperLimit.ToString();
            CameraObject.NominalBristle = (int)testSpecificationModel.TestTarget;
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            W = (648 * this.ActualWidth) / 934.5;
            H = (486 * this.ActualHeight) / 610.5;

            viewSDK_.Width = W;
            viewSDK_.Height = H;


            if (automaticBristleClassificationPredictionLayer != null)
            {
                automaticBristleClassificationPredictionLayer.Width = W;
                automaticBristleClassificationPredictionLayer.Height = H;

                automaticBristleClassificationPredictionLayer.viewSDKImage.Width = W;
                automaticBristleClassificationPredictionLayer.viewSDKImage.Height = H;



                automaticBristleClassificationPredictionLayer.canvas.Width = viewSDK.Width;
                automaticBristleClassificationPredictionLayer.canvas.Height = viewSDK.Height;

                automaticBristleClassificationPredictionLayer.canvasMask.Width = viewSDK.Width;
                automaticBristleClassificationPredictionLayer.canvasMask.Height = viewSDK.Height;

                System.Windows.Point locationFromScreen = this.viewSDK.PointToScreen(new System.Windows.Point(0, 0));

                // Transform screen point to WPF device independent point

                PresentationSource source = PresentationSource.FromVisual(this);

                System.Windows.Point targetPoints = source.CompositionTarget.TransformFromDevice.Transform(locationFromScreen);

                // Set coordinates

                automaticBristleClassificationPredictionLayer.Top = targetPoints.Y;

                automaticBristleClassificationPredictionLayer.Left = targetPoints.X;
            }
        }

        private void Analysis_1_Copy_Click(object sender, RoutedEventArgs e)
        {
            automaticBristleClassificationPredictionLayer.Left += 1;
            automaticBristleClassificationPredictionLayer.Top += 1;
        }

        private void ClassifyAsOkBristle_Click(object sender, MouseButtonEventArgs e)
        {
            ToolboxOperation = 10;
            automaticBristleClassificationPredictionLayer.EnableSelectMultiple = false;
        }

        private void ClassifyAsError1Bristle_Click(object sender, MouseButtonEventArgs e)
        {
            ToolboxOperation = 20;
            automaticBristleClassificationPredictionLayer.EnableSelectMultiple = false;
        }

        private void ClassifyAsError2Bristle_Click(object sender, MouseButtonEventArgs e)
        {
            ToolboxOperation = 30;
            automaticBristleClassificationPredictionLayer.EnableSelectMultiple = false;
        }

        private void ClassifyAsError3Bristle_Click(object sender, MouseButtonEventArgs e)
        {
            ToolboxOperation = 40;
            automaticBristleClassificationPredictionLayer.EnableSelectMultiple = false;
        }

        private void SetBrislteSizeI_Click(object sender, MouseButtonEventArgs e)
        {
            ToolboxOperation = 50;
            automaticBristleClassificationPredictionLayer.EnableSelectMultiple = false;
        }

        private void SetBrislteSizeII_Click(object sender, MouseButtonEventArgs e)
        {
            ToolboxOperation = 60;
            automaticBristleClassificationPredictionLayer.EnableSelectMultiple = false;
        }

        private void SetBrislteSizeIII_Click(object sender, MouseButtonEventArgs e)
        {
            ToolboxOperation = 70;
            automaticBristleClassificationPredictionLayer.EnableSelectMultiple = false;
        }

        private void MoveBrislte_Click(object sender, MouseButtonEventArgs e)
        {
            ToolboxOperation = 80;
            automaticBristleClassificationPredictionLayer.canvasMask.Visibility = Visibility.Collapsed;
            automaticBristleClassificationPredictionLayer.EnableSelectMultiple = false;
        }

        private void CreateNewBristle_Click(object sender, MouseButtonEventArgs e)
        {
            ToolboxOperation = 90;
            automaticBristleClassificationPredictionLayer.EnableSelectMultiple = false;
        }

        private void SelectMultiple_MouseUp(object sender, MouseButtonEventArgs e)
        {
            automaticBristleClassificationPredictionLayer.canvasMask.Visibility = Visibility.Visible;
            automaticBristleClassificationPredictionLayer.EnableSelectMultiple = true;
        }

        private void DeleteBristle_Click(object sender, MouseButtonEventArgs e)
        {
            ToolboxOperation = 100;
        }

        private void ButtonCalculator_MouseClick(object sender, MouseButtonEventArgs e)
        {
            System.Diagnostics.Process.Start(ConfigurationConstants.PathToCalculatorInit);
        }

        private void RetakePhoto_Click(object sender, RoutedEventArgs e)
        {
            SocketModels.Clear();
            automaticBristleClassificationPredictionLayer.canvas.Children.Clear();
            BoundingBox.Clear();
            totalBristlesFound.Text = "0";
            totalDefectiveBristles.Text = "0";
            LiveConditions(true);

            automaticBristleClassificationPredictionLayer.warning.Visibility = Visibility.Collapsed;
            result_view.Visibility = Visibility.Collapsed;

            startAnalyse_view.Visibility = Visibility.Visible;

            next.Content = "Next";
            next.IsEnabled = true;

            automaticBristleClassificationPredictionLayer.photo.Visibility = Visibility.Collapsed;
        }

        private void ReloadBoundingBoxes_MouseUp(object sender, MouseButtonEventArgs e)
        {
            automaticBristleClassificationPredictionLayer.canvas.Children.Clear();
            DrawBoundingBox();
        }

        public void SQLAndVisionConnectionStatus(Object myObject, EventArgs myEventArgs)
        {
            if (NetworkTestUseCases.TestIpPortConnection(GeneralSettings.IpPrediction, GeneralSettings.PortPrediction))
            {
                visionConnStat = true;
            }
            else
            {
                visionConnStat = false;
            }

            if (ColgateSkeltaEntities.Database.Exists())
            {
                sqlConnStat = true;
            }
            else
            {
                try
                {
                    ColgateSkeltaEntities = new ColgateSkeltaEntities();

                    if (ColgateSkeltaEntities.Database.Exists())
                    {
                        sqlConnStat = true;
                    }
                    else
                    {
                        sqlConnStat = false;
                    }
                }
                catch(Exception ex)
                {
                    Log.CustomLog.LogMessage("An error occured in SQLAndVisionConnectionStatus method while verifing conn status: " + ex.Message);
                }
            }
        }

        private void RefreshBarGaugeOfBristles()
        {
            int totalBristles = SocketModels.Count;
            int maximumBristlesAllowed = Convert.ToInt16(lblUpperAcceptLimit.Text);
            int minimumBristlesAllowed = Convert.ToInt16(lblLowerAcceptLimit.Text);

            if (maximumBristlesAllowed == 0)
                return;

            int totalPercentGoodBristles = Convert.ToInt16(totalBristles * 100 / maximumBristlesAllowed);

            if(totalPercentGoodBristles > 100)
            {
                pgbBristlesFound.Value = 100;
                pgbBristlesFound.Foreground = System.Windows.Media.Brushes.Red;

                int exceededBristlesPercent = (totalBristles - maximumBristlesAllowed) * 100 / 10;

                if(exceededBristlesPercent > 100)
                {
                    pgbBristlesFoundExceed.Value = 100;
                }
                else
                {
                    pgbBristlesFoundExceed.Value = exceededBristlesPercent;
                }

            }
            else
            {
                pgbBristlesFound.Value = totalPercentGoodBristles;

                pgbBristlesFoundExceed.Value = 0;

                if (totalBristles < minimumBristlesAllowed)
                {
                    pgbBristlesFound.Foreground = System.Windows.Media.Brushes.Red;
                }
                else
                {
                    pgbBristlesFound.Foreground = System.Windows.Media.Brushes.Green;
                }
            }

            divTestMetrics.UpdateLayout();
        }

        private void BtnTeste_Click(object sender, RoutedEventArgs e)
        {
            ClassifyAsOkBristle_Click(null, null);
            
            TurnToolboxButtonsBackgoundDefault();

            BorderButtonOk.BorderBrush = System.Windows.Media.Brushes.Navy;
            BorderButtonOk.Background = System.Windows.Media.Brushes.CornflowerBlue;
        }

        private void BtnClassifyError1_Click_1(object sender, RoutedEventArgs e)
        {
            ClassifyAsError1Bristle_Click(null, null);

            TurnToolboxButtonsBackgoundDefault();

            BorderButtonError1.BorderBrush = System.Windows.Media.Brushes.Navy;
            BorderButtonError1.Background = System.Windows.Media.Brushes.CornflowerBlue;
        }

        private void BtnClassifyError2_Click(object sender, RoutedEventArgs e)
        {
            ClassifyAsError2Bristle_Click(null, null);

            TurnToolboxButtonsBackgoundDefault();

            BorderButtonError2.BorderBrush = System.Windows.Media.Brushes.Navy;
            BorderButtonError2.Background = System.Windows.Media.Brushes.CornflowerBlue;
        }

        private void BtnClassifyError3_Click(object sender, RoutedEventArgs e)
        {
            ClassifyAsError3Bristle_Click(null, null);

            TurnToolboxButtonsBackgoundDefault();

            BorderButtonError3.BorderBrush = System.Windows.Media.Brushes.Navy;
            BorderButtonError3.Background = System.Windows.Media.Brushes.CornflowerBlue;
        }

        private void BtnSetBristleSize1_Click(object sender, RoutedEventArgs e)
        {
            SetBrislteSizeI_Click(null, null);

            TurnToolboxButtonsBackgoundDefault();

            BorderButtonResizeS.BorderBrush = System.Windows.Media.Brushes.Navy;
            BorderButtonResizeS.Background = System.Windows.Media.Brushes.CornflowerBlue;
        }

        private void BtnSetBristleSize2_Click(object sender, RoutedEventArgs e)
        {
            SetBrislteSizeII_Click(null, null);

            TurnToolboxButtonsBackgoundDefault();

            BorderButtonResizeM.BorderBrush = System.Windows.Media.Brushes.Navy;
            BorderButtonResizeM.Background = System.Windows.Media.Brushes.CornflowerBlue;
        }

        private void BtnSetBristleSize3_Click(object sender, RoutedEventArgs e)
        {
            SetBrislteSizeIII_Click(null, null);

            TurnToolboxButtonsBackgoundDefault();

            BorderButtonResizeL.BorderBrush = System.Windows.Media.Brushes.Navy;
            BorderButtonResizeL.Background = System.Windows.Media.Brushes.CornflowerBlue;
        }

        private void BtnMove_Click(object sender, RoutedEventArgs e)
        {
            MoveBrislte_Click(null, null);

            TurnToolboxButtonsBackgoundDefault();

            BorderButtonMove.BorderBrush = System.Windows.Media.Brushes.Navy;
            BorderButtonMove.Background = System.Windows.Media.Brushes.CornflowerBlue;
        }

        private void BtnSelectMultiple_Click(object sender, RoutedEventArgs e)
        {
            SelectMultiple_MouseUp(null, null);

            TurnToolboxButtonsBackgoundDefault();

            BorderButtonSelectMultiple.BorderBrush = System.Windows.Media.Brushes.Navy;
            BorderButtonSelectMultiple.Background = System.Windows.Media.Brushes.CornflowerBlue;
        }

        private void BtnDeleteBoxes_Click(object sender, RoutedEventArgs e)
        {
            DeleteBristle_Click(null, null);

            TurnToolboxButtonsBackgoundDefault();

            BorderButtonDelete.BorderBrush = System.Windows.Media.Brushes.Navy;
            BorderButtonDelete.Background = System.Windows.Media.Brushes.CornflowerBlue;
        }

        private void BtnReloadBoxes_Click(object sender, RoutedEventArgs e)
        {
            ReloadBoundingBoxes_MouseUp(null, null);

            TurnToolboxButtonsBackgoundDefault();

            BorderButtonReload.BorderBrush = System.Windows.Media.Brushes.Navy;
            BorderButtonReload.Background = System.Windows.Media.Brushes.CornflowerBlue;
        }

        public void TurnToolboxButtonsBackgoundDefault()
        {
            BorderButtonOk.BorderBrush = System.Windows.Media.Brushes.Black;
            BorderButtonOk.Background = System.Windows.Media.Brushes.Ivory;
            BorderButtonError1.BorderBrush = System.Windows.Media.Brushes.Black;
            BorderButtonError1.Background = System.Windows.Media.Brushes.Ivory;
            BorderButtonError2.BorderBrush = System.Windows.Media.Brushes.Black;
            BorderButtonError2.Background = System.Windows.Media.Brushes.Ivory;
            BorderButtonError3.BorderBrush = System.Windows.Media.Brushes.Black;
            BorderButtonError3.Background = System.Windows.Media.Brushes.Ivory;

            BorderButtonResizeS.BorderBrush = System.Windows.Media.Brushes.Black;
            BorderButtonResizeS.Background = System.Windows.Media.Brushes.Ivory;
            BorderButtonResizeM.BorderBrush = System.Windows.Media.Brushes.Black;
            BorderButtonResizeM.Background = System.Windows.Media.Brushes.Ivory;
            BorderButtonResizeL.BorderBrush = System.Windows.Media.Brushes.Black;
            BorderButtonResizeL.Background = System.Windows.Media.Brushes.Ivory;

            BorderButtonDelete.BorderBrush = System.Windows.Media.Brushes.Black;
            BorderButtonDelete.Background = System.Windows.Media.Brushes.Ivory;
            BorderButtonOk.BorderBrush = System.Windows.Media.Brushes.Black;
            BorderButtonOk.Background = System.Windows.Media.Brushes.Ivory;
            BorderButtonMove.BorderBrush = System.Windows.Media.Brushes.Black;
            BorderButtonMove.Background = System.Windows.Media.Brushes.Ivory;
            BorderButtonSelectMultiple.BorderBrush = System.Windows.Media.Brushes.Black;
            BorderButtonSelectMultiple.Background = System.Windows.Media.Brushes.Ivory;
            BorderButtonReload.BorderBrush = System.Windows.Media.Brushes.Black;
            BorderButtonReload.Background = System.Windows.Media.Brushes.Ivory;
        }

        private void PythonConnectionStatus_Click(object sender, RoutedEventArgs e)
        {
            CameraConfigure();
        }

        private void ButtonBristleRegister_Click(object sender, RoutedEventArgs e)
        {
            if (ScreenNavigationUseCases.OpenGeneralSettingsScreen(businessSystem.UserSystemCurrent, businessSystem.NetworkUserModel, GeneralSettings, businessSystem, ColgateSkeltaEntities, Maximized))
            {
                this.Close();
            }
            else
            {
                MessageBox.Show("Necessary administrative rights!");
            }
        }
    }
}