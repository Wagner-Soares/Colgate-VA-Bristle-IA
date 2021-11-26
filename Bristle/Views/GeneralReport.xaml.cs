using APIVision;
using APIVision.Controllers;
using APIVision.Models;
using System;
using System.Collections.Generic;
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

namespace Bristle.Views
{
    /// <summary>
    /// Lógica interna para GeneralReport.xaml
    /// </summary>
    public partial class GeneralReport : Window
    {
        private static bool maximized = true;
        private BusinessSystem businessSystem;
        private bool maximized__ = false;
        private Views.AutomaticBristleClassification automaticBristleClassification;
        private Thread threadLoadingWait;
        private Thread threadLoadingWait_;
        private LoadingAnimation loading;
        public GeneralReport(bool maximized_, BusinessSystem businessSystem_, Views.AutomaticBristleClassification automaticBristleClassification_)
        {
            StartLoadingWait(true);

            businessSystem = businessSystem_;
            maximized__ = maximized_;
            automaticBristleClassification = automaticBristleClassification_;

            InitializeComponent();
           
            this.Loaded += GeneralReport_Loaded;

            LoadDB();

            LoadPieChartDataA();
            LoadPieChartDataB();
            LoadPieChartDataC();

            skuSelect.Items.Add("Select SKU");
            foreach (var item in businessSystem.dataBaseController.listSKUsModel())
            {
                skuSelect.Items.Add(item.sSKU);
            }
            skuSelect.SelectedIndex = 0;

            machineSelect.Items.Add("Select Equipment");
            foreach (var item in businessSystem.dataBaseController.listEquipmentModel("*"))
            {
                machineSelect.Items.Add(item.iEquipment_id);
            }
            machineSelect.SelectedIndex = 0;

            StartLoadingWait(false);
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
            loading = new Views.LoadingAnimation("other");
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
                datagridBrushAnalysisResult.ItemsSource = businessSystem.dataBaseController.listBrushAnalysisResultModel(-1, null, null, null, null);
                datagridBrushAnalysisResult.Visibility = Visibility.Visible;
                datagridBristleAnalysisResult.Visibility = Visibility.Collapsed;
                datagridTuffAnalysisResult.Visibility = Visibility.Collapsed;
            }
            catch 
            {
            }
        }

        private void LoadPieChartDataA()
        {
            //((PieSeries)mcChartA.Series[0]).ItemsSource =
            //    new KeyValuePair<string, int>[]{
            //    new KeyValuePair<string,int>(12+"%"+" :Defect A", 12),
            //    new KeyValuePair<string,int>(25+"%"+" :Defect B", 25),
            //    new KeyValuePair<string,int>(5+"%"+" :Defect C", 5),
            //    new KeyValuePair<string,int>(6+"%"+" :Defect D", 6),
            //    new KeyValuePair<string,int>(10+"%"+" :Defect E", 10),
            //    new KeyValuePair<string,int>(4+"%"+" :Defect F", 4) };
        }

        private void LoadPieChartDataB()
        {
            //((PieSeries)mcChartB.Series[0]).ItemsSource =
            //    new KeyValuePair<string, int>[]{
            //    new KeyValuePair<string,int>(12+"%"+" :Defect A", 12),
            //    new KeyValuePair<string,int>(25+"%"+" :Defect B", 25),
            //    new KeyValuePair<string,int>(5+"%"+" :Defect C", 5),
            //    new KeyValuePair<string,int>(6+"%"+" :Defect D", 6),
            //    new KeyValuePair<string,int>(10+"%"+" :Defect E", 10),
            //    new KeyValuePair<string,int>(4+"%"+" :Defect F", 4) };
        }

