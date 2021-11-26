using APIVision;
using APIVision.Controllers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Rectangle = System.Drawing.Rectangle;
using System.Runtime.InteropServices;
using APIVision.Models;
using Newtonsoft.Json;

namespace Bristle.Views
{
    /// <summary>
    /// Internal logic for AutomaticBristleClassification.xaml
    /// </summary>
    public partial class AutomaticBristleClassification : Window
    {
        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);

        private static bool maximized = true;
        private BusinessSystem businessSystem;
        private MemoryStream ms;
        private BitmapImage bi;
        private static bool registerBox = true;
        private static int countBox = 0;
        private int X;
        private int Y;
        private int size_ = 75;
        private bool captureImage_ = true;
        private int SelectAnalysis = 0;
        private int cont = 0;
        private double acumula = 0;
        private string operacao = "";
        private bool sum = false;
        private bool subtraction = false;
        private bool multiplication = false;
        private bool division = false;
        private string position = "T";
        private string positionResultManual = "T";
        //private List<APIVision.Models.Box> boundingBox = new List<APIVision.Models.Box>();
        private List<APIVision.Models.SocketModel> boundingBox = new List<APIVision.Models.SocketModel>();
        private List<string> defect = new List<string>();
        private int boundingBoxSelect = 0;
        private bool boundingBoxEdit = false;
        private double w = 0;
        private double h = 0;
        private static bool LED1On = true;
        private static bool LED2On = true;
        private static bool LED3On = true;
        private static bool LED4On = true;
        private System.Drawing.Image img;
        private System.Windows.Forms.Timer myTimer = new System.Windows.Forms.Timer();
        private System.Windows.Forms.Timer myTimer2 = new System.Windows.Forms.Timer();
        private System.Windows.Forms.Timer myTimer3 = new System.Windows.Forms.Timer();
        //private static System.Windows.Forms.Timer myTimer4 = new System.Windows.Forms.Timer();
        private System.Windows.Forms.Timer myTimer5 = new System.Windows.Forms.Timer();
        //private static double cont = 0;
        private System.Drawing.Image img2;
        private System.Drawing.Image photo_;
        //private Bitmap frame;
        private System.Drawing.Image frameCopy;
        private bool startAnalyzing = false;
        private int focus = 0;
        private int currentFocus = 0;
        public bool live = false;
        private int selectBoundingBoxDelete = -1;
        private bool editBoundingBoxLeft = false;
        private bool editBoundingBoxRight = false;
        private bool editBoundingBoxUp = false;
        private bool editBoundingBoxDown = false;
        private int totalNumberBristles_T = 0;
        private int totalNumberBristles_M1 = 0;
        private int totalNumberBristles_M2 = 0;
        private int totalNumberBristles_M3 = 0;
        private int totalNumberBristles_N = 0;
        private string boundingBoxSelectType = "undefined";
        private bool frameHolderMouseMove = true;
        private int xCurrent = 0;
        private int yCurrent = 0;
        private int analyzedId = 0;
        private double oldExposureValue = 0;
        private bool startCameraParam = false;
        private bool startCameraParamSetLensPos = false;
        private bool StartAutoFocus = true;
        private bool StopAutoFocus = true;
        private bool manualFocusChanged = false;
        private float adjustment = 0.093F;
        private int imageCount = 0;
        private bool photoResult = false;
        private int Error1 = 0;
        private int Error2 = 0;
        private int Error3 = 0;
        private int discard = 0;
        private float WidthR_;
        private float HeightR_;
        private float xR_;
        private float yR_;
        private Bitmap b;
        private Graphics g;
        private object bitmapClear;
        private double nameImage = 0;
        private IntPtr bh;
        private IntPtr oldbh;
        private static bool liveOn_ = true;
        private System.Windows.Shapes.Rectangle mask = new System.Windows.Shapes.Rectangle();
        private System.Windows.Point maskPointStart = new System.Windows.Point();
        private System.Windows.Point maskPointStartMemory = new System.Windows.Point();
        private System.Windows.Point maskPointStopMemory = new System.Windows.Point();
        private bool maskDelete = false;
        private int numberMask = -1;
        private bool menuBeingUsed = false;
        private bool startNotConfigureCamera = true;
        private bool startNotConfigureCameraInit = false;
        private bool maximized__ = false;
        private System.Windows.Forms.Integration.WindowsFormsHost host;
        private Thread threadLoadingWait;
        private LoadingAnimation loading;
        private bool verySmallMask = false;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="maximized_"></param>
        /// <param name="businessSystem_"></param>
        public AutomaticBristleClassification(bool maximized_, BusinessSystem businessSystem_)
        {
            businessSystem = businessSystem_;
            maximized__ = maximized_;

            InitializeComponent();

            this.Loaded += AutomaticBristleClassification_Loaded;
            this.Closed += new EventHandler(AutomaticBristleClassification_Closed);

            //canvasMask.Visibility = Visibility.Collapsed;
            startNotConfigureCamera = false;
        }
        private void AutomaticBristleClassification_Loaded(object sender, RoutedEventArgs e)
        {
            StartLoadingWait(true);
            /*
             * SDK Dino-lite
             */
            host = new System.Windows.Forms.Integration.WindowsFormsHost();
            host.Child = ProductionObject.dinoLiteSDK;
            this.viewSDK.Children.Add(host);

            //ProductionObject.dinoLiteSDK.Preview = false;

            if (Directory.Exists("img"))
            {
                Directory.Delete("img", true);
            }

            windowSize();
            //Updates some camera parameters
            myTimer2.Tick += new EventHandler(cameraParam);
            myTimer2.Interval = 500;
            myTimer2.Start();

            //Updates the images in the grid when live is active
            myTimer5.Tick += new EventHandler(run);
            myTimer5.Interval = 200;// 300;
            myTimer5.Start();

            //Monitors if there is boundingbox to draw
            myTimer.Tick += new EventHandler(IA);
            myTimer.Interval = 50;
            myTimer.Start();

            warningText1.Visibility = Visibility.Visible;
            warningButton1.Visibility = Visibility.Visible;
            modalL.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 188, 64, 49));
            modalR.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 188, 64, 49));

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
        /// StartLoadingWait
        /// </summary>
        /// <param name="start"></param>
        private void StartLoadingWait(bool start)
        {
            if (start)
            {
                threadLoadingWait = new Thread(() => ThreadMethod());
                threadLoadingWait.SetApartmentState(ApartmentState.STA); //Set the thread to STA
                //threadLoadingWait.IsBackground = true;
                threadLoadingWait.Start();
            }
            else
            {
                Thread wait__ = new Thread(() => wait_());
                wait__.SetApartmentState(ApartmentState.STA); //Set the thread to STA
                //wait__.IsBackground = true;
                wait__.Start();
            }
        }

        private void wait_()
        {
            ThreadMethod();
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

        private void resetValue()
        {
            if (Directory.Exists("img"))
            {
                Directory.Delete("img", true);
            }
            position = "T";
            positionResultManual = "T";
            businessSystem.boundingBoxDiscards.Clear();
            businessSystem.brushAnalysisResultModel.TotalBristles = 0;
            businessSystem.brushAnalysisResultModel.TotalBristlesAnalyzed = 0;
            businessSystem.brushAnalysisResultModel.TotalGoodBristles = 0;
            boundingBox.Clear();
            businessSystem.tuffAnalysisResultModel.SelectedManual = false;
            registerBox = true;
            countBox = 0;
            captureImage_ = true;
            SelectAnalysis = 0;
            cont = 0;
            acumula = 0;
            operacao = "";
            sum = false;
            subtraction = false;
            multiplication = false;
            division = false;
            defect.Clear();
            boundingBoxSelect = 0;
            boundingBoxEdit = false;
            startAnalyzing = false;
            selectBoundingBoxDelete = -1;
            editBoundingBoxLeft = false;
            editBoundingBoxRight = false;
            editBoundingBoxUp = false;
            editBoundingBoxDown = false;
            totalNumberBristles_T = 0;
            totalNumberBristles_M1 = 0;
            totalNumberBristles_M2 = 0;
            totalNumberBristles_M3 = 0;
            totalNumberBristles_N = 0;
            boundingBoxSelectType = "undefined";
            frameHolderMouseMove = true;
            xCurrent = 0;
            yCurrent = 0;
            analyzedId = 0;
            oldExposureValue = 0;
            startCameraParam = false;
            startCameraParamSetLensPos = false;
            StartAutoFocus = true;
            StopAutoFocus = true;
            manualFocusChanged = false;
            adjustment = 0.093F;
            imageCount = 0;
            photoResult = false;
            Error1 = 0;
            Error2 = 0;
            Error3 = 0;
            discard = 0;
            nameImage = 0;
            liveOn_ = true;
            maskDelete = false;
            numberMask = -1;
            menuBeingUsed = false;
            startNotConfigureCamera = true;

            totalNumberBristlesTtype1.Text = "0";
            totalNumberBristlesTtype2.Text = "0";
            totalNumberBristlesTtype3.Text = "0";
            totalNumberBristlesTdiscard.Text = "0";
            totalNumberBristlesT.Text = "0";
            totalNumberBristlesTnok.Text = "0";

            totalNumberBristlesM1type1.Text = "0";
            totalNumberBristlesM1type2.Text = "0";
            totalNumberBristlesM1type3.Text = "0";
            totalNumberBristlesM1discard.Text = "0";
            totalNumberBristlesM1.Text = "0";
            totalNumberBristlesM1nok.Text = "0";

            totalNumberBristlesM2type1.Text = "0";
            totalNumberBristlesM2type2.Text = "0";
            totalNumberBristlesM2type3.Text = "0";
            totalNumberBristlesM2discard.Text = "0";
            totalNumberBristlesM2.Text = "0";
            totalNumberBristlesM2nok.Text = "0";

            totalNumberBristlesM3type1.Text = "0";
            totalNumberBristlesM3type2.Text = "0";
            totalNumberBristlesM3type3.Text = "0";
            totalNumberBristlesM3discard.Text = "0";
            totalNumberBristlesM3.Text = "0";
            totalNumberBristlesM3nok.Text = "0";

            totalNumberBristlesNtype1.Text = "0";
            totalNumberBristlesNtype2.Text = "0";
            totalNumberBristlesNtype3.Text = "0";
            totalNumberBristlesNdiscard.Text = "0";
            totalNumberBristlesN.Text = "0";
            totalNumberBristlesNnok.Text = "0";

            warning.Visibility = Visibility.Collapsed;
            result_view.Visibility = Visibility.Collapsed;
            startAnalyse5_view.Visibility = Visibility.Collapsed;

            startAnalyse_view.Visibility = Visibility.Visible;

            next.Content = "Next";
            //next.Click += nextAnalysis_Click;
            next.IsEnabled = true;
        }

        /// <summary>
        /// Draw the frames on the grid
        /// </summary>
        /// <param name="myObject"></param>
        /// <param name="myEventArgs"></param>
        private void run(Object myObject, EventArgs myEventArgs)
        {
            MemoryStream ms;

            try
            {
                if (liveOn_ && live)
                {               
                    if (ProductionObject.dinoLiteSDK.GrabFrame() != null)
                    {
                        ms = null;
                        System.Runtime.GCSettings.LargeObjectHeapCompactionMode = System.Runtime.GCLargeObjectHeapCompactionMode.CompactOnce;
                        Bitmap frame = (Bitmap)ProductionObject.dinoLiteSDK.GrabFrame().Clone();
                        ProductionObject.dinoLiteSDK.GrabFrame().Dispose();
                        if (frame != null)
                        {
                            photo_ = (Bitmap)frame.Clone();
                            ms = new MemoryStream();
                            frame.Save(ms, ImageFormat.Jpeg);                            
                            ms.Seek(0, SeekOrigin.Begin);
                            BitmapImage bi = new BitmapImage();
                            bi.BeginInit();
                            bi.StreamSource = ms;
                            bi.EndInit();                         
                            bi.Freeze();                         
                            Dispatcher.BeginInvoke(new ThreadStart(delegate
                            {
                                photo.Source = bi;
                            }));

                            
                            frame.Dispose();
                            DeleteObject(frame.GetHbitmap());
                        }
                    }               
                }
            }
            catch (Exception e)
            {
                System.Runtime.GCSettings.LargeObjectHeapCompactionMode = System.Runtime.GCLargeObjectHeapCompactionMode.CompactOnce;
                GC.Collect();
            }
            myTimer5.Stop();
            myTimer5.Enabled = true;
        }

        private void v_MicroTouchPress(object sender, EventArgs e)
        {
            ProductionObject.dinoLiteSDK.SaveFrame("c:\\55.bmp");
        }

        private void cameraParam(Object myObject, EventArgs myEventArgs)
        {
            try
            {
                if (!startCameraParam && businessSystem.generalSettings.MissingCamera)
                {
                    init();
                    startCameraParam = true;
                    cameraConfigure();
                    live = true;
                    kickStart();

                    StartLoadingWait(false);
                }
            }
            catch
            {

            }

            Thread updateMnualFocusValue_ = new Thread(() => updateMnualFocusValue());
            updateMnualFocusValue_.IsBackground = true;
            updateMnualFocusValue_.Start();
            if (!startCameraParamSetLensPos)
            {
                //focusUpdate();
            }

            try
            {
                if (manualFocusChanged)
                {
                    focusValue.Text = Math.Round(businessSystem.cameraSettingsModel.Focus, 2).ToString();
                    manualFocusChanged = false;
                }
            }
            catch
            {
            }


            if (!StartAutoFocus)
            {
                focusValue.Text = "WAIT";
                Thread autoFocusUpdateWait_ = new Thread(() => autoFocusUpdateWait());
                autoFocusUpdateWait_.IsBackground = true;
                autoFocusUpdateWait_.Start();
                StartAutoFocus = true;
            }

            if (!StopAutoFocus)
            {
                try
                {
                    double tmp = ProductionObject.dinoLiteSDK.GetAMRwithLensPos(ProductionObject.VideoDeviceIndex);
                    focusValue.Text = Math.Round(tmp, 2).ToString();
                    currentFocus = (int)(((tmp - businessSystem.cameraSettingsModel.Focus) * 1) / adjustment);
                    businessSystem.cameraSettingsModel.Focus = tmp;
                    focus -= currentFocus;
                    StopAutoFocus = true;
                }
                catch
                {
                }
            }

            myTimer2.Stop();
            myTimer2.Enabled = true;
        }

        private void focusUpdate()
        {
            try
            {
                double tmp = ProductionObject.dinoLiteSDK.GetAMRwithLensPos(ProductionObject.VideoDeviceIndex);
                if (tmp > 10)
                {
                    startCameraParamSetLensPos = true;
                    Thread.Sleep(50);
                    currentFocus = (int)(((tmp - businessSystem.cameraSettingsModel.Focus) * 1) / adjustment);
                    focus += currentFocus;
                    ProductionObject.dinoLiteSDK.SetLensPos(ProductionObject.VideoDeviceIndex, focus);
                    Thread.Sleep(50);
                    focusValue.Text = Math.Round(ProductionObject.dinoLiteSDK.GetAMRwithLensPos(ProductionObject.VideoDeviceIndex), 2).ToString();
                }
            }
            catch
            {
            }
        }

        private void autoFocusUpdateWait()
        {
            Thread.Sleep(8000);
            StopAutoFocus = false;
        }

        private void updateMnualFocusValue()
        {
            try
            {
                double tmpFocus = ProductionObject.dinoLiteSDK.GetAMRwithLensPos(ProductionObject.VideoDeviceIndex);
                if (businessSystem.cameraSettingsModel.Focus != tmpFocus)
                {
                    focus = 0;
                    ProductionObject.dinoLiteSDK.SetLensPos(ProductionObject.VideoDeviceIndex, focus);
                    businessSystem.cameraSettingsModel.Focus = tmpFocus;
                    Thread.Sleep(25);
                    manualFocusChanged = true;
                }
            }
            catch (Exception e)
            {
                businessSystem.cameraSettingsModel.Focus = ProductionObject.dinoLiteSDK.GetAMR(ProductionObject.VideoDeviceIndex);
                manualFocusChanged = true;
                Console.WriteLine(e.Message);
            }
        }

        /// <summary>
        /// Resizing images
        /// </summary>
        private void windowSize()
        {
            if (maximized)
            {
                w = (648 * SystemParameters.PrimaryScreenWidth) / 934.5;
                h = (486 * SystemParameters.PrimaryScreenHeight) / 610.5;

                viewSDK_.Width = w;
                viewSDK_.Height = h;

                viewSDKImage.Width = w;
                viewSDKImage.Height = h;

                //frameHolder.Width = viewSDKRealTime.Width;
                //frameHolder.Height = viewSDKRealTime.Height;
                //frameHolder.Stretch = Stretch.Fill;

                photo.Width = viewSDK.Width;
                photo.Height = viewSDK.Height;
                photo.Stretch = Stretch.Fill;

                canvas.Width = viewSDK.Width;
                canvas.Height = viewSDK.Height;

                canvasMask.Width = viewSDK.Width;
                canvasMask.Height = viewSDK.Height;
            }
            else
            {
            }
        }

        private void sizeChanged(object sender, SizeChangedEventArgs e)
        {
            w = (648 * this.ActualWidth) / 934.5;
            h = (486 * this.ActualHeight) / 610.5;

            viewSDK_.Width = w;
            viewSDK_.Height = h;

            viewSDKImage.Width = w;
            viewSDKImage.Height = h;

            //frameHolder.Width = viewSDKRealTime.Width;
            //frameHolder.Height = viewSDKRealTime.Height;
            //frameHolder.Stretch = Stretch.Fill;

            photo.Width = viewSDK.Width;
            photo.Height = viewSDK.Height;
            photo.Stretch = Stretch.Fill;

            canvas.Width = viewSDK.Width;
            canvas.Height = viewSDK.Height;

            canvasMask.Width = viewSDK.Width;
            canvasMask.Height = viewSDK.Height;
        }

        /// <summary>
        /// Initializes the selection inputs by the user
        /// </summary>
        private void init()
        {
            businessSystem.testModels = businessSystem.dataBaseController.listTestModel(-1);
            List<int> idSKUTest = new List<int>();
            foreach (var itemTest in businessSystem.testModels)
            {
                if (itemTest.sDescription == "Arredondamento")
                {
                    idSKUTest.Add((int)itemTest.iSKU);
                }
            }

            skuSelect.Items.Add("SKU Select");
            businessSystem.sKU1Model = businessSystem.dataBaseController.listSKUsModel();
            foreach (var item in businessSystem.sKU1Model)
            {
                foreach (var id in idSKUTest)
                {
                    if (id == item.iID)
                    {
                        skuSelect.Items.Add(item.sSKU);
                    }
                }
            }
            skuSelect.SelectedIndex = 0;

            try
            {
                businessSystem.cameraSettingsModels = JsonConvert.DeserializeObject<List<CameraSettingsModel>>(File.ReadAllText(businessSystem.localRepository + @"\cameraSettings.json"));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            foreach (var item in businessSystem.cameraSettingsModels)
            {
                cameraConfigurationSelection.Items.Add(item.Name);
            }
            cameraConfigurationSelection.SelectedItem = businessSystem.generalSettings.CurrentCameraConfiguration;

            GridMenuRight.Visibility = Visibility.Collapsed;

            //slValue_view.Text = businessSystem.generalSystemSettingsModel.Threshold.ToString();
            //slValue.Value = businessSystem.generalSystemSettingsModel.Threshold;

            ledBrightness.Items.Add(1);
            ledBrightness.Items.Add(2);
            ledBrightness.Items.Add(3);
            ledBrightness.Items.Add(4);
            ledBrightness.Items.Add(5);
            ledBrightness.Items.Add(6);
            ledBrightness.SelectedIndex = businessSystem.cameraSettingsModel.LedBrightness - 1;

            businessSystem.boundingBoxDiscards.Clear();
            businessSystem.brushAnalysisResultModel.TotalBristles = 0;
            businessSystem.brushAnalysisResultModel.TotalBristlesAnalyzed = 0;
            businessSystem.brushAnalysisResultModel.TotalGoodBristles = 0;
            boundingBox.Clear();
            businessSystem.tuffAnalysisResultModel.SelectedManual = false;
        }

        /// <summary>
        /// Sends an S100 command to the artificial intelligence module to start predictions
        /// </summary>
        private void kickStart()
        {
            lock (this)
            {
                startAnalyzing = true;
                businessSystem.SendCommand("S300", businessSystem.generalSettings.Model);
                businessSystem.data = "";
                Thread.Sleep(1000);
                businessSystem.SendCommand("S100", (Bitmap)ProductionObject.dinoLiteSDK.GrabFrame().Clone());//(Bitmap)photo_);
                ProductionObject.dinoLiteSDK.GrabFrame().Dispose();
                businessSystem.data = "";
            }
        }

        public void ButtonPopUpLogout_Click(object sender, RoutedEventArgs e)
        {
            live = false;

            ProductionObject.dinoLiteSDK.MicroTouchPressed -= v_MicroTouchPress;
            ProductionObject.dinoLiteSDK.Connected = false;
            Thread.Sleep(280);
            myTimer.Dispose();
            myTimer2.Dispose();
            myTimer3.Dispose();
            myTimer5.Dispose();
            Views.UserControl userControl = new UserControl();
            liveOn_ = true;
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
            calculator.Visibility = Visibility.Collapsed;
        }

        private void ButtonHome_Click(object sender, RoutedEventArgs e)
        {
            live = false;

            Views.MainWindow mainWindow = new Views.MainWindow(maximized, businessSystem);
            mainWindow.Show();
            this.Close();
        }

        private void ButtonBristleRegister_Click(object sender, RoutedEventHandler e)
        {

        }

        private void ButtonAutomaticBristleClassification_Click(object sender, RoutedEventArgs e)
        {
            //Views.AutomaticBristleClassification automaticBristleClassification = new Views.AutomaticBristleClassification(maximized, businessSystem);
            //automaticBristleClassification.Show();
            //this.Close();
        }

        private void ButtonWindowMaximize_Click(object sender, RoutedEventArgs e)
        {
            if (!maximized)
            {
                this.WindowState = WindowState.Maximized;
                maximized = true;
                windowSize();
            }
            else
            {
                this.WindowState = WindowState.Normal;
                maximized = false;
                windowSize();
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

        /// <summary>
        /// Check if the user has administrative rights to access this class
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonNeuralNetworkRetraining_Click(object sender, RoutedEventArgs e)
        {
            if (businessSystem.userSystemCurrent.Type == "administrator")
            {
                Views.NeuralNetworkRetraining neuralNetworkRetraining = new NeuralNetworkRetraining(maximized, businessSystem, this);
                neuralNetworkRetraining.Show();
                this.Hide();
            }
            else
            {
                //Validate the group that can access this part 
                foreach (var group in businessSystem.networkUserModel.NetworkUserGroup)
                {
                    if (group == businessSystem.AD_Admin || group == businessSystem.AD_Quality)
                    {
                        live = false;

                        Views.NeuralNetworkRetraining neuralNetworkRetraining = new NeuralNetworkRetraining(maximized, businessSystem, this);
                        neuralNetworkRetraining.Show();
                        this.Hide();
                    }
                }
                MessageBox.Show("Necessary administrative rights!");
            }
        }

        private void ButtonBristleRegister_Click_1(object sender, RoutedEventArgs e)
        {           
            if (businessSystem.userSystemCurrent.Type == "administrator")
            {
                Views.GeneralSettings generalSettings = new Views.GeneralSettings(maximized, businessSystem, this);
                generalSettings.Show();
                this.Hide();
            }
            else
            {
                //Validate the group that can access this part 
                foreach (var group in businessSystem.networkUserModel.NetworkUserGroup)
                {
                    if (group == businessSystem.AD_Admin || group == businessSystem.AD_Quality)
                    {
                        live = false;

                        Views.GeneralSettings generalSettings = new Views.GeneralSettings(maximized, businessSystem, this);
                        generalSettings.Show();
                        this.Hide();
                    }
                }
                MessageBox.Show("Necessary administrative rights!");
            }           
        }

        private void ButtonGeneralReport_Click_1(object sender, RoutedEventArgs e)
        {
            live = false;

            Views.GeneralReport generalReport = new GeneralReport(maximized, businessSystem, this);
            generalReport.Show();
            this.Close();
        }

        private void ButtonWindowMaximize__Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// Button for capturing images, it generates auxiliary images and modifying 
        /// conditions when live is active or not.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void captureImage_Click(object sender, RoutedEventArgs e)
        {
            if (skuSelect.SelectedIndex != 0 && equipamentSelect.SelectedIndex != 0 && areaSelect.SelectedIndex != 0 && batchLoteT.Text != "_")
            {
                //INFO.Text = " ";                

                if (captureImage_)
                {
                    ConditionsNotAlive();
                }
                else
                {
                    liveConditions(false);
                }

                changeCaptureIcon();
            }
            else
            {
                warning.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// Apply the conditions to be able to run the live.
        /// </summary>
        /// <param name="next_"></param>
        private void liveConditions(bool next_)
        {
            live = true;
            boundingBoxSelect = 0;
            lock (this)
            {
                startAnalyzing = true;
                submenuBox.Visibility = Visibility.Collapsed;
                submenuBox.IsEnabled = false;
                businessSystem.SendCommand("S100", (Bitmap)ProductionObject.dinoLiteSDK.GrabFrame().Clone());
                ProductionObject.dinoLiteSDK.GrabFrame().Dispose();
                businessSystem.data = "";
            }

            GridMenuRight.Visibility = Visibility.Collapsed;
            next.IsEnabled = false;

            if (!next_)
            {
                imageCount++;

                try
                {
                    ///Prepares to generate auxiliary images for the user. If there is no 
                    ///boudingBox and send only images of the frame.

                    if (canvas.Children.Count > 0)
                    {
                        Bitmap bitmap2;
                        lock (this)
                        {
                            img2 = (Bitmap)ProductionObject.dinoLiteSDK.GrabFrame().Clone();
                            ProductionObject.dinoLiteSDK.GrabFrame().Dispose();
                            bitmap2 = businessSystem.generalController.ResizeImage(img2, (int)img2.Width, (int)img2.Height);  
                            
                        }

                        RenderTargetBitmap rtb = new RenderTargetBitmap((int)canvas.RenderSize.Width,
                        (int)canvas.RenderSize.Height, 96d, 96d, System.Windows.Media.PixelFormats.Default);
                        rtb.Render(canvas);

                        BitmapEncoder pngEncoder = new PngBitmapEncoder();
                        pngEncoder.Frames.Add(BitmapFrame.Create(rtb));

                        using (var fs = System.IO.File.OpenWrite("imageAux.png"))
                        {
                            pngEncoder.Save(fs);
                        }

                        System.Drawing.Image image2;
                        lock (this)
                        {
                            image2 = System.Drawing.Image.FromFile("imageAux.png");
                        }

                        System.Drawing.Bitmap bitmap = businessSystem.generalController.ResizeImage(image2, (int)img2.Width, (int)img2.Height);


                        Thread t = new Thread(() => newImageShow(bitmap2, bitmap, imageCount));
                        t.Name = "t";
                        t.SetApartmentState(ApartmentState.STA); //Set the thread to STA
                        t.IsBackground = true;
                        t.Start();

                    }
                    else
                    {
                        img2 = (Bitmap)ProductionObject.dinoLiteSDK.GrabFrame().Clone();
                        ProductionObject.dinoLiteSDK.GrabFrame().Dispose();
                        System.Drawing.Bitmap bitmap2 = businessSystem.generalController.ResizeImage(img2, (int)img2.Width, (int)img2.Height);
                        System.Drawing.Bitmap bitmap = null;
                        Thread t = new Thread(() => newImageShow(bitmap2, bitmap, imageCount));
                        t.Name = "t";
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
        private void newImageShow(System.Drawing.Bitmap image, System.Drawing.Bitmap boundingBox, int imageCount)
        {
            try
            {
                Directory.CreateDirectory("img");
                string path = Directory.GetCurrentDirectory() + "\\img\\auxImage" + imageCount;
                image.Save(path);
                //boundingBox.Save("auxBoundingBox" + imageCount);
                List<System.Drawing.Bitmap> images = new List<System.Drawing.Bitmap>();
                images.Add(image);
                images.Add(boundingBox);

                System.Drawing.Bitmap imageWithBoundingBox;
                lock (this)
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

                //Views.ImageView imageView = new Views.ImageView(image, boundingBox, imageCount);
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
            live = false;
            IAPhoto();
            submenuBox.IsEnabled = true;
            next.IsEnabled = true;
            submenuBox.Visibility = Visibility.Visible;
            //viewSDKImage.Visibility = Visibility.Visible;
            GridMenuRight.Visibility = Visibility.Visible;
            //viewSDK.Visibility = Visibility.Collapsed;
        }

        private void traceIcon(string select)
        {
        }

        private void changeCaptureIcon()
        {
            captureImage_ = !captureImage_;

            if (captureImage_)
            {

                camera.Kind = MaterialDesignThemes.Wpf.PackIconKind.Camera;
                cameraStartAnalysis.Kind = MaterialDesignThemes.Wpf.PackIconKind.Camera;
            }
            else
            {
                camera.Kind = MaterialDesignThemes.Wpf.PackIconKind.TelevisionPause;
                cameraStartAnalysis.Kind = MaterialDesignThemes.Wpf.PackIconKind.TelevisionPause;
            }
        }

        /// <summary>
        /// warning
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void startAnalysis_Click(object sender, RoutedEventArgs e)
        {
            if (skuSelect.SelectedIndex != 0 && equipamentSelect.SelectedIndex != 0 && areaSelect.SelectedIndex != 0 && batchLoteT.Text != "_")
            {

                warningText1.Visibility = Visibility.Collapsed;
                warningButton1.Visibility = Visibility.Collapsed;
                warningText2.Visibility = Visibility.Visible;
                warningButton2.Visibility = Visibility.Visible;
                modalL.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 188, 64, 49));
                modalR.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 188, 64, 49));

                analysis_();
            }
            else
            {
                //INFO.Text = "Select a SKU and Equipment";
                warning.Visibility = Visibility.Visible;
            }

            //LoadingWait.Visibility = Visibility.Collapsed;          
        }

        private void LoadingWait_(WindowStartupLocation windowStartupLocation)
        {
            //Views.LoadingAnimation loading = new Views.LoadingAnimation();
            //loading.WindowStartupLocation = windowStartupLocation;
            //loading.ShowDialog();
            //while (true) { };
        }

        /// <summary>
        /// Applications for analysis of the bristles and everything that will be checked.
        /// </summary>
        private void analysis_()
        {
            switch (SelectAnalysis)
            {
                case 0:
                    viewSDKImage.Visibility = Visibility.Visible;
                    viewSDK.Visibility = Visibility.Collapsed;
                    viewSDK_.Visibility = Visibility.Collapsed;
                    liveOn.Background = System.Windows.Media.Brushes.Green;
                    businessSystem.analyzeModel.Name = skuSelect.Items[skuSelect.SelectedIndex].ToString();
                    businessSystem.analyzeModel.Timestamp = DateTime.UtcNow;
                    businessSystem.analyzeModel.Equipament = equipamentSelect.Items[equipamentSelect.SelectedIndex].ToString();
                    SKU_select_M1_view.Text = businessSystem.analyzeModel.Name;
                    SKU_select_M2_view.Text = businessSystem.analyzeModel.Name;
                    SKU_select_M3_view.Text = businessSystem.analyzeModel.Name;
                    SKU_select_N_view.Text = businessSystem.analyzeModel.Name;
                    businessSystem.generalSettings.Test = testSelect.Items[testSelect.SelectedIndex].ToString();
                    //businessSystem.generalSettings.IdTest_1 = testFindId(testSelect.Items[testSelect.SelectedIndex].ToString());
                    test_select_M1_view.Text = businessSystem.generalSettings.Test;
                    test_select_M2_view.Text = businessSystem.generalSettings.Test;
                    test_select_M3_view.Text = businessSystem.generalSettings.Test;
                    test_select_N_view.Text = businessSystem.generalSettings.Test;
                    businessSystem.generalSettings.Area = areaSelect.Items[areaSelect.SelectedIndex].ToString();
                    area_select_M1_view.Text = businessSystem.generalSettings.Area;
                    area_select_M2_view.Text = businessSystem.generalSettings.Area;
                    area_select_M3_view.Text = businessSystem.generalSettings.Area;
                    area_select_N_view.Text = businessSystem.generalSettings.Area;
                    equipament_select_M1_view.Text = businessSystem.analyzeModel.Equipament;
                    equipament_select_M2_view.Text = businessSystem.analyzeModel.Equipament;
                    equipament_select_M3_view.Text = businessSystem.analyzeModel.Equipament;
                    equipament_select_N_view.Text = businessSystem.analyzeModel.Equipament;
                    businessSystem.generalSettings.BatchLote = batchLoteT.Text;
                    batchLote_select_M1_view.Text = businessSystem.generalSettings.BatchLote;
                    batchLote_select_M2_view.Text = businessSystem.generalSettings.BatchLote;
                    batchLote_select_M3_view.Text = businessSystem.generalSettings.BatchLote;
                    batchLote_select_N_view.Text = businessSystem.generalSettings.BatchLote;
                    camera_configuration_M1_selection.Text = cameraConfigurationSelection.Text;
                    camera_configuration_M2_selection.Text = cameraConfigurationSelection.Text;
                    camera_configuration_M3_selection.Text = cameraConfigurationSelection.Text;
                    camera_configuration_N_selection.Text = cameraConfigurationSelection.Text;
                    nominalNBristleM1.Text = ProductionObject.nominalBristle.ToString();
                    nominalNBristleM2.Text = ProductionObject.nominalBristle.ToString();
                    nominalNBristleM3.Text = ProductionObject.nominalBristle.ToString();
                    nominalNBristleN.Text = ProductionObject.nominalBristle.ToString();
                    sku.Text = businessSystem.analyzeModel.Name;
                    analyzedId = businessSystem.dataBaseController.updateAnalyzeModel(businessSystem.analyzeModel, null, null);

                    ConditionsNotAlive();
                    result_view.Visibility = Visibility.Visible;
                    captureImage_ = false;
                    captureImage.IsEnabled = false;
                    startAnalyse_view.Visibility = Visibility.Collapsed;
                    submenuBox.IsEnabled = true;
                    camera.Kind = MaterialDesignThemes.Wpf.PackIconKind.TelevisionPause;
                    cameraStartAnalysis.Kind = MaterialDesignThemes.Wpf.PackIconKind.TelevisionPause;
                    break;
                case 1:
                    viewSDKImage.Visibility = Visibility.Visible;
                    viewSDK.Visibility = Visibility.Collapsed;
                    viewSDK_.Visibility = Visibility.Collapsed;
                    liveOn.Background = System.Windows.Media.Brushes.Green;
                    result_view.Visibility = Visibility.Visible;
                    captureImage.IsEnabled = false;
                    captureImage_ = false;
                    startAnalyse2_view.Visibility = Visibility.Collapsed;
                    ConditionsNotAlive();
                    submenuBox.IsEnabled = true;
                    camera.Kind = MaterialDesignThemes.Wpf.PackIconKind.TelevisionPause;
                    cameraStartAnalysis.Kind = MaterialDesignThemes.Wpf.PackIconKind.TelevisionPause;
                    break;
                case 2:
                    viewSDKImage.Visibility = Visibility.Visible;
                    viewSDK.Visibility = Visibility.Collapsed;
                    viewSDK_.Visibility = Visibility.Collapsed;
                    liveOn.Background = System.Windows.Media.Brushes.Green;
                    result_view.Visibility = Visibility.Visible;
                    captureImage.IsEnabled = false;
                    captureImage_ = false;
                    startAnalyse3_view.Visibility = Visibility.Collapsed;
                    ConditionsNotAlive();
                    submenuBox.IsEnabled = true;
                    camera.Kind = MaterialDesignThemes.Wpf.PackIconKind.TelevisionPause;
                    cameraStartAnalysis.Kind = MaterialDesignThemes.Wpf.PackIconKind.TelevisionPause;
                    break;
                case 3:
                    viewSDKImage.Visibility = Visibility.Visible;
                    viewSDK.Visibility = Visibility.Collapsed;
                    viewSDK_.Visibility = Visibility.Collapsed;
                    liveOn.Background = System.Windows.Media.Brushes.Green;
                    result_view.Visibility = Visibility.Visible;
                    captureImage.IsEnabled = false;
                    captureImage_ = false;
                    startAnalyse4_view.Visibility = Visibility.Collapsed;
                    ConditionsNotAlive();
                    submenuBox.IsEnabled = true;
                    camera.Kind = MaterialDesignThemes.Wpf.PackIconKind.TelevisionPause;
                    cameraStartAnalysis.Kind = MaterialDesignThemes.Wpf.PackIconKind.TelevisionPause;
                    break;
                case 4:
                    viewSDKImage.Visibility = Visibility.Visible;
                    viewSDK.Visibility = Visibility.Collapsed;
                    viewSDK_.Visibility = Visibility.Collapsed;
                    liveOn.Background = System.Windows.Media.Brushes.Green;
                    result_view.Visibility = Visibility.Visible;
                    captureImage.IsEnabled = false;
                    captureImage_ = false;
                    next.Content = "Result";
                    next.Click += analysisFinish_Click;
                    submenuBox.IsEnabled = true;
                    startAnalyse5_view.Visibility = Visibility.Collapsed;
                    ConditionsNotAlive();
                    camera.Kind = MaterialDesignThemes.Wpf.PackIconKind.TelevisionPause;
                    cameraStartAnalysis.Kind = MaterialDesignThemes.Wpf.PackIconKind.TelevisionPause;
                    break;
            }
            traceIcon("KeyboardReturn");
        }

        private int testFindId(string item)
        {
            foreach (var i in businessSystem.testModels)
            {
                if (i.sDescription == item)
                {
                    return i.Id;
                }
            }

            return 0;
        }

        private void analysisRecord()
        {
            //businessSystem.analyzeModel.iSKU_id = 
            //businessSystem.analyzeModel.Timestamp = 
        }

        /// <summary>
        /// Sends 1 frame for prediction via S100 command.
        /// </summary>
        private void IAPhoto()
        {
            try
            {
                lock (this)
                {
                    //photo_ = (Bitmap)ProductionObject.dinoLiteSDK.GrabFrame().Clone();
                    ms = new MemoryStream();
                    photo_.Save(ms, ImageFormat.Png);
                    ms.Seek(0, SeekOrigin.Begin);
                    bi = new BitmapImage();
                    bi.BeginInit();
                    bi.StreamSource = ms;
                    bi.EndInit();
                    photo.Source = bi;

                    startAnalyzing = true;
                    businessSystem.SendCommand("S100", (Bitmap)ProductionObject.dinoLiteSDK.GrabFrame().Clone());
                    ProductionObject.dinoLiteSDK.GrabFrame().Dispose();
                    businessSystem.data = "";

                    photoResult = true;
                }
            }
            catch (Exception e)
            {
                //lock (this)
                //{
                //    //photo_ = (Bitmap)ProductionObject.dinoLiteSDK.GrabFrame().Clone();
                //    ms = new MemoryStream();
                //    photo_.Save(ms, ImageFormat.Png);
                //    ms.Seek(0, SeekOrigin.Begin);
                //    bi = new BitmapImage();
                //    bi.BeginInit();
                //    bi.StreamSource = ms;
                //    bi.EndInit();
                //    photo.Source = bi;

                //    startAnalyzing = true;
                //    businessSystem.SendCommand("S100", (Bitmap)photo_);
                //    businessSystem.data = "";

                //    photoResult = true;
                //}

                //Console.WriteLine(e.Message);
            }
           

            myTimer3.Tick += new EventHandler(photoResult_);
            myTimer3.Interval = 10;
            myTimer3.Enabled = true;
            myTimer3.Start();

        }

        /// <summary>
        /// Monitors the result of the prediction of the sending done in the IAPhoto () method.
        /// </summary>
        /// <param name="myObject"></param>
        /// <param name="myEventArgs"></param>
        private void photoResult_(Object myObject, EventArgs myEventArgs)
        {
            if (photoResult)
            {
                if (businessSystem.data != null && businessSystem.data.Length > 0)
                {
                    if (businessSystem.data != "*")
                    {
                        cont = 0;
                        lock (this)
                        {
                            businessSystem.socketModels = businessSystem.generalController.converterJSON(businessSystem.data, businessSystem);
                            businessSystem.data = "";
                            drawBoundingBox();
                            photoResult = false;
                        }
                    }
                }
                else
                {
                    cont++;
                    if (cont > 1000)
                    {
                        lock (this)
                        {
                            businessSystem.SendCommand("S100", (Bitmap)ProductionObject.dinoLiteSDK.GrabFrame().Clone());
                            ProductionObject.dinoLiteSDK.GrabFrame().Dispose();
                            businessSystem.data = "";
                        }
                        cont = 0;
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
            if (live)
            {
                if (startAnalyzing)
                {
                    if (businessSystem.data != null && businessSystem.data.Length > 0)
                    {
                        if (businessSystem.data != "*")
                        {
                            cont = 0;
                            lock (this)
                            {
                                businessSystem.socketModels = businessSystem.generalController.converterJSON(businessSystem.data, businessSystem);
                                businessSystem.data = "";
                                drawBoundingBox();
                            }
                            try
                            {
                                lock (this)
                                {
                                    businessSystem.SendCommand("S100", (Bitmap)ProductionObject.dinoLiteSDK.GrabFrame().Clone());
                                    ProductionObject.dinoLiteSDK.GrabFrame().Dispose();
                                    businessSystem.data = "";
                                }
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.Message);
                            }
                        }
                        else
                        {
                            cont++;
                            if (cont > 10)
                            {
                                lock (this)
                                {
                                    recoverCommunicationWithAI();
                                }
                                cont = 0;
                            }
                        }
                    }
                    else
                    {
                        cont++;
                        if (cont > 10)
                        {
                            lock (this)
                            {
                                recoverCommunicationWithAI();
                            }
                            cont = 0;
                        }                   
                    }
                }
            }

            myTimer.Stop();
            myTimer.Enabled = true;
        }

        /// <summary>
        /// recoverCommunicationWithAI
        /// </summary>
        private void recoverCommunicationWithAI()
        {
            try
            {
                businessSystem.SendCommand("S300", businessSystem.generalSettings.Model);
                businessSystem.data = "";
                businessSystem.SendCommand("S100", (Bitmap)ProductionObject.dinoLiteSDK.GrabFrame().Clone());
                ProductionObject.dinoLiteSDK.GrabFrame().Dispose();
                businessSystem.data = "";

                businessSystem.generalController.CMD("./StartSocketAI.vbs");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private void drawBoundingBoxDinolite()
        {

            try
            {
                lock (this)
                {
                    b = new Bitmap((int)w, (int)h);
                    g = Graphics.FromImage(b);
                    g.Clear(System.Drawing.Color.White);
                    if (businessSystem.socketModels != null)
                    {
                        foreach (var d in businessSystem.socketModels)
                        {
                            g.DrawRectangle(System.Drawing.Pens.Yellow, d.x, d.y, d.Width, d.Height);
                        }
                        bh = b.GetHbitmap();
                        g.Dispose();
                        b.Dispose();
                        ProductionObject.dinoLiteSDK.SetBitmapOverlay((int)bh, 0, 0, 0xffffff, 255);
                        if (oldbh != (IntPtr)0)
                        {
                            DeleteObject(oldbh);
                        }
                        oldbh = bh;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                GC.Collect();
            }
        }

        /// <summary>
        /// Draw the bounding box
        /// </summary>
        private void drawBoundingBox()
        {
            try
            {
                int contDefect = 0;

                canvas.Children.Clear();
                boundingBox.Clear(); //teste

                if (live)
                {
                    foreach (var d in businessSystem.socketModels)
                    {
                        double WidthR = (d.Width * w) / ProductionObject.ResolutionWidth;
                        double HeightR = (d.Height * h) / ProductionObject.ResolutionHeight;
                        double xR = (d.x * w) / ProductionObject.ResolutionWidth;
                        double yR = (d.y * h) / ProductionObject.ResolutionHeight;

                        System.Windows.Shapes.Rectangle rect = new System.Windows.Shapes.Rectangle
                        {
                            Stroke = System.Windows.Media.Brushes.Yellow,
                            Opacity = 100,
                            Width = WidthR,
                            Height = HeightR
                        };
                        Canvas.SetLeft(rect, xR);
                        Canvas.SetTop(rect, yR);
                        canvas.Children.Add(rect);


                        APIVision.Models.Box box = new APIVision.Models.Box();
                    }
                    countBox = canvas.Children.Count;
                    totalBristlesFound.Text = businessSystem.socketModels.Count().ToString();
                    totalDefectiveBristles.Text = contDefect.ToString();

                    canvas.UpdateLayout();
                    canvas.UpdateDefaultStyle();
                    viewSDKImage.UpdateLayout();
                }
                else
                {
                    Error1 = 0;
                    Error2 = 0;
                    Error3 = 0;
                    discard = 0;
                    foreach (var d in businessSystem.socketModels)
                    {
                        //if (d.obj_class != "discard")
                        //{
                        double WidthR = (d.Width * w) / ProductionObject.ResolutionWidth;
                        double HeightR = (d.Height * h) / ProductionObject.ResolutionHeight;
                        double xR = (d.x * w) / ProductionObject.ResolutionWidth;
                        double yR = (d.y * h) / ProductionObject.ResolutionHeight;

                        if (d.obj_class == "none")
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
                            canvas.Children.Add(rect);
                        }
                        else if (d.obj_class == "discard")
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
                            canvas.Children.Add(rect);
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
                            canvas.Children.Add(rect);
                        }
                        TextBlock textBlock = new TextBlock();
                        textBlock.FontSize = 18;

                        if (d.obj_class != "none")
                        {
                            switch (positionResultManual)
                            {
                                case "T":
                                    if (d.obj_class == "type1")
                                    {
                                        Error1++;
                                        contDefect++;
                                        totalNumberBristlesTtype1.Text = Error1.ToString();
                                    }
                                    else if (d.obj_class == "type2")
                                    {
                                        Error2++;
                                        contDefect++;
                                        totalNumberBristlesTtype2.Text = Error2.ToString();
                                    }
                                    else if (d.obj_class == "type3" || d.obj_class == "reb")
                                    {
                                        Error3++;
                                        contDefect++;
                                        totalNumberBristlesTtype3.Text = Error3.ToString();
                                    }
                                    else if (d.obj_class == "discard")
                                    {
                                        discard++;
                                        totalNumberBristlesTdiscard.Text = discard.ToString();
                                    }
                                    break;
                                case "M1":
                                    if (d.obj_class == "type1")
                                    {
                                        Error1++;
                                        contDefect++;
                                        totalNumberBristlesM1type1.Text = Error1.ToString();
                                    }
                                    else if (d.obj_class == "type2")
                                    {
                                        Error2++;
                                        contDefect++;
                                        totalNumberBristlesM1type2.Text = Error2.ToString();
                                    }
                                    else if (d.obj_class == "type3" || d.obj_class == "reb")
                                    {
                                        Error3++;
                                        contDefect++;
                                        totalNumberBristlesM1type3.Text = Error3.ToString();
                                    }
                                    else if (d.obj_class == "discard")
                                    {
                                        discard++;
                                        totalNumberBristlesM1discard.Text = discard.ToString();
                                    }
                                    break;
                                case "M2":
                                    if (d.obj_class == "type1")
                                    {
                                        Error1++;
                                        contDefect++;
                                        totalNumberBristlesM2type1.Text = Error1.ToString();
                                    }
                                    else if (d.obj_class == "type2")
                                    {
                                        Error2++;
                                        contDefect++;
                                        totalNumberBristlesM2type2.Text = Error2.ToString();
                                    }
                                    else if (d.obj_class == "type3" || d.obj_class == "reb")
                                    {
                                        Error3++;
                                        contDefect++;
                                        totalNumberBristlesM2type3.Text = Error3.ToString();
                                    }
                                    else if (d.obj_class == "discard")
                                    {
                                        discard++;
                                        totalNumberBristlesM2discard.Text = discard.ToString();
                                    }
                                    break;
                                case "M3":
                                    if (d.obj_class == "type1")
                                    {
                                        Error1++;
                                        contDefect++;
                                        totalNumberBristlesM3type1.Text = Error1.ToString();
                                    }
                                    else if (d.obj_class == "type2")
                                    {
                                        Error2++;
                                        contDefect++;
                                        totalNumberBristlesM3type2.Text = Error2.ToString();
                                    }
                                    else if (d.obj_class == "type3" || d.obj_class == "reb")
                                    {
                                        Error3++;
                                        contDefect++;
                                        totalNumberBristlesM3type3.Text = Error3.ToString();
                                    }
                                    else if (d.obj_class == "discard")
                                    {
                                        discard++;
                                        totalNumberBristlesM3discard.Text = discard.ToString();
                                    }
                                    break;
                                case "N":
                                    if (d.obj_class == "type1")
                                    {
                                        Error1++;
                                        contDefect++;
                                        totalNumberBristlesNtype1.Text = Error1.ToString();
                                    }
                                    else if (d.obj_class == "type2")
                                    {
                                        Error2++;
                                        contDefect++;
                                        totalNumberBristlesNtype2.Text = Error2.ToString();
                                    }
                                    else if (d.obj_class == "type3" || d.obj_class == "reb")
                                    {
                                        Error3++;
                                        contDefect++;
                                        totalNumberBristlesNtype3.Text = Error3.ToString();
                                    }
                                    else if (d.obj_class == "discard")
                                    {
                                        discard++;
                                        totalNumberBristlesNdiscard.Text = discard.ToString();
                                    }
                                    break;
                            }

                            textBlock.Foreground = new SolidColorBrush(Colors.DarkRed);
                            //textBlock.Text = ((int)(d.probability * 100)).ToString() + "%" + " : " + d.obj_class;

                            if (d.obj_class == "discard")
                            {
                                textBlock.Foreground = new SolidColorBrush(Colors.Yellow);
                                textBlock.Text = "Undefined";
                            }
                            else
                            {
                                textBlock.Text = businessSystem.generalController.convertDefaultToIA(d.obj_class);
                            }
                        }
                        else
                        {
                            textBlock.Foreground = new SolidColorBrush(Colors.DarkGreen);
                            //textBlock.Text = ((int)(d.probability * 100)).ToString() + "%" + " : OK";
                            textBlock.Text = "Ok";
                        }
                        Canvas.SetLeft(textBlock, xR);
                        Canvas.SetTop(textBlock, yR);
                        canvas.Children.Add(textBlock);

                        APIVision.Models.SocketModel box = new APIVision.Models.SocketModel();

                        box.Width = (int)d.Width;
                        box.Height = (int)d.Height;
                        box.x = d.x;
                        box.y = d.y;
                        boundingBox.Add(box);
                        //Para contabilizar a porcentagem adicionada para o canvas 
                        //                  
                        APIVision.Models.SocketModel box2 = new APIVision.Models.SocketModel();
                        box2.Width = 0;
                        box2.Height = 0;
                        box2.x = 0;
                        box2.y = 0;
                        boundingBox.Add(box2);
                        //}

                        switch (positionResultManual)
                        {
                            case "T":
                                if ((businessSystem.socketModels.Count() - contDefect) <= 0)
                                {
                                    totalNumberBristlesT.Text = 0.ToString();
                                }
                                else
                                {
                                    totalNumberBristlesT.Text = (businessSystem.socketModels.Count() - contDefect - discard).ToString();
                                }

                                totalNumberBristlesTnok.Text = contDefect.ToString();
                                break;
                            case "M1":
                                if ((businessSystem.socketModels.Count() - contDefect) <= 0)
                                {
                                    totalNumberBristlesM1.Text = 0.ToString();
                                }
                                else
                                {
                                    totalNumberBristlesM1.Text = (businessSystem.socketModels.Count() - contDefect - discard).ToString();
                                }
                                totalNumberBristlesM1nok.Text = contDefect.ToString();
                                break;
                            case "M2":
                                if ((businessSystem.socketModels.Count() - contDefect) <= 0)
                                {
                                    totalNumberBristlesM2.Text = 0.ToString();
                                }
                                else
                                {
                                    totalNumberBristlesM2.Text = (businessSystem.socketModels.Count() - contDefect - discard).ToString();
                                }
                                totalNumberBristlesM2nok.Text = contDefect.ToString();
                                break;
                            case "M3":
                                if ((businessSystem.socketModels.Count() - contDefect) <= 0)
                                {
                                    totalNumberBristlesM3.Text = 0.ToString();
                                }
                                else
                                {
                                    totalNumberBristlesM3.Text = (businessSystem.socketModels.Count() - contDefect - discard).ToString();
                                }
                                totalNumberBristlesM3nok.Text = contDefect.ToString();
                                break;
                            case "N":
                                if ((businessSystem.socketModels.Count() - contDefect) <= 0)
                                {
                                    totalNumberBristlesN.Text = 0.ToString();
                                }
                                else
                                {
                                    totalNumberBristlesN.Text = (businessSystem.socketModels.Count() - contDefect - discard).ToString();
                                }
                                totalNumberBristlesNnok.Text = contDefect.ToString();
                                break;
                        }
                        countBox = canvas.Children.Count;
                        totalBristlesFound.Text = businessSystem.socketModels.Count().ToString();
                        totalDefectiveBristles.Text = contDefect.ToString();

                        canvas.UpdateLayout();
                        canvas.UpdateDefaultStyle();
                        viewSDKImage.UpdateLayout();
                    }

                    switch (positionResultManual)
                    {
                        case "T":
                            if (Error1 == 0) totalNumberBristlesTtype1.Text = "0";
                            if (Error2 == 0) totalNumberBristlesTtype2.Text = "0";
                            if (Error3 == 0) totalNumberBristlesTtype3.Text = "0";
                            if (discard == 0) totalNumberBristlesTdiscard.Text = "0";
                            if ((businessSystem.socketModels.Count() - contDefect) <= 0) totalNumberBristlesT.Text = "0";
                            if (contDefect == 0) totalNumberBristlesTnok.Text = "0";
                            break;
                        case "M1":
                            if (Error1 == 0) totalNumberBristlesM1type1.Text = "0";
                            if (Error2 == 0) totalNumberBristlesM1type2.Text = "0";
                            if (Error3 == 0) totalNumberBristlesM1type3.Text = "0";
                            if (discard == 0) totalNumberBristlesM1discard.Text = "0";
                            if ((businessSystem.socketModels.Count() - contDefect) <= 0) totalNumberBristlesM1.Text = "0";
                            if (contDefect == 0) totalNumberBristlesM1nok.Text = "0";
                            break;
                        case "M2":
                            if (Error1 == 0) totalNumberBristlesM2type1.Text = "0";
                            if (Error2 == 0) totalNumberBristlesM2type2.Text = "0";
                            if (Error3 == 0) totalNumberBristlesM2type3.Text = "0";
                            if (discard == 0) totalNumberBristlesM2discard.Text = "0";
                            if ((businessSystem.socketModels.Count() - contDefect) <= 0) totalNumberBristlesM2.Text = "0";
                            if (contDefect == 0) totalNumberBristlesM2nok.Text = "0";
                            break;
                        case "M3":
                            if (Error1 == 0) totalNumberBristlesM3type1.Text = "0";
                            if (Error2 == 0) totalNumberBristlesM3type2.Text = "0";
                            if (Error3 == 0) totalNumberBristlesM3type3.Text = "0";
                            if (discard == 0) totalNumberBristlesM3discard.Text = "0";
                            if ((businessSystem.socketModels.Count() - contDefect) <= 0) totalNumberBristlesM3.Text = "0";
                            if (contDefect == 0) totalNumberBristlesM3nok.Text = "0";
                            break;
                        case "N":
                            if (Error1 == 0) totalNumberBristlesNtype1.Text = "0";
                            if (Error2 == 0) totalNumberBristlesNtype2.Text = "0";
                            if (Error3 == 0) totalNumberBristlesNtype3.Text = "0";
                            if (discard == 0) totalNumberBristlesNdiscard.Text = "0";
                            if ((businessSystem.socketModels.Count() - contDefect) <= 0) totalNumberBristlesN.Text = "0";
                            if (contDefect == 0) totalNumberBristlesNnok.Text = "0";
                            break;
                    }
                    if (businessSystem.socketModels.Count() <= 0)
                    {
                        totalBristlesFound.Text = "0";
                    }
                    if (contDefect == 0)
                    {
                        totalDefectiveBristles.Text = "0";
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            frameHolderMouseMove = true;
        }

        private void RemoveAllBoxes_Click(object sender, RoutedEventArgs e)
        {
            canvas.Children.Clear();
            boundingBox.Clear();
        }

        private void TextBox_ContextMenuClosing(object sender, ContextMenuEventArgs e)
        {
            //Console.WriteLine("saida:" + boundingBoxSelectType);          

            if (boundingBoxSelectType == "undefined")
            {
                withdrawalOfBoundingBoxMarking();
            }

            menuBeingUsed = false;

            try
            {
                if (submenuBox.IsEnabled)
                {
                    if (!registerBox)
                    {
                        if (canvas.Children.Count > 0)
                        {
                            //canvas.Children.RemoveAt((int)canvas.Children.Count - 1);
                            //boundingBox.RemoveAt((int)canvas.Children.Count - 1);
                        }
                    }
                    registerBox = false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("{0}", ex.Message);
            }
            canvas.UpdateLayout();

            frameHolderMouseMove = true;
        }

        private void TextBox_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            menuBeingUsed = true;

            xCurrent = (int)e.CursorLeft;
            yCurrent = (int)e.CursorTop;
            writeBoundingBox();
        }

        /// <summary>
        /// Design user manual bounding boxes
        /// </summary>
        private void writeBoundingBox()
        {
            if (selectBoundingBoxDelete == -1)
            {
                int contDefectManual = 0;

                try
                {
                    if (submenuBox.IsEnabled)
                    {
                        if (canvas.Children.Count > countBox)
                        {
                            canvas.Children.RemoveAt(countBox);
                            //boundingBox.RemoveAt(countBox);
                            frameHolderMouseMove = true;
                        }
                        else
                        {
                            frameHolderMouseMove = false;
                        }

                        X = (int)xCurrent - (size_ / 2);
                        Y = (int)yCurrent - (size_ / 2);

                        if (boundingBoxSelectType == "undefined")
                        {
                            System.Windows.Shapes.Rectangle rect = new System.Windows.Shapes.Rectangle
                            {
                                Stroke = System.Windows.Media.Brushes.Yellow,
                                Opacity = 100,
                                Width = size_,
                                Height = size_
                            };
                            APIVision.Models.SocketModel box = new APIVision.Models.SocketModel();
                            box.Width = (int)size_;
                            box.Height = (int)size_;
                            box.x = X;
                            box.y = Y;
                            box.probability = -1;
                            Canvas.SetLeft(rect, X);
                            Canvas.SetTop(rect, Y);
                            canvas.Children.Add(rect);
                        }
                        else if (boundingBoxSelectType == "Ok")
                        {
                            System.Windows.Shapes.Rectangle rect = new System.Windows.Shapes.Rectangle
                            {
                                Stroke = System.Windows.Media.Brushes.Green,
                                Opacity = 100,
                                Width = size_,
                                Height = size_
                            };
                            APIVision.Models.SocketModel box = new APIVision.Models.SocketModel();
                            box.Width = (int)size_;
                            box.Height = (int)size_;
                            box.x = X;
                            box.y = Y;
                            Canvas.SetLeft(rect, X);
                            Canvas.SetTop(rect, Y);
                            canvas.Children.Add(rect);

                            //scale conversion
                            box.Width = (int)((box.Width * ProductionObject.ResolutionWidth) / w);
                            box.Height = (int)((box.Height * ProductionObject.ResolutionHeight) / h);
                            box.x = (int)((box.x * ProductionObject.ResolutionWidth) / w);
                            box.y = (int)((box.y * ProductionObject.ResolutionHeight) / h);
                            box.obj_class = businessSystem.generalController.convertIAToDefault(boundingBoxSelectType);
                            box.probability = -1;

                            TextBlock textBlock = new TextBlock();
                            textBlock.FontSize = 18;
                            textBlock.Foreground = new SolidColorBrush(Colors.DarkGreen);
                            textBlock.Text = ("Ok");
                            Canvas.SetLeft(textBlock, X);
                            Canvas.SetTop(textBlock, Y);
                            canvas.Children.Add(textBlock);

                            businessSystem.socketModels.Add(box);
                            boundingBox.Add(box);

                            totalBristlesFound.Text = businessSystem.socketModels.Count().ToString();
                            contDefectManual += Error1 + Error2 + Error3;// + discard;

                            switch (positionResultManual)
                            {
                                case "T":
                                    if ((int.Parse(totalBristlesFound.Text) - contDefectManual) <= 0)
                                    {
                                        totalNumberBristlesT.Text = 0.ToString();
                                    }
                                    else
                                    {
                                        totalNumberBristlesT.Text = (int.Parse(totalBristlesFound.Text) - contDefectManual - discard).ToString();
                                    }

                                    totalNumberBristlesTnok.Text = contDefectManual.ToString();
                                    break;
                                case "M1":
                                    if ((int.Parse(totalBristlesFound.Text) - contDefectManual) <= 0)
                                    {
                                        totalNumberBristlesM1.Text = 0.ToString();
                                    }
                                    else
                                    {
                                        totalNumberBristlesM1.Text = (int.Parse(totalBristlesFound.Text) - contDefectManual - discard).ToString();
                                    }
                                    totalNumberBristlesM1nok.Text = contDefectManual.ToString();
                                    break;
                                case "M2":
                                    if ((int.Parse(totalBristlesFound.Text) - contDefectManual) <= 0)
                                    {
                                        totalNumberBristlesM2.Text = 0.ToString();
                                    }
                                    else
                                    {
                                        totalNumberBristlesM2.Text = (int.Parse(totalBristlesFound.Text) - contDefectManual - discard).ToString();
                                    }
                                    totalNumberBristlesM2nok.Text = contDefectManual.ToString();
                                    break;
                                case "M3":
                                    if ((int.Parse(totalBristlesFound.Text) - contDefectManual) <= 0)
                                    {
                                        totalNumberBristlesM3.Text = 0.ToString();
                                    }
                                    else
                                    {
                                        totalNumberBristlesM3.Text = (int.Parse(totalBristlesFound.Text) - contDefectManual - discard).ToString();
                                    }
                                    totalNumberBristlesM3nok.Text = contDefectManual.ToString();
                                    break;
                                case "N":
                                    if ((int.Parse(totalBristlesFound.Text) - contDefectManual) <= 0)
                                    {
                                        totalNumberBristlesN.Text = 0.ToString();
                                    }
                                    else
                                    {
                                        totalNumberBristlesN.Text = (int.Parse(totalBristlesFound.Text) - contDefectManual - discard).ToString();
                                    }
                                    totalNumberBristlesNnok.Text = contDefectManual.ToString();
                                    break;
                            }

                            frameHolderMouseMove = true;
                        }
                        else
                        {
                            APIVision.Models.SocketModel box = new APIVision.Models.SocketModel();
                            if (boundingBoxSelectType != "discard")
                            {
                                System.Windows.Shapes.Rectangle rect = new System.Windows.Shapes.Rectangle
                                {
                                    Stroke = System.Windows.Media.Brushes.Red,
                                    Opacity = 100,
                                    Width = size_,
                                    Height = size_
                                };

                                box.Width = (int)size_;
                                box.Height = (int)size_;
                                box.x = X;
                                box.y = Y;
                                box.probability = -1;
                                Canvas.SetLeft(rect, X);
                                Canvas.SetTop(rect, Y);

                                canvas.Children.Add(rect);
                            }
                            else
                            {
                                System.Windows.Shapes.Rectangle rect = new System.Windows.Shapes.Rectangle
                                {
                                    Stroke = System.Windows.Media.Brushes.Yellow,
                                    Opacity = 100,
                                    Width = size_,
                                    Height = size_
                                };
                                box.Width = (int)size_;
                                box.Height = (int)size_;
                                box.x = X;
                                box.y = Y;
                                box.probability = -1;
                                Canvas.SetLeft(rect, X);
                                Canvas.SetTop(rect, Y);

                                canvas.Children.Add(rect);
                            }

                            switch (positionResultManual)
                            {
                                case "T":
                                    if (boundingBoxSelectType == "Error1")
                                    {
                                        Error1++;
                                        totalNumberBristlesTtype1.Text = Error1.ToString();
                                    }
                                    else if (boundingBoxSelectType == "Error2")
                                    {
                                        Error2++;
                                        totalNumberBristlesTtype2.Text = Error2.ToString();
                                    }
                                    else if (boundingBoxSelectType == "Error3" || boundingBoxSelectType == "reb")
                                    {
                                        Error3++;
                                        totalNumberBristlesTtype3.Text = Error3.ToString();
                                    }
                                    else if (boundingBoxSelectType == "discard")
                                    {
                                        discard++;
                                        totalNumberBristlesTdiscard.Text = discard.ToString();
                                    }
                                    break;
                                case "M1":
                                    if (boundingBoxSelectType == "Error1")
                                    {
                                        Error1++;
                                        totalNumberBristlesM1type1.Text = Error1.ToString();
                                    }
                                    else if (boundingBoxSelectType == "Error2")
                                    {
                                        Error2++;
                                        totalNumberBristlesM1type2.Text = Error2.ToString();
                                    }
                                    else if (boundingBoxSelectType == "Error3" || boundingBoxSelectType == "reb")
                                    {
                                        Error3++;
                                        totalNumberBristlesM1type3.Text = Error3.ToString();
                                    }
                                    else if (boundingBoxSelectType == "discard")
                                    {
                                        discard++;
                                        totalNumberBristlesM1discard.Text = discard.ToString();
                                    }
                                    break;
                                case "M2":
                                    if (boundingBoxSelectType == "Error1")
                                    {
                                        Error1++;
                                        totalNumberBristlesM2type1.Text = Error1.ToString();
                                    }
                                    else if (boundingBoxSelectType == "Error2")
                                    {
                                        Error2++;
                                        totalNumberBristlesM2type2.Text = Error2.ToString();
                                    }
                                    else if (boundingBoxSelectType == "Error3" || boundingBoxSelectType == "reb")
                                    {
                                        Error3++;
                                        totalNumberBristlesM2type3.Text = Error3.ToString();
                                    }
                                    else if (boundingBoxSelectType == "discard")
                                    {
                                        discard++;
                                        totalNumberBristlesM2discard.Text = discard.ToString();
                                    }
                                    break;
                                case "M3":
                                    if (boundingBoxSelectType == "Error1")
                                    {
                                        Error1++;
                                        totalNumberBristlesM3type1.Text = Error1.ToString();
                                    }
                                    else if (boundingBoxSelectType == "Error2")
                                    {
                                        Error2++;
                                        totalNumberBristlesM3type2.Text = Error2.ToString();
                                    }
                                    else if (boundingBoxSelectType == "Error3" || boundingBoxSelectType == "reb")
                                    {
                                        Error3++;
                                        totalNumberBristlesM3type3.Text = Error3.ToString();
                                    }
                                    else if (boundingBoxSelectType == "discard")
                                    {
                                        discard++;
                                        totalNumberBristlesM3discard.Text = discard.ToString();
                                    }
                                    break;
                                case "N":
                                    if (boundingBoxSelectType == "Error1")
                                    {
                                        Error1++;
                                        totalNumberBristlesNtype1.Text = Error1.ToString();
                                    }
                                    else if (boundingBoxSelectType == "Error2")
                                    {
                                        Error2++;
                                        totalNumberBristlesNtype2.Text = Error2.ToString();
                                    }
                                    else if (boundingBoxSelectType == "Error3" || boundingBoxSelectType == "reb")
                                    {
                                        Error3++;
                                        totalNumberBristlesNtype3.Text = Error3.ToString();
                                    }
                                    else if (boundingBoxSelectType == "discard")
                                    {
                                        discard++;
                                        totalNumberBristlesNdiscard.Text = discard.ToString();
                                    }
                                    break;
                            }
                            contDefectManual += Error1 + Error2 + Error3;

                            switch (positionResultManual)
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
                            box.Width = (int)((box.Width * ProductionObject.ResolutionWidth) / w);
                            box.Height = (int)((box.Height * ProductionObject.ResolutionHeight) / h);
                            box.x = (int)((box.x * ProductionObject.ResolutionWidth) / w);
                            box.y = (int)((box.y * ProductionObject.ResolutionHeight) / h);
                            box.obj_class = businessSystem.generalController.convertIAToDefault(boundingBoxSelectType);
                            box.probability = -1;

                            if (boundingBoxSelectType != "discard")
                            {
                                TextBlock textBlock = new TextBlock();
                                textBlock.FontSize = 18;
                                textBlock.Foreground = new SolidColorBrush(Colors.DarkRed);
                                textBlock.Text = boundingBoxSelectType;
                                Canvas.SetLeft(textBlock, X);
                                Canvas.SetTop(textBlock, Y);
                                canvas.Children.Add(textBlock);
                            }
                            else
                            {
                                TextBlock textBlock = new TextBlock();
                                textBlock.FontSize = 18;
                                textBlock.Foreground = new SolidColorBrush(Colors.Yellow);
                                textBlock.Text = "Undefined";
                                Canvas.SetLeft(textBlock, X);
                                Canvas.SetTop(textBlock, Y);
                                canvas.Children.Add(textBlock);
                            }

                            businessSystem.socketModels.Add(box);
                            boundingBox.Add(box);

                            totalBristlesFound.Text = businessSystem.socketModels.Count().ToString();
                            totalDefectiveBristles.Text = (int.Parse(totalDefectiveBristles.Text) + 1).ToString(); //discutir 

                            frameHolderMouseMove = true;
                        }
                    }
                    boundingBoxSelectType = "undefined";
                }
                catch
                {

                }
            }

            frameHolderMouseMove = true;
        }

        private void saveCutImage(string classification)
        {

        }

        public Bitmap CropImage(Bitmap source, Rectangle section)
        {
            var bitmap = new Bitmap(section.Width, section.Height);
            using (var g = Graphics.FromImage(bitmap))
            {
                g.DrawImage(source, 0, 0, section, GraphicsUnit.Pixel);
                return bitmap;
            }
        }

        private void Cancelar_Click(object sender, RoutedEventArgs e)
        {
            withdrawalOfBoundingBoxMarking();
        }

        private void withdrawalOfBoundingBoxMarking()
        {
            try
            {
                if (selectBoundingBoxDelete == -1)
                {
                    if (canvas.Children.Count > countBox)
                    {
                        frameHolderMouseMove = false;
                        canvas.Children.RemoveAt(countBox);
                        boundingBox.RemoveAt((int)canvas.Children.Count - 1);
                    }
                }

                frameHolderMouseMove = true;
            }
            catch
            {
            }
        }


        private void newDefect_Click(object sender, RoutedEventArgs e)
        {
            //saveCutImage();
            //registerBox = true;
            //countBox++;
        }

        private void property_Click(object sender, RoutedEventArgs e)
        {

        }

        private void size1_Click(object sender, RoutedEventArgs e)
        {
            size_ = 85;
        }

        private void size2_Click(object sender, RoutedEventArgs e)
        {
            size_ = 80;
        }

        private void size3_Click(object sender, RoutedEventArgs e)
        {
            size_ = 75;
        }

        private void size4_Click(object sender, RoutedEventArgs e)
        {
            size_ = 70;
        }

        private void size5_Click(object sender, RoutedEventArgs e)
        {
            size_ = 65;
        }

        private void color1_Click(object sender, RoutedEventArgs e)
        {

        }

        private void color2_Click(object sender, RoutedEventArgs e)
        {

        }

        private void color3_Click(object sender, RoutedEventArgs e)
        {

        }

        private void stopAnalysis_Click(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// Save the analysis results to the database.
        /// </summary>
        private void saveResultDatabase()
        {
            businessSystem.tuffAnalysisResultModel.TotalBristlesFoundNN = 0;
            businessSystem.tuffAnalysisResultModel.TotalBristleFoundManual = 0;

            for (int i = 0; i < businessSystem.socketModels.Count; i++)
            {
                businessSystem.bristleAnalysisResultModel = new APIVision.Models.BristleAnalysisResultModel();
                //businessSystem.bristleAnalysisResultModel.Probability = (int)slValue.Value;
                if (businessSystem.socketModels[i].obj_class == "discard")
                {
                    businessSystem.bristleAnalysisResultModel.DefectClassification = "discard";
                }
                else if (businessSystem.socketModels[i].obj_class == "none")
                {
                    businessSystem.bristleAnalysisResultModel.DefectClassification = "Ok";
                }
                else
                {
                    //converter names
                    if (businessSystem.socketModels[i].obj_class == "type1")
                    {
                        businessSystem.bristleAnalysisResultModel.DefectClassification = "Error1";
                    }
                    else if (businessSystem.socketModels[i].obj_class == "type2")
                    {
                        businessSystem.bristleAnalysisResultModel.DefectClassification = "Error2";
                    }
                    else if (businessSystem.socketModels[i].obj_class == "type3" || businessSystem.socketModels[i].obj_class == "reb")
                    {
                        businessSystem.bristleAnalysisResultModel.DefectClassification = "Error3";
                    }
                }
                if (businessSystem.bristleAnalysisResultModel.DefectClassification != "Ok")
                {
                    businessSystem.bristleAnalysisResultModel.DefectIdentified = "yes";
                }
                else
                {
                    businessSystem.bristleAnalysisResultModel.DefectIdentified = "No";
                    businessSystem.brushAnalysisResultModel.TotalGoodBristles++;
                }
                businessSystem.brushAnalysisResultModel.TotalBristles++;

                businessSystem.bristleAnalysisResultModel.X = businessSystem.socketModels[i].x;
                businessSystem.bristleAnalysisResultModel.Y = businessSystem.socketModels[i].y;
                businessSystem.bristleAnalysisResultModel.Width = businessSystem.socketModels[i].Height;
                businessSystem.bristleAnalysisResultModel.Height = businessSystem.socketModels[i].Width;

                if (businessSystem.socketModels[i].probability == -1)
                {
                    businessSystem.bristleAnalysisResultModel.SelectedManual = true;
                    businessSystem.tuffAnalysisResultModel.SelectedManual = true;
                    businessSystem.tuffAnalysisResultModel.TotalBristleFoundManual++;
                }
                else
                {
                    businessSystem.bristleAnalysisResultModel.SelectedManual = false;
                    businessSystem.tuffAnalysisResultModel.TotalBristlesFoundNN++;
                }
                businessSystem.bristleAnalysisResultModel.Position = position;
                businessSystem.bristleAnalysisResultModels.Add(businessSystem.bristleAnalysisResultModel);
            }
            businessSystem.tuffAnalysisResultModel.Position = position;
            businessSystem.tuffAnalysisResultModel.Probability = "95";//slValue_view.Text;
            businessSystem.dataBaseController.updateTuffAnalysisResultModel(businessSystem.tuffAnalysisResultModel, null, null);
            businessSystem.dataBaseController.updateBristleAnalysisResultModel(businessSystem.bristleAnalysisResultModels, null, null);
            businessSystem.bristleAnalysisResultModels.Clear();
        }

        /// <summary>
        /// Saves image in the database for later analysis by the curator.
        /// </summary>
        private void saveImageToDatabase()
        {
            try
            {
                int cont = 0;
                if ((boundingBox.Count - (defect.Count) < boundingBox.Count) || businessSystem.boundingBoxDiscards.Count > 0 || boundingBoxEdit)
                {
                    businessSystem.registrationWaitingModel.AnalyzeSet_id = analyzedId.ToString();
                    businessSystem.registrationWaitingModel.DataSet_id = "0";
                    businessSystem.registrationWaitingModel.Sample_id = "0";
                    businessSystem.dataBaseController.updateRegistrationWaitingModel(businessSystem.registrationWaitingModel, null, null);

                    Bitmap source = (Bitmap)photo_.Clone();
                    string name = businessSystem.generalController.formatTimesTemp("image", DateTime.Now.ToString());
                    string nameExt = name + ".png";
                    string nameExtB = "\\" + nameExt;
                    string nameAll = @businessSystem.generalSettings.NamePrefix + nameExtB;
                    ProductionObject.dinoLiteSDK.SaveFrame(nameAll);
                    businessSystem.tuftTempModel.Position = position;
                    businessSystem.dataBaseController.updateTuftTempModel(businessSystem.tuftTempModel, null, null);
                    businessSystem.imageTempModel.Path = nameExt + "@" + reason.Text;
                    businessSystem.dataBaseController.updateImageTempModel(businessSystem.imageTempModel, null, null);

                    boundingBoxEdit = false;
                }
                else
                {
                    Bitmap source = (Bitmap)photo_.Clone();
                    string name = businessSystem.generalController.formatTimesTemp("image", DateTime.Now.ToString());
                    string nameExt = name + ".png";
                    string nameExtB = "\\" + nameExt;
                    string nameAll = @businessSystem.generalSettings.NamePrefix + nameExtB;
                    ProductionObject.dinoLiteSDK.SaveFrame(nameAll);
                    businessSystem.registrationWaitingModel.AnalyzeSet_id = analyzedId.ToString();
                    businessSystem.registrationWaitingModel.DataSet_id = "0";
                    businessSystem.registrationWaitingModel.Sample_id = "0";
                    businessSystem.dataBaseController.updateRegistrationWaitingModel(businessSystem.registrationWaitingModel, null, null);
                    businessSystem.tuftTempModel.Position = position;
                    businessSystem.dataBaseController.updateTuftTempModel(businessSystem.tuftTempModel, null, null);
                    businessSystem.imageTempModel.Path = nameExt + "@" + reason.Text;
                    businessSystem.dataBaseController.updateImageTempModel(businessSystem.imageTempModel, null, null);

                    boundingBoxEdit = false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERRO: {0}", ex.Message);
            }
        }

        /// <summary>
        /// Configures the system for the next tuft
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void nextAnalysis_Click(object sender, RoutedEventArgs e)
        {
            if(liveOn_)
            {
                if ((int.Parse(totalNumberBristlesTdiscard.Text) + int.Parse(totalNumberBristlesM1discard.Text) +
            int.Parse(totalNumberBristlesM2discard.Text) + int.Parse(totalNumberBristlesM3discard.Text) +
            int.Parse(totalNumberBristlesNdiscard.Text)) != 0)
                {
                    warningText2.Visibility = Visibility.Collapsed;
                    warningButton2.Visibility = Visibility.Collapsed;
                    warningText3.Visibility = Visibility.Visible;
                    warningButton3.Visibility = Visibility.Visible;
                    modalL.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 218, 140, 16));
                    modalR.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 218, 140, 16));
                    warning.Visibility = Visibility.Visible;
                }
                else
                {
                    warningText3.Visibility = Visibility.Collapsed;
                    warningButton3.Visibility = Visibility.Collapsed;
                    warningText2.Visibility = Visibility.Visible;
                    warningButton2.Visibility = Visibility.Visible;
                    modalL.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 188, 64, 49));
                    modalR.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 188, 64, 49));

                    traceIcon("Play");
                    GridMenuRight.Visibility = Visibility.Collapsed;
                    imageCount = 0;

                    SelectAnalysis++;

                    switch (SelectAnalysis)
                    {
                        case 1:
                            warning.Visibility = Visibility.Visible;
                            businessSystem.tuffAnalysisResultModel.SelectedManual = false;
                            result_view.Visibility = Visibility.Collapsed;
                            startAnalyse2_view.Visibility = Visibility.Visible;
                            submenuBox.IsEnabled = false;
                            captureImage_ = false;
                            liveConditions(true);
                            camera.Kind = MaterialDesignThemes.Wpf.PackIconKind.Camera;
                            cameraStartAnalysis.Kind = MaterialDesignThemes.Wpf.PackIconKind.Camera;
                            position = "T";
                            positionResultManual = "M1";
                            saveResultDatabase();
                            saveResult("T");
                            businessSystem.socketModels.Clear();
                            canvas.Children.Clear();
                            boundingBox.Clear();
                            totalBristlesFound.Text = "0";
                            totalDefectiveBristles.Text = "0";

                            break;
                        case 2:
                            warning.Visibility = Visibility.Visible;
                            businessSystem.tuffAnalysisResultModel.SelectedManual = false;
                            canvas.Children.Clear();
                            result_view.Visibility = Visibility.Collapsed;
                            startAnalyse3_view.Visibility = Visibility.Visible;
                            captureImage_ = false;
                            liveConditions(true);
                            position = "M1";
                            positionResultManual = "M2";
                            saveResultDatabase();
                            saveResult("M1");
                            businessSystem.socketModels.Clear();
                            canvas.Children.Clear();
                            boundingBox.Clear();
                            totalBristlesFound.Text = "0";
                            totalDefectiveBristles.Text = "0";
                            break;
                        case 3:
                            warning.Visibility = Visibility.Visible;
                            businessSystem.tuffAnalysisResultModel.SelectedManual = false;
                            canvas.Children.Clear();
                            result_view.Visibility = Visibility.Collapsed;
                            startAnalyse4_view.Visibility = Visibility.Visible;
                            captureImage_ = false;
                            liveConditions(true);
                            camera.Kind = MaterialDesignThemes.Wpf.PackIconKind.Camera;
                            cameraStartAnalysis.Kind = MaterialDesignThemes.Wpf.PackIconKind.Camera;
                            position = "M2";
                            positionResultManual = "M3";
                            saveResultDatabase();
                            saveResult("M2");
                            businessSystem.socketModels.Clear();
                            canvas.Children.Clear();
                            boundingBox.Clear();
                            totalBristlesFound.Text = "0";
                            totalDefectiveBristles.Text = "0";
                            break;
                        case 4:
                            warning.Visibility = Visibility.Visible;
                            businessSystem.tuffAnalysisResultModel.SelectedManual = false;
                            canvas.Children.Clear();
                            result_view.Visibility = Visibility.Collapsed;
                            startAnalyse5_view.Visibility = Visibility.Visible;
                            captureImage_ = false;
                            liveConditions(true);
                            camera.Kind = MaterialDesignThemes.Wpf.PackIconKind.Camera;
                            cameraStartAnalysis.Kind = MaterialDesignThemes.Wpf.PackIconKind.Camera;
                            position = "M3";
                            positionResultManual = "N";
                            saveResultDatabase();
                            saveResult("M3");
                            businessSystem.socketModels.Clear();
                            canvas.Children.Clear();
                            boundingBox.Clear();
                            totalBristlesFound.Text = "0";
                            totalDefectiveBristles.Text = "0";
                            break;
                        case 5:
                            warning.Visibility = Visibility.Visible;
                            businessSystem.tuffAnalysisResultModel.SelectedManual = false;
                            canvas.Children.Clear();
                            //result_view.Visibility = Visibility.Collapsed;
                            startAnalyse5_view.Visibility = Visibility.Collapsed;
                            next.Content = "Result";
                            SelectAnalysis = 0;
                            captureImage_ = false;
                            liveConditions(true);
                            camera.Kind = MaterialDesignThemes.Wpf.PackIconKind.Camera;
                            cameraStartAnalysis.Kind = MaterialDesignThemes.Wpf.PackIconKind.Camera;
                            position = "N";
                            saveResultDatabase();
                            saveResult("N");
                            businessSystem.socketModels.Clear();
                            canvas.Children.Clear();
                            boundingBox.Clear();
                            totalBristlesFound.Text = "0";
                            totalDefectiveBristles.Text = "0";
                            break;
                    }
                }
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
        private void saveResult(string position)
        {
            if (position == "N")
            {
                if (int.Parse(totalNumberBristlesT.Text) != 0 || int.Parse(totalNumberBristlesM1.Text) != 0 || int.Parse(totalNumberBristlesM2.Text) != 0
               || int.Parse(totalNumberBristlesM3.Text) != 0 || int.Parse(totalNumberBristlesN.Text) != 0)
                {
                    businessSystem.brushAnalysisResultModel.Hybrid = true;
                }
                else
                {
                    businessSystem.brushAnalysisResultModel.Hybrid = false;
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

                int result = (businessSystem.brushAnalysisResultModel.TotalGoodBristles * 100) / total;

                if (result >= 95)
                {
                    businessSystem.brushAnalysisResultModel.AnalysisResult = "APPROVED";
                    businessSystem.brushAnalysisResultModel.Signaling_Id = 1;
                }
                else if (result >= 90 && result < 95)
                {
                    businessSystem.brushAnalysisResultModel.AnalysisResult = "ALERT";
                    businessSystem.brushAnalysisResultModel.Signaling_Id = 2;
                }
                else
                {
                    businessSystem.brushAnalysisResultModel.AnalysisResult = "NOT APPROVED";
                    businessSystem.brushAnalysisResultModel.Signaling_Id = 3;
                }

                businessSystem.dataBaseController.updateBrushAnalysisResultModel(businessSystem.brushAnalysisResultModel, null, null);
            }
        }

        private void sliderMove(object sender, MouseEventArgs e)
        {
            //slValue_view.Text = slValue.Value.ToString();
            Thread.Sleep(10);
        }

        private void agree_Click(object sender, RoutedEventArgs e)
        {

        }

        private void disagree_Click(object sender, RoutedEventArgs e)
        {

        }

        private void editValue_Click(object sender, RoutedEventArgs e)
        {
            if (totalBristlesFound.IsReadOnly)
            {
                totalBristlesFound.IsReadOnly = false;
            }
            else
            {
                totalBristlesFound.IsReadOnly = true;
            }
        }

        private void SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int idSKU = 0;
            string area = "";
            foreach (var item in businessSystem.sKU1Model)
            {
                if (skuSelect.SelectedItem.ToString() == item.sSKU)
                {
                    idSKU = item.iID;
                    area = item.iArea_id;
                }
            }

            testSelect.Items.Clear();
            //testSelect.Items.Add("Test Select");
            testSelect.Items.Add("Arredondamento");
            businessSystem.testModels = businessSystem.dataBaseController.listTestModel(idSKU);
            foreach (var item in businessSystem.testModels)
            {
                //testSelect.Items.Add(item.sDescription);
                if (item.sDescription == "Arredondamento")
                {
                    businessSystem.generalSettings.IdTest_1 = item.Id;
                }
                else if (item.sDescription == "NFiosSpec1")
                {
                    businessSystem.generalSettings.IdTest_2 = item.Id;
                }
            }
            testSelect.SelectedIndex = 0;

            areaSelect.Items.Clear();
            areaSelect.Items.Add("Area Select");
            areaSelect.Items.Add(area);
            areaSelect.SelectedIndex = 0;

            if (areaSelect.Items.Count > 1)
            {
                areaSelect.SelectedIndex = 2;
            }

            equipamentSelect.Items.Clear();
            equipamentSelect.Items.Add("Equipment Select");
            foreach (var item in businessSystem.dataBaseController.listEquipmentModel(area))
            {
                equipamentSelect.Items.Add(item.iEquipment_id);
            }
            equipamentSelect.SelectedIndex = 0;

            if (idSKU > 0)
            {
                var listTests = businessSystem.testModels.Where(test => (test
                                                                            .sDescription.Contains("NFiosSpec") && test.iSKU == idSKU));

                var listspecs = businessSystem.dataBaseController
                                    .listQM_SpecModel();

                var nominalNBristles = listspecs.Where(specModel => (specModel.iTest_id == listTests.FirstOrDefault().Id)).FirstOrDefault().fTarget * 2;

                ProductionObject.nominalBristle = Convert.ToInt32(nominalNBristles);

                nominalNBristle.Text = ProductionObject.nominalBristle.ToString();
            }
        }

        /// <summary>
        /// Add 1 bounding box of type 1
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void error_1_Click(object sender, RoutedEventArgs e)
        {
            if (selectBoundingBoxDelete == -1)
            {
                defect.Add("Error1");
                registerBox = true;
                countBox += 2;
                boundingBoxSelectType = "Error1";
                canvas.Children.RemoveAt(canvas.Children.Count - 1);
                writeBoundingBox();
            }
            else
            {
                boundingBoxSelectType = "Error1";
                changeClass();
            }
        }

        /// <summary>
        /// add 2 bounding box of type 2
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void error_2_Click(object sender, RoutedEventArgs e)
        {
            if (selectBoundingBoxDelete == -1)
            {
                defect.Add("Error2");
                registerBox = true;
                countBox += 2;
                boundingBoxSelectType = "Error2";
                canvas.Children.RemoveAt(canvas.Children.Count - 1);
                writeBoundingBox();
            }
            else
            {
                boundingBoxSelectType = "Error2";
                changeClass();
            }
        }

        /// <summary>
        /// Add 3 bounding box of type 3
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void error_3_Click(object sender, RoutedEventArgs e)
        {
            if (selectBoundingBoxDelete == -1)
            {
                defect.Add("Error3");
                registerBox = true;
                countBox += 2;
                boundingBoxSelectType = "Error3";
                canvas.Children.RemoveAt(canvas.Children.Count - 1);
                writeBoundingBox();
            }
            else
            {
                boundingBoxSelectType = "Error3";
                changeClass();
            }
        }

        /// <summary>
        /// Add 1 bounding box of type undefined
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void undefined_Click(object sender, RoutedEventArgs e)
        {
            if (selectBoundingBoxDelete == -1)
            {
                defect.Add("discard");
                registerBox = true;
                countBox += 2;
                boundingBoxSelectType = "discard";
                canvas.Children.RemoveAt(canvas.Children.Count - 1);
                writeBoundingBox();
            }
            else
            {
                boundingBoxSelectType = "discard";
                changeClass();
            }
        }

        /// <summary>
        /// Add 3 bounding box of type none
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void none_Click(object sender, RoutedEventArgs e)
        {
            if (selectBoundingBoxDelete == -1)
            {
                defect.Add("Ok");
                registerBox = true;
                countBox += 2;
                boundingBoxSelectType = "Ok";
                canvas.Children.RemoveAt(canvas.Children.Count - 1);
                writeBoundingBox();
            }
            else
            {
                boundingBoxSelectType = "Ok";
                changeClass();
            }
        }

        /// <summary>
        /// Add 3 bounding box of type reb
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void reb_Click(object sender, RoutedEventArgs e)
        {
            if (selectBoundingBoxDelete == -1)
            {
                defect.Add("reb");
                registerBox = true;
                countBox += 2;
                boundingBoxSelectType = "reb";
                canvas.Children.RemoveAt(canvas.Children.Count - 1);
                writeBoundingBox();
            }
            else
            {
                boundingBoxSelectType = "reb";
                changeClass();
            }
        }

        private void changeClass()
        {
            if (numberMask == -1)
            {
                if (selectBoundingBoxDelete > -1)
                {
                    try
                    {
                        businessSystem.socketModel = businessSystem.socketModels[selectBoundingBoxDelete - 1];
                        businessSystem.socketModels.RemoveAt(selectBoundingBoxDelete - 1);
                        businessSystem.socketModel.obj_class = businessSystem.generalController.convertIAToDefault(boundingBoxSelectType);
                        businessSystem.socketModel.probability = -1;
                        businessSystem.socketModels.Add(businessSystem.socketModel);
                        drawBoundingBox();
                        boundingBoxSelect = 0;
                        selectBoundingBoxDelete = -1;
                        boundingBoxSelectType = "undefined";
                    }
                    catch 
                    {
                    }                 
                }
            }
        }

        private void analysisFinish_Click(object sender, RoutedEventArgs e)
        {
            if ((int.Parse(totalNumberBristlesTdiscard.Text) + int.Parse(totalNumberBristlesM1discard.Text) +
              int.Parse(totalNumberBristlesM2discard.Text) + int.Parse(totalNumberBristlesM3discard.Text) +
              int.Parse(totalNumberBristlesNdiscard.Text)) != 0)
            {
                warningText2.Visibility = Visibility.Collapsed;
                warningButton2.Visibility = Visibility.Collapsed;
                warningText3.Visibility = Visibility.Visible;
                warningButton3.Visibility = Visibility.Visible;
                modalL.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 218, 140, 16));
                modalR.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 218, 140, 16));
                warning.Visibility = Visibility.Visible;
            }
            else
            {
                warningText3.Visibility = Visibility.Collapsed;
                warningButton3.Visibility = Visibility.Collapsed;
                warningText2.Visibility = Visibility.Visible;
                warningButton2.Visibility = Visibility.Visible;
                modalL.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 188, 64, 49));
                modalR.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 188, 64, 49));

                //traceIcon("Play");
                //GridMenuRight.Visibility = Visibility.Collapsed;
            }
        }

        private void CameraCalibration_Click(object sender, RoutedEventArgs e)
        {
            live = false;

            Views.CameraCalibration cameraCalibration = new Views.CameraCalibration(maximized, businessSystem);
            cameraCalibration.Show();
            this.Hide();
        }

        private void password_Click(object sender, RoutedEventArgs e)
        {            
            if (businessSystem.userSystemCurrent.Type == "administrator")
            {
                Views.Password password = new Views.Password(maximized, businessSystem, this);
                password.Show();
                this.Hide();
            }
            else
            {
                //Validate the group that can access this part 
                foreach (var group in businessSystem.networkUserModel.NetworkUserGroup)
                {
                    if (group == businessSystem.AD_Admin || group == businessSystem.AD_Quality)
                    {
                        live = false;

                        Views.Password password = new Views.Password(maximized, businessSystem, this);
                        password.Show();
                        this.Hide();
                    }
                }
                MessageBox.Show("Necessary administrative rights!");
            }
        }

        private void user_Click(object sender, RoutedEventArgs e)
        {
            if (businessSystem.userSystemCurrent.Type == "administrator")
            {
                Views.User user = new Views.User(maximized, businessSystem, this);
                user.Show();
                this.Hide();
            }
            else
            {
                //Validate the group that can access this part 
                foreach (var group in businessSystem.networkUserModel.NetworkUserGroup)
                {
                    if (group == businessSystem.AD_Admin || group == businessSystem.AD_Quality)
                    {
                        live = false;

                        Views.User user = new Views.User(maximized, businessSystem, this);
                        user.Show();
                        this.Hide();
                    }
                }
                MessageBox.Show("Necessary administrative rights!");
            }
        }

        /// <summary>
        /// calculator
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void sete_Click(object sender, RoutedEventArgs e)
        {
            if (sum || subtraction || multiplication || division)
            {
                sum = false;
                subtraction = false;
                multiplication = false;
                division = false;
                display.Text = "";
            }
            display.Text += "7";
        }

        /// <summary>
        /// calculator
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void zero_Click(object sender, RoutedEventArgs e)
        {
            if (sum || subtraction || multiplication || division)
            {
                sum = false;
                subtraction = false;
                multiplication = false;
                division = false;
                display.Text = "";
            }
            display.Text += "0";
        }

        /// <summary>
        /// calculator
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void um_Click(object sender, RoutedEventArgs e)
        {
            if (sum || subtraction || multiplication || division)
            {
                sum = false;
                subtraction = false;
                multiplication = false;
                division = false;
                display.Text = "";
            }
            display.Text += "1";
        }

        /// <summary>
        /// calculator
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dois_Click(object sender, RoutedEventArgs e)
        {
            if (sum || subtraction || multiplication || division)
            {
                sum = false;
                subtraction = false;
                multiplication = false;
                division = false;
                display.Text = "";
            }
            display.Text += "2";
        }

        /// <summary>
        /// calculator
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tres_Click(object sender, RoutedEventArgs e)
        {
            if (sum || subtraction || multiplication || division)
            {
                sum = false;
                subtraction = false;
                multiplication = false;
                division = false;
                display.Text = "";
            }
            display.Text += "3";
        }

        /// <summary>
        /// calculator
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void quatro_Click(object sender, RoutedEventArgs e)
        {
            if (sum || subtraction || multiplication || division)
            {
                sum = false;
                subtraction = false;
                multiplication = false;
                division = false;
                display.Text = "";
            }
            display.Text += "4";
        }

        /// <summary>
        /// calculator
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cinco_Click(object sender, RoutedEventArgs e)
        {
            if (sum || subtraction || multiplication || division)
            {
                sum = false;
                subtraction = false;
                multiplication = false;
                division = false;
                display.Text = "";
            }
            display.Text += "5";
        }

        /// <summary>
        /// calculator
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void seis_Click(object sender, RoutedEventArgs e)
        {
            if (sum || subtraction || multiplication || division)
            {
                sum = false;
                subtraction = false;
                multiplication = false;
                division = false;
                display.Text = "";
            }
            display.Text += "6";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void oito_Click(object sender, RoutedEventArgs e)
        {
            if (sum || subtraction || multiplication || division)
            {
                sum = false;
                subtraction = false;
                multiplication = false;
                division = false;
                display.Text = "";
            }
            display.Text += "8";
        }

        /// <summary>
        /// calculator
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void nove_Click(object sender, RoutedEventArgs e)
        {
            if (sum || subtraction || multiplication || division)
            {
                sum = false;
                subtraction = false;
                multiplication = false;
                division = false;
                display.Text = "";
            }
            display.Text += "9";
        }

        /// <summary>
        /// calculator add
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void add_Click(object sender, RoutedEventArgs e)
        {

            if (operacao == "*" || operacao == "-" || operacao == "/")
            {
                operacao = "+";
            }
            else
            {
                acumula += double.Parse(display.Text);
                division = true;
                operacao = "+";
            }

        }

        /// <summary>
        /// calculator equal
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void equal_Click(object sender, RoutedEventArgs e)
        {
            if (operacao == "+")
            {
                acumula += double.Parse(display.Text);
                display.Text = acumula.ToString();
            }
            else if (operacao == "-")
            {
                acumula -= double.Parse(display.Text);
                display.Text = acumula.ToString();
            }
            else if (operacao == "*")
            {
                acumula *= double.Parse(display.Text);
                display.Text = acumula.ToString();
            }
            else if (operacao == "/")
            {
                if (double.Parse(display.Text) != 0)
                {
                    acumula /= double.Parse(display.Text);
                    display.Text = acumula.ToString();
                }
                else
                {
                    display.Text = "Dividindo por zero";
                }
            }
        }

        /// <summary>
        /// calculator 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comma_Click(object sender, RoutedEventArgs e)
        {
            display.Text += ",";
        }

        /// <summary>
        /// calculator subtraction
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void subtraction_Click(object sender, RoutedEventArgs e)
        {
            if (operacao == "*" || operacao == "+" || operacao == "/")
            {
                operacao = "-";
            }
            else
            {
                acumula = double.Parse(display.Text);
                subtraction = true;
                operacao = "-";
            }
        }

        /// <summary>
        /// calculator multiplication
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void multiplication_Click(object sender, RoutedEventArgs e)
        {
            if (operacao == "-" || operacao == "+" || operacao == "/")
            {
                operacao = "*";
            }
            else
            {
                acumula = double.Parse(display.Text);
                multiplication = true;
                operacao = "*";
            }
        }

        /// <summary>
        /// calculator division
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void division_Click(object sender, RoutedEventArgs e)
        {
            if (operacao == "*" || operacao == "+" || operacao == "-")
            {
                operacao = "/";
            }
            else
            {
                acumula = double.Parse(display.Text);
                division = true;
                operacao = "/";
            }
        }

        /// <summary>
        /// calculator
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void return_Click(object sender, RoutedEventArgs e)
        {
            int x = display.Text.Length - 1;
            if (x >= 0)
            {
                display.Text = display.Text.Substring(0, x);
            }
        }

        /// <summary>
        /// calculator
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void inverso_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                double d = double.Parse(display.Text);
                if (d == 0)
                {
                    display.Text = "Dividindo por zero!!!";
                }
                else
                {
                    d = 1 / d;
                    display.Text = d.ToString();
                }
            }
            catch (Exception Ex)
            {

                for (int i = 0; i < 100000; i++)
                {
                    display.Text = "";
                    // em espera.
                }
                display.Text = "";

            }
        }

        /// <summary>
        /// calculator
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void source_Click(object sender, RoutedEventArgs e)
        {
            double x = double.Parse(display.Text);
            if (x < 0)
            {
                display.Text = "Este valor não é valido.";
            }
            else
            {
                x = Math.Sqrt(x);
                display.Text = x.ToString();
            }

        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            acumula = 0;
            display.Text = "";
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            display.Text = "";
            operacao = "";
        }

        private void maisoumenos_Click(object sender, RoutedEventArgs e)
        {
            double x = double.Parse(display.Text) * (-1);
            display.Text = x.ToString();
        }

        private void percente_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void buttonCalculator_Click(object sender, RoutedEventArgs e)
        {
            if (calculator.Visibility == Visibility.Visible)
            {
                calculator.Visibility = Visibility.Collapsed;
            }
            else
            {
                calculator.Visibility = Visibility.Visible;
            }
        }

        private void calc(object sender, MouseButtonEventArgs e)
        {
            if (calculator.Visibility == Visibility.Visible)
            {
                calculator.Visibility = Visibility.Collapsed;
            }
            else
            {
                calculator.Visibility = Visibility.Visible;
            }
        }

        private void buttonCameraConfiguration1_Click(object sender, RoutedEventArgs e)
        {
            cameraCalibrationClose();
            cameraConfiguration1.Visibility = Visibility.Visible;
        }

        private void buttonCameraConfiguration2_Click(object sender, RoutedEventArgs e)
        {
            cameraCalibrationClose();
            cameraConfiguration2.Visibility = Visibility.Visible;
        }

        private void buttonCameraConfiguration3_Click(object sender, RoutedEventArgs e)
        {
            cameraCalibrationClose();
            cameraConfiguration3.Visibility = Visibility.Visible;
        }

        private void buttonCameraConfiguration4_Click(object sender, RoutedEventArgs e)
        {
            cameraCalibrationClose();
            cameraConfiguration4.Visibility = Visibility.Visible;
        }

        private void buttonCameraConfiguration5_Click(object sender, RoutedEventArgs e)
        {
            cameraCalibrationClose();

            //focus++;
            //ProductionObject.dinoLiteSDK.SetLensPos(ProductionObject.VideoDeviceIndex, 42);
            //ProductionObject.dinoLiteSDK.SetLensInitPos(0);
            //MessageBox.Show(ProductionObject.dinoLiteSDK.GetLensInitPos(ProductionObject.VideoDeviceIndex).ToString());
            //MessageBox.Show(ProductionObject.dinoLiteSDK.GetReFocusSignal(ProductionObject.VideoDeviceIndex).ToString());
            //MessageBox.Show(ProductionObject.dinoLiteSDK.GetFocusLength(ProductionObject.VideoDeviceIndex).ToString());
            //MessageBox.Show(ProductionObject.dinoLiteSDK.GetAMRwithLensPos(ProductionObject.VideoDeviceIndex).ToString());
            //MessageBox.Show(ProductionObject.dinoLiteSDK.SetLensInitPos(ProductionObject.VideoDeviceIndex).ToString());
   
            if(!liveOn_) viewSDK.Visibility = Visibility.Collapsed;
            cameraConfigure_.Visibility = Visibility.Visible;          
        }
        private void buttonBack_Click(object sender, RoutedEventArgs e)
        {
            cameraCalibrationClose();
        }

        private void buttonCameraConfiguration1_(object sender, MouseButtonEventArgs e)
        {
            cameraCalibrationClose();
            cameraConfiguration1.Visibility = Visibility.Visible;
        }

        private void buttonCameraConfiguration2_(object sender, MouseButtonEventArgs e)
        {
            cameraCalibrationClose();
            cameraConfiguration2.Visibility = Visibility.Visible;
        }

        private void buttonCameraConfiguration3_(object sender, MouseButtonEventArgs e)
        {
            cameraCalibrationClose();
            cameraConfiguration3.Visibility = Visibility.Visible;
        }
        private void buttonCameraConfiguration4_(object sender, MouseButtonEventArgs e)
        {
            cameraCalibrationClose();
            cameraConfiguration4.Visibility = Visibility.Visible;
        }
        private void buttonCameraConfiguration5_(object sender, MouseButtonEventArgs e)
        {
            cameraCalibrationClose();
            cameraConfiguration5.Visibility = Visibility.Visible;
        }

        private void PackIcon_MouseDown(object sender, MouseButtonEventArgs e)
        {
            cameraCalibrationClose();
        }

        private void cameraCalibrationClose()
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
            int value = 0;

            lock (this)
            {
                switch (LED)
                {
                    case 1:
                        if (!LED1On)
                        {
                            if (LED2On && !LED3On && !LED4On)
                            {
                                ProductionObject.dinoLiteSDK.SetFLCSwitch(ProductionObject.VideoDeviceIndex, 3);
                                value = 3;
                                Thread.Sleep(280);
                                businessSystem.cameraSettingsModel.LedsOn = 3;
                            }
                            else if (!LED2On && LED3On && !LED4On)
                            {
                                ProductionObject.dinoLiteSDK.SetFLCSwitch(ProductionObject.VideoDeviceIndex, 5);
                                value = 5;
                                Thread.Sleep(280);
                                businessSystem.cameraSettingsModel.LedsOn = 5;
                            }
                            else if (!LED2On && !LED3On && LED4On)
                            {
                                ProductionObject.dinoLiteSDK.SetFLCSwitch(ProductionObject.VideoDeviceIndex, 9);
                                value = 9;
                                Thread.Sleep(280);
                                businessSystem.cameraSettingsModel.LedsOn = 9;
                            }
                            else if (LED2On && LED3On && !LED4On)
                            {
                                ProductionObject.dinoLiteSDK.SetFLCSwitch(ProductionObject.VideoDeviceIndex, 7);
                                value = 7;
                                Thread.Sleep(280);
                                businessSystem.cameraSettingsModel.LedsOn = 7;
                            }
                            else if (LED3On && LED4On && !LED2On)
                            {
                                ProductionObject.dinoLiteSDK.SetFLCSwitch(ProductionObject.VideoDeviceIndex, 13);
                                value = 13;
                                Thread.Sleep(280);
                                businessSystem.cameraSettingsModel.LedsOn = 13;
                            }
                            else if (LED2On && LED4On && !LED3On)
                            {
                                ProductionObject.dinoLiteSDK.SetFLCSwitch(ProductionObject.VideoDeviceIndex, 11);
                                value = 11;
                                Thread.Sleep(50);
                                businessSystem.cameraSettingsModel.LedsOn = 11;
                            }
                            else if (!LED2On && !LED4On && !LED3On)
                            {
                                ProductionObject.dinoLiteSDK.SetFLCSwitch(ProductionObject.VideoDeviceIndex, 1);
                                value = 1;
                                Thread.Sleep(280);
                                businessSystem.cameraSettingsModel.LedsOn = 1;
                            }
                            else if (LED2On && LED4On && LED3On)
                            {
                                ProductionObject.dinoLiteSDK.SetFLCSwitch(ProductionObject.VideoDeviceIndex, 15);
                                value = 15;
                                Thread.Sleep(280);
                                businessSystem.cameraSettingsModel.LedsOn = 15;
                            }
                            LED1On = true;
                        }
                        else
                        {
                            if (LED2On && !LED3On && !LED4On)
                            {
                                ProductionObject.dinoLiteSDK.SetFLCSwitch(ProductionObject.VideoDeviceIndex, 2);
                                value = 2;
                                Thread.Sleep(280);
                                businessSystem.cameraSettingsModel.LedsOn = 2;
                            }
                            else if (!LED2On && LED3On && !LED4On)
                            {
                                ProductionObject.dinoLiteSDK.SetFLCSwitch(ProductionObject.VideoDeviceIndex, 4);
                                value = 4;
                                Thread.Sleep(280);
                                businessSystem.cameraSettingsModel.LedsOn = 4;
                            }
                            else if (!LED2On && !LED3On && LED4On)
                            {
                                ProductionObject.dinoLiteSDK.SetFLCSwitch(ProductionObject.VideoDeviceIndex, 8);
                                value = 8;
                                Thread.Sleep(280);
                                businessSystem.cameraSettingsModel.LedsOn = 8;
                            }
                            else if (LED2On && LED3On && !LED4On)
                            {
                                ProductionObject.dinoLiteSDK.SetFLCSwitch(ProductionObject.VideoDeviceIndex, 6);
                                value = 6;
                                Thread.Sleep(280);
                                businessSystem.cameraSettingsModel.LedsOn = 6;
                            }
                            else if (LED3On && LED4On && !LED2On)
                            {
                                ProductionObject.dinoLiteSDK.SetFLCSwitch(ProductionObject.VideoDeviceIndex, 12);
                                value = 12;
                                Thread.Sleep(280);
                                businessSystem.cameraSettingsModel.LedsOn = 12;
                            }
                            else if (LED2On && LED4On && !LED3On)
                            {
                                ProductionObject.dinoLiteSDK.SetFLCSwitch(ProductionObject.VideoDeviceIndex, 10);
                                value = 10;
                                Thread.Sleep(280);
                                businessSystem.cameraSettingsModel.LedsOn = 10;
                            }
                            else if (!LED2On && !LED4On && !LED3On)
                            {
                                ProductionObject.dinoLiteSDK.SetFLCSwitch(ProductionObject.VideoDeviceIndex, 16);
                                value = 16;
                                Thread.Sleep(280);
                                businessSystem.cameraSettingsModel.LedsOn = 16;
                            }
                            else if (LED2On && LED4On && LED3On)
                            {
                                ProductionObject.dinoLiteSDK.SetFLCSwitch(ProductionObject.VideoDeviceIndex, 14);
                                value = 14;
                                Thread.Sleep(280);
                                businessSystem.cameraSettingsModel.LedsOn = 14;
                            }
                            LED1On = false;
                        }
                        break;
                    case 2:
                        if (!LED2On)
                        {
                            if (LED1On && !LED4On && !LED3On)
                            {
                                ProductionObject.dinoLiteSDK.SetFLCSwitch(ProductionObject.VideoDeviceIndex, 3);
                                value = 3;
                                Thread.Sleep(280);
                                businessSystem.cameraSettingsModel.LedsOn = 3;
                            }
                            else if (LED3On && !LED1On && !LED4On)
                            {
                                ProductionObject.dinoLiteSDK.SetFLCSwitch(ProductionObject.VideoDeviceIndex, 6);
                                value = 6;
                                Thread.Sleep(280);
                                businessSystem.cameraSettingsModel.LedsOn = 6;
                            }
                            else if (LED4On && !LED1On && !LED3On)
                            {
                                ProductionObject.dinoLiteSDK.SetFLCSwitch(ProductionObject.VideoDeviceIndex, 10);
                                value = 10;
                                Thread.Sleep(280);
                                businessSystem.cameraSettingsModel.LedsOn = 10;
                            }
                            else if (LED1On && LED3On && !LED4On)
                            {
                                ProductionObject.dinoLiteSDK.SetFLCSwitch(ProductionObject.VideoDeviceIndex, 7);
                                value = 7;
                                Thread.Sleep(280);
                                businessSystem.cameraSettingsModel.LedsOn = 7;
                            }
                            else if (LED3On && LED4On && !LED1On)
                            {
                                ProductionObject.dinoLiteSDK.SetFLCSwitch(ProductionObject.VideoDeviceIndex, 14);
                                value = 14;
                                Thread.Sleep(280);
                                businessSystem.cameraSettingsModel.LedsOn = 14;
                            }
                            else if (LED1On && LED4On && !LED3On)
                            {
                                ProductionObject.dinoLiteSDK.SetFLCSwitch(ProductionObject.VideoDeviceIndex, 11);
                                value = 11;
                                Thread.Sleep(280);
                                businessSystem.cameraSettingsModel.LedsOn = 11;
                            }
                            else if (!LED1On && !LED4On && !LED3On)
                            {
                                ProductionObject.dinoLiteSDK.SetFLCSwitch(ProductionObject.VideoDeviceIndex, 2);
                                value = 2;
                                Thread.Sleep(280);
                                businessSystem.cameraSettingsModel.LedsOn = 2;
                            }
                            else if (LED1On && LED4On && LED3On)
                            {
                                ProductionObject.dinoLiteSDK.SetFLCSwitch(ProductionObject.VideoDeviceIndex, 15);
                                value = 15;
                                Thread.Sleep(280);
                                businessSystem.cameraSettingsModel.LedsOn = 15;
                            }
                            LED2On = true;
                        }
                        else
                        {
                            if (LED1On && !LED4On && !LED3On)
                            {
                                ProductionObject.dinoLiteSDK.SetFLCSwitch(ProductionObject.VideoDeviceIndex, 1);
                                value = 1;
                                Thread.Sleep(280);
                                businessSystem.cameraSettingsModel.LedsOn = 1;
                            }
                            else if (LED3On && !LED4On && !LED1On)
                            {
                                ProductionObject.dinoLiteSDK.SetFLCSwitch(ProductionObject.VideoDeviceIndex, 4);
                                value = 4;
                                Thread.Sleep(280);
                                businessSystem.cameraSettingsModel.LedsOn = 4;
                            }
                            else if (LED4On && !LED3On && !LED1On)
                            {
                                ProductionObject.dinoLiteSDK.SetFLCSwitch(ProductionObject.VideoDeviceIndex, 8);
                                value = 8;
                                Thread.Sleep(280);
                                businessSystem.cameraSettingsModel.LedsOn = 8;
                            }
                            else if (LED1On && LED3On && !LED4On)
                            {
                                ProductionObject.dinoLiteSDK.SetFLCSwitch(ProductionObject.VideoDeviceIndex, 5);
                                value = 5;
                                Thread.Sleep(280);
                                businessSystem.cameraSettingsModel.LedsOn = 5;
                            }
                            else if (LED3On && LED4On && !LED1On)
                            {
                                ProductionObject.dinoLiteSDK.SetFLCSwitch(ProductionObject.VideoDeviceIndex, 12);
                                value = 12;
                                Thread.Sleep(280);
                                businessSystem.cameraSettingsModel.LedsOn = 12;
                            }
                            else if (LED1On && LED4On && !LED3On)
                            {
                                ProductionObject.dinoLiteSDK.SetFLCSwitch(ProductionObject.VideoDeviceIndex, 9);
                                value = 9;
                                Thread.Sleep(280);
                                businessSystem.cameraSettingsModel.LedsOn = 9;
                            }
                            else if (!LED1On && !LED4On && !LED3On)
                            {
                                ProductionObject.dinoLiteSDK.SetFLCSwitch(ProductionObject.VideoDeviceIndex, 16);
                                value = 16;
                                Thread.Sleep(280);
                                businessSystem.cameraSettingsModel.LedsOn = 16;
                            }
                            else if (LED1On && LED4On && LED3On)
                            {
                                ProductionObject.dinoLiteSDK.SetFLCSwitch(ProductionObject.VideoDeviceIndex, 13);
                                value = 13;
                                Thread.Sleep(280);
                                businessSystem.cameraSettingsModel.LedsOn = 13;
                            }
                            LED2On = false;
                        }
                        break;
                    case 3:
                        if (!LED3On)
                        {
                            if (LED2On && !LED4On && !LED1On)
                            {
                                ProductionObject.dinoLiteSDK.SetFLCSwitch(ProductionObject.VideoDeviceIndex, 6);
                                value = 6;
                                Thread.Sleep(280);
                                businessSystem.cameraSettingsModel.LedsOn = 6;
                            }
                            else if (LED1On && !LED4On && !LED2On)
                            {
                                ProductionObject.dinoLiteSDK.SetFLCSwitch(ProductionObject.VideoDeviceIndex, 5);
                                value = 5;
                                Thread.Sleep(280);
                                businessSystem.cameraSettingsModel.LedsOn = 5;
                            }
                            else if (LED4On && !LED2On && !LED1On)
                            {
                                ProductionObject.dinoLiteSDK.SetFLCSwitch(ProductionObject.VideoDeviceIndex, 12);
                                value = 12;
                                Thread.Sleep(280);
                                businessSystem.cameraSettingsModel.LedsOn = 12;
                            }
                            else if (LED2On && LED1On && !LED4On)
                            {
                                ProductionObject.dinoLiteSDK.SetFLCSwitch(ProductionObject.VideoDeviceIndex, 7);
                                value = 7;
                                Thread.Sleep(280);
                                businessSystem.cameraSettingsModel.LedsOn = 7;
                            }
                            else if (LED1On && LED4On && !LED2On)
                            {
                                ProductionObject.dinoLiteSDK.SetFLCSwitch(ProductionObject.VideoDeviceIndex, 13);
                                value = 13;
                                Thread.Sleep(280);
                                businessSystem.cameraSettingsModel.LedsOn = 13;
                            }
                            else if (LED2On && LED4On && !LED1On)
                            {
                                ProductionObject.dinoLiteSDK.SetFLCSwitch(ProductionObject.VideoDeviceIndex, 14);
                                value = 14;
                                Thread.Sleep(280);
                                businessSystem.cameraSettingsModel.LedsOn = 14;
                            }
                            else if (!LED2On && !LED4On && !LED1On)
                            {
                                ProductionObject.dinoLiteSDK.SetFLCSwitch(ProductionObject.VideoDeviceIndex, 4);
                                value = 4;
                                Thread.Sleep(280);
                                businessSystem.cameraSettingsModel.LedsOn = 4;
                            }
                            else if (LED2On && LED4On && LED1On)
                            {
                                ProductionObject.dinoLiteSDK.SetFLCSwitch(ProductionObject.VideoDeviceIndex, 15);
                                value = 15;
                                Thread.Sleep(280);
                                businessSystem.cameraSettingsModel.LedsOn = 15;
                            }
                            LED3On = true;
                        }
                        else
                        {
                            if (LED2On && !LED4On && !LED1On)
                            {
                                ProductionObject.dinoLiteSDK.SetFLCSwitch(ProductionObject.VideoDeviceIndex, 2);
                                value = 2;
                                Thread.Sleep(280);
                                businessSystem.cameraSettingsModel.LedsOn = 2;
                            }
                            else if (LED1On && !LED4On && !LED2On)
                            {
                                ProductionObject.dinoLiteSDK.SetFLCSwitch(ProductionObject.VideoDeviceIndex, 1);
                                value = 1;
                                Thread.Sleep(280);
                                businessSystem.cameraSettingsModel.LedsOn = 1;
                            }
                            else if (LED4On && !LED4On && !LED2On)
                            {
                                ProductionObject.dinoLiteSDK.SetFLCSwitch(ProductionObject.VideoDeviceIndex, 8);
                                value = 8;
                                Thread.Sleep(280);
                                businessSystem.cameraSettingsModel.LedsOn = 8;
                            }
                            else if (LED2On && LED1On && !LED4On)
                            {
                                ProductionObject.dinoLiteSDK.SetFLCSwitch(ProductionObject.VideoDeviceIndex, 3);
                                value = 3;
                                Thread.Sleep(280);
                                businessSystem.cameraSettingsModel.LedsOn = 3;
                            }
                            else if (LED1On && LED4On && !LED2On)
                            {
                                ProductionObject.dinoLiteSDK.SetFLCSwitch(ProductionObject.VideoDeviceIndex, 9);
                                value = 9;
                                Thread.Sleep(280);
                                businessSystem.cameraSettingsModel.LedsOn = 9;
                            }
                            else if (LED2On && LED4On && !LED1On)
                            {
                                ProductionObject.dinoLiteSDK.SetFLCSwitch(ProductionObject.VideoDeviceIndex, 10);
                                value = 10;
                                Thread.Sleep(280);
                                businessSystem.cameraSettingsModel.LedsOn = 10;
                            }
                            else if (!LED2On && !LED4On && !LED1On)
                            {
                                ProductionObject.dinoLiteSDK.SetFLCSwitch(ProductionObject.VideoDeviceIndex, 16);
                                value = 16;
                                Thread.Sleep(280);
                                businessSystem.cameraSettingsModel.LedsOn = 16;
                            }
                            else if (LED2On && LED4On && LED1On)
                            {
                                ProductionObject.dinoLiteSDK.SetFLCSwitch(ProductionObject.VideoDeviceIndex, 11);
                                value = 11;
                                Thread.Sleep(280);
                                businessSystem.cameraSettingsModel.LedsOn = 11;
                            }
                            LED3On = false;
                        }
                        break;
                    case 4:
                        if (!LED4On)
                        {
                            if (LED2On && !LED3On && !LED1On)
                            {
                                ProductionObject.dinoLiteSDK.SetFLCSwitch(ProductionObject.VideoDeviceIndex, 10);
                                value = 10;
                                Thread.Sleep(280);
                                businessSystem.cameraSettingsModel.LedsOn = 10;
                            }
                            else if (LED3On && !LED2On && !LED1On)
                            {
                                ProductionObject.dinoLiteSDK.SetFLCSwitch(ProductionObject.VideoDeviceIndex, 6);
                                value = 6;
                                Thread.Sleep(280);
                                businessSystem.cameraSettingsModel.LedsOn = 6;
                            }
                            else if (LED1On && !LED3On && !LED2On)
                            {
                                ProductionObject.dinoLiteSDK.SetFLCSwitch(ProductionObject.VideoDeviceIndex, 3);
                                value = 3;
                                Thread.Sleep(280);
                                businessSystem.cameraSettingsModel.LedsOn = 3;
                            }
                            else if (LED2On && LED3On && !LED1On)
                            {
                                ProductionObject.dinoLiteSDK.SetFLCSwitch(ProductionObject.VideoDeviceIndex, 14);
                                value = 14;
                                Thread.Sleep(280);
                                businessSystem.cameraSettingsModel.LedsOn = 14;
                            }
                            else if (LED3On && LED1On && !LED2On)
                            {
                                ProductionObject.dinoLiteSDK.SetFLCSwitch(ProductionObject.VideoDeviceIndex, 13);
                                value = 13;
                                Thread.Sleep(280);
                                businessSystem.cameraSettingsModel.LedsOn = 13;
                            }
                            else if (LED2On && LED1On && !LED3On)
                            {
                                ProductionObject.dinoLiteSDK.SetFLCSwitch(ProductionObject.VideoDeviceIndex, 11);
                                value = 11;
                                Thread.Sleep(280);
                                businessSystem.cameraSettingsModel.LedsOn = 11;
                            }
                            else if (!LED2On && !LED1On && !LED3On)
                            {
                                ProductionObject.dinoLiteSDK.SetFLCSwitch(ProductionObject.VideoDeviceIndex, 8);
                                value = 8;
                                Thread.Sleep(280);
                                businessSystem.cameraSettingsModel.LedsOn = 8;
                            }
                            else if (LED2On && LED1On && LED3On)
                            {
                                ProductionObject.dinoLiteSDK.SetFLCSwitch(ProductionObject.VideoDeviceIndex, 15);
                                value = 15;
                                Thread.Sleep(280);
                                businessSystem.cameraSettingsModel.LedsOn = 15;
                            }
                            LED4On = true;
                        }
                        else
                        {
                            if (LED2On && !LED1On && !LED3On)
                            {
                                ProductionObject.dinoLiteSDK.SetFLCSwitch(ProductionObject.VideoDeviceIndex, 2);
                                value = 2;
                                Thread.Sleep(280);
                                businessSystem.cameraSettingsModel.LedsOn = 2;
                            }
                            else if (LED3On && !LED1On && !LED2On)
                            {
                                ProductionObject.dinoLiteSDK.SetFLCSwitch(ProductionObject.VideoDeviceIndex, 4);
                                value = 4;
                                Thread.Sleep(280);
                                businessSystem.cameraSettingsModel.LedsOn = 4;
                            }
                            else if (LED1On && !LED2On && !LED3On)
                            {
                                ProductionObject.dinoLiteSDK.SetFLCSwitch(ProductionObject.VideoDeviceIndex, 1);
                                value = 1;
                                Thread.Sleep(280);
                                businessSystem.cameraSettingsModel.LedsOn = 1;
                            }
                            else if (LED2On && LED3On && !LED1On)
                            {
                                ProductionObject.dinoLiteSDK.SetFLCSwitch(ProductionObject.VideoDeviceIndex, 6);
                                value = 6;
                                Thread.Sleep(280);
                                businessSystem.cameraSettingsModel.LedsOn = 6;
                            }
                            else if (LED3On && LED1On && !LED2On)
                            {
                                ProductionObject.dinoLiteSDK.SetFLCSwitch(ProductionObject.VideoDeviceIndex, 5);
                                value = 5;
                                Thread.Sleep(280);
                                businessSystem.cameraSettingsModel.LedsOn = 5;
                            }
                            else if (LED2On && LED1On && !LED3On)
                            {
                                ProductionObject.dinoLiteSDK.SetFLCSwitch(ProductionObject.VideoDeviceIndex, 2);
                                value = 2;
                                Thread.Sleep(280);
                                businessSystem.cameraSettingsModel.LedsOn = 2;
                            }
                            else if (!LED2On && !LED1On && !LED3On)
                            {
                                ProductionObject.dinoLiteSDK.SetFLCSwitch(ProductionObject.VideoDeviceIndex, 16);
                                value = 16;
                                Thread.Sleep(280);
                                businessSystem.cameraSettingsModel.LedsOn = 16;
                            }
                            else if (LED2On && LED1On && LED3On)
                            {
                                ProductionObject.dinoLiteSDK.SetFLCSwitch(ProductionObject.VideoDeviceIndex, 7);
                                value = 7;
                                Thread.Sleep(280);
                                businessSystem.cameraSettingsModel.LedsOn = 7;
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
        /// Mouse movement control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frameHolder_MouseMove(object sender, MouseEventArgs e)
        {
            if (!menuBeingUsed)
            {
                int i = 0;                
                                //MASK : mask to remove multiple bounding box
                if (frameHolderMouseMove && !maskDelete)
                {
                    if (e.LeftButton == MouseButtonState.Pressed && !live)
                    {
                        System.Windows.Point maskPointStop = e.GetPosition(canvas);

                        mask.StrokeThickness = 3;
                        mask.Fill = new SolidColorBrush(Colors.DarkRed);
                        mask.Opacity = 0.2;

                        if ((maskPointStop.X - maskPointStart.X) >= 0 && (maskPointStop.Y - maskPointStart.Y >= 0))
                        {
                            mask.Width = maskPointStop.X - maskPointStart.X;
                            mask.Height = maskPointStop.Y - maskPointStart.Y;

                            maskPointStartMemory = maskPointStart;
                            maskPointStopMemory = maskPointStop;

                            //delimits minimum mask size
                            if (mask.Width > 50 && mask.Height > 50)
                            {
                                Canvas.SetLeft(mask, maskPointStart.X);
                                Canvas.SetTop(mask, maskPointStart.Y);
                                canvas.UpdateLayout();
                                canvasMask.UpdateLayout();

                                verySmallMask = false;
                            }
                            else
                            {
                                mask.Width = 0;
                                mask.Height = 0;
                                verySmallMask = true;
                            }
                        }
                        else if ((maskPointStop.X - maskPointStart.X) >= 0 && (maskPointStop.Y - maskPointStart.Y < 0))
                        {
                            mask.Width = maskPointStop.X - maskPointStart.X;
                            mask.Height = maskPointStart.Y - maskPointStop.Y;

                            maskPointStartMemory.X = maskPointStart.X;
                            maskPointStopMemory.X = maskPointStop.X;
                            maskPointStartMemory.Y = maskPointStop.Y;
                            maskPointStopMemory.Y = maskPointStart.Y;

                            //delimits minimum mask size
                            if (mask.Width > 50 && mask.Height > 50)
                            {
                                Canvas.SetLeft(mask, maskPointStart.X);
                                Canvas.SetTop(mask, maskPointStop.Y);
                                canvas.UpdateLayout();
                                canvasMask.UpdateLayout();

                                verySmallMask = false;
                            }
                            else
                            {
                                verySmallMask = true;
                            }
                        }
                        else if ((maskPointStop.X - maskPointStart.X) < 0 && (maskPointStop.Y - maskPointStart.Y >= 0))
                        {
                            mask.Width = maskPointStart.X - maskPointStop.X;
                            mask.Height = maskPointStop.Y - maskPointStart.Y;

                            maskPointStartMemory.X = maskPointStop.X;
                            maskPointStopMemory.X = maskPointStart.X;
                            maskPointStartMemory.Y = maskPointStart.Y;
                            maskPointStopMemory.Y = maskPointStop.Y;

                            //delimits minimum mask size
                            if (mask.Width > 50 && mask.Height > 50)
                            {
                                Canvas.SetLeft(mask, maskPointStop.X);
                                Canvas.SetTop(mask, maskPointStart.Y);
                                canvas.UpdateLayout();
                                canvasMask.UpdateLayout();

                                verySmallMask = false;
                            }
                            else
                            {
                                verySmallMask = true;
                            }
                        }
                        else if ((maskPointStop.X - maskPointStart.X) < 0 && (maskPointStop.Y - maskPointStart.Y < 0))
                        {
                            mask.Width = maskPointStart.X - maskPointStop.X;
                            mask.Height = maskPointStart.Y - maskPointStop.Y;

                            maskPointStartMemory.X = maskPointStop.X;
                            maskPointStopMemory.X = maskPointStart.X;
                            maskPointStartMemory.Y = maskPointStop.Y;
                            maskPointStopMemory.Y = maskPointStart.Y;

                            //delimits minimum mask size
                            if (mask.Width > 50 && mask.Height > 50)
                            {
                                Canvas.SetLeft(mask, maskPointStop.X);
                                Canvas.SetTop(mask, maskPointStop.Y);
                                canvas.UpdateLayout();
                                canvasMask.UpdateLayout();

                                verySmallMask = false;
                            }
                            else
                            {
                                verySmallMask = true;
                            }
                        }

                        try
                        {
                            if (numberMask == -1)
                            {
                                canvasMask.Children.Add(mask);
                                numberMask = canvasMask.Children.Count - 1;
                            }
                        }
                        catch
                        {

                        }
                    }
                    else
                    {
                        maskPointStart = e.GetPosition(canvas);
                    }

                    //selectBoundingBoxDelete = -1;

                    //dio
                    if (!live && !verySmallMask)
                    {
                        bool valid = false;
                        double WidthR;
                        double HeightR;
                        double xR;
                        double yR;

                        SolidColorBrush colorStroke = new SolidColorBrush();
                        SolidColorBrush colorFill = new SolidColorBrush();
                        colorStroke = System.Windows.Media.Brushes.Black;
                        colorFill = System.Windows.Media.Brushes.Black;

                        foreach (var bTest in businessSystem.socketModels)
                        {
                            i++;

                            System.Windows.Point point = e.GetPosition(canvas);

                            WidthR = (bTest.Width * w) / ProductionObject.ResolutionWidth;
                            HeightR = (bTest.Height * h) / ProductionObject.ResolutionHeight;
                            xR = (bTest.x * w) / ProductionObject.ResolutionWidth;
                            yR = (bTest.y * h) / ProductionObject.ResolutionHeight;

                            if (!(bTest.x == 0 && bTest.y == 0))
                            {
                                if (point.X > xR && point.Y > yR && point.X < (xR + WidthR) && point.Y < (yR + HeightR))
                                {
                                    if ((i != selectBoundingBoxDelete) && selectBoundingBoxDelete != -1)
                                    {
                                        try
                                        {
                                            canvas.Children.RemoveAt(boundingBoxSelect);
                                            canvas.Children.RemoveAt(boundingBoxSelect - 1);
                                            canvas.Children.RemoveAt(boundingBoxSelect - 2);
                                            canvas.Children.RemoveAt(boundingBoxSelect - 3);
                                            canvas.Children.RemoveAt(boundingBoxSelect - 4);
                                            boundingBoxSelect = 0;
                                            selectBoundingBoxDelete = -1;
                                        }
                                        catch
                                        {
                                            boundingBoxSelect = 0;
                                            selectBoundingBoxDelete = -1;
                                        }

                                    }
                                    selectBoundingBoxDelete = i;

                                    valid = true;
                                    if (boundingBoxSelect == 0)
                                    {
                                        System.Windows.Shapes.Rectangle maskUp;
                                        System.Windows.Shapes.Rectangle maskDown;
                                        System.Windows.Shapes.Rectangle maskLeft;
                                        System.Windows.Shapes.Rectangle maskRight;

                                        if (!editBoundingBoxUp)
                                        {
                                            maskUp = new System.Windows.Shapes.Rectangle
                                            {
                                                Stroke = System.Windows.Media.Brushes.Red,
                                                //Fill = System.Windows.Media.Brushes.Red,
                                                StrokeThickness = 2, 
                                                Width = 12,
                                                Height = 12
                                            };
                                        }
                                        else
                                        {
                                            maskUp = new System.Windows.Shapes.Rectangle
                                            {
                                                Stroke = System.Windows.Media.Brushes.Green,
                                                //Fill = System.Windows.Media.Brushes.Red,
                                                StrokeThickness = 2,
                                                Width = 12,
                                                Height = 12
                                            };
                                        }

                                        if (!editBoundingBoxDown)
                                        {
                                            maskDown = new System.Windows.Shapes.Rectangle
                                            {
                                                Stroke = System.Windows.Media.Brushes.Red,
                                                //Fill = System.Windows.Media.Brushes.Red,
                                                StrokeThickness = 2,
                                                Width = 12,
                                                Height = 12
                                            };
                                        }
                                        else
                                        {
                                            maskDown = new System.Windows.Shapes.Rectangle
                                            {
                                                Stroke = System.Windows.Media.Brushes.Green,
                                                //Fill = System.Windows.Media.Brushes.Red,
                                                StrokeThickness = 2,
                                                Width = 12,
                                                Height = 12
                                            };
                                        }

                                        if (!editBoundingBoxLeft)
                                        {
                                            maskLeft = new System.Windows.Shapes.Rectangle
                                            {
                                                Stroke = System.Windows.Media.Brushes.Red,
                                                //Fill = System.Windows.Media.Brushes.Red,
                                                StrokeThickness = 2,
                                                Width = 12,
                                                Height = 12
                                            };
                                        }
                                        else
                                        {
                                            maskLeft = new System.Windows.Shapes.Rectangle
                                            {
                                                Stroke = System.Windows.Media.Brushes.Green,
                                                //Fill = System.Windows.Media.Brushes.Red,
                                                StrokeThickness = 2,
                                                Width = 12,
                                                Height = 12
                                            };
                                        }

                                        if (!editBoundingBoxRight)
                                        {
                                            maskRight = new System.Windows.Shapes.Rectangle
                                            {
                                                Stroke = System.Windows.Media.Brushes.Red,
                                                StrokeThickness = 2,
                                                //Fill = System.Windows.Media.Brushes.Red,
                                                Width = 12,
                                                Height = 12
                                            };
                                        }
                                        else
                                        {
                                            maskRight = new System.Windows.Shapes.Rectangle
                                            {
                                                Stroke = System.Windows.Media.Brushes.Green,
                                                StrokeThickness = 2,
                                                //Fill = System.Windows.Media.Brushes.Red,
                                                Width = 12,
                                                Height = 12
                                            };
                                        }

                                        System.Windows.Shapes.Rectangle rect = new System.Windows.Shapes.Rectangle
                                        {
                                            StrokeThickness = 3,
                                            Stroke = colorStroke,
                                            Fill = colorFill,
                                            Width = WidthR,
                                            Height = HeightR
                                        };
                                        rect.Fill = new SolidColorBrush(Colors.DarkRed);
                                        rect.Opacity = 0.2;

                                        Canvas.SetLeft(rect, xR);
                                        Canvas.SetTop(rect, yR);
                                        canvas.Children.Add(rect);

                                        Canvas.SetLeft(maskUp, xR + (WidthR / 2));
                                        Canvas.SetTop(maskUp, yR - 5);
                                        canvas.Children.Add(maskUp);

                                        Canvas.SetLeft(maskDown, xR + (WidthR / 2));
                                        Canvas.SetTop(maskDown, yR + HeightR - 7);
                                        canvas.Children.Add(maskDown);

                                        Canvas.SetLeft(maskLeft, xR - 5);
                                        Canvas.SetTop(maskLeft, yR + (HeightR / 2));
                                        canvas.Children.Add(maskLeft);

                                        Canvas.SetLeft(maskRight, xR + WidthR - 7);
                                        Canvas.SetTop(maskRight, yR + (HeightR / 2));
                                        canvas.Children.Add(maskRight);

                                        boundingBoxSelect = canvas.Children.Count - 1;
                                    }
                                }
                            }

                        }

                        if (!valid)
                        {
                            if (boundingBoxSelect != 0)
                            {
                                try
                                {
                                    canvas.Children.RemoveAt(boundingBoxSelect);
                                    canvas.Children.RemoveAt(boundingBoxSelect - 1);
                                    canvas.Children.RemoveAt(boundingBoxSelect - 2);
                                    canvas.Children.RemoveAt(boundingBoxSelect - 3);
                                    canvas.Children.RemoveAt(boundingBoxSelect - 4);
                                    boundingBoxSelect = 0;
                                    selectBoundingBoxDelete = -1;
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex.Message);
                                }
                            }
                        }
                        valid = false;
                    }
                }
            }

            verySmallMask = false;
        }

        /// <summary>
        /// LED brightness control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ledBrightness_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            if (!startNotConfigureCamera)
            {
                int value = 0;
                lock (this)
                {
                    try
                    {
                        switch (ledBrightness.SelectedValue)
                        {
                            case 1:
                                ProductionObject.dinoLiteSDK.SetFLCLevel(ProductionObject.VideoDeviceIndex, (int)ledBrightness.SelectedItem);
                                value = (int)ledBrightness.SelectedItem;
                                Thread.Sleep(280);
                                break;
                            case 2:
                                ProductionObject.dinoLiteSDK.SetFLCLevel(ProductionObject.VideoDeviceIndex, (int)ledBrightness.SelectedItem);
                                value = (int)ledBrightness.SelectedItem;
                                Thread.Sleep(280);
                                break;
                            case 3:
                                ProductionObject.dinoLiteSDK.SetFLCLevel(ProductionObject.VideoDeviceIndex, (int)ledBrightness.SelectedItem);
                                value = (int)ledBrightness.SelectedItem;
                                Thread.Sleep(280);
                                break;
                            case 4:
                                ProductionObject.dinoLiteSDK.SetFLCLevel(ProductionObject.VideoDeviceIndex, (int)ledBrightness.SelectedItem);
                                value = (int)ledBrightness.SelectedItem;
                                Thread.Sleep(280);
                                break;
                            case 5:
                                ProductionObject.dinoLiteSDK.SetFLCLevel(ProductionObject.VideoDeviceIndex, (int)ledBrightness.SelectedItem);
                                value = (int)ledBrightness.SelectedItem;
                                Thread.Sleep(280);
                                break;
                            case 6:
                                ProductionObject.dinoLiteSDK.SetFLCLevel(ProductionObject.VideoDeviceIndex, (int)ledBrightness.SelectedItem);
                                value = (int)ledBrightness.SelectedItem;
                                Thread.Sleep(280);
                                break;
                        }
                        businessSystem.cameraSettingsModel.LedBrightness = value;
                    }
                    catch
                    {
                    }
                }
            }
        }

        private void brightness__ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!startNotConfigureCamera)
            {
                lock (this)
                {
                    ProductionObject.dinoLiteSDK.set_VideoProcAmp(0, (int)brightness_.Value);
                    Thread.Sleep(50);
                    businessSystem.cameraSettingsModel.Brightness = (int)brightness_.Value;
                }
            }
        }

        /// <summary>
        /// Contrast camera control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void contrast_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!startNotConfigureCamera)
            {
                lock (this)
                {
                    ProductionObject.dinoLiteSDK.set_VideoProcAmp(1, (int)contrast.Value);
                    Thread.Sleep(50);
                    businessSystem.cameraSettingsModel.Contrast = (int)contrast.Value;
                }
            }
        }

        /// <summary>
        /// HUE camera control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hue_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!startNotConfigureCamera)
            {
                lock (this)
                {
                    ProductionObject.dinoLiteSDK.set_VideoProcAmp(2, (int)hue.Value);
                    Thread.Sleep(50);
                    businessSystem.cameraSettingsModel.Hue = (int)hue.Value;
                }
            }
        }

        /// <summary>
        /// White Balance Red camera control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void whiteBalanceRed_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!startNotConfigureCamera)
            {
                lock (this)
                {
                    ProductionObject.dinoLiteSDK.FreezeAWB(ProductionObject.VideoDeviceIndex, 0);
                    ProductionObject.dinoLiteSDK.SetAWBR(ProductionObject.VideoDeviceIndex, (int)whiteBalanceRed.Value);
                    ProductionObject.dinoLiteSDK.FreezeAWB(ProductionObject.VideoDeviceIndex, 1);
                    Thread.Sleep(50);
                    businessSystem.cameraSettingsModel.AWBR = (int)whiteBalanceRed.Value;
                }
            }
        }

        /// <summary>
        /// White Balance Green camera control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void whiteBalanceGreen_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!startNotConfigureCamera)
            {
                lock (this)
                {
                    ProductionObject.dinoLiteSDK.FreezeAWB(ProductionObject.VideoDeviceIndex, 0);
                    ProductionObject.dinoLiteSDK.SetAWBG(ProductionObject.VideoDeviceIndex, (int)whiteBalanceGreen.Value);
                    ProductionObject.dinoLiteSDK.FreezeAWB(ProductionObject.VideoDeviceIndex, 1);
                    Thread.Sleep(50);
                    businessSystem.cameraSettingsModel.AWBG = (int)whiteBalanceGreen.Value;
                }
            }
        }

        /// <summary>
        /// White Balance Blue camera control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void whiteBalanceBlue_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!startNotConfigureCamera)
            {
                lock (this)
                {
                    ProductionObject.dinoLiteSDK.FreezeAWB(ProductionObject.VideoDeviceIndex, 0);
                    ProductionObject.dinoLiteSDK.SetAWBB(ProductionObject.VideoDeviceIndex, (int)whiteBalanceBlue.Value);
                    ProductionObject.dinoLiteSDK.FreezeAWB(ProductionObject.VideoDeviceIndex, 1);
                    Thread.Sleep(50);
                    businessSystem.cameraSettingsModel.AWBB = (int)whiteBalanceBlue.Value;
                }
            }
        }

        /// <summary>
        /// Saturation camera control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saturation_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!startNotConfigureCamera)
            {
                lock (this)
                {
                    ProductionObject.dinoLiteSDK.set_VideoProcAmp(3, (int)saturation.Value);
                    Thread.Sleep(50);
                    businessSystem.cameraSettingsModel.Saturation = (int)saturation.Value;
                }
            }
        }

        /// <summary>
        /// Sharpness camera control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void sharpness_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!startNotConfigureCamera)
            {
                lock (this)
                {
                    ProductionObject.dinoLiteSDK.set_VideoProcAmp(4, (int)sharpness.Value);
                    Thread.Sleep(50);
                    businessSystem.cameraSettingsModel.Sharpness = (int)sharpness.Value;
                }
            }
        }

        /// <summary>
        /// Gamma camera control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gamma_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!startNotConfigureCamera)
            {
                lock (this)
                {
                    ProductionObject.dinoLiteSDK.set_VideoProcAmp(5, (int)gamma.Value);
                    Thread.Sleep(50);
                    businessSystem.cameraSettingsModel.Gamma = (int)gamma.Value;
                }
            }
        }

        /// <summary>
        ///  Auto Focus camera control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void autoFocus_Click(object sender, RoutedEventArgs e)
        {
            if (!startNotConfigureCamera)
            {
                lock (this)
                {
                    ProductionObject.dinoLiteSDK.AutoFocus(ProductionObject.VideoDeviceIndex);
                    Thread.Sleep(50);
                    StartAutoFocus = false;
                }
            }
        }

        private void manualFocus_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SetLensPos_(object sender, MouseWheelEventArgs e)
        {
            //if (e.Delta > mouseWeel)
            //{
            //    focus += 1;
            //    ProductionObject.dinoLiteSDK.LightOn(1);
            //    ProductionObject.dinoLiteSDK.SetLensPos(0, focus);
            //}
            //else
            //{
            //    focus -= 1;
            //    ProductionObject.dinoLiteSDK.LightOn(1);
            //    ProductionObject.dinoLiteSDK.SetLensPos(0, focus);
            //}

            //mouseWeel = e.Delta;
        }

        /// <summary>
        /// Expoure camera control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBoxExpoure_Checked(object sender, RoutedEventArgs e)
        {
            checkBoxStatus();
        }

        private void checkBoxStatus()
        {
            if (!startNotConfigureCamera)
            {
                //ProductionObject.dinoLiteSDK.Connected = true;
                if (checkBoxExpoure.IsChecked == true)
                {
                    try
                    {
                        lock (this)
                        {
                            checkBoxExpoure.IsEnabled = true;
                            ProductionObject.dinoLiteSDK.SetAutoExposure(ProductionObject.VideoDeviceIndex, 1);
                            ProductionObject.dinoLiteSDK.SetExposureStability(ProductionObject.VideoDeviceIndex, 1);
                            Thread.Sleep(50);
                            businessSystem.cameraSettingsModel.AutomaticExposure = 1;
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
                        lock (this)
                        {
                            checkBoxExpoure.IsEnabled = false;
                            ProductionObject.dinoLiteSDK.SetAutoExposure(ProductionObject.VideoDeviceIndex, 0);
                            Thread.Sleep(50);
                            businessSystem.cameraSettingsModel.AutomaticExposure = 0;
                            applyExposureValue();
                        }
                    }
                    catch
                    {
                        checkBoxExpoure.IsEnabled = false;
                    }
                }
                //ProductionObject.dinoLiteSDK.Connected = false;
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (!startNotConfigureCamera)
            {
                if (mirrorHorizontal.IsChecked == true)
                {
                    lock (this)
                    {
                        //ProductionObject.dinoLiteSDK.SetMirror(ProductionObject.VideoDeviceIndex, 2);
                        ProductionObject.dinoLiteSDK.VideoRotateAngle = -90;
                        Thread.Sleep(280);
                        businessSystem.cameraSettingsModel.Mirror = -90;
                    }
                }
                else if (mirrorHorizontal.IsChecked == true && mirrorVertical.IsChecked == true)
                {
                    lock (this)
                    {
                        //ProductionObject.dinoLiteSDK.SetMirror(ProductionObject.VideoDeviceIndex, (int)3);                     
                        Thread.Sleep(280);//led on need to wait it for reading 
                        businessSystem.cameraSettingsModel.Mirror = 0;
                    }
                }
                else
                {
                    lock (this)
                    {
                        //ProductionObject.dinoLiteSDK.SetMirror(ProductionObject.VideoDeviceIndex, 0);
                        ProductionObject.dinoLiteSDK.VideoRotateAngle = 0;
                        Thread.Sleep(280);
                        businessSystem.cameraSettingsModel.Mirror = 0;
                    }
                }
            }
        }

        /// <summary>
        /// Mirror Vertical camera control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mirrorVertical_Checked(object sender, RoutedEventArgs e)
        {
            if (!startNotConfigureCamera)
            {
                if (mirrorVertical.IsChecked == true)
                {
                    lock (this)
                    {
                        //ProductionObject.dinoLiteSDK.SetMirror(ProductionObject.VideoDeviceIndex, 1);
                        Thread.Sleep(280);//led on need to wait it for reading 
                    }
                }
                else if (mirrorHorizontal.IsChecked == true && mirrorVertical.IsChecked == true)
                {
                    lock (this)
                    {
                        //ProductionObject.dinoLiteSDK.SetMirror(ProductionObject.VideoDeviceIndex, 3);
                        Thread.Sleep(280);//led on need to wait it for reading 
                    }
                }
                else
                {
                    lock (this)
                    {
                        //ProductionObject.dinoLiteSDK.SetMirror(ProductionObject.VideoDeviceIndex, 0);
                        Thread.Sleep(50);
                    }
                }
            }
        }

        private void CheckBox_Checked_1(object sender, RoutedEventArgs e)
        {
            if (!startNotConfigureCamera)
            {
                if (monochrome.IsChecked == true)
                {
                    lock (this)
                    {
                        ProductionObject.dinoLiteSDK.SetMonochrome(ProductionObject.VideoDeviceIndex, 1);
                        Thread.Sleep(50);
                        businessSystem.cameraSettingsModel.Monochrome = 1;
                    }
                }
                else
                {
                    lock (this)
                    {
                        ProductionObject.dinoLiteSDK.SetMonochrome(ProductionObject.VideoDeviceIndex, 0);
                        Thread.Sleep(50);
                        businessSystem.cameraSettingsModel.Monochrome = 0;
                    }
                }
            }
        }

        /// <summary>
        /// Negative camera control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void negative_Checked(object sender, RoutedEventArgs e)
        {
            if (!startNotConfigureCamera)
            {
                if (negative.IsChecked == true)
                {
                    lock (this)
                    {
                        ProductionObject.dinoLiteSDK.SetNegative(ProductionObject.VideoDeviceIndex, 1);
                        Thread.Sleep(50);
                        businessSystem.cameraSettingsModel.Negative = 1;
                    }
                }
                else
                {
                    lock (this)
                    {
                        ProductionObject.dinoLiteSDK.SetNegative(ProductionObject.VideoDeviceIndex, 0);
                        Thread.Sleep(50);
                        businessSystem.cameraSettingsModel.Negative = 0;
                    }
                }
            }
        }

        /// <summary>
        /// Mirror Horizontal camera control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mirrorHorizontal_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void AutomaticBristleClassification_Closed(object sender, EventArgs e)
        {
            myTimer5.Stop();
            myTimer5.Tick -= new EventHandler(run);

            myTimer2.Stop();
            myTimer2.Tick -= new EventHandler(cameraParam);

            myTimer3.Stop();
            myTimer3.Tick -= new EventHandler(cameraParam);

            myTimer.Stop();
            myTimer.Tick -= new EventHandler(IA);
        }

        private void whiteBalance_Checked(object sender, RoutedEventArgs e)
        {
            if (!startNotConfigureCamera)
            {
                if (whiteBalance.IsChecked == true)
                {
                    lock (this)
                    {
                        ProductionObject.dinoLiteSDK.set_VideoProcAmp(7, 1);
                        Thread.Sleep(50);
                    }
                }
                else
                {
                    ProductionObject.dinoLiteSDK.set_VideoProcAmp(7, 0);
                    Thread.Sleep(50);
                }
            }
        }

        private void whiteBalance_Unchecked(object sender, RoutedEventArgs e)
        {
            if (!startNotConfigureCamera)
            {
                if (whiteBalance.IsChecked == true)
                {
                    lock (this)
                    {
                        ProductionObject.dinoLiteSDK.set_VideoProcAmp(7, 1);
                        Thread.Sleep(50);
                    }
                }
                else
                {
                    ProductionObject.dinoLiteSDK.set_VideoProcAmp(7, 0);
                    Thread.Sleep(50);
                }
            }
        }

        /// <summary>
        /// Manual Focus camera control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void manualFocusS_Click(object sender, RoutedEventArgs e)
        {
            if (!startNotConfigureCamera)
            {
                try
                {
                    focus += 1;
                    ProductionObject.dinoLiteSDK.SetLensPos(ProductionObject.VideoDeviceIndex, focus);
                    Thread.Sleep(25);
                    businessSystem.cameraSettingsModel.Focus = ProductionObject.dinoLiteSDK.GetAMRwithLensPos(ProductionObject.VideoDeviceIndex);
                    focusValue.Text = Math.Round(businessSystem.cameraSettingsModel.Focus, 2).ToString();
                    Thread.Sleep(25);
                }
                catch
                {
                }
            }
        }

        /// <summary>
        /// Manual Focus camera control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void manualFocusA_Click(object sender, RoutedEventArgs e)
        {
            if (!startNotConfigureCamera)
            {
                try
                {
                    focus -= 1;
                    ProductionObject.dinoLiteSDK.SetLensPos(ProductionObject.VideoDeviceIndex, focus);
                    Thread.Sleep(25);
                    businessSystem.cameraSettingsModel.Focus = ProductionObject.dinoLiteSDK.GetAMRwithLensPos(ProductionObject.VideoDeviceIndex);
                    focusValue.Text = Math.Round(businessSystem.cameraSettingsModel.Focus, 2).ToString();
                    Thread.Sleep(25);
                }
                catch
                {
                }
            }
        }

        /// <summary>
        /// Keyboard control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void keyboardControl(object sender, KeyEventArgs e)
        {
            maskDelete = true;

            if (!live && frameHolderMouseMove)
            {
                try
                {
                    if (e.Key == Key.Delete)
                    {
                        if (numberMask == -1)
                        {
                            if (selectBoundingBoxDelete > -1)
                            {
                                businessSystem.socketModels[selectBoundingBoxDelete - 1].obj_class = "discard";
                                businessSystem.boundingBoxDiscards.Add(businessSystem.socketModels[selectBoundingBoxDelete - 1]);
                                businessSystem.socketModels.RemoveAt(selectBoundingBoxDelete - 1);
                                drawBoundingBox();
                                boundingBoxSelect = 0;
                                selectBoundingBoxDelete = -1;
                            }
                            e.Handled = true;
                        }
                        else
                        {
                            bool remove = true;
                            while (remove)
                            {
                                int i = 0;
                                foreach (var bTest in businessSystem.socketModels)
                                {
                                    double WidthR = (bTest.Width * w) / ProductionObject.ResolutionWidth;
                                    double HeightR = (bTest.Height * h) / ProductionObject.ResolutionHeight;
                                    double xR = (bTest.x * w) / ProductionObject.ResolutionWidth;
                                    double yR = (bTest.y * h) / ProductionObject.ResolutionHeight;

                                    if (!(bTest.x == 0 && bTest.y == 0))
                                    {
                                        if ((xR > maskPointStartMemory.X - 2) && (yR > maskPointStartMemory.Y - 2) && ((xR + WidthR) < maskPointStopMemory.X + 2) && ((yR + HeightR) < maskPointStopMemory.Y + 2))
                                        {
                                            businessSystem.socketModels[i].obj_class = "discard";
                                            businessSystem.boundingBoxDiscards.Add(businessSystem.socketModels[i]);
                                            businessSystem.socketModels.RemoveAt(i);
                                            boundingBoxSelect = 0;
                                            break;
                                        }
                                    }
                                    i++;
                                    if (i == businessSystem.socketModels.Count)
                                    {
                                        remove = false;
                                    }
                                }

                                if (businessSystem.socketModels.Count == 0)
                                {
                                    remove = false;
                                    canvas.Children.Clear();

                                    switch (positionResultManual)
                                    {
                                        case "T":
                                            totalNumberBristlesTtype1.Text = "0";
                                            totalNumberBristlesTtype2.Text = "0";
                                            totalNumberBristlesTtype3.Text = "0";
                                            totalNumberBristlesTdiscard.Text = "0";
                                            totalNumberBristlesT.Text = "0";
                                            totalNumberBristlesTnok.Text = "0";
                                            break;
                                        case "M1":
                                            totalNumberBristlesM1type1.Text = "0";
                                            totalNumberBristlesM1type2.Text = "0";
                                            totalNumberBristlesM1type3.Text = "0";
                                            totalNumberBristlesM1discard.Text = "0";
                                            totalNumberBristlesM1.Text = "0";
                                            totalNumberBristlesM1nok.Text = "0";
                                            break;
                                        case "M2":
                                            totalNumberBristlesM2type1.Text = "0";
                                            totalNumberBristlesM2type2.Text = "0";
                                            totalNumberBristlesM2type3.Text = "0";
                                            totalNumberBristlesM2discard.Text = "0";
                                            totalNumberBristlesM2.Text = "0";
                                            totalNumberBristlesM2nok.Text = "0";
                                            break;
                                        case "M3":
                                            totalNumberBristlesM3type1.Text = "0";
                                            totalNumberBristlesM3type2.Text = "0";
                                            totalNumberBristlesM3type3.Text = "0";
                                            totalNumberBristlesM3discard.Text = "0";
                                            totalNumberBristlesM3.Text = "0";
                                            totalNumberBristlesM3nok.Text = "0";
                                            break;
                                        case "N":
                                            totalNumberBristlesNtype1.Text = "0";
                                            totalNumberBristlesNtype2.Text = "0";
                                            totalNumberBristlesNtype3.Text = "0";
                                            totalNumberBristlesNdiscard.Text = "0";
                                            totalNumberBristlesN.Text = "0";
                                            totalNumberBristlesNnok.Text = "0";
                                            break;
                                    }
                                    totalBristlesFound.Text = "0";
                                    totalDefectiveBristles.Text = "0";
                                }
                            }

                            drawBoundingBox();

                            if (numberMask != -1)
                            {
                                canvasMask.Children.RemoveAt(numberMask);
                                numberMask = -1;
                            }
                            e.Handled = true;
                            selectBoundingBoxDelete = -1;
                        }

                        frameHolderMouseMove = true;
                    }
                    else if (e.Key == Key.Left)
                    {
                        if (!editBoundingBoxRight)
                        {
                            if (!editBoundingBoxLeft)
                            {
                                if (businessSystem.socketModels[selectBoundingBoxDelete - 1].x > 0)
                                {
                                    boundingBoxEdit = true;
                                    businessSystem.socketModels[selectBoundingBoxDelete - 1].x -= businessSystem.resizingOfBoundingBoxWeight;
                                    drawBoundingBox();
                                    boundingBoxSelect = 0;
                                }
                            }
                            else
                            {
                                if (businessSystem.socketModels[selectBoundingBoxDelete - 1].x > 0)
                                {
                                    boundingBoxEdit = true;
                                    businessSystem.socketModels[selectBoundingBoxDelete - 1].x -= businessSystem.resizingOfBoundingBoxWeight;
                                    businessSystem.socketModels[selectBoundingBoxDelete - 1].Width += businessSystem.resizingOfBoundingBoxWeight;
                                    drawBoundingBox();
                                    boundingBoxSelect = 0;
                                }
                            }
                        }
                        else
                        {
                            if (businessSystem.socketModels[selectBoundingBoxDelete - 1].x > 0)
                            {
                                boundingBoxEdit = true;
                                businessSystem.socketModels[selectBoundingBoxDelete - 1].Width -= businessSystem.resizingOfBoundingBoxWeight;
                                drawBoundingBox();
                                boundingBoxSelect = 0;
                            }
                        }
                    }
                    else if (e.Key == Key.Right)
                    {
                        if (!editBoundingBoxLeft)
                        {
                            if (!editBoundingBoxRight)
                            {
                                if (businessSystem.socketModels[selectBoundingBoxDelete - 1].x > 0)
                                {
                                    boundingBoxEdit = true;
                                    businessSystem.socketModels[selectBoundingBoxDelete - 1].x += businessSystem.resizingOfBoundingBoxWeight;
                                    drawBoundingBox();
                                    boundingBoxSelect = 0;
                                }
                            }
                            else
                            {
                                if (businessSystem.socketModels[selectBoundingBoxDelete - 1].x > 0)
                                {
                                    boundingBoxEdit = true;
                                    businessSystem.socketModels[selectBoundingBoxDelete - 1].Width += businessSystem.resizingOfBoundingBoxWeight;
                                    drawBoundingBox();
                                    boundingBoxSelect = 0;
                                }
                            }
                        }
                        else
                        {
                            if (businessSystem.socketModels[selectBoundingBoxDelete - 1].x > 0)
                            {
                                boundingBoxEdit = true;
                                businessSystem.socketModels[selectBoundingBoxDelete - 1].x += businessSystem.resizingOfBoundingBoxWeight;
                                businessSystem.socketModels[selectBoundingBoxDelete - 1].Width -= businessSystem.resizingOfBoundingBoxWeight;
                                drawBoundingBox();
                                boundingBoxSelect = 0;
                            }
                        }
                    }
                    else if (e.Key == Key.Up)
                    {
                        if (!editBoundingBoxDown)
                        {
                            if (!editBoundingBoxUp)
                            {
                                if (businessSystem.socketModels[selectBoundingBoxDelete - 1].y > 0)
                                {
                                    boundingBoxEdit = true;
                                    businessSystem.socketModels[selectBoundingBoxDelete - 1].y -= businessSystem.resizingOfBoundingBoxWeight;
                                    drawBoundingBox();
                                    boundingBoxSelect = 0;
                                }
                            }
                            else
                            {
                                if (businessSystem.socketModels[selectBoundingBoxDelete - 1].y > 0)
                                {
                                    boundingBoxEdit = true;
                                    businessSystem.socketModels[selectBoundingBoxDelete - 1].y -= businessSystem.resizingOfBoundingBoxWeight;
                                    businessSystem.socketModels[selectBoundingBoxDelete - 1].Height += businessSystem.resizingOfBoundingBoxWeight;
                                    drawBoundingBox();
                                    boundingBoxSelect = 0;
                                }
                            }
                        }

                        else
                        {
                            if (businessSystem.socketModels[selectBoundingBoxDelete - 1].y > 0)
                            {
                                boundingBoxEdit = true;
                                businessSystem.socketModels[selectBoundingBoxDelete - 1].Height -= businessSystem.resizingOfBoundingBoxWeight;
                                drawBoundingBox();
                                boundingBoxSelect = 0;
                            }
                        }
                    }
                    else if (e.Key == Key.Down)
                    {
                        if (!editBoundingBoxUp)
                        {
                            if (!editBoundingBoxDown)
                            {
                                if (businessSystem.socketModels[selectBoundingBoxDelete - 1].y > 0)
                                {
                                    boundingBoxEdit = true;
                                    businessSystem.socketModels[selectBoundingBoxDelete - 1].y += businessSystem.resizingOfBoundingBoxWeight;
                                    drawBoundingBox();
                                    boundingBoxSelect = 0;
                                }
                            }
                            else
                            {
                                if (businessSystem.socketModels[selectBoundingBoxDelete - 1].y > 0)
                                {
                                    boundingBoxEdit = true;
                                    businessSystem.socketModels[selectBoundingBoxDelete - 1].Height += businessSystem.resizingOfBoundingBoxWeight;
                                    drawBoundingBox();
                                    boundingBoxSelect = 0;
                                }
                            }
                        }

                        else
                        {
                            if (businessSystem.socketModels[selectBoundingBoxDelete - 1].y > 0)
                            {
                                boundingBoxEdit = true;
                                businessSystem.socketModels[selectBoundingBoxDelete - 1].y += businessSystem.resizingOfBoundingBoxWeight;
                                businessSystem.socketModels[selectBoundingBoxDelete - 1].Height -= businessSystem.resizingOfBoundingBoxWeight;
                                drawBoundingBox();
                                boundingBoxSelect = 0;
                            }
                        }
                    }
                    else if (e.Key == Key.Escape)
                    {
                        if (numberMask != -1)
                        {
                            canvasMask.Children.RemoveAt(numberMask);
                            numberMask = -1;
                        }
                        else
                        {
                            frameHolderMouseMove = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            maskDelete = false;
        }

        private void SetLensPos_1(object sender, MouseButtonEventArgs e)
        {

            if (live)
            {
                //focus += 1;
                //try
                //{
                //    ProductionObject.dinoLiteSDK.SetLensPos(0, focus);
                //    Thread.Sleep(50);
                //    Thread.Sleep(0);
                //}
                //catch
                //{
                //}
            }
            else
            {
                try
                {
                    System.Windows.Point point = e.GetPosition(canvas);
                    double WidthR = (businessSystem.socketModels[selectBoundingBoxDelete - 1].Width * w) / ProductionObject.ResolutionWidth;
                    double HeightR = (businessSystem.socketModels[selectBoundingBoxDelete - 1].Height * h) / ProductionObject.ResolutionHeight;
                    double xR = (businessSystem.socketModels[selectBoundingBoxDelete - 1].x * w) / ProductionObject.ResolutionWidth;
                    double yR = (businessSystem.socketModels[selectBoundingBoxDelete - 1].y * h) / ProductionObject.ResolutionHeight;

                    if (!editBoundingBoxUp)
                    {
                        if (point.X >= ((xR + (WidthR / 2) - 6)) && point.X <= ((xR + (WidthR / 2) + 6)) && point.Y >= (yR - 14) && point.Y <= (yR + 14))
                        //    if (point.X >= ((xR + (WidthR / 2) - 3)) && point.X <= ((xR + (WidthR / 2) + 3)) && point.Y >= yR + ((HeightR / 2) - 6) && point.Y <= yR + ((HeightR / 2) + 3))
                        {
                            System.Windows.Shapes.Rectangle maskUp = new System.Windows.Shapes.Rectangle
                            {
                                Stroke = System.Windows.Media.Brushes.Green,
                                //Fill = System.Windows.Media.Brushes.Green,
                                StrokeThickness = 2,
                                Width = 12,
                                Height = 12
                            };
                            Canvas.SetLeft(maskUp, xR + (WidthR / 2));
                            Canvas.SetTop(maskUp, yR - 5);
                            canvas.Children.RemoveAt(boundingBoxSelect - 3);
                            canvas.Children.Insert(boundingBoxSelect - 3, maskUp);

                            editBoundingBoxUp = true;
                        }
                    }
                    else
                    {
                        if (point.X >= ((xR + (WidthR / 2) - 6)) && point.X <= ((xR + (WidthR / 2) + 6)) && point.Y >= (yR - 14) && point.Y <= (yR + 14))
                        {
                            System.Windows.Shapes.Rectangle maskUp = new System.Windows.Shapes.Rectangle
                            {
                                Stroke = System.Windows.Media.Brushes.Red,
                                //Fill = System.Windows.Media.Brushes.Green,
                                StrokeThickness = 2,
                                Width = 12,
                                Height = 12
                            };
                            Canvas.SetLeft(maskUp, xR + (WidthR / 2));
                            Canvas.SetTop(maskUp, yR - 5);
                            canvas.Children.RemoveAt(boundingBoxSelect - 3);
                            canvas.Children.Insert(boundingBoxSelect - 3, maskUp);

                            editBoundingBoxUp = false;
                        }
                    }

                    if (!editBoundingBoxDown)
                    {
                        if (point.X >= ((xR + (WidthR / 2) - 6)) && point.X <= ((xR + (WidthR / 2) + 6)) && point.Y >= (yR + HeightR - 14) && point.Y <= (yR + HeightR + 14))
                        {
                            System.Windows.Shapes.Rectangle maskDown = new System.Windows.Shapes.Rectangle
                            {
                                Stroke = System.Windows.Media.Brushes.Green,
                                //Fill = System.Windows.Media.Brushes.Green,
                                StrokeThickness = 2,
                                Width = 12,
                                Height = 12
                            };

                            Canvas.SetLeft(maskDown, xR + (WidthR / 2));
                            Canvas.SetTop(maskDown, yR + HeightR - 7);
                            canvas.Children.RemoveAt(boundingBoxSelect - 2);
                            canvas.Children.Insert(boundingBoxSelect - 2, maskDown);

                            editBoundingBoxDown = true;
                        }
                    }
                    else
                    {
                        if (point.X >= ((xR + (WidthR / 2) - 6)) && point.X <= ((xR + (WidthR / 2) + 6)) && point.Y >= (yR + HeightR - 14) && point.Y <= (yR + HeightR + 14))
                        {
                            System.Windows.Shapes.Rectangle maskDown = new System.Windows.Shapes.Rectangle
                            {
                                Stroke = System.Windows.Media.Brushes.Red,
                                //Fill = System.Windows.Media.Brushes.Green,
                                StrokeThickness = 2,
                                Width = 12,
                                Height = 12
                            };
                            Canvas.SetLeft(maskDown, xR + (WidthR / 2));
                            Canvas.SetTop(maskDown, yR + HeightR - 7);
                            canvas.Children.RemoveAt(boundingBoxSelect - 2);
                            canvas.Children.Insert(boundingBoxSelect - 2, maskDown);

                            editBoundingBoxDown = false;
                        }
                    }

                    if (!editBoundingBoxLeft)
                    {
                        if (point.X >= (xR - 14) && point.X <= (xR + 14) && point.Y >= (yR + (HeightR / 2) - 6) && point.Y <= (yR + (HeightR / 2) + 6))
                        {
                            System.Windows.Shapes.Rectangle maskLeft = new System.Windows.Shapes.Rectangle
                            {
                                Stroke = System.Windows.Media.Brushes.Green,
                                //Fill = System.Windows.Media.Brushes.Green,
                                StrokeThickness = 2,
                                Width = 12,
                                Height = 12
                            };
                            Canvas.SetLeft(maskLeft, xR - 5);
                            Canvas.SetTop(maskLeft, yR + (HeightR / 2));
                            canvas.Children.RemoveAt(boundingBoxSelect - 1);
                            canvas.Children.Insert(boundingBoxSelect - 1, maskLeft);

                            editBoundingBoxLeft = true;
                        }
                    }
                    else
                    {
                        if (point.X >= (xR - 14) && point.X <= (xR + 14) && point.Y >= (yR + (HeightR / 2) - 6) && point.Y <= (yR + (HeightR / 2) + 6))
                        {
                            System.Windows.Shapes.Rectangle maskLeft = new System.Windows.Shapes.Rectangle
                            {
                                Stroke = System.Windows.Media.Brushes.Red,
                                //Fill = System.Windows.Media.Brushes.Green,
                                StrokeThickness = 2,
                                Width = 12,
                                Height = 12
                            };
                            Canvas.SetLeft(maskLeft, xR - 5);
                            Canvas.SetTop(maskLeft, yR + (HeightR / 2));
                            canvas.Children.RemoveAt(boundingBoxSelect - 1);
                            canvas.Children.Insert(boundingBoxSelect - 1, maskLeft);

                            editBoundingBoxLeft = false;
                        }
                    }

                    if (!editBoundingBoxRight)
                    {
                        if (point.X >= (xR + WidthR - 14) && point.X <= (xR + WidthR + 14) && point.Y >= (yR + (HeightR / 2) - 6) && point.Y <= (yR + (HeightR / 2) + 6))
                        {
                            System.Windows.Shapes.Rectangle maskRight = new System.Windows.Shapes.Rectangle
                            {
                                Stroke = System.Windows.Media.Brushes.Green,
                                //Fill = System.Windows.Media.Brushes.Green,
                                StrokeThickness = 2,
                                Width = 12,
                                Height = 12
                            };
                            Canvas.SetLeft(maskRight, xR + WidthR - 7);
                            Canvas.SetTop(maskRight, yR + (HeightR / 2));
                            canvas.Children.RemoveAt(boundingBoxSelect);
                            canvas.Children.Insert(boundingBoxSelect, maskRight);

                            editBoundingBoxRight = true;
                        }
                    }
                    else
                    {
                        if (point.X >= (xR + WidthR - 14) && point.X <= (xR + WidthR + 14) && point.Y >= (yR + (HeightR / 2) - 6) && point.Y <= (yR + (HeightR / 2) + 6))
                        {
                            System.Windows.Shapes.Rectangle maskRight = new System.Windows.Shapes.Rectangle
                            {
                                Stroke = System.Windows.Media.Brushes.Red,
                                //Fill = System.Windows.Media.Brushes.Green,
                                StrokeThickness = 2,
                                Width = 12,
                                Height = 12
                            };
                            Canvas.SetLeft(maskRight, xR + WidthR - 7);
                            Canvas.SetTop(maskRight, yR + (HeightR / 2));
                            canvas.Children.RemoveAt(boundingBoxSelect);
                            canvas.Children.Insert(boundingBoxSelect, maskRight);

                            editBoundingBoxRight = false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
            e.Handled = true;
        }

        private void SetLensPos_2(object sender, MouseButtonEventArgs e)
        {
            //if(live)
            //{
            //    focus -= 1;
            //    //ProductionObject.dinoLiteSDK.LightOn(1);
            //    ProductionObject.dinoLiteSDK.SetLensPos(0, focus);
            //    Thread.Sleep(50);
            //    Thread.Sleep(0);
            //}
        }

        private void discard_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {

        }

        private void totalNumberBristlesT_TextChanged(object sender, TextChangedEventArgs e)
        {
            //totalNumberBristles_T = totalNumberBristlesT.Text;
        }

        private void sliderExpoure_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }

        private void exposure_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (checkBoxExpoure.IsChecked == false)
            {
                applyExposureValue();
            }
        }

        private void applyExposureValue()
        {
            if (!startNotConfigureCamera)
            {
                businessSystem.cameraSettingsModel.Exposure = int.Parse(expureValue.Text);
                Thread.Sleep(50);
            }
        }

        private void slValue_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }

        private void controlGeneral_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        /// <summary>
        /// Exposure time control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void applyExp_Click(object sender, RoutedEventArgs e)
        {
            applyExp_();
        }

        private void applyExp_()
        {
            if (!startNotConfigureCamera)
            {
                businessSystem.cameraSettingsModel.Exposure = double.Parse(expureValue.Text);
                ProductionObject.dinoLiteSDK.SetAutoExposure(ProductionObject.VideoDeviceIndex, 0);
                //ProductionObject.dinoLiteSDK.SetExposureStability(ProductionObject.VideoDeviceIndex, 1);
                ProductionObject.dinoLiteSDK.SetExposureValue(ProductionObject.VideoDeviceIndex, (int)businessSystem.cameraSettingsModel.Exposure);
                Thread.Sleep(50);
            }
        }

        private void accumulated__TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void accumulated_Click(object sender, RoutedEventArgs e)
        {
            if (GridMenuRight_Accumulated.Visibility == Visibility.Visible)
            {
                GridMenuRight_Accumulated.Visibility = Visibility.Collapsed;
                Thickness thickness = new Thickness(0, 50, 450, 0);
                calculator.Margin = thickness;
            }
            else
            {
                GridMenuRight_Accumulated.Visibility = Visibility.Visible;
                Thickness thickness = new Thickness(0, 50, 607, 0);
                calculator.Margin = thickness;
            }
        }

        /// <summary>
        /// Enable live
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void liveOn_Click(object sender, RoutedEventArgs e)
        {
            if (liveOn_)
            {
                liveOn_ = false;

                liveOn.Background = System.Windows.Media.Brushes.Red;
                //canvasMask.Visibility = Visibility.Visible;
                viewSDKImage.Visibility = Visibility.Collapsed;
                viewSDK.Visibility = Visibility.Visible;
                viewSDK_.Visibility = Visibility.Visible;
                analysis_1.IsEnabled = false;
            }
            else
            {
                liveOn_ = true;
                //canvasMask.Visibility = Visibility.Collapsed;
                liveOn.Background = System.Windows.Media.Brushes.Green;
                viewSDKImage.Visibility = Visibility.Visible;
                viewSDK.Visibility = Visibility.Collapsed;
                viewSDK_.Visibility = Visibility.Collapsed;

                analysis_1.IsEnabled = true;
            }
        }

        private void MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();

        }

        private void buttonWarningNOK_Click(object sender, RoutedEventArgs e)
        {            
            warning.Visibility = Visibility.Collapsed;

            if (position == "N")
            {
                StartLoadingWait(false);
                
                live = false;

                Views.Report report = new Report(maximized, businessSystem, this);
                report.Show();

                resetValue();
                this.Hide();
            }
        }

        private void nominalNBristleN_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void nominalNBristle_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                ProductionObject.nominalBristle = int.Parse(nominalNBristle.Text);
            }
            catch
            {
            }
        }

        private void MouseMove(object sender, MouseEventArgs e)
        {

        }

        private void buttonWarningOK_Click(object sender, RoutedEventArgs e)
        {           
            warning.Visibility = Visibility.Collapsed;
            //reason.Text;
            saveImageToDatabase();

            if (position == "N")
            {
                warning.Visibility = Visibility.Visible;
                StartLoadingWait(false);
                live = false;
                Views.Report report = new Report(maximized, businessSystem, this);
                report.Show();
                resetValue();
                this.Hide();
            }
        }

        /// <summary>
        /// Selection of camera settings
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cameraConfigurationSelection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            businessSystem.generalSettings.CurrentCameraConfiguration = (string)cameraConfigurationSelection.SelectedItem;

            foreach (var item in businessSystem.cameraSettingsModels)
            {
                if (item.Name == businessSystem.generalSettings.CurrentCameraConfiguration)
                {
                    businessSystem.cameraSettingsModel = item;
                }
            }

            startNotConfigureCameraInit = false;
            cameraConfigure();
            businessSystem.generalController.SaveJson(businessSystem.generalSettings, businessSystem.localRepository + @"\generalSettings.json");
        }

        /// <summary>
        /// Application of the settings save locally from the camera
        /// </summary>
        private void cameraConfigure()
        {
            if (!startNotConfigureCameraInit && businessSystem.generalSettings.MissingCamera)
            {
                ledBrightness.SelectedIndex = businessSystem.cameraSettingsModel.LedBrightness;
                ProductionObject.dinoLiteSDK.SetFLCLevel(ProductionObject.VideoDeviceIndex, businessSystem.cameraSettingsModel.LedBrightness);
                Thread.Sleep(50);

                //expureValue.Text = businessSystem.cameraSettingsModel.Exposure.ToString();
                //ProductionObject.dinoLiteSDK.SetAutoExposure(ProductionObject.VideoDeviceIndex, 0);
                //ProductionObject.dinoLiteSDK.SetExposureStability(ProductionObject.VideoDeviceIndex, 1);
                //ProductionObject.dinoLiteSDK.SetExposureValue(ProductionObject.VideoDeviceIndex, (int)businessSystem.cameraSettingsModel.Exposure);
                //Thread.Sleep(50);

                brightness_.Value = businessSystem.cameraSettingsModel.Brightness;
                ProductionObject.dinoLiteSDK.set_VideoProcAmp(ProductionObject.VideoDeviceIndex, businessSystem.cameraSettingsModel.Brightness);
                Thread.Sleep(50);

                contrast.Value = businessSystem.cameraSettingsModel.Contrast;
                ProductionObject.dinoLiteSDK.set_VideoProcAmp(1, businessSystem.cameraSettingsModel.Contrast);
                Thread.Sleep(50);

                hue.Value = businessSystem.cameraSettingsModel.Hue;
                ProductionObject.dinoLiteSDK.set_VideoProcAmp(2, businessSystem.cameraSettingsModel.Hue);
                Thread.Sleep(50);

                saturation.Value = businessSystem.cameraSettingsModel.Saturation;
                ProductionObject.dinoLiteSDK.set_VideoProcAmp(3, businessSystem.cameraSettingsModel.Saturation);
                Thread.Sleep(50);

                sharpness.Value = businessSystem.cameraSettingsModel.Sharpness;
                ProductionObject.dinoLiteSDK.set_VideoProcAmp(4, businessSystem.cameraSettingsModel.Sharpness);
                Thread.Sleep(50);

                gamma.Value = businessSystem.cameraSettingsModel.Gamma;
                ProductionObject.dinoLiteSDK.set_VideoProcAmp(5, businessSystem.cameraSettingsModel.Gamma);
                Thread.Sleep(50);

                whiteBalanceRed.Value = businessSystem.cameraSettingsModel.AWBR;
                ProductionObject.dinoLiteSDK.FreezeAWB(ProductionObject.VideoDeviceIndex, 0);
                Thread.Sleep(50);
                ProductionObject.dinoLiteSDK.SetAWBR(ProductionObject.VideoDeviceIndex, businessSystem.cameraSettingsModel.AWBR);
                Thread.Sleep(50);
                ProductionObject.dinoLiteSDK.FreezeAWB(ProductionObject.VideoDeviceIndex, 1);
                Thread.Sleep(50);

                whiteBalanceGreen.Value = businessSystem.cameraSettingsModel.AWBG;
                ProductionObject.dinoLiteSDK.FreezeAWB(ProductionObject.VideoDeviceIndex, 0);
                Thread.Sleep(50);
                ProductionObject.dinoLiteSDK.SetAWBG(ProductionObject.VideoDeviceIndex, businessSystem.cameraSettingsModel.AWBG);
                Thread.Sleep(50);
                ProductionObject.dinoLiteSDK.FreezeAWB(ProductionObject.VideoDeviceIndex, 1);
                Thread.Sleep(50);

                whiteBalanceBlue.Value = businessSystem.cameraSettingsModel.AWBB;
                ProductionObject.dinoLiteSDK.FreezeAWB(ProductionObject.VideoDeviceIndex, 0);
                Thread.Sleep(50);
                ProductionObject.dinoLiteSDK.SetAWBB(ProductionObject.VideoDeviceIndex, businessSystem.cameraSettingsModel.AWBB);
                Thread.Sleep(50);
                ProductionObject.dinoLiteSDK.FreezeAWB(ProductionObject.VideoDeviceIndex, 1);
                Thread.Sleep(50);

                switch (businessSystem.cameraSettingsModel.Mirror)
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
                //ProductionObject.dinoLiteSDK.SetMirror(ProductionObject.VideoDeviceIndex, (int)businessSystem.cameraSettingsModel.Mirror);
                //Thread.Sleep(280);

                //switch (businessSystem.cameraSettingsModel.Negative)
                //{
                //    case 0:
                //        negative.IsChecked = false;
                //        break;
                //    case 1:
                //        negative.IsChecked = true;
                //        break;
                //}
                //ProductionObject.dinoLiteSDK.SetNegative(ProductionObject.VideoDeviceIndex, businessSystem.cameraSettingsModel.Negative);
                //Thread.Sleep(50);

                //switch (businessSystem.cameraSettingsModel.Monochrome)
                //{
                //    case 0:
                //        monochrome.IsChecked = false;
                //        break;
                //    case 1:
                //        monochrome.IsChecked = true;
                //        break;
                //}
                //ProductionObject.dinoLiteSDK.SetMonochrome(ProductionObject.VideoDeviceIndex, businessSystem.cameraSettingsModel.Monochrome);
                //Thread.Sleep(50);

                //ProductionObject.dinoLiteSDK.SetAutoExposure(ProductionObject.VideoDeviceIndex, businessSystem.cameraSettingsModel.AutomaticExposure);
                //ProductionObject.dinoLiteSDK.SetExposureStability(ProductionObject.VideoDeviceIndex, businessSystem.cameraSettingsModel.AutomaticExposure);
                //Thread.Sleep(50);

                //ProductionObject.dinoLiteSDK.SetFLCSwitch(ProductionObject.VideoDeviceIndex, businessSystem.cameraSettingsModel.LedsOn);
                //Thread.Sleep(280);

                focusUpdate();

                startNotConfigureCameraInit = true;
            }
        }

        private void buttonWarningSave_Click(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// Save settings camera settings
        /// </summary>
        private void saveCameraConfig()
        {
            businessSystem.cameraSettingsModel.Name = cameraConfigureName.Text;
            businessSystem.cameraSettingsModel.SKU = skuSelect.SelectedItem.ToString();
            businessSystem.generalSettings.Test = testSelect.SelectedItem.ToString();
            businessSystem.generalSettings.Area = areaSelect.SelectedItem.ToString();
            businessSystem.cameraSettingsModel.Equipment = equipamentSelect.SelectedItem.ToString();
            businessSystem.generalSettings.BatchLote = batchLoteT.Text;
            businessSystem.generalSettings.CurrentCameraConfiguration = businessSystem.cameraSettingsModel.Name;
            businessSystem.generalController.SaveJson(businessSystem.cameraSettingsModel, businessSystem.localRepository + @"\cameraSettings.json", true);
            businessSystem.cameraSettingsModels.Add(businessSystem.cameraSettingsModel);
            cameraConfigurationSelection.Items.Add(businessSystem.cameraSettingsModel.Name);
            cameraConfigurationSelection.SelectedItem = businessSystem.cameraSettingsModel.Name;
        }

        private void buttonWarningCancel_Click(object sender, RoutedEventArgs e)
        {

        }

        private void buttonWarningSave_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            cameraConfigure_.Visibility = Visibility.Collapsed;
            warningText1.Visibility = Visibility.Visible;
            warningButton1.Visibility = Visibility.Visible;
            cameraConfigure_.Visibility = Visibility.Collapsed;

            if (!liveOn_) viewSDK.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Remove a selected camera configuration
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonRemove_Click(object sender, RoutedEventArgs e)
        {
            if (businessSystem.generalSettings.CurrentCameraConfiguration != "Default")
            {
                if (File.Exists(businessSystem.localRepository + @"\cameraSettings.json"))
                {
                    File.Delete(businessSystem.localRepository + @"\cameraSettings.json");
                }           
                foreach (var item in businessSystem.cameraSettingsModels)
                {
                    if (item.Name == businessSystem.generalSettings.CurrentCameraConfiguration)
                    {
                        cameraConfigurationSelection.Items.Remove(businessSystem.generalSettings.CurrentCameraConfiguration);
                    }
                    else
                    {
                        businessSystem.generalController.SaveJson(item, businessSystem.localRepository + @"\cameraSettings.json", true);
                    }
                }
                cameraConfigure_.Visibility = Visibility.Collapsed;
            }
            else
            {
                cameraConfigure_.Visibility = Visibility.Collapsed;
                MessageBox.Show("You do not remove the default configuration!");
            }

            if (!liveOn_) viewSDK.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Subscribe to a selected camera configuration
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonOverride_Click(object sender, RoutedEventArgs e)
        {
            if (businessSystem.generalSettings.CurrentCameraConfiguration != "Default")
            {
                if (File.Exists(businessSystem.localRepository + @"\cameraSettings.json"))
                {
                    File.Delete(businessSystem.localRepository + @"\cameraSettings.json");
                }

                foreach (var item in businessSystem.cameraSettingsModels)
                {
                    if (item.Name == businessSystem.generalSettings.CurrentCameraConfiguration)
                    {
                        businessSystem.generalController.SaveJson(businessSystem.cameraSettingsModel, businessSystem.localRepository + @"\cameraSettings.json", true);
                    }
                    else
                    {
                        businessSystem.generalController.SaveJson(item, businessSystem.localRepository + @"\cameraSettings.json", true);
                    }
                }
                cameraConfigure_.Visibility = Visibility.Collapsed;
            }
            else
            {
                cameraConfigure_.Visibility = Visibility.Collapsed;
                MessageBox.Show("Default setting cannot be changed!");
            }

            if (!liveOn_) viewSDK.Visibility = Visibility.Visible;
        }

        private void buttonNew_Click(object sender, RoutedEventArgs e)
        {
            cameraConfigure_.Visibility = Visibility.Collapsed;
            newCameraConfigure.Visibility = Visibility.Visible;
        }

        private void save_Click(object sender, RoutedEventArgs e)
        {
            warning.Visibility = Visibility.Collapsed;
            warningText1.Visibility = Visibility.Visible;
            warningButton1.Visibility = Visibility.Visible;
            newCameraConfigure.Visibility = Visibility.Collapsed;
            saveCameraConfig();

            if (!liveOn_) viewSDK.Visibility = Visibility.Visible;
        }

        private void cancel_Click(object sender, RoutedEventArgs e)
        {
            warning.Visibility = Visibility.Collapsed;
            warningText1.Visibility = Visibility.Visible;
            warningButton1.Visibility = Visibility.Visible;
            newCameraConfigure.Visibility = Visibility.Collapsed;
            newCameraConfigure.Visibility = Visibility.Collapsed;

            if (!liveOn_) viewSDK.Visibility = Visibility.Visible;
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            Views.Help help = new Views.Help(maximized, businessSystem, this);
            help.Hide();
        }

        private void areaSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
