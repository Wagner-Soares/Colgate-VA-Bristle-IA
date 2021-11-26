using APIVision;
using APIVision.Controllers;
using APIVision.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.DataVisualization.Charting;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Bristle.Views
{
    /// <summary>
    /// Lógica interna para NeuralNetworkRetraining.xaml
    /// </summary>
    public partial class NeuralNetworkRetraining : Window
    {
        private static bool maximized = true;
        private BusinessSystem businessSystem;
        private List<int> validationImages = new List<int>();
        private string data = String.Empty;
        private List<int> TrainDatasetId = new List<int>();
        private List<int> ValidationDatasetId = new List<int>();
        private System.Drawing.Image Image_;
        private System.Drawing.Image aux;
        private MemoryStream ms;
        private BitmapImage bi;
        private double w = 0;
        private double h = 0;
        private List<ImageTempModel> selectImageTempModel = new List<ImageTempModel>();
        private System.Windows.Forms.Timer myTimer = new System.Windows.Forms.Timer();
        private System.Windows.Forms.Timer myTimer2 = new System.Windows.Forms.Timer();
        private int cont = 0;
        private bool photoResult = false;
        private List<APIVision.Models.SocketModel> boundingBox = new List<APIVision.Models.SocketModel>();
        private System.Drawing.Image photo_;
        private bool menuBeingUsed = false;
        private int boundingBoxSelect = 0;
        private int selectBoundingBoxDelete = -1;
        private bool editBoundingBoxLeft = false;
        private bool editBoundingBoxRight = false;
        private bool editBoundingBoxUp = false;
        private bool editBoundingBoxDown = false;
        private System.Windows.Shapes.Rectangle mask = new System.Windows.Shapes.Rectangle();
        private System.Windows.Point maskPointStart = new System.Windows.Point();
        private System.Windows.Point maskPointStartMemory = new System.Windows.Point();
        private System.Windows.Point maskPointStopMemory = new System.Windows.Point();
        private bool maskDelete = false;
        private int numberMask = -1;
        private bool frameHolderMouseMove = true;
        private bool boundingBoxEdit = false;
        private List<string> defect = new List<string>();
        private static bool registerBox = true;
        private static int countBox = 0;
        private string boundingBoxSelectType = "undefined";
        private int xCurrent = 0;
        private int yCurrent = 0;
        private int X;
        private int Y;
        private int size_ = 75;
        private static object cellCurrent;
        private List<DataGridRow> row = new List<DataGridRow>();
        private List<DataGridCell> column = new List<DataGridCell>();
        private List<DataGridCell> columnCheckBox = new List<DataGridCell>();
        private List<string> validationImagesPreview = new List<string>();
        private int indexValidationImages = 0;
        private int TuftTempIdCurrent;
        private string name;
        private bool maximized__ = false;
        private Views.AutomaticBristleClassification automaticBristleClassification;
        private Thread threadLoadingWait;
        private Thread threadLoadingWait_;
        private LoadingAnimation loading;
        private bool verySmallMask = false;
        public NeuralNetworkRetraining(bool maximized_, BusinessSystem businessSystem_, Views.AutomaticBristleClassification automaticBristleClassification_)
        {
            StartLoadingWait(true);

            businessSystem = businessSystem_;
            maximized__ = maximized_;
            automaticBristleClassification = automaticBristleClassification_;

            InitializeComponent();

            this.Loaded += NeuralNetworkRetraining_Loaded;

            user_.Text = businessSystem.userSystemCurrent.Name;

            LoadPieChartData1();
            LoadPieChartData2();

            LoadDB();

            //businessSystem.commandS300Model.NewModelId = 0;
            //businessSystem.commandS300Model.OldModelId = 0;
            initSelectModel();

            myTimer.Tick += new EventHandler(photoResult_);
            myTimer.Interval = 50;
            myTimer.Enabled = true;
            myTimer.Start();

            myTimer2.Tick += new EventHandler(InitCheckbox);
            myTimer2.Interval = 300;
            myTimer2.Enabled = true;
            myTimer2.Start();

            if (automaticBristleClassification != null)
            {
                automaticBristleClassification.live = false;
            }
        }
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
                threadLoadingWait_ = new Thread(() => ThreadMethod());
                threadLoadingWait_.SetApartmentState(ApartmentState.STA); //Set the thread to STA
                //threadLoadingWait_.IsBackground = true;
                threadLoadingWait_.Start();
                // ThreadMethod();
            }
        }

        private void NeuralNetworkRetraining_Loaded(object sender, RoutedEventArgs e)
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

            w = (648 * SystemParameters.PrimaryScreenWidth) / 934.5;
            h = (486 * SystemParameters.PrimaryScreenHeight) / 610.5;

            imagePreview.Width = w;
            imagePreview.Height = h;

            imagePreview_.Width = w;
            imagePreview_.Height = h;
            imagePreview_.Stretch = Stretch.Fill;

            validationImageViewer.Width = w;
            validationImageViewer.Height = h;
            validationImageViewer.Stretch = Stretch.Fill;

            canvas.Width = imagePreview_.Width;
            canvas.Height = imagePreview_.Height;

            canvasMask.Width = canvas.Width;
            canvasMask.Height = canvas.Height;

            StartLoadingWait(false);
        }

        private void sizeChanged(object sender, SizeChangedEventArgs e)
        {
            w = (648 * this.ActualWidth) / 934.5;
            h = (486 * this.ActualHeight) / 610.5;

            imagePreview.Width = w;
            imagePreview.Height = h;

            imagePreview_.Width = w;
            imagePreview_.Height = h;
            imagePreview_.Stretch = Stretch.Fill;

            validationImageViewer.Width = w;
            validationImageViewer.Height = h;
            validationImageViewer.Stretch = Stretch.Fill;

            canvas.Width = imagePreview_.Width;
            canvas.Height = imagePreview_.Height;

            canvasMask.Width = canvas.Width;
            canvasMask.Height = canvas.Height;
        }

        /// <summary>
        /// Initializes datagrids
        /// </summary>
        /// <param name="myObject"></param>
        /// <param name="myEventArgs"></param>
        private void InitCheckbox(Object myObject, EventArgs myEventArgs)
        {
            foreach (var cell in dataGridImageAwaiting.Items)
            {
                try
                {
                    DataGridRow row_ = (DataGridRow)dataGridImageAwaiting.ItemContainerGenerator.ContainerFromItem(cell);
                    DataGridCell column_ = dataGridImageAwaiting.Columns[2].GetCellContent(row_).Parent as DataGridCell;
                    DataGridCell columnCheckBox_ = dataGridImageAwaiting.Columns[0].GetCellContent(row_).Parent as DataGridCell;
                    column_.Foreground = System.Windows.Media.Brushes.DarkOrange;
                    column_.Content = "Waiting";
                    column.Add(column_);

                    columnCheckBox_.IsEnabled = false;
                    columnCheckBox.Add(columnCheckBox_);
                }
                catch
                {

                }

            }

            canvas.Children.Clear();
            canvasMask.Children.Clear();

            dataGridImageAwaiting.UpdateLayout();
            UpdatingColumnNames();

            myTimer2.Enabled = false;
            myTimer2.Stop();
        }

        private void UpdatingColumnNames()
        {
            int i = 0;
            try
            {
                foreach (var item in dataGridModel.Columns)
                {
                    switch (i)
                    {
                        case 3:
                            item.Width = 0;
                            item.Visibility = Visibility.Collapsed;
                            break;
                        case 4:
                            item.Width = 100;
                            item.Header = "Error1-F1";
                            break;
                        case 5:
                            item.Width = 100;
                            item.Header = "Error2-F1";
                            break;
                        case 6:
                            item.Width = 100;
                            item.Header = "Error3-F1";
                            break;
                        case 7:
                            item.Width = 100;
                            item.Header = "Ok-F1";
                            break;
                        case 8:
                            item.Width = 150;
                            item.Header = "Localization-F1";
                            break;
                        case 9:
                            item.Width = 0;
                            item.Visibility = Visibility.Collapsed;
                            break;
                    }
                    i++;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("DB: " + e.Message);
            }

            i = 0;
            try
            {
                foreach (var item in dataGridModelSelect.Columns)
                {
                    switch (i)
                    {
                        case 2:
                            item.Width = 0;
                            item.Visibility = Visibility.Collapsed;
                            break;
                        case 3:
                            item.Width = 100;
                            item.Header = "Error1-F1";
                            break;
                        case 4:
                            item.Width = 100;
                            item.Header = "Error2-F1";
                            break;
                        case 5:
                            item.Width = 100;
                            item.Header = "Error3-F1";
                            break;
                        case 6:
                            item.Width = 100;
                            item.Header = "Ok-F1";
                            break;
                        case 7:
                            item.Width = 150;
                            item.Header = "Localization-F1";
                            break;
                        case 8:
                            item.Width = 0;
                            item.Visibility = Visibility.Collapsed;
                            break;
                    }
                    i++;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("DB: " + e.Message);
            }
        }

        /// <summary>
        /// Searching the database for information to popularize datagris
        /// </summary>
        private void LoadDB()
        {
            dataGridDataset.ItemsSource = businessSystem.dataBaseController.listDatasetModel();
            dataGridImageAwaiting.ItemsSource = businessSystem.dataBaseController.listRegistrationWaitingModel();
            dataGridValidationImage.ItemsSource = businessSystem.dataBaseController.listValidationDatasetModel();
            dataGridModel.ItemsSource = businessSystem.dataBaseController.listModelsModel();
        }

        private void LoadPieChartData1()
        {
            ((LineSeries)mcChart1.Series[0]).ItemsSource =
                new KeyValuePair<string, int>[]{
                new KeyValuePair<string,int>(0+"%"+" :precision 1", 70),
                new KeyValuePair<string,int>(2+"%"+" :precision 2", 72),
                new KeyValuePair<string,int>(1+"%"+" :precision 3", 71),
                new KeyValuePair<string,int>(0+"%"+" :precision 4", 90),
                new KeyValuePair<string,int>(1+"%"+" :precision 5", 81),
                new KeyValuePair<string,int>(1+"%"+" :precision 6", 90),
                new KeyValuePair<string,int>(1+"%"+" :precision 7", 93),
                new KeyValuePair<string,int>(0+"%"+" :precision 8", 50) };
        }

        private void LoadPieChartData2()
        {
            ((LineSeries)mcChart2.Series[0]).ItemsSource =
                new KeyValuePair<string, int>[]{
                new KeyValuePair<string,int>(0+"%"+" :recall 1", 80),
                new KeyValuePair<string,int>(2+"%"+" :recall 2", 98),
                new KeyValuePair<string,int>(1+"%"+" :recall 3", 50),
                new KeyValuePair<string,int>(0+"%"+" :recall 4", 80),
                new KeyValuePair<string,int>(1+"%"+" :recall 5", 75),
                new KeyValuePair<string,int>(1+"%"+" :recall 6", 80),
                new KeyValuePair<string,int>(1+"%"+" :recall 7", 20),
                new KeyValuePair<string,int>(0+"%"+" :recall 8", 54) };
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
        }

        private void ButtonAutomaticBristleClassification_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (automaticBristleClassification == null)
                {
                    automaticBristleClassification = new Views.AutomaticBristleClassification(maximized, businessSystem);
                }
                automaticBristleClassification.live = true;
                automaticBristleClassification.Show();
            }
            catch 
            {
                automaticBristleClassification = new Views.AutomaticBristleClassification(maximized, businessSystem);               
                automaticBristleClassification.Show();
            }
            
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
            //Views.NeuralNetworkRetraining neuralNetworkRetraining = new NeuralNetworkRetraining(maximized, businessSystem);
            //neuralNetworkRetraining.Show();
            //this.Close();
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
                Views.GeneralSettings generalSettings = new Views.GeneralSettings(maximized, businessSystem, null);
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
                        Views.GeneralSettings generalSettings = new Views.GeneralSettings(maximized, businessSystem, null);
                        generalSettings.Show();
                        this.Close();
                    }
                }
                MessageBox.Show("Necessary administrative rights!");
            }
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
                if (businessSystem.networkUserModel.NetworkUserGroup != null)
                {
                    //Validate the group that can access this part 
                    foreach (var group in businessSystem.networkUserModel.NetworkUserGroup)
                    {
                        if(group == businessSystem.AD_Admin || group == businessSystem.AD_Quality)
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
                if (businessSystem.networkUserModel.NetworkUserGroup != null)
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

        private void datasetAnalysis_Click_1(object sender, RoutedEventArgs e)
        {
            if (graph.Visibility != Visibility.Visible)
            {
                graph.Visibility = Visibility.Visible;
                dataset.Visibility = Visibility.Collapsed;
            }
            else
            {
                graph.Visibility = Visibility.Collapsed;
                dataset.Visibility = Visibility.Visible;
            }
        }

        private void dataGridImageAwaiting_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //preview imagem
        }

        /// <summary>
        /// Apply config dataset
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void apply_Click(object sender, RoutedEventArgs e)
        {

            addDataset.Visibility = Visibility.Collapsed;
            menuRetrain.Visibility = Visibility.Visible;

            if (nameDataset.Text == "")
            {
                businessSystem.datasetModel.Name = DateTime.UtcNow.ToString();
            }
            else
            {
                businessSystem.datasetModel.Name = nameDataset.Text;
            }
            businessSystem.datasetModel.Historic = DateTime.UtcNow.ToString();
            businessSystem.datasetModel.Type = "";

            businessSystem.registrationWaitingModels = businessSystem.dataBaseController.listRegistrationWaitingModel();

            // tempValidationImages
            for (int i = 0; i < validationImages.Count; i++)
            {
                for (int j = 0; j < businessSystem.registrationWaitingModels.Count; j++)
                {
                    if (validationImages[i] == businessSystem.registrationWaitingModels[j].Id)
                    {
                        validationImages[i] = j;
                    }
                }
            }

            for (int i = 0; i < validationImages.Count; i++)
            {
                businessSystem.validationRegistrationWaitingModels.Add(businessSystem.registrationWaitingModels.ElementAt(validationImages[i]));
            }

            bool validationImages_ = false;
            for (int j = 0; j < businessSystem.registrationWaitingModels.Count; j++)
            {
                for (int i = 0; i < validationImages.Count; i++)
                {
                    if (j == validationImages[i])
                    {
                        validationImages_ = true;
                    }
                }
                if (!validationImages_)
                {
                    businessSystem.datasetRegistrationWaitingModels.Add(businessSystem.registrationWaitingModels.ElementAt(j));
                }
                validationImages_ = false;
            }

            if (businessSystem.validationRegistrationWaitingModels.Count > 0)
            {
                businessSystem.dataBaseController.updateValidationDatasetModel(businessSystem.datasetModel, null, null, businessSystem.validationRegistrationWaitingModels);
            }
            businessSystem.dataBaseController.updateDatasetModel(businessSystem.datasetModel, null, null, businessSystem.datasetRegistrationWaitingModels);

            dataGridDataset.ItemsSource = businessSystem.dataBaseController.listDatasetModel();
            dataGridValidationImage.ItemsSource = businessSystem.dataBaseController.listValidationDatasetModel();

            businessSystem.dataBaseController.updateRegistrationWaitingModel(null, null, null);
            dataGridImageAwaiting.ItemsSource = businessSystem.dataBaseController.listRegistrationWaitingModel();

            businessSystem.validationRegistrationWaitingModels.Clear();
            businessSystem.datasetRegistrationWaitingModels.Clear();
        }

        /// <summary>
        /// Add Dataset
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void addBaseDataset__Click(object sender, RoutedEventArgs e)
        {
            bool releasedForDataset = true;

            foreach (var cell in dataGridImageAwaiting.Items)
            {
                try
                {
                    DataGridRow row_ = (DataGridRow)dataGridImageAwaiting.ItemContainerGenerator.ContainerFromItem(cell);

                    if ((string)column[row_.GetIndex()].Content == "Waiting")
                    {
                        releasedForDataset = false;
                    }
                }
                catch
                {
                }
            }

            if (releasedForDataset && dataGridImageAwaiting.Items.Count != 0)
            {
                addDataset.Visibility = Visibility.Visible;
                menuRetrain.Visibility = Visibility.Collapsed;
            }
            else
            {
                if (dataGridImageAwaiting.Items.Count == 0)
                {
                    MessageBox.Show("There are no images!");
                }
                else
                {
                    MessageBox.Show("There are images without analysis!");
                }
            }
        }

        /// <summary>
        /// Fetched from the database an image
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void openImage_Click(object sender, RoutedEventArgs e)
        {
            preview.Visibility = Visibility.Visible;
            dataset.Visibility = Visibility.Collapsed;
            menuRetrain.Visibility = Visibility.Collapsed;
            menuCuratorship.Visibility = Visibility.Visible;

            datasetAnalysis.IsEnabled = false;
            addBaseDataset_.IsEnabled = false;
            applyModel.IsEnabled = false;
            retrain.IsEnabled = false;

            cellCurrent = dataGridImageAwaiting.SelectedItem;

            RegistrationWaitingModel cell_ = (RegistrationWaitingModel)cellCurrent;
            List<BristleTempModel> selectBristleTempModel = new List<BristleTempModel>();

            selectBristleTempModel.Clear();
            selectImageTempModel.Clear();

            foreach (var TuftTemp in businessSystem.dataBaseController.listTuftTempModel())
            {
                if (TuftTemp.Id == cell_.Id)
                {
                    TuftTempIdCurrent = TuftTemp.Id;
                    selectBristleTempModel = businessSystem.dataBaseController.listBristleTempModel(TuftTempIdCurrent);
                    selectImageTempModel = businessSystem.dataBaseController.listImageTempModel(TuftTempIdCurrent);
                }
            }

            try
            {
                string nameNote = selectImageTempModel[selectImageTempModel.Count - 1].Path;

                string[] aux = nameNote.Split('@');

                if (aux.Count() > 1)
                {
                    name = aux[0];
                    note.Text = aux[1];
                }
                else
                {
                    name = aux[0];
                }
            }
            catch 
            {
                MessageBox.Show("Problems loading the image!");
            }

            try 
            { 
                string path = businessSystem.generalSettings.NamePrefix + name;
                imagePreview_.Source = BitmapFromUri(new Uri(path));
                imagePreview_.Stretch = Stretch.Fill;
                photo_ = Bitmap.FromFile(path);
            }
            catch
            {
                MessageBox.Show("Problems loading the image!");
                name = @"";
                imagePreview_.Source = null;
            }

            businessSystem.socketModels.Clear();
            boundingBox.Clear();
            canvas.Children.Clear();
            canvasMask.Children.Clear();
            canvas.UpdateLayout();
            canvasMask.UpdateLayout();
        }

        /// <summary>
        /// Opening an image
        /// </summary>
        /// <param name="source"></param>
        /// <returns>bitmap</returns>
        public static BitmapImage BitmapFromUri(Uri source)
        {
            try
            {              
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = source;
                bitmap.CacheOption = BitmapCacheOption.OnLoad; //Default;// OnLoad;
                bitmap.EndInit();
                return bitmap;               
            }
            catch
            {
                MessageBox.Show("File not found");
                return null;
            }
        }

        private void selectAnalyses_Click(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// Dataset selection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void selectDatasets_Click(object sender, RoutedEventArgs e)
        {

            var cell = dataGridDataset.SelectedItem;
            DatasetModel cell_ = (DatasetModel)cell;
            int remove = -1;

            if (TrainDatasetId.Count != 0)
            {
                foreach (var imageId in TrainDatasetId)
                {
                    if (cell_.Id == imageId)
                    {
                        remove = cell_.Id;
                    }
                }

                if (remove >= 0)
                {
                    TrainDatasetId.Remove(cell_.Id);
                }
                else
                {
                    TrainDatasetId.Add(cell_.Id);
                }
            }
            else
            {
                TrainDatasetId.Add(cell_.Id);
            }
        }

        /// <summary>
        /// Retraining method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void retrain_Click_1(object sender, RoutedEventArgs e)
        {
            businessSystem.commandS200Model.TrainDatasetId = TrainDatasetId.ToArray();
            businessSystem.commandS200Model.ValidationDatasetId = ValidationDatasetId.ToArray();
            string command = businessSystem.generalController.SaveJson(businessSystem.commandS200Model, businessSystem.localRepository + @"\commandS200.json");
            businessSystem.SendCommand("S200", businessSystem.generalSettings.IpTrain, businessSystem.generalSettings.PortTrain, command);
            businessSystem.data = "";
            Thread.Sleep(1000);
            dataGridModel.ItemsSource = businessSystem.dataBaseController.listModelsModel();
        }

        /// <summary>
        /// Image that will be used for validation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void selectValidationImages_Click(object sender, RoutedEventArgs e)
        {
            var cell = dataGridImageAwaiting.SelectedItem;

            DataGridRow row = (DataGridRow)dataGridImageAwaiting.ItemContainerGenerator.ContainerFromItem(cell);

            try
            {
                if ((string)column[row.GetIndex()].Content == "Analyzed")
                {
                    RegistrationWaitingModel cell_ = (RegistrationWaitingModel)cell;
                    FrameworkElement frameworkElement = dataGridImageAwaiting.Columns[0].GetCellContent(cell);

                    bool teste = frameworkElement.IsEnabled;

                    int remove = -1;

                    if (validationImages.Count != 0)
                    {
                        foreach (var imageId in validationImages)
                        {
                            if (cell_.Id == imageId)
                            {
                                remove = cell_.Id;
                            }
                        }

                        if (remove >= 0)
                        {
                            validationImages.Remove(cell_.Id);
                        }
                        else
                        {
                            validationImages.Add(cell_.Id);
                        }
                    }
                    else
                    {
                        validationImages.Add(cell_.Id);
                    }
                }
                else
                {
                }
            }
            catch
            {

            }

        }

        /// <summary>
        /// Image that will be used for validation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void selectValidationImages_Click_1(object sender, RoutedEventArgs e)
        {
            var cell = dataGridValidationImage.SelectedItem;
            ValidationDatasetModel cell_ = (ValidationDatasetModel)cell;



            int remove = -1;

            if (ValidationDatasetId.Count != 0)
            {
                foreach (var imageId in ValidationDatasetId)
                {
                    if (cell_.Id == imageId)
                    {
                        remove = cell_.Id;
                    }
                }

                if (remove >= 0)
                {
                    ValidationDatasetId.Remove(cell_.Id);
                }
                else
                {
                    ValidationDatasetId.Add(cell_.Id);
                }
            }
            else
            {
                ValidationDatasetId.Add(cell_.Id);
            }
        }

        /// <summary>
        /// Model selection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void selectModel_Click(object sender, RoutedEventArgs e)
        {
            var cell = dataGridModel.SelectedItem;
            ModelsModel cell_ = (ModelsModel)cell;
            businessSystem.commandS300Model.OldModelId = businessSystem.commandS300Model.NewModelId;
            businessSystem.commandS300Model.NewModelId = cell_.Id;
            List<ModelsModel> cell_s = new List<ModelsModel>();
            cell_s.Add(cell_);
            dataGridModelSelect.ItemsSource = cell_s;
            dataGridModel.ItemsSource = businessSystem.dataBaseController.listModelsModel();
            UpdatingColumnNames();
        }

        /// <summary>
        /// Initializes template
        /// </summary>
        private void initSelectModel()
        {
            List<ModelsModel> cell_s = new List<ModelsModel>();
            List<ModelsModel> cell_s_ = businessSystem.dataBaseController.listModelsModel();

            foreach (ModelsModel cell_ in cell_s_)
            {
                if (cell_.Id == businessSystem.commandS300Model.NewModelId)
                {
                    cell_s.Add(cell_);
                    dataGridModelSelect.ItemsSource = cell_s;
                }
            }
        }

        /// <summary>
        /// Apply template
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void applyModel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                businessSystem.generalController.SaveJson2(businessSystem.commandS300Model, businessSystem.localRepository + @"\commandS300.json");
                object obj = businessSystem.generalController.ReadJson(businessSystem.localRepository + @"\commandS300.json");
                businessSystem.generalSettings.Model = businessSystem.generalController.SaveJson(obj);
                businessSystem.SendCommand("S300", businessSystem.generalSettings.Model);
                businessSystem.data = "";

                //photoResult = true;
                warning.Visibility = Visibility.Visible;
                //teste
                startServeTCP();
            }
            catch
            {
                throw;
            }
        }

        private void startServeTCP()
        {
            //businessSystem.StartComunication(businessSystem.generalSettings.IpInterface, businessSystem.generalSettings.PortInterface);//("0.0.0.0", 5050); //"10.167.1.212"           
        }

        private void applyPrefix_Click(object sender, RoutedEventArgs e)
        {
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
        }

        /// <summary>
        /// Save curator analysis results
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void validate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                saveDB();

                preview.Visibility = Visibility.Collapsed;
                dataset.Visibility = Visibility.Visible;

                dataGridImageAwaiting.UpdateLayout();

                DataGridRow row = (DataGridRow)dataGridImageAwaiting.ItemContainerGenerator.ContainerFromItem(cellCurrent);

                column[row.GetIndex()].Content = "Analyzed";
                column[row.GetIndex()].Foreground = System.Windows.Media.Brushes.DarkGreen;
                columnCheckBox[row.GetIndex()].IsEnabled = true;

                datasetAnalysis.IsEnabled = true;
                addBaseDataset_.IsEnabled = true;
                applyModel.IsEnabled = true;
                retrain.IsEnabled = true;
            }
            catch
            {

            }
            menuCuratorship.Visibility = Visibility.Collapsed;
            menuRetrain.Visibility = Visibility.Visible;
        }

        private void dataGridModelSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();

        }

        /// <summary>
        /// Remove image
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void remove_Click(object sender, RoutedEventArgs e)
        {
            var cell = dataGridImageAwaiting.SelectedItem;
            RegistrationWaitingModel cell_ = (RegistrationWaitingModel)cell;
            businessSystem.dataBaseController.updateRegistrationWaitingModel(null, null, cell_);
            //dataGridImageAwaiting.ItemsSource = businessSystem.dataBaseController.listRegistrationWaitingModel();

            DataGridRow row = (DataGridRow)dataGridImageAwaiting.ItemContainerGenerator.ContainerFromItem(cell);
            column[row.GetIndex()].Content = "Removed";
            column[row.GetIndex()].Foreground = System.Windows.Media.Brushes.DarkRed;
            row.IsEnabled = false;

            preview.Visibility = Visibility.Collapsed;
            dataset.Visibility = Visibility.Visible;

            imagePreview_.Source = null;
            string pathImage = selectImageTempModel[selectImageTempModel.Count - 1].Path;
            lock (this)
            {
                if (photo_ != null) photo_.Dispose();

                try
                {
                    File.Delete(@businessSystem.generalSettings.NamePrefix + "\\" + pathImage);
                }
                catch
                {

                }

            }
            datasetAnalysis.IsEnabled = true;
            addBaseDataset_.IsEnabled = true;
            applyModel.IsEnabled = true;
            retrain.IsEnabled = true;

            menuCuratorship.Visibility = Visibility.Collapsed;
            menuRetrain.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Checking the image by the curator
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void analyze_Click(object sender, RoutedEventArgs e)
        {
            businessSystem.SendCommand("S100", (Bitmap)photo_);
            businessSystem.data = "";

            photoResult = true;
        }

        /// <summary>
        /// Monitoring the return of the verification made by the curator
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
                            drawBoundingBox();
                            businessSystem.data = "";
                        }
                
                        photoResult = false;
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
                if (businessSystem.data != "*")
                {
                    if (businessSystem.data == "OK")
                    {
                        warningText1.Visibility = Visibility.Collapsed;
                        warningText2.Visibility = Visibility.Visible;
                        yesButton.Visibility = Visibility.Visible;
                        noButton.Visibility = Visibility.Collapsed;
                        businessSystem.data = "";
                    }
                    else if (businessSystem.data == "NOK")
                    {
                        warningText1.Visibility = Visibility.Collapsed;
                        warningText3.Visibility = Visibility.Visible;
                        yesButton.Visibility = Visibility.Visible;
                        noButton.Visibility = Visibility.Collapsed;
                        businessSystem.data = "";
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
            businessSystem.generalController.CMD("./StartSocketAI.vbs");
            businessSystem.SendCommand("S300", businessSystem.generalSettings.Model);
            businessSystem.data = "";
            businessSystem.SendCommand("S100", (Bitmap)photo_);
            businessSystem.data = "";
        }

        /// <summary>
        /// Drawing the bounding box
        /// </summary>
        private void drawBoundingBox()
        {
            try
            {
                int contDefect = 0;

                canvas.Children.Clear();
                boundingBox.Clear(); //teste

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
                        textBlock.Foreground = new SolidColorBrush(Colors.DarkRed);

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
                    countBox = canvas.Children.Count;
                    canvas.UpdateLayout();
                    canvas.UpdateDefaultStyle();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            frameHolderMouseMove = true;
        }

        /// <summary>
        /// Moving mouse event
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
                    if (e.LeftButton == MouseButtonState.Pressed)
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

                    if (!verySmallMask)
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
        /// Keyboard Control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void keyboardControl(object sender, KeyEventArgs e)
        {
            maskDelete = true;

            if (frameHolderMouseMove)
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

            e.Handled = true;
        }
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

        private void property_Click(object sender, RoutedEventArgs e)
        {

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

        private void CheckBoxMouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void dataGridImageAwaiting_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void checkbox_checked(object sender, RoutedEventArgs e)
        {

        }

        private void checkboxEnable(object sender, DependencyPropertyChangedEventArgs e)
        {

        }

        private void selectValidationImages_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

        }

        private void selectValidationImages_MouseMove(object sender, MouseEventArgs e)
        {

        }

        /// <summary>
        /// Save to database (Curador)
        /// </summary>
        private void saveDB()
        {
            try
            {
                int cont = 0;
                if ((boundingBox.Count - (defect.Count) < boundingBox.Count) || businessSystem.boundingBoxDiscards.Count > 0 || boundingBoxEdit)
                {
                    for (int i = boundingBox.Count - (defect.Count); i < boundingBox.Count; i++)
                    {
                        businessSystem.bristleTempModel_ = new APIVision.Models.BristleTempModel();
                        businessSystem.bristleTempModel_.Name = name;
                        businessSystem.bristleTempModel_.X = boundingBox[i].x;
                        businessSystem.bristleTempModel_.Y = boundingBox[i].y;
                        businessSystem.bristleTempModel_.Height = boundingBox[i].Height;
                        businessSystem.bristleTempModel_.Width = boundingBox[i].Width;
                        businessSystem.bristleTempModel_.Classification = businessSystem.generalController.convertIAToDefault(defect[cont]); //defect[cont];
                        businessSystem.bristleTempModel.Add(businessSystem.bristleTempModel_);
                        cont++;
                    }

                    cont = 0;
                    for (int i = 0; i < boundingBox.Count - (defect.Count); i += 2)
                    {
                        businessSystem.bristleTempModel_ = new APIVision.Models.BristleTempModel();
                        businessSystem.bristleTempModel_.Name = name;
                        businessSystem.bristleTempModel_.X = boundingBox[i].x;
                        businessSystem.bristleTempModel_.Y = boundingBox[i].y;
                        businessSystem.bristleTempModel_.Height = boundingBox[i].Height;
                        businessSystem.bristleTempModel_.Width = boundingBox[i].Width;
                        businessSystem.bristleTempModel_.Classification = businessSystem.generalController.convertIAToDefault(businessSystem.socketModels[cont].obj_class); //businessSystem.socketModels[cont].obj_class;
                        businessSystem.bristleTempModel_.Probability = businessSystem.socketModels[cont].probability;
                        businessSystem.bristleTempModel.Add(businessSystem.bristleTempModel_);
                        cont++;
                    }

                    foreach (var descast_ in businessSystem.boundingBoxDiscards)
                    {
                        businessSystem.bristleTempModel_ = new APIVision.Models.BristleTempModel();
                        businessSystem.bristleTempModel_.Name = name;
                        businessSystem.bristleTempModel_.X = descast_.x;
                        businessSystem.bristleTempModel_.Y = descast_.y;
                        businessSystem.bristleTempModel_.Height = descast_.Height;
                        businessSystem.bristleTempModel_.Width = descast_.Width;
                        businessSystem.bristleTempModel_.Classification = businessSystem.generalController.convertIAToDefault(descast_.obj_class); //descast_.obj_class;
                        businessSystem.bristleTempModel.Add(businessSystem.bristleTempModel_);
                    }

                    businessSystem.boundingBoxDiscards.Clear();
                    businessSystem.dataBaseController.updateBristleTempModel(businessSystem.bristleTempModel, null, null, TuftTempIdCurrent);

                    boundingBoxEdit = false;
                }
                else
                {
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERRO: {0}", ex.Message);
            }
        }

        private void return_Click(object sender, RoutedEventArgs e)
        {
            preview.Visibility = Visibility.Collapsed;
            dataset.Visibility = Visibility.Visible;
            menuRetrain.Visibility = Visibility.Visible;
            menuCuratorship.Visibility = Visibility.Collapsed;
            datasetAnalysis.IsEnabled = true;
            addBaseDataset_.IsEnabled = true;
            applyModel.IsEnabled = true;
            retrain.IsEnabled = true;
        }

        private void buttonWarning1_Click(object sender, RoutedEventArgs e)
        {
            warning.Visibility = Visibility.Collapsed;
            warningText1.Visibility = Visibility.Visible;
            warningText2.Visibility = Visibility.Collapsed;
            yesButton.Visibility = Visibility.Collapsed;
            noButton.Visibility = Visibility.Visible;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Views.Help help = new Views.Help(maximized, businessSystem, automaticBristleClassification);
            help.Show();
        }

        private void buttonCancel_Click(object sender, RoutedEventArgs e)
        {
            warning.Visibility = Visibility.Collapsed;
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

        private void valiationImagesPreview_Click(object sender, RoutedEventArgs e)
        {         
            StartLoadingWait(true);

            cellCurrent = dataGridValidationImage.SelectedItem;

            ValidationDatasetModel cell_ = (ValidationDatasetModel)cellCurrent;

            validationImagesPreview.Clear();
            validationImagesPreview = businessSystem.dataBaseController.listVimageSetModel(cell_);

            totalValidationImages.Text = validationImagesPreview.Count().ToString();
            indexValidationImages = 0;            
            string path = businessSystem.generalSettings.NamePrefix + validationImagesPreview[indexValidationImages];

            Thread.Sleep(1000);
            
            validationImageViewer.Source = BitmapFromUri(new Uri(path));

            validationImageViewer_.Visibility = Visibility.Visible;
            menuRetrain.Visibility = Visibility.Collapsed;
            menuValidationImage.Visibility = Visibility.Visible;

            validationImageViewer.Stretch = Stretch.Fill;
            //photo_ = Bitmap.FromFile(@businessSystem.generalSettings.NamePrefix + "\\" + name);

            StartLoadingWait(false);
        }

        private void returnMenuValidationImage_Click(object sender, RoutedEventArgs e)
        {
            validationImageViewer_.Visibility = Visibility.Collapsed;
            menuRetrain.Visibility = Visibility.Visible;
            menuValidationImage.Visibility = Visibility.Collapsed;
        }

        private void advance_Click(object sender, RoutedEventArgs e)
        {
            if(indexValidationImages < validationImagesPreview.Count() - 1)
            {
                indexValidationImages++;

                numberValidationImages.Text = indexValidationImages.ToString();
                string path = businessSystem.generalSettings.NamePrefix + validationImagesPreview[indexValidationImages];
                validationImageViewer.Source = BitmapFromUri(new Uri(path));
                validationImageViewer.Stretch = Stretch.Fill;
            }
        }
        private void comeBack_Click(object sender, RoutedEventArgs e)
        {
            if (indexValidationImages > 0)
            {
                indexValidationImages--;

                numberValidationImages.Text = indexValidationImages.ToString();
                string path = businessSystem.generalSettings.NamePrefix + validationImagesPreview[indexValidationImages];
                validationImageViewer.Source = BitmapFromUri(new Uri(path));
                validationImageViewer.Stretch = Stretch.Fill;
            }
        }
    }
}