        private void LoadPieChartDataC()
        {
            //((PieSeries)mcChartC.Series[0]).ItemsSource =
            //    new KeyValuePair<string, int>[]{
            //    new KeyValuePair<string,int>(12+"%"+" :Defect A", 12),
            //    new KeyValuePair<string,int>(25+"%"+" :Defect B", 25),
            //    new KeyValuePair<string,int>(5+"%"+" :Defect C", 5),
            //    new KeyValuePair<string,int>(6+"%"+" :Defect D", 6),
            //    new KeyValuePair<string,int>(10+"%"+" :Defect E", 10),
            //    new KeyValuePair<string,int>(4+"%"+" :Defect F", 4) };
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
            if (businessSystem.userSystemCurrent.Type == "administrator")
            {
                Views.NeuralNetworkRetraining neuralNetworkRetraining = new NeuralNetworkRetraining(maximized, businessSystem, automaticBristleClassification);
                neuralNetworkRetraining.Show();
                this.Close();
            }
            else
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
                MessageBox.Show("Necessary administrative rights!");
            }
        }

        private void ButtonGeneralReport_Click(object sender, RoutedEventArgs e)
        {
            //Views.GeneralReport generalReport = new GeneralReport(maximized, businessSystem);
            //generalReport.Show();
            //this.Close();
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

        private void ButtonHome_Click_1(object sender, RoutedEventArgs e)
        {
            Views.MainWindow mainWindow = new Views.MainWindow(maximized, businessSystem);
            mainWindow.Show();
            this.Close();
        }

        private void ButtonBristleRegister_Click_2(object sender, RoutedEventArgs e)
        {
            Views.GeneralSettings generalSettings = new Views.GeneralSettings(maximized, businessSystem, null);
            generalSettings.Show();
            this.Close();
        }

        private void ButtonGeneralReport_Click_1(object sender, RoutedEventArgs e)
        {
            Views.GeneralReport generalReport = new GeneralReport(maximized, businessSystem, automaticBristleClassification);
            generalReport.Show();
            this.Close();
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
                MessageBox.Show("Necessary administrative rights!");
            }
        }

        private void datagridGeneralReport_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void bristle_Click(object sender, RoutedEventArgs e)
        {
            var cell = datagridBrushAnalysisResult.SelectedItem;
            BrushAnalysisResultModel cell_ = (BrushAnalysisResultModel)cell;   
            datagridBristleAnalysisResult.ItemsSource = businessSystem.dataBaseController.listBristleAnalysisResultModel(-1, cell_);
            datagridBrushAnalysisResult.Visibility = Visibility.Collapsed;
            datagridBristleAnalysisResult.Visibility = Visibility.Visible;
            datagridTuffAnalysisResult.Visibility = Visibility.Collapsed;
            return_.Visibility = Visibility.Visible;
            apply.Visibility = Visibility.Collapsed;
        }

        private void tuft_Click(object sender, RoutedEventArgs e)
        {
            var cell = datagridBrushAnalysisResult.SelectedItem;
            BrushAnalysisResultModel cell_ = (BrushAnalysisResultModel)cell;
            datagridTuffAnalysisResult.ItemsSource = businessSystem.dataBaseController.listTuffAnalysisResultModel(-1, cell_);
            datagridBrushAnalysisResult.Visibility = Visibility.Collapsed;
            datagridBristleAnalysisResult.Visibility = Visibility.Collapsed;
            datagridTuffAnalysisResult.Visibility = Visibility.Visible;
            return_.Visibility = Visibility.Visible;
            apply.Visibility = Visibility.Collapsed;
        }

        private void return__Click(object sender, RoutedEventArgs e)
        {
            datagridBrushAnalysisResult.ItemsSource = businessSystem.dataBaseController.listBrushAnalysisResultModel(-1, null, null, null, null);
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

        private void apply_Click(object sender, RoutedEventArgs e)
        {
            SelectedDatesCollection dates = Calendar.SelectedDates;

            datagridBrushAnalysisResult.ItemsSource = businessSystem.dataBaseController.listBrushAnalysisResultModel
                (-2, null, (string)skuSelect.Items[skuSelect.SelectedIndex], 
                (string)machineSelect.Items[machineSelect.SelectedIndex], dates);

            datagridBrushAnalysisResult.Visibility = Visibility.Visible;
            datagridBristleAnalysisResult.Visibility = Visibility.Collapsed;
            datagridTuffAnalysisResult.Visibility = Visibility.Collapsed;
        }

        private void MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Views.Help help = new Views.Help(maximized, businessSystem, automaticBristleClassification);
            help.Show();
        }
    }
}
