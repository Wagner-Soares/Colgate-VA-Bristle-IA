using APIVision.Controllers;
using Bristle.UseCases;
using Bristle.utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Bristle.Views
{
    /// <summary>
    /// Interaction logic for AutomaticBristleClassificationPredictionLayer.xaml
    /// </summary>
    public partial class AutomaticBristleClassificationPredictionLayer : Window
    {
        private Point startPoint;
        private Rectangle maskToMultipleSelection;
        protected bool isDragging;
        protected bool isDrawing;
        private Point clickPosition;
        readonly AutomaticBristleClassification _automaticBristleClassification;

        public int XCurrent { get; set; } = 0;
        public int YCurrent { get; set; } = 0;
        public int AnalyzedId { get; set; } = 0;
        public bool EnableSelectMultiple { get; set; }
        public bool EnableBoundignBoxMove { get; set; }
        public List<int> ListOfSelectedItems { get; set; } = new List<int>();

        public AutomaticBristleClassificationPredictionLayer(AutomaticBristleClassification automaticBristleClassification)
        {
            InitializeComponent();

            _automaticBristleClassification = automaticBristleClassification;

            this.MouseLeave += MouseLeaveScreen;
        }

        private void MouseLeaveScreen(object sender, MouseEventArgs e)
        {
            if (EnableSelectMultiple && isDrawing)
            {
                isDrawing = false;

                MultipleSelectionDecision.Visibility = Visibility.Visible;

                FocusSelectedRectangles();
                maskToMultipleSelection = null;
            }
        }

        /// <summary>
        /// Mouse movement control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrameHolder_MouseMove(object sender, MouseEventArgs e)
        {
            //dio
            if (!_automaticBristleClassification.Live && !isDragging)
            {
                int i = 0;
                bool valid = false;
                double WidthR;
                double HeightR;
                double xR;
                double yR;

                SolidColorBrush colorStroke;
                SolidColorBrush colorFill;
                colorStroke = System.Windows.Media.Brushes.Black;
                colorFill = System.Windows.Media.Brushes.Black;

                foreach (var bTest in _automaticBristleClassification.SocketModels)
                {
                    i++;

                    System.Windows.Point mouseActualPosition = e.GetPosition(canvas);

                    WidthR = (bTest.Width * _automaticBristleClassification.W) / CameraObject.ResolutionWidth;
                    HeightR = (bTest.Height * _automaticBristleClassification.H) / CameraObject.ResolutionHeight;
                    xR = (bTest.X * _automaticBristleClassification.W) / CameraObject.ResolutionWidth;
                    yR = (bTest.Y * _automaticBristleClassification.H) / CameraObject.ResolutionHeight;

                    Rect rectForHitTest = new Rect(xR, yR, WidthR, HeightR);

                    if (!(bTest.X == 0 && bTest.Y == 0) && rectForHitTest.Contains(mouseActualPosition))
                    {
                        if ((i != _automaticBristleClassification.SelectedCanvasBoundingBox) && _automaticBristleClassification.SelectedCanvasBoundingBox != -1)
                        {
                            try
                            {
                                canvas.Children.RemoveAt(_automaticBristleClassification.BoundingBoxSelect);
                                canvas.Children.RemoveAt(_automaticBristleClassification.BoundingBoxSelect - 1);
                                canvas.Children.RemoveAt(_automaticBristleClassification.BoundingBoxSelect - 2);
                                canvas.Children.RemoveAt(_automaticBristleClassification.BoundingBoxSelect - 3);
                                canvas.Children.RemoveAt(_automaticBristleClassification.BoundingBoxSelect - 4);
                                _automaticBristleClassification.BoundingBoxSelect = 0;
                                _automaticBristleClassification.SelectedCanvasBoundingBox = -1;
                            }
                            catch
                            {
                                _automaticBristleClassification.BoundingBoxSelect = 0;
                                _automaticBristleClassification.SelectedCanvasBoundingBox = -1;
                            }

                        }
                        _automaticBristleClassification.SelectedCanvasBoundingBox = i;

                        valid = true;
                        if (_automaticBristleClassification.BoundingBoxSelect == 0)
                        {
                            CreateHighlightBox(WidthR, HeightR, xR, yR, colorStroke, colorFill);

                            _automaticBristleClassification.BoundingBoxSelect = canvas.Children.Count - 1;
                        }
                    }
                }

                if (!valid && _automaticBristleClassification.BoundingBoxSelect != 0)
                {
                    try
                    {
                        canvas.Children.RemoveAt(_automaticBristleClassification.BoundingBoxSelect);
                        canvas.Children.RemoveAt(_automaticBristleClassification.BoundingBoxSelect - 1);
                        canvas.Children.RemoveAt(_automaticBristleClassification.BoundingBoxSelect - 2);
                        canvas.Children.RemoveAt(_automaticBristleClassification.BoundingBoxSelect - 3);
                        canvas.Children.RemoveAt(_automaticBristleClassification.BoundingBoxSelect - 4);
                        _automaticBristleClassification.BoundingBoxSelect = 0;
                        _automaticBristleClassification.SelectedCanvasBoundingBox = -1;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
            }           

            _automaticBristleClassification.VerySmallMask = false;
        }

        private void CreateHighlightBox(double WidthR, double HeightR, double xR, double yR, SolidColorBrush colorStroke, SolidColorBrush colorFill)
        {
            var maskUp = new Rectangle
            {
                Stroke = System.Windows.Media.Brushes.Red,
                StrokeThickness = 2,
                Width = 12,
                Height = 12
            };

            var maskDown = new Rectangle
            {
                Stroke = System.Windows.Media.Brushes.Red,
                StrokeThickness = 2,
                Width = 12,
                Height = 12
            };

            var maskLeft = new System.Windows.Shapes.Rectangle
            {
                Stroke = System.Windows.Media.Brushes.Red,
                StrokeThickness = 2,
                Width = 12,
                Height = 12
            };

            var maskRight = new System.Windows.Shapes.Rectangle
            {
                Stroke = System.Windows.Media.Brushes.Red,
                StrokeThickness = 2,
                Width = 12,
                Height = 12
            };

            var rect = new Rectangle
            {
                StrokeThickness = 3,
                Stroke = colorStroke,
                Fill = colorFill,
                Width = WidthR,
                Height = HeightR
            };

            rect.MouseDown += new MouseButtonEventHandler(Control_MouseLeftButtonDown);
            rect.MouseUp += new MouseButtonEventHandler(Control_MouseLeftButtonUp);
            rect.MouseMove += new MouseEventHandler(Control_MouseMove);

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
        }

        private void PerformBristleOperationAccordingToolbox_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _automaticBristleClassification.MaskDelete = true;

            if (!_automaticBristleClassification.Live && _automaticBristleClassification.FrameHolderMouseMove && !isDragging)
            {                
                try
                {
                    switch(_automaticBristleClassification.ToolboxOperation)
                    {
                        case 100:
                            ToolBoxOperationsUseCases.DeleteBoundBoxFromPrediction(_automaticBristleClassification, _automaticBristleClassification.SelectedCanvasBoundingBox - 1);
                            _automaticBristleClassification.DrawBoundingBox();
                            _automaticBristleClassification.FrameHolderMouseMove = true;
                            EnableBoundignBoxMove = false;
                            EnableSelectMultiple = false;
                            MultipleSelectionDecision.Visibility = Visibility.Collapsed;
                            break;
                        case 10:
                            ToolBoxOperationsUseCases.ClassifyBristleAsOk(_automaticBristleClassification, _automaticBristleClassification.SelectedCanvasBoundingBox - 1);
                            _automaticBristleClassification.DrawBoundingBox();
                            EnableBoundignBoxMove = false;
                            EnableSelectMultiple = false;
                            break;
                        case 20:
                            ToolBoxOperationsUseCases.ClassifyBristleAsError1(_automaticBristleClassification, _automaticBristleClassification.SelectedCanvasBoundingBox - 1);
                            _automaticBristleClassification.DrawBoundingBox();
                            EnableBoundignBoxMove = false;
                            EnableSelectMultiple = false;
                            break;
                        case 30:
                            ToolBoxOperationsUseCases.ClassifyBristleAsError2(_automaticBristleClassification, _automaticBristleClassification.SelectedCanvasBoundingBox - 1);
                            _automaticBristleClassification.DrawBoundingBox();
                            EnableBoundignBoxMove = false;
                            EnableSelectMultiple = false;
                            break;
                        case 40:
                            ToolBoxOperationsUseCases.ClassifyBristleAsError3(_automaticBristleClassification, _automaticBristleClassification.SelectedCanvasBoundingBox - 1);
                            _automaticBristleClassification.DrawBoundingBox();
                            EnableBoundignBoxMove = false;
                            EnableSelectMultiple = false;
                            break;
                        case 50:
                            ToolBoxOperationsUseCases.ResizeBristleToSize1(_automaticBristleClassification, _automaticBristleClassification.SelectedCanvasBoundingBox - 1);
                            _automaticBristleClassification.DrawBoundingBox();
                            EnableBoundignBoxMove = false;
                            EnableSelectMultiple = false;
                            break;
                        case 60:
                            ToolBoxOperationsUseCases.ResizeBristleToSize2(_automaticBristleClassification, _automaticBristleClassification.SelectedCanvasBoundingBox - 1);
                            _automaticBristleClassification.DrawBoundingBox();
                            EnableBoundignBoxMove = false;
                            EnableSelectMultiple = false;
                            break;
                        case 70:
                            ToolBoxOperationsUseCases.ResizeBristleToSize3(_automaticBristleClassification, _automaticBristleClassification.SelectedCanvasBoundingBox - 1);
                            _automaticBristleClassification.DrawBoundingBox();
                            EnableBoundignBoxMove = false;
                            EnableSelectMultiple = false;
                            break;
                        case 80:
                            EnableBoundignBoxMove = true;
                            EnableSelectMultiple = false;
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            _automaticBristleClassification.MaskDelete = false;
            e.Handled = true;
        }

        private void ButtonWarningNOK_Click(object sender, RoutedEventArgs e)
        {
            warning.Visibility = Visibility.Collapsed;

            if (_automaticBristleClassification.Position == "N")
            {
                _automaticBristleClassification.StartLoadingWait(false);
                _automaticBristleClassification.Live = false;

                Views.Report report = new Report(_automaticBristleClassification.Maximized, _automaticBristleClassification.businessSystem, _automaticBristleClassification, _automaticBristleClassification.ColgateSkeltaEntities);
                report.Show();

                ResetValue();
                this.Close();
            }
        }

        private void ButtonWarningOK_Click(object sender, RoutedEventArgs e)
        {
            warning.Visibility = Visibility.Collapsed;
            SaveImageToDatabase();

            if (_automaticBristleClassification.Position == "N")
            {
                warning.Visibility = Visibility.Visible;
                _automaticBristleClassification.StartLoadingWait(false);
                _automaticBristleClassification.Live = false;
                Views.Report report = new Report(_automaticBristleClassification.Maximized, _automaticBristleClassification.businessSystem, _automaticBristleClassification, _automaticBristleClassification.ColgateSkeltaEntities);
                report.Show();
                ResetValue();
                this.Close();
            }
        }

        /// <summary>
        /// Saves image in the database for later analysis by the curator.
        /// </summary>
        private void SaveImageToDatabase()
        {
            try
            {
                if ((_automaticBristleClassification.BoundingBox.Count - (_automaticBristleClassification.Defect.Count) < _automaticBristleClassification.BoundingBox.Count) || _automaticBristleClassification.businessSystem.BoundingBoxDiscards.Count > 0 || _automaticBristleClassification.BoundingBoxEdit)
                {
                    _automaticBristleClassification.businessSystem.RegistrationWaitingModel.AnalyzeSet_id = AnalyzedId.ToString();
                    _automaticBristleClassification.businessSystem.RegistrationWaitingModel.DataSet_id = "0";
                    _automaticBristleClassification.businessSystem.RegistrationWaitingModel.Sample_id = "0";
                    _automaticBristleClassification._registrationWaitingController.UpdateRegistrationWaitingModel(_automaticBristleClassification.businessSystem.RegistrationWaitingModel, null);

                    string name = DataHandlerUseCases.FormatTimestamp("image", DateTime.Now.ToString());
                    string nameExt = name + ".png";
                    string nameExtB = "\\" + nameExt;
                    string nameAll = _automaticBristleClassification.GeneralSettings.NamePrefix + nameExtB;
                    CameraObject.DinoLiteSDK.SaveFrame(nameAll);
                    _automaticBristleClassification.businessSystem.TuftTempModel.Position = _automaticBristleClassification.Position;
                    _automaticBristleClassification._tuftTempSetSetController.UpdateTuftTempModel(_automaticBristleClassification.businessSystem.TuftTempModel);
                    _automaticBristleClassification.businessSystem.ImageTempModel.Path = nameExt + "@" + reason.Text;
                    _automaticBristleClassification._imageTempSetSetController.UpdateImageTempModel(_automaticBristleClassification.businessSystem.ImageTempModel);

                    _automaticBristleClassification.BoundingBoxEdit = false;
                }
                else
                {
                    string name = DataHandlerUseCases.FormatTimestamp("image", DateTime.Now.ToString());
                    string nameExt = name + ".png";
                    string nameExtB = "\\" + nameExt;
                    string nameAll = _automaticBristleClassification.GeneralSettings.NamePrefix + nameExtB;
                    CameraObject.DinoLiteSDK.SaveFrame(nameAll);
                    _automaticBristleClassification.businessSystem.RegistrationWaitingModel.AnalyzeSet_id = AnalyzedId.ToString();
                    _automaticBristleClassification.businessSystem.RegistrationWaitingModel.DataSet_id = "0";
                    _automaticBristleClassification.businessSystem.RegistrationWaitingModel.Sample_id = "0";
                    _automaticBristleClassification._registrationWaitingController.UpdateRegistrationWaitingModel(_automaticBristleClassification.businessSystem.RegistrationWaitingModel, null);
                    _automaticBristleClassification.businessSystem.TuftTempModel.Position = _automaticBristleClassification.Position;
                    _automaticBristleClassification._tuftTempSetSetController.UpdateTuftTempModel(_automaticBristleClassification.businessSystem.TuftTempModel);
                    _automaticBristleClassification.businessSystem.ImageTempModel.Path = nameExt + "@" + reason.Text;
                    _automaticBristleClassification._imageTempSetSetController.UpdateImageTempModel(_automaticBristleClassification.businessSystem.ImageTempModel);

                    _automaticBristleClassification.BoundingBoxEdit = false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERRO: {0}", ex.Message);
            }
        }

        private void ResetValue()
        {
            if (Directory.Exists("img"))
            {
                Directory.Delete("img", true);
            }

            _automaticBristleClassification.startAnalyse2_view.Visibility = Visibility.Collapsed;
            XCurrent = 0;
            YCurrent = 0;
            AnalyzedId = 0;

            _automaticBristleClassification.Position = "T";
            _automaticBristleClassification.PositionResultManual = "T";
            _automaticBristleClassification.businessSystem.BoundingBoxDiscards.Clear();
            _automaticBristleClassification.businessSystem.BrushAnalysisResultModel.TotalBristles = 0;
            _automaticBristleClassification.businessSystem.BrushAnalysisResultModel.TotalBristlesAnalyzed = 0;
            _automaticBristleClassification.businessSystem.BrushAnalysisResultModel.TotalGoodBristles = 0;
            _automaticBristleClassification.BoundingBox.Clear();
            _automaticBristleClassification.businessSystem.TuffAnalysisResultModel.SelectedManual = false;
            _automaticBristleClassification.CountBox = 0;
            _automaticBristleClassification.CaptureImage_ = true;
            _automaticBristleClassification.SelectAnalysis = 0;
            _automaticBristleClassification.Cont = 0;
            _automaticBristleClassification.Defect.Clear();
            _automaticBristleClassification.BoundingBoxSelect = 0;
            _automaticBristleClassification.BoundingBoxEdit = false;
            _automaticBristleClassification.StartAnalyzing = false;
            _automaticBristleClassification.SelectedCanvasBoundingBox = -1;
            _automaticBristleClassification.EditBoundingBoxLeft = false;
            _automaticBristleClassification.EditBoundingBoxRight = false;
            _automaticBristleClassification.EditBoundingBoxUp = false;
            _automaticBristleClassification.EditBoundingBoxDown = false;
            _automaticBristleClassification.BoundingBoxSelectType = "undefined";
            _automaticBristleClassification.FrameHolderMouseMove = true;
            _automaticBristleClassification.StartCameraParam = false;
            _automaticBristleClassification.StartAutoFocus = true;
            _automaticBristleClassification.StopAutoFocus = true;
            _automaticBristleClassification.ManualFocusChanged = false;
            _automaticBristleClassification.Adjustment = 0.093F;
            _automaticBristleClassification.ImageCount = 0;
            _automaticBristleClassification.PhotoResult = false;
            _automaticBristleClassification.Error1 = 0;
            _automaticBristleClassification.Error2 = 0;
            _automaticBristleClassification.Error3 = 0;
            _automaticBristleClassification.Discard = 0;
            _automaticBristleClassification.LiveOn_ = true;
            _automaticBristleClassification.MaskDelete = false;
            _automaticBristleClassification.NumberMask = -1;
            _automaticBristleClassification.StartNotConfigureCamera = true;

            _automaticBristleClassification.totalNumberBristlesTtype1.Text = "0";
            _automaticBristleClassification.totalNumberBristlesTtype2.Text = "0";
            _automaticBristleClassification.totalNumberBristlesTtype3.Text = "0";
            _automaticBristleClassification.totalNumberBristlesTdiscard.Text = "0";
            _automaticBristleClassification.totalNumberBristlesT.Text = "0";
            _automaticBristleClassification.totalNumberBristlesTnok.Text = "0";

            _automaticBristleClassification.totalNumberBristlesM1type1.Text = "0";
            _automaticBristleClassification.totalNumberBristlesM1type2.Text = "0";
            _automaticBristleClassification.totalNumberBristlesM1type3.Text = "0";
            _automaticBristleClassification.totalNumberBristlesM1discard.Text = "0";
            _automaticBristleClassification.totalNumberBristlesM1.Text = "0";
            _automaticBristleClassification.totalNumberBristlesM1nok.Text = "0";

            _automaticBristleClassification.totalNumberBristlesM2type1.Text = "0";
            _automaticBristleClassification.totalNumberBristlesM2type2.Text = "0";
            _automaticBristleClassification.totalNumberBristlesM2type3.Text = "0";
            _automaticBristleClassification.totalNumberBristlesM2discard.Text = "0";
            _automaticBristleClassification.totalNumberBristlesM2.Text = "0";
            _automaticBristleClassification.totalNumberBristlesM2nok.Text = "0";

            _automaticBristleClassification.totalNumberBristlesM3type1.Text = "0";
            _automaticBristleClassification.totalNumberBristlesM3type2.Text = "0";
            _automaticBristleClassification.totalNumberBristlesM3type3.Text = "0";
            _automaticBristleClassification.totalNumberBristlesM3discard.Text = "0";
            _automaticBristleClassification.totalNumberBristlesM3.Text = "0";
            _automaticBristleClassification.totalNumberBristlesM3nok.Text = "0";

            _automaticBristleClassification.totalNumberBristlesNtype1.Text = "0";
            _automaticBristleClassification.totalNumberBristlesNtype2.Text = "0";
            _automaticBristleClassification.totalNumberBristlesNtype3.Text = "0";
            _automaticBristleClassification.totalNumberBristlesNdiscard.Text = "0";
            _automaticBristleClassification.totalNumberBristlesN.Text = "0";
            _automaticBristleClassification.totalNumberBristlesNnok.Text = "0";

            warning.Visibility = Visibility.Collapsed;
            _automaticBristleClassification.result_view.Visibility = Visibility.Collapsed;

            _automaticBristleClassification.startAnalyse_view.Visibility = Visibility.Visible;

            _automaticBristleClassification.next.Content = "Next";
            _automaticBristleClassification.next.IsEnabled = true;
        }

        private void ViewSDKImage_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            XCurrent = (int)e.GetPosition(viewSDKImage).X;
            YCurrent = (int)e.GetPosition(viewSDKImage).Y;
            _automaticBristleClassification.BoundingBoxSelectType = "Ok";
            _automaticBristleClassification.WriteBoundingBox();
            _automaticBristleClassification.DrawBoundingBox();

            MultipleSelectionDecision.Visibility = Visibility.Collapsed;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            warning.Visibility = Visibility.Collapsed;
            warningText1.Visibility = Visibility.Visible;
            warningButton1.Visibility = Visibility.Visible;
            newCameraConfigure.Visibility = Visibility.Collapsed;
            SaveCameraConfig();

            if (!_automaticBristleClassification.LiveOn_) _automaticBristleClassification.viewSDK.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Save settings camera settings
        /// </summary>
        private void SaveCameraConfig()
        {
            _automaticBristleClassification.businessSystem.CameraSettingsModel.Name = cameraConfigureName.Text;
            _automaticBristleClassification.businessSystem.CameraSettingsModel.SKU = _automaticBristleClassification.skuSelect.SelectedItem.ToString();
            _automaticBristleClassification.GeneralSettings.Test = _automaticBristleClassification.testSelect.SelectedItem.ToString();
            _automaticBristleClassification.GeneralSettings.Area = _automaticBristleClassification.areaSelect.SelectedItem.ToString();
            _automaticBristleClassification.businessSystem.CameraSettingsModel.Equipment = _automaticBristleClassification.equipmentSelect.SelectedItem.ToString();
            _automaticBristleClassification.GeneralSettings.BatchLote = _automaticBristleClassification.batchLoteT.Text;
            _automaticBristleClassification.GeneralSettings.CurrentCameraConfiguration = _automaticBristleClassification.businessSystem.CameraSettingsModel.Name;
            _automaticBristleClassification.CameraSettingsModels.Add(_automaticBristleClassification.businessSystem.CameraSettingsModel);
            DataHandlerUseCases.SaveOrAppendToSettings(_automaticBristleClassification.businessSystem.CameraSettingsModel, ConfigurationConstants.CameraConfigurationName);
            _automaticBristleClassification.CameraSettingsModels.Clear();
            _automaticBristleClassification.CameraSettingsModels = DataHandlerUseCases.ReadJsonCameraSettings(ConfigurationConstants.CameraConfigurationName);
            _automaticBristleClassification.cameraConfigurationSelection.Items.Add(_automaticBristleClassification.businessSystem.CameraSettingsModel.Name);
            _automaticBristleClassification.cameraConfigurationSelection.SelectedItem = _automaticBristleClassification.businessSystem.CameraSettingsModel.Name;
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            cameraConfigure_.Visibility = Visibility.Collapsed;
            warningText1.Visibility = Visibility.Visible;
            warningButton1.Visibility = Visibility.Visible;
            cameraConfigure_.Visibility = Visibility.Collapsed;

            if (!_automaticBristleClassification.LiveOn_) _automaticBristleClassification.viewSDK.Visibility = Visibility.Visible;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            warning.Visibility = Visibility.Collapsed;
            warningText1.Visibility = Visibility.Visible;
            warningButton1.Visibility = Visibility.Visible;
            newCameraConfigure.Visibility = Visibility.Collapsed;
            newCameraConfigure.Visibility = Visibility.Collapsed;

            if (!_automaticBristleClassification.LiveOn_) _automaticBristleClassification.viewSDK.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Remove a selected camera configuration
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonRemove_Click(object sender, RoutedEventArgs e)
        {
            if (_automaticBristleClassification.GeneralSettings.CurrentCameraConfiguration != ConfigurationConstants.DefaultCameraConfigurationName)
            {
                var configNameToErase = _automaticBristleClassification.GeneralSettings.CurrentCameraConfiguration;

                _automaticBristleClassification.CameraSettingsModels.Remove(_automaticBristleClassification.businessSystem.CameraSettingsModel);

                _automaticBristleClassification.GeneralSettings.CurrentCameraConfiguration = ConfigurationConstants.DefaultCameraConfigurationName;

                _automaticBristleClassification.cameraConfigurationSelection.Items.Remove(configNameToErase);

                if (DataHandlerUseCases.SettingExists(ConfigurationConstants.CameraConfigurationName))
                    DataHandlerUseCases.EraseSetting(ConfigurationConstants.CameraConfigurationName);

                foreach (var item in _automaticBristleClassification.CameraSettingsModels)
                {
                    DataHandlerUseCases.SaveOrAppendToSettings(item, ConfigurationConstants.CameraConfigurationName);
                }

                cameraConfigure_.Visibility = Visibility.Collapsed;
            }
            else
            {
                cameraConfigure_.Visibility = Visibility.Collapsed;
                MessageBox.Show("You cannot remove the default configuration!");
            }

            if (!_automaticBristleClassification.LiveOn_) _automaticBristleClassification.viewSDK.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Subscribe to a selected camera configuration
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonOverride_Click(object sender, RoutedEventArgs e)
        {
            if (_automaticBristleClassification.GeneralSettings.CurrentCameraConfiguration != ConfigurationConstants.DefaultCameraConfigurationName)
            {
                if (DataHandlerUseCases.SettingExists(ConfigurationConstants.CameraConfigurationName))
                {
                    DataHandlerUseCases.EraseSetting(ConfigurationConstants.CameraConfigurationName);
                }

                foreach (var item in _automaticBristleClassification.CameraSettingsModels)
                {
                    if (item.Name == _automaticBristleClassification.GeneralSettings.CurrentCameraConfiguration)
                    {
                        DataHandlerUseCases.SaveOrAppendToSettings(_automaticBristleClassification.businessSystem.CameraSettingsModel, ConfigurationConstants.CameraConfigurationName);
                    }
                    else
                    {
                        DataHandlerUseCases.SaveOrAppendToSettings(item, ConfigurationConstants.CameraConfigurationName);
                    }
                }
                cameraConfigure_.Visibility = Visibility.Collapsed;
            }
            else
            {
                cameraConfigure_.Visibility = Visibility.Collapsed;
                MessageBox.Show("Default setting cannot be changed!");
            }

            if (!_automaticBristleClassification.LiveOn_) _automaticBristleClassification.viewSDK.Visibility = Visibility.Visible;
        }

        private void ButtonNew_Click(object sender, RoutedEventArgs e)
        {
            cameraConfigure_.Visibility = Visibility.Collapsed;
            newCameraConfigure.Visibility = Visibility.Visible;
        }

        #region Move Function

        private void Control_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (EnableBoundignBoxMove)
            {
                isDragging = true;

                var draggableControl = sender as Rectangle;
                clickPosition = e.GetPosition(this);
                draggableControl.CaptureMouse();
            }
        }

        private void Control_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (EnableBoundignBoxMove)
            {
                isDragging = false;
                var draggable = sender as Rectangle;

                var clickReleasePosition = e.GetPosition(this);

                Point translateDiff = new Point(clickReleasePosition.X - clickPosition.X, clickReleasePosition.Y - clickPosition.Y);

                Point newBoundingBoxPosition = new Point(Canvas.GetLeft(canvas.Children[(_automaticBristleClassification.SelectedCanvasBoundingBox - 1) * 2]) + translateDiff.X,
                                                        Canvas.GetTop(canvas.Children[(_automaticBristleClassification.SelectedCanvasBoundingBox - 1) * 2]) + translateDiff.Y);

                var moveBoundingBoxAnimX = new DoubleAnimation(Canvas.GetLeft(canvas.Children[(_automaticBristleClassification.SelectedCanvasBoundingBox - 1) * 2]), newBoundingBoxPosition.X, new Duration(TimeSpan.FromSeconds(1)));
                var moveBoundingBoxAnimY = new DoubleAnimation(Canvas.GetTop(canvas.Children[(_automaticBristleClassification.SelectedCanvasBoundingBox - 1) * 2]), newBoundingBoxPosition.Y, new Duration(TimeSpan.FromSeconds(1)));

                Point newBoundingBoxClassPosition = new Point(Canvas.GetLeft(canvas.Children[((_automaticBristleClassification.SelectedCanvasBoundingBox - 1) * 2) + 1]) + translateDiff.X,
                                                        Canvas.GetTop(canvas.Children[((_automaticBristleClassification.SelectedCanvasBoundingBox - 1) * 2) + 1]) + translateDiff.Y);

                var moveBoundingBoxClassAnimX = new DoubleAnimation(Canvas.GetLeft(canvas.Children[((_automaticBristleClassification.SelectedCanvasBoundingBox - 1) * 2) + 1]), newBoundingBoxClassPosition.X, new Duration(TimeSpan.FromSeconds(1)));
                var moveBoundingBoxClassAnimY = new DoubleAnimation(Canvas.GetTop(canvas.Children[((_automaticBristleClassification.SelectedCanvasBoundingBox - 1) * 2) + 1]), newBoundingBoxClassPosition.Y, new Duration(TimeSpan.FromSeconds(1)));

                canvas.Children[(_automaticBristleClassification.SelectedCanvasBoundingBox - 1) * 2].BeginAnimation(Canvas.LeftProperty, moveBoundingBoxAnimX);
                canvas.Children[(_automaticBristleClassification.SelectedCanvasBoundingBox - 1) * 2].BeginAnimation(Canvas.TopProperty, moveBoundingBoxAnimY);
                canvas.Children[((_automaticBristleClassification.SelectedCanvasBoundingBox - 1) * 2) + 1].BeginAnimation(Canvas.LeftProperty, moveBoundingBoxClassAnimX);
                canvas.Children[((_automaticBristleClassification.SelectedCanvasBoundingBox - 1) * 2) + 1].BeginAnimation(Canvas.TopProperty, moveBoundingBoxClassAnimY);

                MoveBox(_automaticBristleClassification.SelectedCanvasBoundingBox - 1, translateDiff.X, translateDiff.Y);

                draggable.ReleaseMouseCapture();
            }
        }

        private void Control_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging && sender is Rectangle draggableControl)
            {
                Point currentPosition = e.GetPosition(this.Parent as UIElement);

                if (!(draggableControl.RenderTransform is TranslateTransform transform))
                {
                    transform = new TranslateTransform();
                    draggableControl.RenderTransform = transform;
                }

                transform.X = currentPosition.X - clickPosition.X;
                transform.Y = currentPosition.Y - clickPosition.Y;
            }
        }

        private void MoveBox(int socketPredictionNumber, double deltaPosX, double deltaPosY)
        {
            double xR = (deltaPosX * CameraObject.ResolutionWidth) / _automaticBristleClassification.W;
            double yR = (deltaPosY * CameraObject.ResolutionHeight) / _automaticBristleClassification.H;

            _automaticBristleClassification.SocketModels[socketPredictionNumber].X += (int)xR;
            _automaticBristleClassification.SocketModels[socketPredictionNumber].Y += (int)yR;
        }

        #endregion

        #region Mask For Multiple Selection

        private void CanvasMask_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (EnableSelectMultiple)
            {
                isDrawing = true;

                BtnCancelMultiple_Click(null, null);

                startPoint = e.GetPosition(canvasMask);

                maskToMultipleSelection = new Rectangle
                {
                    Stroke = Brushes.LightBlue,
                    StrokeThickness = 2
                };
                Canvas.SetLeft(maskToMultipleSelection, startPoint.X);
                Canvas.SetTop(maskToMultipleSelection, startPoint.Y);
                canvasMask.Children.Add(maskToMultipleSelection);
            }
        }

        private void CanvasMask_MouseMove(object sender, MouseEventArgs e)
        {
            if (EnableSelectMultiple)
            {
                if (e.LeftButton == MouseButtonState.Released || maskToMultipleSelection == null)
                    return;

                var pos = e.GetPosition(canvasMask);

                var x = Math.Min(pos.X, startPoint.X);
                var y = Math.Min(pos.Y, startPoint.Y);

                var w = Math.Max(pos.X, startPoint.X) - x;
                var h = Math.Max(pos.Y, startPoint.Y) - y;

                maskToMultipleSelection.Width = w;
                maskToMultipleSelection.Height = h;

                Canvas.SetLeft(maskToMultipleSelection, x);
                Canvas.SetTop(maskToMultipleSelection, y);
            }
        }

        private void CanvasMask_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (EnableSelectMultiple && isDrawing)
            {
                isDrawing = false;

                MultipleSelectionDecision.Visibility = Visibility.Visible;

                FocusSelectedRectangles();
                maskToMultipleSelection = null;
            }
        }

        private void FocusSelectedRectangles()
        {
            Rect rectFromMultipleSelectionMask = new Rect(Canvas.GetLeft(canvasMask.Children[0]), Canvas.GetTop(canvasMask.Children[0]), maskToMultipleSelection.Width, maskToMultipleSelection.Height);
            ListOfSelectedItems.Clear();

            for (int i = 0; i < canvas.Children.Count; i++)
            {
                if (i % 2 == 0)
                {
                    try
                    {
                        Rectangle rectangleTemp = (Rectangle)canvas.Children[i];
                        Rect rectToAnalyze = new Rect(Canvas.GetLeft(canvas.Children[i]), Canvas.GetTop(canvas.Children[i]), rectangleTemp.Width, rectangleTemp.Height);

                        bool intersects = rectFromMultipleSelectionMask.Contains(rectToAnalyze);

                        if (intersects)
                        {
                            ListOfSelectedItems.Add(i);

                            canvas.Children[i].Visibility = Visibility.Collapsed;

                            Rectangle rectFocus = new Rectangle
                            {
                                StrokeThickness = 3,
                                Width = rectToAnalyze.Width,
                                Height = rectToAnalyze.Height
                            };

                            rectFocus.Fill = new SolidColorBrush(Colors.Blue);
                            rectFocus.Opacity = 0.2;

                            Canvas.SetLeft(rectFocus, rectToAnalyze.X);
                            Canvas.SetTop(rectFocus, rectToAnalyze.Y);

                            canvasMask.Children.Add(rectFocus);
                        }
                    }
                    catch
                    {
                        //Prevent crash for itens that cannot be cast to Rectangles
                    }
                }
            }
        }

        private void BtnCancelMultiple_Click(object sender, RoutedEventArgs e)
        {
            MultipleSelectionDecision.Visibility = Visibility.Collapsed;
            canvasMask.Children.Clear();
            canvas.Children.Clear();
            _automaticBristleClassification.DrawBoundingBox();
        }

        private void BtnOkMultiple_Click(object sender, RoutedEventArgs e)
        {
            foreach(var boxToClassify in ListOfSelectedItems)
            {
                ToolBoxOperationsUseCases.ClassifyBristleAsOk(_automaticBristleClassification, (boxToClassify/2));
            }

            _automaticBristleClassification.DrawBoundingBox();
            MultipleSelectionDecision.Visibility = Visibility.Collapsed;
            canvasMask.Children.Clear();
        }

        private void BtnError1Multiple_Click(object sender, RoutedEventArgs e)
        {
            foreach (var boxToClassify in ListOfSelectedItems)
            {
                ToolBoxOperationsUseCases.ClassifyBristleAsError1(_automaticBristleClassification, (boxToClassify / 2));
            }

            _automaticBristleClassification.DrawBoundingBox();
            MultipleSelectionDecision.Visibility = Visibility.Collapsed;
            canvasMask.Children.Clear();
        }

        private void BtnError2Multiple_Click(object sender, RoutedEventArgs e)
        {
            foreach (var boxToClassify in ListOfSelectedItems)
            {
                ToolBoxOperationsUseCases.ClassifyBristleAsError2(_automaticBristleClassification, (boxToClassify / 2));
            }

            _automaticBristleClassification.DrawBoundingBox();
            MultipleSelectionDecision.Visibility = Visibility.Collapsed;
            canvasMask.Children.Clear();
        }

        private void BtnError3Multiple_Click(object sender, RoutedEventArgs e)
        {
            foreach (var boxToClassify in ListOfSelectedItems)
            {
                ToolBoxOperationsUseCases.ClassifyBristleAsError3(_automaticBristleClassification, (boxToClassify / 2));
            }

            _automaticBristleClassification.DrawBoundingBox();
            MultipleSelectionDecision.Visibility = Visibility.Collapsed;
            canvasMask.Children.Clear();
        }

        private void BtnDeleteAllMultiple_Click(object sender, RoutedEventArgs e)
        {
            foreach (var boxToClassify in ListOfSelectedItems.OrderByDescending(item => item))
            {
                ToolBoxOperationsUseCases.DeleteBoundBoxFromPrediction(_automaticBristleClassification, (boxToClassify / 2));
            }

            _automaticBristleClassification.DrawBoundingBox();
            MultipleSelectionDecision.Visibility = Visibility.Collapsed;
            canvasMask.Children.Clear();
        }

        #endregion

    }
}
