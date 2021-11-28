using APIVision;
using APIVision.Controllers;
using APIVision.Controllers.DataBaseControllers;
using APIVision.DataModels;
using Bristle.UseCases;
using Database;
using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Bristle.Views
{
    /// <summary>
    /// Lógica interna para GeneralReport.xaml
    /// </summary>
    public partial class GeneralReport : Window
    {
        private bool maximized = true;
        private readonly BusinessSystem businessSystem;
        private readonly bool maximized__ = false;
        private Thread threadLoadingWait;
        private Thread threadLoadingWait_;
        private LoadingAnimation loading;

        private readonly ColgateSkeltaEntities _colgateSkeltaEntities;

        private readonly SkuController _sKUController;
        private readonly EquipmentController _equipmentController;
        private readonly BrushAnalysisResultSetController _brushAnalysisResultSetController;
        private readonly BristleAnalysisResultSetController _bristleAnalysisResultSetController;
        private readonly TuffAnalysisResultSetController _tuffAnalysisResultSetController;

        public GeneralReport(bool maximized_, BusinessSystem businessSystem_, ColgateSkeltaEntities colgateSkeltaEntities)
        {
            StartLoadingWait(true);

            businessSystem = businessSystem_;
            maximized__ = maximized_;

            _colgateSkeltaEntities = colgateSkeltaEntities;

            _sKUController = new SkuController(colgateSkeltaEntities);
            _equipmentController = new EquipmentController(colgateSkeltaEntities);
            _brushAnalysisResultSetController = new BrushAnalysisResultSetController(colgateSkeltaEntities);
            _bristleAnalysisResultSetController = new BristleAnalysisResultSetController(colgateSkeltaEntities);
            _tuffAnalysisResultSetController = new TuffAnalysisResultSetController(colgateSkeltaEntities);

            InitializeComponent();
           
            this.Loaded += GeneralReport_Loaded;

            LoadDB();

            skuSelect.Items.Add("Select SKU");
            foreach (var item in _sKUController.ListSKUsModel())
            {
                skuSelect.Items.Add(item.SSKU);
            }
            skuSelect.SelectedIndex = 0;

            machineSelect.Items.Add("Select Equipment");
            foreach (var item in _equipmentController.ListEquipmentModel("*"))
            {
                machineSelect.Items.Add(item.IEquipment_id);
            }
            machineSelect.SelectedIndex = 0;

            StartLoadingWait(false);
        }

        private void StartLoadingWait(bool start)
        {
            if (start)
            {
                threadLoadingWait = new Thread(() => ThreadMethod())
                {
                    Name = "threadLoadingWait"
                };
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
        private void LoadingWait_()
        {
            loading = new Views.LoadingAnimation("other");
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

        private void GeneralReport_Loaded(object sender, RoutedEventArgs e)
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

        private void LoadDB()
        {
            try
            {
                datagridBrushAnalysisResult.ItemsSource = _brushAnalysisResultSetController.ListBrushAnalysisResultModel(-1, null, null, null);
                datagridBrushAnalysisResult.Visibility = Visibility.Visible;
                datagridBristleAnalysisResult.Visibility = Visibility.Collapsed;
                datagridTuffAnalysisResult.Visibility = Visibility.Collapsed;
            }
            catch 
            {
                //tryCatch to avoid crash
            }
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
            if (ScreenNavigationUseCases.OpenMainScreen(ScreenNavigationUseCases.GetGeneralLocalSettings(), businessSystem, _colgateSkeltaEntities, maximized))
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

        private void ButtonBristleRegister_Click_2(object sender, RoutedEventArgs e)
        {
            Views.GeneralSettings generalSettings = new Views.GeneralSettings(maximized, businessSystem, _colgateSkeltaEntities);
            generalSettings.Show();
            this.Close();
        }

        private void ButtonGeneralReport_Click_1(object sender, RoutedEventArgs e)
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
        private void CameraCalibration_Click(object sender, RoutedEventArgs e)
        {
            Views.CameraCalibration cameraCalibration = new Views.CameraCalibration(maximized, businessSystem, _colgateSkeltaEntities);
            cameraCalibration.Show();
            this.Close();
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

        private void Bristle_Click(object sender, RoutedEventArgs e)
        {
            var cell = datagridBrushAnalysisResult.SelectedItem;
            BrushAnalysisResultModel cell_ = (BrushAnalysisResultModel)cell;   
            datagridBristleAnalysisResult.ItemsSource = _bristleAnalysisResultSetController.ListBristleAnalysisResultModel(-1, cell_);
            datagridBrushAnalysisResult.Visibility = Visibility.Collapsed;
            datagridBristleAnalysisResult.Visibility = Visibility.Visible;
            datagridTuffAnalysisResult.Visibility = Visibility.Collapsed;
            return_.Visibility = Visibility.Visible;
            apply.Visibility = Visibility.Collapsed;
        }

        private void Tuft_Click(object sender, RoutedEventArgs e)
        {
            var cell = datagridBrushAnalysisResult.SelectedItem;
            BrushAnalysisResultModel cell_ = (BrushAnalysisResultModel)cell;
            datagridTuffAnalysisResult.ItemsSource = _tuffAnalysisResultSetController.ListTuffAnalysisResultModel(-1, cell_);
            datagridBrushAnalysisResult.Visibility = Visibility.Collapsed;
            datagridBristleAnalysisResult.Visibility = Visibility.Collapsed;
            datagridTuffAnalysisResult.Visibility = Visibility.Visible;
            return_.Visibility = Visibility.Visible;
            apply.Visibility = Visibility.Collapsed;
        }

        private void Return__Click(object sender, RoutedEventArgs e)
        {
            datagridBrushAnalysisResult.ItemsSource = _brushAnalysisResultSetController.ListBrushAnalysisResultModel(-1, null, null, null);
            datagridBrushAnalysisResult.Visibility = Visibility.Visible;
            datagridBristleAnalysisResult.Visibility = Visibility.Collapsed;
            datagridTuffAnalysisResult.Visibility = Visibility.Collapsed;
            return_.Visibility = Visibility.Collapsed;
            apply.Visibility = Visibility.Visible;
        }
        private void SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if(Calendar.Visibility == Visibility.Visible)
            {
                Calendar.Visibility = Visibility.Collapsed;
            }
            else
            {
                Calendar.Visibility = Visibility.Visible;
            }
        }

        private void Apply_Click(object sender, RoutedEventArgs e)
        {
            SelectedDatesCollection dates = Calendar.SelectedDates;

            datagridBrushAnalysisResult.ItemsSource = _brushAnalysisResultSetController.ListBrushAnalysisResultModel
                (-2, (string)skuSelect.Items[skuSelect.SelectedIndex], 
                (string)machineSelect.Items[machineSelect.SelectedIndex], dates);

            datagridBrushAnalysisResult.Visibility = Visibility.Visible;
            datagridBristleAnalysisResult.Visibility = Visibility.Collapsed;
            datagridTuffAnalysisResult.Visibility = Visibility.Collapsed;
        }

        private new void MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Views.Help help = new Views.Help();
            help.Show();
        }
    }
}
