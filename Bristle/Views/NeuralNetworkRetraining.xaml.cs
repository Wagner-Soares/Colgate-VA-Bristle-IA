using APIVision;
using APIVision.Controllers;
using APIVision.Controllers.DataBaseControllers;
using APIVision.DataModels;
using Bristle.UseCases;
using Database;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.DataVisualization.Charting;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Bristle.Views
{
    /// <summary>
    /// Lógica interna para NeuralNetworkRetraining.xaml
    /// </summary>
    public partial class NeuralNetworkRetraining : Window
    {
        private bool maximized = true;
        private readonly BusinessSystem businessSystem;
        private readonly List<int> validationImages = new List<int>();
        private readonly List<int> TrainDatasetId = new List<int>();
        private readonly List<int> ValidationDatasetId = new List<int>();
        private double w = 0;
        private double h = 0;
        private List<ImageTempModel> selectImageTempModel = new List<ImageTempModel>();
        private readonly System.Windows.Forms.Timer myTimer = new System.Windows.Forms.Timer();
        private static readonly System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
        private readonly System.Windows.Forms.Timer myTimer2 = timer;
        private int cont = 0;
        private bool photoResult = false;
        private readonly List<APIVision.DataModels.SocketModel> boundingBox = new List<APIVision.DataModels.SocketModel>();
        private System.Drawing.Image photo_;
        private bool menuBeingUsed = false;
        private int boundingBoxSelect = 0;
        private int selectBoundingBoxDelete = -1;
        private bool editBoundingBoxLeft = false;
        private bool editBoundingBoxRight = false;
        private bool editBoundingBoxUp = false;
        private bool editBoundingBoxDown = false;
        private readonly System.Windows.Shapes.Rectangle mask = new System.Windows.Shapes.Rectangle();
        private System.Windows.Point maskPointStart = new System.Windows.Point();
        private System.Windows.Point maskPointStartMemory = new System.Windows.Point();
        private System.Windows.Point maskPointStopMemory = new System.Windows.Point();
        private bool maskDelete = false;
        private int numberMask = -1;
        private bool frameHolderMouseMove = true;
        private bool boundingBoxEdit = false;
        private readonly List<string> defect = new List<string>();
        private int countBox = 0;
        private string boundingBoxSelectType = "undefined";
        private int xCurrent = 0;
        private int yCurrent = 0;
        private int X;
        private int Y;
        private int size_ = 75;
        private object cellCurrent;
        private readonly List<DataGridCell> column = new List<DataGridCell>();
        private readonly List<DataGridCell> columnCheckBox = new List<DataGridCell>();
        private List<string> validationImagesPreview = new List<string>();
        private int indexValidationImages = 0;
        private int TuftTempIdCurrent;
        private string name;
        private readonly bool maximized__ = false;
        private Views.AutomaticBristleClassification automaticBristleClassification;
        private Thread threadLoadingWait;
        private Thread threadLoadingWait_;
        private LoadingAnimation loading;
        private bool verySmallMask = false;
        public List<SocketModel> SocketModels { get; set; } = new List<SocketModel>();
        private readonly ColgateSkeltaEntities _colgateSkeltaEntities;

        private readonly DataSetSetController _dataSetSetController;
        private readonly RegistrationWaitingController _registrationWaitingController;
        private readonly ValidationDatasetController _validationDatasetController;
        private readonly ModelsController _modelsController;
        private readonly TuftTempSetSetController _tuftTempSetSetController;
        private readonly BristleSetController _bristleSetController;
        private readonly ImageTempSetSetController _imageTempSetSetController;
        private readonly VimageSetController _vimageSetController;

        private readonly GeneralLocalSettings _generalSettings;

        public NeuralNetworkRetraining(bool maximized_, BusinessSystem businessSystem_, ColgateSkeltaEntities colgateSkeltaEntities)
        {
            StartLoadingWait(true);

            businessSystem = businessSystem_;
            maximized__ = maximized_;

            _colgateSkeltaEntities = colgateSkeltaEntities;

            _dataSetSetController = new DataSetSetController(colgateSkeltaEntities);
            _registrationWaitingController = new RegistrationWaitingController(colgateSkeltaEntities);
            _validationDatasetController = new ValidationDatasetController(colgateSkeltaEntities);
            _modelsController = new ModelsController(colgateSkeltaEntities);
            _tuftTempSetSetController = new TuftTempSetSetController(colgateSkeltaEntities);
            _bristleSetController = new BristleSetController(colgateSkeltaEntities);
            _imageTempSetSetController = new ImageTempSetSetController(colgateSkeltaEntities);
            _vimageSetController = new VimageSetController(colgateSkeltaEntities);

            _generalSettings = ScreenNavigationUseCases.GetGeneralLocalSettings();

            InitializeComponent();

            this.Loaded += NeuralNetworkRetraining_Loaded;

            user_.Text = businessSystem.UserSystemCurrent.Name;

            LoadPieChartData1();
            LoadPieChartData2();

            LoadDB();

            InitSelectModel();

            myTimer.Tick += new EventHandler(PhotoResult_);
            myTimer.Interval = 50;
            myTimer.Enabled = true;
            myTimer.Start();

            myTimer2.Tick += new EventHandler(InitCheckbox);
            myTimer2.Interval = 300;
            myTimer2.Enabled = true;
            myTimer2.Start();

            if (automaticBristleClassification != null)
            {
                automaticBristleClassification.Live = false;
            }
        }
        private void StartLoadingWait(bool start)
        {
            if (start)
            {
                threadLoadingWait = new Thread(() => ThreadMethod());
                threadLoadingWait.SetApartmentState(ApartmentState.STA); //Set the thread to STA
                threadLoadingWait.Start();
            }
            else 
            {
                threadLoadingWait_ = new Thread(() => ThreadMethod());
                threadLoadingWait_.SetApartmentState(ApartmentState.STA); //Set the thread to STA
                threadLoadingWait_.Start();
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

        private new void SizeChanged(object sender, SizeChangedEventArgs e)
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
                    //tryCatch to avoid crash
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
                    if (i == 3 || i == 9)
                    {
                        item.Width = 0;
                        item.Visibility = Visibility.Collapsed;
                    }

                    switch (i)
                    {
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
                    if(i == 2 || i == 8)
                    {
                        item.Width = 0;
                        item.Visibility = Visibility.Collapsed;
                    }

                    switch (i)
                    {
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
            dataGridDataset.ItemsSource = _dataSetSetController.ListDatasetModel();
            dataGridImageAwaiting.ItemsSource = _registrationWaitingController.ListRegistrationWaitingModel();
            dataGridValidationImage.ItemsSource = _validationDatasetController.ListValidationDatasetModel();
            dataGridModel.ItemsSource = _modelsController.ListModelsModel();
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
            CameraObject.DinoLiteSDK.MicroTouchPressed -= V_MicroTouchPress;
            CameraObject.DinoLiteSDK.Connected = false;
            Thread.Sleep(280);
            Views.UserControl userControl = new UserControl();
            userControl.Show();
            this.Close();
        }

        private void V_MicroTouchPress(object sender, EventArgs e)
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
            if (ScreenNavigationUseCases.OpenMainScreen(_generalSettings, businessSystem, _colgateSkeltaEntities, maximized))
                this.Close();
        }

        private void ButtonAutomaticBristleClassification_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (automaticBristleClassification == null)
                {
                    automaticBristleClassification = new Views.AutomaticBristleClassification(maximized, businessSystem, _colgateSkeltaEntities);
                }
                automaticBristleClassification.Live = true;
                automaticBristleClassification.Show();
            }
            catch 
            {
                automaticBristleClassification = new Views.AutomaticBristleClassification(maximized, businessSystem, _colgateSkeltaEntities);               
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

        private void ButtonWindowMinimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }


        private void ButtonGeneralReport_Click(object sender, RoutedEventArgs e)
        {
            if (ScreenNavigationUseCases.OpenGeneralReportScreen(businessSystem.UserSystemCurrent, businessSystem.NetworkUserModel, ScreenNavigationUseCases.GetGeneralLocalSettings(), businessSystem, _colgateSkeltaEntities, maximized))
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
            if (ScreenNavigationUseCases.OpenGeneralSettingsScreen(businessSystem.UserSystemCurrent, businessSystem.NetworkUserModel, _generalSettings, businessSystem, _colgateSkeltaEntities, maximized))
            {
                this.Close();
            }
            else
            {
                MessageBox.Show("Necessary administrative rights!");
            }
        }

        private void Password_Click(object sender, RoutedEventArgs e)
        {
            if (ScreenNavigationUseCases.OpenPasswordScreen(businessSystem.UserSystemCurrent, businessSystem.NetworkUserModel, _generalSettings, businessSystem, _colgateSkeltaEntities, maximized))
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
            if (ScreenNavigationUseCases.OpenUserScreen(businessSystem.UserSystemCurrent, businessSystem.NetworkUserModel, _generalSettings, businessSystem, _colgateSkeltaEntities, maximized))
            {
                this.Close();
            }
            else
            {
                MessageBox.Show("Necessary administrative rights!");
            }
        }

        private void DatasetAnalysis_Click_1(object sender, RoutedEventArgs e)
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

        /// <summary>
        /// Apply config dataset
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Apply_Click(object sender, RoutedEventArgs e)
        {

            addDataset.Visibility = Visibility.Collapsed;
            menuRetrain.Visibility = Visibility.Visible;

            if (nameDataset.Text == "")
            {
                businessSystem.DatasetModel.Name = DateTime.UtcNow.ToString();
            }
            else
            {
                businessSystem.DatasetModel.Name = nameDataset.Text;
            }
            businessSystem.DatasetModel.Historic = DateTime.UtcNow.ToString();
            businessSystem.DatasetModel.Type = "";

            businessSystem.RegistrationWaitingModels = _registrationWaitingController.ListRegistrationWaitingModel();

            // tempValidationImages
            for (int i = 0; i < validationImages.Count; i++)
            {
                for (int j = 0; j < businessSystem.RegistrationWaitingModels.Count; j++)
                {
                    if (validationImages[i] == businessSystem.RegistrationWaitingModels[j].Id)
                    {
                        validationImages[i] = j;
                    }
                }
            }

            for (int i = 0; i < validationImages.Count; i++)
            {
                businessSystem.ValidationRegistrationWaitingModels.Add(businessSystem.RegistrationWaitingModels.ElementAt(validationImages[i]));
            }

            bool validationImages_ = false;
            for (int j = 0; j < businessSystem.RegistrationWaitingModels.Count; j++)
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
                    businessSystem.DatasetRegistrationWaitingModels.Add(businessSystem.RegistrationWaitingModels.ElementAt(j));
                }
                validationImages_ = false;
            }

            if (businessSystem.ValidationRegistrationWaitingModels.Count > 0)
            {
                _validationDatasetController.UpdateValidationDatasetModel(businessSystem.DatasetModel, businessSystem.ValidationRegistrationWaitingModels);
            }
            _dataSetSetController.UpdateDatasetModel(businessSystem.DatasetModel, businessSystem.DatasetRegistrationWaitingModels);

            dataGridDataset.ItemsSource = _dataSetSetController.ListDatasetModel();
            dataGridValidationImage.ItemsSource = _validationDatasetController.ListValidationDatasetModel();

            _registrationWaitingController.UpdateRegistrationWaitingModel(null, null);
            dataGridImageAwaiting.ItemsSource = _registrationWaitingController.ListRegistrationWaitingModel();

            businessSystem.ValidationRegistrationWaitingModels.Clear();
            businessSystem.DatasetRegistrationWaitingModels.Clear();
        }

        /// <summary>
        /// Add Dataset
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddBaseDataset__Click(object sender, RoutedEventArgs e)
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
                    //tryCatch to avoid crash
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
        private void OpenImage_Click(object sender, RoutedEventArgs e)
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

            selectImageTempModel.Clear();

            TuftTempIdCurrent = _tuftTempSetSetController.ListTuftTempModel().FirstOrDefault(TuftTemp => TuftTemp.Id == cell_.Id).Id;
            selectImageTempModel = _imageTempSetSetController.ListImageTempModelByTuft(TuftTempIdCurrent);

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
                string path = $@"{_generalSettings.NamePrefix}\{name}";
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

            SocketModels.Clear();
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
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                return bitmap;               
            }
            catch
            {
                MessageBox.Show("File not found");
                return null;
            }
        }

        /// <summary>
        /// Dataset selection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectDatasets_Click(object sender, RoutedEventArgs e)
        {

            var cell = dataGridDataset.SelectedItem;
            DatasetModel cell_ = (DatasetModel)cell;
            int removeValidation = -1;

            if (TrainDatasetId.Count != 0)
            {
                foreach (var imageId in TrainDatasetId)
                {
                    if (cell_.Id == imageId)
                    {
                        removeValidation = cell_.Id;
                    }
                }

                if (removeValidation >= 0)
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
        private void Retrain_Click_1(object sender, RoutedEventArgs e)
        {
            businessSystem.CommandS200Model.TrainDatasetId = TrainDatasetId.ToArray();
            businessSystem.CommandS200Model.ValidationDatasetId = ValidationDatasetId.ToArray();
            string command = DataHandlerUseCases.SaveJsonWithStringReturn(businessSystem.CommandS200Model, @"commandS200");
            businessSystem.SendCommand("S200", _generalSettings.IpTrain, _generalSettings.PortTrain, command);
            businessSystem.Data = "";
            Thread.Sleep(1000);
            dataGridModel.ItemsSource = _modelsController.ListModelsModel();
        }

        /// <summary>
        /// Image that will be used for validation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectValidationImages_Click(object sender, RoutedEventArgs e)
        {
            var cell = dataGridImageAwaiting.SelectedItem;

            DataGridRow validationImagerow = (DataGridRow)dataGridImageAwaiting.ItemContainerGenerator.ContainerFromItem(cell);

            try
            {
                if ((string)column[validationImagerow.GetIndex()].Content == "Analyzed")
                {
                    RegistrationWaitingModel cell_ = (RegistrationWaitingModel)cell;

                    int removeOption = -1;

                    if (validationImages.Count != 0)
                    {
                        foreach (var imageId in validationImages)
                        {
                            if (cell_.Id == imageId)
                            {
                                removeOption = cell_.Id;
                            }
                        }

                        if (removeOption >= 0)
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
            }
            catch
            {
                //tryCatch to avoid crash
            }

        }

        /// <summary>
        /// Image that will be used for validation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectValidationImages_Click_1(object sender, RoutedEventArgs e)
        {
            var cell = dataGridValidationImage.SelectedItem;
            ValidationDatasetModel cell_ = (ValidationDatasetModel)cell;

            int removeOption = -1;

            if (ValidationDatasetId.Count != 0)
            {
                foreach (var imageId in ValidationDatasetId)
                {
                    if (cell_.Id == imageId)
                    {
                        removeOption = cell_.Id;
                    }
                }

                if (removeOption >= 0)
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
        private void SelectModel_Click(object sender, RoutedEventArgs e)
        {
            var cell = dataGridModel.SelectedItem;
            ModelsModel cell_ = (ModelsModel)cell;
            businessSystem.CommandS300Model.OldModelId = businessSystem.CommandS300Model.NewModelId;
            businessSystem.CommandS300Model.NewModelId = cell_.Id;
            List<ModelsModel> cell_s = new List<ModelsModel>
            {
                cell_
            };
            dataGridModelSelect.ItemsSource = cell_s;
            dataGridModel.ItemsSource = _modelsController.ListModelsModel();
            UpdatingColumnNames();
        }

        /// <summary>
        /// Initializes template
        /// </summary>
        private void InitSelectModel()
        {
            List<ModelsModel> cell_s = new List<ModelsModel>();
            List<ModelsModel> cell_s_ = _modelsController.ListModelsModel();

            foreach (ModelsModel cell_ in cell_s_)
            {
                if (cell_.Id == businessSystem.CommandS300Model.NewModelId)
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
        private void ApplyModel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DataHandlerUseCases.SaveJsonIntoSettings(businessSystem.CommandS300Model, @"commandS300");
                object obj = DataHandlerUseCases.ReadJsonFromSettingsKey(businessSystem.LocalRepository + @"\commandS300.json");
                _generalSettings.Model = DataHandlerUseCases.ConvertObjectToJson(obj);
                businessSystem.SendCommand("S300", _generalSettings.Model, _generalSettings.IpPrediction, _generalSettings.PortPrediction);
                businessSystem.Data = "";

                warning.Visibility = Visibility.Visible;
            }
            catch
            {
                //tryCatch to avoid crash
            }
        }

        /// <summary>
        /// Save curator analysis results
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Validate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SaveDB();

                preview.Visibility = Visibility.Collapsed;
                dataset.Visibility = Visibility.Visible;

                dataGridImageAwaiting.UpdateLayout();

                DataGridRow validateRow = (DataGridRow)dataGridImageAwaiting.ItemContainerGenerator.ContainerFromItem(cellCurrent);

                column[validateRow.GetIndex()].Content = "Analyzed";
                column[validateRow.GetIndex()].Foreground = System.Windows.Media.Brushes.DarkGreen;
                columnCheckBox[validateRow.GetIndex()].IsEnabled = true;

                datasetAnalysis.IsEnabled = true;
                addBaseDataset_.IsEnabled = true;
                applyModel.IsEnabled = true;
                retrain.IsEnabled = true;
            }
            catch
            {
                //tryCatch to avoid crash
            }
            menuCuratorship.Visibility = Visibility.Collapsed;
            menuRetrain.Visibility = Visibility.Visible;
        }

        private new void MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();

        }

        /// <summary>
        /// Remove image
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            var cell = dataGridImageAwaiting.SelectedItem;
            RegistrationWaitingModel cell_ = (RegistrationWaitingModel)cell;
            _registrationWaitingController.UpdateRegistrationWaitingModel(null, cell_);

            DataGridRow rowToRemove = (DataGridRow)dataGridImageAwaiting.ItemContainerGenerator.ContainerFromItem(cell);
            column[rowToRemove.GetIndex()].Content = "Removed";
            column[rowToRemove.GetIndex()].Foreground = System.Windows.Media.Brushes.DarkRed;
            rowToRemove.IsEnabled = false;

            preview.Visibility = Visibility.Collapsed;
            dataset.Visibility = Visibility.Visible;

            imagePreview_.Source = null;
            string pathImage = selectImageTempModel[selectImageTempModel.Count - 1].Path;
            NeuralNetworkRetraining neuralNetworkRetraining = this;
            lock (neuralNetworkRetraining)
            {
                if (photo_ != null) photo_.Dispose();

                try
                {
                    File.Delete($"{@_generalSettings.NamePrefix}\\{pathImage}");
                }
                catch
                {
                    //tryCatch to avoid crash
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
        private void Analyze_Click(object sender, RoutedEventArgs e)
        {
            businessSystem.SendCommand("S100", (Bitmap)photo_, _generalSettings.IpPrediction, _generalSettings.PortPrediction);
            businessSystem.Data = "";

            photoResult = true;
        }

        /// <summary>
        /// Monitoring the return of the verification made by the curator
        /// </summary>
        /// <param name="myObject"></param>
        /// <param name="myEventArgs"></param>
        private void PhotoResult_(Object myObject, EventArgs myEventArgs)
        {
            if (photoResult)
            {
                if (businessSystem.Data != null && businessSystem.Data.Length > 0)
                {
                    if (businessSystem.Data != "*")
                    {                        
                        cont = 0;
                        NeuralNetworkRetraining neuralNetworkRetraining = this;
                        lock (neuralNetworkRetraining)
                        {
                            SocketModels = DataHandlerUseCases.GetSocketModelsReturned(businessSystem.Data, businessSystem);
                            DrawBoundingBox();
                            businessSystem.Data = "";
                        }
                
                        photoResult = false;
                    }
                }
                else
                {
                    cont++;
                    if (cont > 10)
                    {
                        NeuralNetworkRetraining neuralNetworkRetraining = this;
                        lock (neuralNetworkRetraining)
                        {
                            RecoverCommunicationWithAI();
                        }
                        cont = 0;
                    }
                }
            }
            else
            {
                if (businessSystem.Data != "*")
                {
                    if (businessSystem.Data == "OK")
                    {
                        warningText1.Visibility = Visibility.Collapsed;
                        warningText2.Visibility = Visibility.Visible;
                        yesButton.Visibility = Visibility.Visible;
                        noButton.Visibility = Visibility.Collapsed;
                        businessSystem.Data = "";
                    }
                    else if (businessSystem.Data == "NOK")
                    {
                        warningText1.Visibility = Visibility.Collapsed;
                        warningText3.Visibility = Visibility.Visible;
                        yesButton.Visibility = Visibility.Visible;
                        noButton.Visibility = Visibility.Collapsed;
                        businessSystem.Data = "";
                    }
                }
            }
            myTimer.Stop();
            myTimer.Enabled = true;
        }

        /// <summary>
        /// recoverCommunicationWithAI
        /// </summary>
        private void RecoverCommunicationWithAI()
        {
            DataHandlerUseCases.CMD("./StartSocketAI.vbs");
            businessSystem.SendCommand("S300", _generalSettings.Model, _generalSettings.IpPrediction, _generalSettings.PortPrediction);
            businessSystem.Data = "";
            businessSystem.SendCommand("S100", (Bitmap)photo_, _generalSettings.IpPrediction, _generalSettings.PortPrediction);
            businessSystem.Data = "";
        }

        /// <summary>
        /// Drawing the bounding box
        /// </summary>
        private void DrawBoundingBox()
        {
            try
            {
                canvas.Children.Clear();
                boundingBox.Clear();

                foreach (var d in SocketModels)
                {
                    double WidthR = (d.Width * w) / CameraObject.ResolutionWidth;
                    double HeightR = (d.Height * h) / CameraObject.ResolutionHeight;
                    double xR = (d.X * w) / CameraObject.ResolutionWidth;
                    double yR = (d.Y * h) / CameraObject.ResolutionHeight;

                    if (d.Obj_class == "none")
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
                    else if (d.Obj_class == "discard")
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
                    TextBlock textBlock = new TextBlock
                    {
                        FontSize = 18
                    };

                    if (d.Obj_class != "none")
                    {
                        textBlock.Foreground = new SolidColorBrush(Colors.DarkRed);

                        if (d.Obj_class == "discard")
                        {
                            textBlock.Foreground = new SolidColorBrush(Colors.Yellow);
                            textBlock.Text = "Undefined";
                        }
                        else
                        {
                            textBlock.Text = DataHandlerUseCases.ConvertDefaultToIA(d.Obj_class);
                        }
                    }
                    else
                    {
                        textBlock.Foreground = new SolidColorBrush(Colors.DarkGreen);
                        textBlock.Text = "Ok";
                    }
                    Canvas.SetLeft(textBlock, xR);
                    Canvas.SetTop(textBlock, yR);
                    canvas.Children.Add(textBlock);

                    APIVision.DataModels.SocketModel box = new APIVision.DataModels.SocketModel
                    {
                        Width = d.Width,
                        Height = d.Height,
                        X = d.X,
                        Y = d.Y
                    };
                    boundingBox.Add(box);
                    //Para contabilizar a porcentagem adicionada para o canvas
                    APIVision.DataModels.SocketModel box2 = new APIVision.DataModels.SocketModel
                    {
                        Width = 0,
                        Height = 0,
                        X = 0,
                        Y = 0
                    };
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
        private void FrameHolder_MouseMove(object sender, MouseEventArgs e)
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
                            //tryCatch to avoid crash
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

                        SolidColorBrush colorStroke;
                        SolidColorBrush colorFill;
                        colorStroke = System.Windows.Media.Brushes.Black;
                        colorFill = System.Windows.Media.Brushes.Black;

                        foreach (var bTest in SocketModels)
                        {
                            i++;

                            System.Windows.Point point = e.GetPosition(canvas);

                            WidthR = (bTest.Width * w) / CameraObject.ResolutionWidth;
                            HeightR = (bTest.Height * h) / CameraObject.ResolutionHeight;
                            xR = (bTest.X * w) / CameraObject.ResolutionWidth;
                            yR = (bTest.Y * h) / CameraObject.ResolutionHeight;

                            if (!(bTest.X == 0 && bTest.Y == 0) && point.X > xR && point.Y > yR && point.X < (xR + WidthR) && point.Y < (yR + HeightR))
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

                        if (!valid && boundingBoxSelect != 0)
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
                }
            }
            verySmallMask = false;
        }

        /// <summary>
        /// Keyboard Control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void KeyboardControl(object sender, KeyEventArgs e)
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
                                SocketModels[selectBoundingBoxDelete - 1].Obj_class = "discard";
                                businessSystem.BoundingBoxDiscards.Add(SocketModels[selectBoundingBoxDelete - 1]);
                                SocketModels.RemoveAt(selectBoundingBoxDelete - 1);
                                DrawBoundingBox();
                                boundingBoxSelect = 0;
                                selectBoundingBoxDelete = -1;
                            }
                            e.Handled = true;
                        }
                        else
                        {
                            bool removeOption = true;
                            while (removeOption)
                            {
                                int i = 0;
                                foreach (var bTest in SocketModels)
                                {
                                    double WidthR = (bTest.Width * w) / CameraObject.ResolutionWidth;
                                    double HeightR = (bTest.Height * h) / CameraObject.ResolutionHeight;
                                    double xR = (bTest.X * w) / CameraObject.ResolutionWidth;
                                    double yR = (bTest.Y * h) / CameraObject.ResolutionHeight;

                                    if (!(bTest.X == 0 && bTest.Y == 0) && (xR > maskPointStartMemory.X - 2) && (yR > maskPointStartMemory.Y - 2) && ((xR + WidthR) < maskPointStopMemory.X + 2) && ((yR + HeightR) < maskPointStopMemory.Y + 2))
                                    {
                                        SocketModels[i].Obj_class = "discard";
                                        businessSystem.BoundingBoxDiscards.Add(SocketModels[i]);
                                        SocketModels.RemoveAt(i);
                                        boundingBoxSelect = 0;
                                        break;
                                    }
                                    i++;
                                    if (i == SocketModels.Count)
                                    {
                                        removeOption = false;
                                    }
                                }

                                if (SocketModels.Count == 0)
                                {
                                    removeOption = false;
                                    canvas.Children.Clear();
                                }
                            }

                            DrawBoundingBox();

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
                                if (SocketModels[selectBoundingBoxDelete - 1].X > 0)
                                {
                                    boundingBoxEdit = true;
                                    SocketModels[selectBoundingBoxDelete - 1].X -= businessSystem.ResizingOfBoundingBoxWeight;
                                    DrawBoundingBox();
                                    boundingBoxSelect = 0;
                                }
                            }
                            else
                            {
                                if (SocketModels[selectBoundingBoxDelete - 1].X > 0)
                                {
                                    boundingBoxEdit = true;
                                    SocketModels[selectBoundingBoxDelete - 1].X -= businessSystem.ResizingOfBoundingBoxWeight;
                                    SocketModels[selectBoundingBoxDelete - 1].Width += businessSystem.ResizingOfBoundingBoxWeight;
                                    DrawBoundingBox();
                                    boundingBoxSelect = 0;
                                }
                            }
                        }
                        else
                        {
                            if (SocketModels[selectBoundingBoxDelete - 1].X > 0)
                            {
                                boundingBoxEdit = true;
                                SocketModels[selectBoundingBoxDelete - 1].Width -= businessSystem.ResizingOfBoundingBoxWeight;
                                DrawBoundingBox();
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
                                if (SocketModels[selectBoundingBoxDelete - 1].X > 0)
                                {
                                    boundingBoxEdit = true;
                                    SocketModels[selectBoundingBoxDelete - 1].X += businessSystem.ResizingOfBoundingBoxWeight;
                                    DrawBoundingBox();
                                    boundingBoxSelect = 0;
                                }
                            }
                            else
                            {
                                if (SocketModels[selectBoundingBoxDelete - 1].X > 0)
                                {
                                    boundingBoxEdit = true;
                                    SocketModels[selectBoundingBoxDelete - 1].Width += businessSystem.ResizingOfBoundingBoxWeight;
                                    DrawBoundingBox();
                                    boundingBoxSelect = 0;
                                }
                            }
                        }
                        else
                        {
                            if (SocketModels[selectBoundingBoxDelete - 1].X > 0)
                            {
                                boundingBoxEdit = true;
                                SocketModels[selectBoundingBoxDelete - 1].X += businessSystem.ResizingOfBoundingBoxWeight;
                                SocketModels[selectBoundingBoxDelete - 1].Width -= businessSystem.ResizingOfBoundingBoxWeight;
                                DrawBoundingBox();
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
                                if (SocketModels[selectBoundingBoxDelete - 1].Y > 0)
                                {
                                    boundingBoxEdit = true;
                                    SocketModels[selectBoundingBoxDelete - 1].Y -= businessSystem.ResizingOfBoundingBoxWeight;
                                    DrawBoundingBox();
                                    boundingBoxSelect = 0;
                                }
                            }
                            else
                            {
                                if (SocketModels[selectBoundingBoxDelete - 1].Y > 0)
                                {
                                    boundingBoxEdit = true;
                                    SocketModels[selectBoundingBoxDelete - 1].Y -= businessSystem.ResizingOfBoundingBoxWeight;
                                    SocketModels[selectBoundingBoxDelete - 1].Height += businessSystem.ResizingOfBoundingBoxWeight;
                                    DrawBoundingBox();
                                    boundingBoxSelect = 0;
                                }
                            }
                        }

                        else
                        {
                            if (SocketModels[selectBoundingBoxDelete - 1].Y > 0)
                            {
                                boundingBoxEdit = true;
                                SocketModels[selectBoundingBoxDelete - 1].Height -= businessSystem.ResizingOfBoundingBoxWeight;
                                DrawBoundingBox();
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
                                if (SocketModels[selectBoundingBoxDelete - 1].Y > 0)
                                {
                                    boundingBoxEdit = true;
                                    SocketModels[selectBoundingBoxDelete - 1].Y += businessSystem.ResizingOfBoundingBoxWeight;
                                    DrawBoundingBox();
                                    boundingBoxSelect = 0;
                                }
                            }
                            else
                            {
                                if (SocketModels[selectBoundingBoxDelete - 1].Y > 0)
                                {
                                    boundingBoxEdit = true;
                                    SocketModels[selectBoundingBoxDelete - 1].Height += businessSystem.ResizingOfBoundingBoxWeight;
                                    DrawBoundingBox();
                                    boundingBoxSelect = 0;
                                }
                            }
                        }

                        else
                        {
                            if (SocketModels[selectBoundingBoxDelete - 1].Y > 0)
                            {
                                boundingBoxEdit = true;
                                SocketModels[selectBoundingBoxDelete - 1].Y += businessSystem.ResizingOfBoundingBoxWeight;
                                SocketModels[selectBoundingBoxDelete - 1].Height -= businessSystem.ResizingOfBoundingBoxWeight;
                                DrawBoundingBox();
                                boundingBoxSelect = 0;
                            }
                        }
                    }
                    else if (e.Key == Key.Escape && numberMask != -1)
                    {
                        canvasMask.Children.RemoveAt(numberMask);
                        numberMask = -1;
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
                double WidthR = (SocketModels[selectBoundingBoxDelete - 1].Width * w) / CameraObject.ResolutionWidth;
                double HeightR = (SocketModels[selectBoundingBoxDelete - 1].Height * h) / CameraObject.ResolutionHeight;
                double xR = (SocketModels[selectBoundingBoxDelete - 1].X * w) / CameraObject.ResolutionWidth;
                double yR = (SocketModels[selectBoundingBoxDelete - 1].Y * h) / CameraObject.ResolutionHeight;

                if (!editBoundingBoxUp)
                {
                    if (point.X >= (xR + (WidthR / 2) - 6) && point.X <= (xR + (WidthR / 2) + 6) && point.Y >= (yR - 14) && point.Y <= (yR + 14))
                    {
                        System.Windows.Shapes.Rectangle maskUp = new System.Windows.Shapes.Rectangle
                        {
                            Stroke = System.Windows.Media.Brushes.Green,
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
                    if (point.X >= (xR + (WidthR / 2) - 6) && point.X <= (xR + (WidthR / 2) + 6) && point.Y >= (yR - 14) && point.Y <= (yR + 14))
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
                    if (point.X >= (xR + (WidthR / 2) - 6) && point.X <= (xR + (WidthR / 2) + 6) && point.Y >= (yR + HeightR - 14) && point.Y <= (yR + HeightR + 14))
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
                    if (point.X >= (xR + (WidthR / 2) - 6) && point.X <= (xR + (WidthR / 2) + 6) && point.Y >= (yR + HeightR - 14) && point.Y <= (yR + HeightR + 14))
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
        private void Error_1_Click(object sender, RoutedEventArgs e)
        {
            if (selectBoundingBoxDelete == -1)
            {
                defect.Add("Error1");
                countBox += 2;
                boundingBoxSelectType = "Error1";
                canvas.Children.RemoveAt(canvas.Children.Count - 1);
                WriteBoundingBox();
            }
            else
            {
                boundingBoxSelectType = "Error1";
                ChangeClass();
            }
        }

        private void Error_2_Click(object sender, RoutedEventArgs e)
        {
            if (selectBoundingBoxDelete == -1)
            {
                defect.Add("Error2");
                countBox += 2;
                boundingBoxSelectType = "Error2";
                canvas.Children.RemoveAt(canvas.Children.Count - 1);
                WriteBoundingBox();
            }
            else
            {
                boundingBoxSelectType = "Error2";
                ChangeClass();
            }
        }

        private void Error_3_Click(object sender, RoutedEventArgs e)
        {
            if (selectBoundingBoxDelete == -1)
            {
                defect.Add("Error3");
                countBox += 2;
                boundingBoxSelectType = "Error3";
                canvas.Children.RemoveAt(canvas.Children.Count - 1);
                WriteBoundingBox();
            }
            else
            {
                boundingBoxSelectType = "Error3";
                ChangeClass();
            }
        }
        private void Undefined_Click(object sender, RoutedEventArgs e)
        {
            if (selectBoundingBoxDelete == -1)
            {
                defect.Add("discard");
                countBox += 2;
                boundingBoxSelectType = "discard";
                canvas.Children.RemoveAt(canvas.Children.Count - 1);
                WriteBoundingBox();
            }
            else
            {
                boundingBoxSelectType = "discard";
                ChangeClass();
            }
        }

        private void None_Click(object sender, RoutedEventArgs e)
        {
            if (selectBoundingBoxDelete == -1)
            {
                defect.Add("Ok");
                countBox += 2;
                boundingBoxSelectType = "Ok";
                canvas.Children.RemoveAt(canvas.Children.Count - 1);
                WriteBoundingBox();
            }
            else
            {
                boundingBoxSelectType = "Ok";
                ChangeClass();
            }
        }

        private void Reb_Click(object sender, RoutedEventArgs e)
        {
            if (selectBoundingBoxDelete == -1)
            {
                defect.Add("reb");
                countBox += 2;
                boundingBoxSelectType = "reb";
                canvas.Children.RemoveAt(canvas.Children.Count - 1);
                WriteBoundingBox();
            }
            else
            {
                boundingBoxSelectType = "reb";
                ChangeClass();
            }
        }

        /// <summary>
        /// Design user manual bounding boxes
        /// </summary>
        private void WriteBoundingBox()
        {
            if (selectBoundingBoxDelete == -1)
            {
                try
                {
                    if (submenuBox.IsEnabled)
                    {
                        if (canvas.Children.Count > countBox)
                        {
                            canvas.Children.RemoveAt(countBox);
                            frameHolderMouseMove = true;
                        }
                        else
                        {
                            frameHolderMouseMove = false;
                        }

                        X = xCurrent - (size_ / 2);
                        Y = yCurrent - (size_ / 2);



                        if (boundingBoxSelectType == "undefined")
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
                            SocketModel box = new SocketModel
                            {
                                Width = size_,
                                Height = size_,
                                X = X,
                                Y = Y
                            };
                            Canvas.SetLeft(rect, X);
                            Canvas.SetTop(rect, Y);
                            canvas.Children.Add(rect);

                            //scale conversion
                            box.Width = (int)((box.Width * CameraObject.ResolutionWidth) / w);
                            box.Height = (int)((box.Height * CameraObject.ResolutionHeight) / h);
                            box.X = (int)((box.X * CameraObject.ResolutionWidth) / w);
                            box.Y = (int)((box.Y * CameraObject.ResolutionHeight) / h);
                            box.Obj_class = DataHandlerUseCases.ConvertIAToDefault(boundingBoxSelectType);
                            box.Probability = -1;

                            TextBlock textBlock = new TextBlock
                            {
                                FontSize = 18,
                                Foreground = new SolidColorBrush(Colors.DarkGreen),
                                Text = ("Ok")
                            };
                            Canvas.SetLeft(textBlock, X);
                            Canvas.SetTop(textBlock, Y);
                            canvas.Children.Add(textBlock);

                            SocketModels.Add(box);
                            boundingBox.Add(box);

                            frameHolderMouseMove = true;
                        }
                        else
                        {
                            SocketModel box = new SocketModel();
                            if (boundingBoxSelectType != "discard")
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
                                box.Width = size_;
                                box.Height = size_;
                                box.X = X;
                                box.Y = Y;
                                box.Probability = -1;
                                Canvas.SetLeft(rect, X);
                                Canvas.SetTop(rect, Y);

                                canvas.Children.Add(rect);
                            }

                            //scale conversion
                            box.Width = (int)((box.Width * CameraObject.ResolutionWidth) / w);
                            box.Height = (int)((box.Height * CameraObject.ResolutionHeight) / h);
                            box.X = (int)((box.X * CameraObject.ResolutionWidth) / w);
                            box.Y = (int)((box.Y * CameraObject.ResolutionHeight) / h);
                            box.Obj_class = DataHandlerUseCases.ConvertIAToDefault(boundingBoxSelectType);
                            box.Probability = -1;

                            if (boundingBoxSelectType != "discard")
                            {
                                TextBlock textBlock = new TextBlock
                                {
                                    FontSize = 18,
                                    Foreground = new SolidColorBrush(Colors.DarkRed),
                                    Text = boundingBoxSelectType
                                };
                                Canvas.SetLeft(textBlock, X);
                                Canvas.SetTop(textBlock, Y);
                                canvas.Children.Add(textBlock);
                            }
                            else
                            {
                                TextBlock textBlock = new TextBlock
                                {
                                    FontSize = 18,
                                    Foreground = new SolidColorBrush(Colors.Yellow),
                                    Text = "Undefined"
                                };
                                Canvas.SetLeft(textBlock, X);
                                Canvas.SetTop(textBlock, Y);
                                canvas.Children.Add(textBlock);
                            }

                            SocketModels.Add(box);
                            boundingBox.Add(box);

                            frameHolderMouseMove = true;
                        }
                    }
                    boundingBoxSelectType = "undefined";
                }
                catch
                {
                    //tryCatch to avoid crash
                }
            }

            frameHolderMouseMove = true;
        }

        private void ChangeClass()
        {
            if (numberMask == -1 && selectBoundingBoxDelete > -1)
            {
                try
                {
                    var SocketModel = SocketModels[selectBoundingBoxDelete - 1];
                    SocketModels.RemoveAt(selectBoundingBoxDelete - 1);
                    SocketModel.Obj_class = DataHandlerUseCases.ConvertIAToDefault(boundingBoxSelectType);
                    SocketModel.Probability = -1;
                    SocketModels.Add(SocketModel);
                    DrawBoundingBox();
                    boundingBoxSelect = 0;
                    selectBoundingBoxDelete = -1;
                    boundingBoxSelectType = "undefined";
                }
                catch
                {
                    //tryCatch to avoid crash
                }
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
                if (selectBoundingBoxDelete == -1 && canvas.Children.Count > countBox)
                {
                    frameHolderMouseMove = false;
                    canvas.Children.RemoveAt(countBox);
                    boundingBox.RemoveAt(canvas.Children.Count - 1);
                }

                frameHolderMouseMove = true;
            }
            catch
            {
                //tryCatch to avoid crash
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

        private void TextBox_ContextMenuClosing(object sender, ContextMenuEventArgs e)
        {   
            if (boundingBoxSelectType == "undefined")
            {
                WithdrawalOfBoundingBoxMarking();
            }

            menuBeingUsed = false;

            canvas.UpdateLayout();

            frameHolderMouseMove = true;
        }

        private void TextBox_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            menuBeingUsed = true;

            xCurrent = (int)e.CursorLeft;
            yCurrent = (int)e.CursorTop;
            WriteBoundingBox();
        }

        /// <summary>
        /// Save to database (Curador)
        /// </summary>
        private void SaveDB()
        {
            try
            {
                int boxCont = 0;
                if ((boundingBox.Count - (defect.Count) < boundingBox.Count) || businessSystem.BoundingBoxDiscards.Count > 0 || boundingBoxEdit)
                {
                    for (int i = boundingBox.Count - (defect.Count); i < boundingBox.Count; i++)
                    {
                        businessSystem.BristleTempModel_ = new APIVision.DataModels.BristleTempModel
                        {
                            Name = name,
                            X = boundingBox[i].X,
                            Y = boundingBox[i].Y,
                            Height = boundingBox[i].Height,
                            Width = boundingBox[i].Width,
                            Classification = DataHandlerUseCases.ConvertIAToDefault(defect[boxCont])
                        };
                        businessSystem.BristleTempModel.Add(businessSystem.BristleTempModel_);
                        boxCont++;
                    }

                    boxCont = 0;
                    for (int i = 0; i < boundingBox.Count - (defect.Count); i += 2)
                    {
                        businessSystem.BristleTempModel_ = new APIVision.DataModels.BristleTempModel
                        {
                            Name = name,
                            X = boundingBox[i].X,
                            Y = boundingBox[i].Y,
                            Height = boundingBox[i].Height,
                            Width = boundingBox[i].Width,
                            Classification = DataHandlerUseCases.ConvertIAToDefault(SocketModels[boxCont].Obj_class),
                            Probability = SocketModels[boxCont].Probability
                        };
                        businessSystem.BristleTempModel.Add(businessSystem.BristleTempModel_);
                        boxCont++;
                    }

                    foreach (var descast_ in businessSystem.BoundingBoxDiscards)
                    {
                        businessSystem.BristleTempModel_ = new APIVision.DataModels.BristleTempModel
                        {
                            Name = name,
                            X = descast_.X,
                            Y = descast_.Y,
                            Height = descast_.Height,
                            Width = descast_.Width,
                            Classification = DataHandlerUseCases.ConvertIAToDefault(descast_.Obj_class)
                        };
                        businessSystem.BristleTempModel.Add(businessSystem.BristleTempModel_);
                    }

                    businessSystem.BoundingBoxDiscards.Clear();
                    _bristleSetController.UpdateBristleTempModel(businessSystem.BristleTempModel, TuftTempIdCurrent);

                    boundingBoxEdit = false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERRO: {0}", ex.Message);
            }
        }

        private void Return_Click(object sender, RoutedEventArgs e)
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

        private void ButtonWarning1_Click(object sender, RoutedEventArgs e)
        {
            warning.Visibility = Visibility.Collapsed;
            warningText1.Visibility = Visibility.Visible;
            warningText2.Visibility = Visibility.Collapsed;
            yesButton.Visibility = Visibility.Collapsed;
            noButton.Visibility = Visibility.Visible;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Views.Help help = new Views.Help();
            help.Show();
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            warning.Visibility = Visibility.Collapsed;
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

        private void ValiationImagesPreview_Click(object sender, RoutedEventArgs e)
        {         
            StartLoadingWait(true);

            cellCurrent = dataGridValidationImage.SelectedItem;

            ValidationDatasetModel cell_ = (ValidationDatasetModel)cellCurrent;

            validationImagesPreview.Clear();
            validationImagesPreview = _vimageSetController.ListVimageSetModel(cell_);

            totalValidationImages.Text = validationImagesPreview.Count.ToString();
            indexValidationImages = 0;            
            string path = _generalSettings.NamePrefix + validationImagesPreview[indexValidationImages];

            Thread.Sleep(1000);
            
            validationImageViewer.Source = BitmapFromUri(new Uri(path));

            validationImageViewer_.Visibility = Visibility.Visible;
            menuRetrain.Visibility = Visibility.Collapsed;
            menuValidationImage.Visibility = Visibility.Visible;

            validationImageViewer.Stretch = Stretch.Fill;

            StartLoadingWait(false);
        }

        private void ReturnMenuValidationImage_Click(object sender, RoutedEventArgs e)
        {
            validationImageViewer_.Visibility = Visibility.Collapsed;
            menuRetrain.Visibility = Visibility.Visible;
            menuValidationImage.Visibility = Visibility.Collapsed;
        }

        private void Advance_Click(object sender, RoutedEventArgs e)
        {
            if(indexValidationImages < validationImagesPreview.Count - 1)
            {
                indexValidationImages++;

                numberValidationImages.Text = indexValidationImages.ToString();
                string path = _generalSettings.NamePrefix + validationImagesPreview[indexValidationImages];
                validationImageViewer.Source = BitmapFromUri(new Uri(path));
                validationImageViewer.Stretch = Stretch.Fill;
            }
        }
        private void ComeBack_Click(object sender, RoutedEventArgs e)
        {
            if (indexValidationImages > 0)
            {
                indexValidationImages--;

                numberValidationImages.Text = indexValidationImages.ToString();
                string path = _generalSettings.NamePrefix + validationImagesPreview[indexValidationImages];
                validationImageViewer.Source = BitmapFromUri(new Uri(path));
                validationImageViewer.Stretch = Stretch.Fill;
            }
        }
    }
}