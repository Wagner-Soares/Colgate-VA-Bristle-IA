using APIVision;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Controls.DataVisualization.Charting;
using APIVision.Controllers;
using System.Threading;

namespace Bristle.Views
{
    /// <summary>
    /// Lógica interna para Report.xaml
    /// </summary>
    public partial class Report : Window
    {
        private static bool maximized = true;
        private BusinessSystem businessSystem;
        private int noneTotal;
        private bool maximized__ = false;
        private Views.AutomaticBristleClassification automaticBristleClassification;
        private Thread threadLoadingWait;
        private LoadingAnimation loading;
        public Report(bool maximized_, BusinessSystem businessSystem_, Views.AutomaticBristleClassification automaticBristleClassification_)
        {           
            businessSystem = businessSystem_;
            maximized__ = maximized_;
            automaticBristleClassification = automaticBristleClassification_;

            InitializeComponent();

            //StartLoadingWait(true);

            try
            {
                user_.Text = businessSystem.userSystemCurrent.Name;

                //saving final result in skelta's sample log table 
                businessSystem.Endround.sOperator = businessSystem.userSystemCurrent.Name;
                businessSystem.Endround.sCreated_by = businessSystem.userSystemCurrent.Name;
                businessSystem.NFiosSpec.sOperator = businessSystem.userSystemCurrent.Name;
                businessSystem.NFiosSpec.sCreated_by = businessSystem.userSystemCurrent.Name;

                try
                {
                    businessSystem.tuffAnalysisResultModels = new List<APIVision.Models.TuffAnalysisResultModel>();
                    businessSystem.tuffAnalysisResultModels = new List<APIVision.Models.TuffAnalysisResultModel>();
                    businessSystem.brushAnalysisResultModels = new List<APIVision.Models.BrushAnalysisResultModel>();
                    businessSystem.bristleAnalysisResultModels = new List<APIVision.Models.BristleAnalysisResultModel>();
                }
                catch
                {
                    
                }

                businessSystem.analyzeModels = businessSystem.dataBaseController.listAnalyzeModel(true);

                equipament.Text = businessSystem.analyzeModels[businessSystem.analyzeModels.Count - 1].Equipament;
                sku.Text = businessSystem.analyzeModels[businessSystem.analyzeModels.Count - 1].Name;

                //saving final result in skelta's sample log table 
                businessSystem.Endround.dtSample = businessSystem.analyzeModels[businessSystem.analyzeModels.Count - 1].Timestamp;
                businessSystem.Endround.dtPublished_at = businessSystem.analyzeModels[businessSystem.analyzeModels.Count - 1].Timestamp;
                businessSystem.Endround.dtCreated_at = businessSystem.analyzeModels[businessSystem.analyzeModels.Count - 1].Timestamp;
                businessSystem.Endround.sEquipament = businessSystem.analyzeModels[businessSystem.analyzeModels.Count - 1].Equipament;
                businessSystem.Endround.iShift = shiftComparison(businessSystem.analyzeModels[businessSystem.analyzeModels.Count - 1].Timestamp);
                businessSystem.Endround.sArea = businessSystem.generalSettings.Area;
                businessSystem.Endround.sBatchLote = businessSystem.generalSettings.BatchLote;
                businessSystem.Endround.iTest_id = businessSystem.generalSettings.IdTest_1;
                businessSystem.Endround.bActive = true;
                businessSystem.Endround.sComments = "";
                businessSystem.NFiosSpec.dtSample = businessSystem.analyzeModels[businessSystem.analyzeModels.Count - 1].Timestamp;
                businessSystem.NFiosSpec.dtPublished_at = businessSystem.analyzeModels[businessSystem.analyzeModels.Count - 1].Timestamp;
                businessSystem.NFiosSpec.dtCreated_at = businessSystem.analyzeModels[businessSystem.analyzeModels.Count - 1].Timestamp;
                businessSystem.NFiosSpec.sEquipament = businessSystem.analyzeModels[businessSystem.analyzeModels.Count - 1].Equipament;
                businessSystem.NFiosSpec.iShift = shiftComparison(businessSystem.analyzeModels[businessSystem.analyzeModels.Count - 1].Timestamp);
                businessSystem.NFiosSpec.sArea = businessSystem.generalSettings.Area;
                businessSystem.NFiosSpec.sBatchLote = businessSystem.generalSettings.BatchLote;
                businessSystem.NFiosSpec.iTest_id = businessSystem.generalSettings.IdTest_2;
                businessSystem.NFiosSpec.bActive = true;
                businessSystem.NFiosSpec.sComments = "";

                businessSystem.shiftsModel = businessSystem.dataBaseController.listShiftsModel();

                //businessSystem.Endround.iTest_id = businessSystem.generalSettings.Test;

                //sku.Text = businessSystem.sKUModel.Name;
                businessSystem.brushAnalysisResultModels = businessSystem.dataBaseController.listBrushAnalysisResultModel(businessSystem.analyzeModels[businessSystem.analyzeModels.Count - 1].Id, null, null, null, null);
                //LoadPieChartData_result();

                totalBristle.Text = businessSystem.brushAnalysisResultModels[businessSystem.brushAnalysisResultModels.Count - 1].TotalBristles.ToString();
                //totalGoodBristles.Text = businessSystem.brushAnalysisResultModels[businessSystem.brushAnalysisResultModels.Count - 1].TotalGoodBristles.ToString();
                businessSystem.tuffAnalysisResultModels = businessSystem.dataBaseController.listTuffAnalysisResultModel(businessSystem.analyzeModels[businessSystem.analyzeModels.Count - 1].Id, null);
                businessSystem.bristleAnalysisResultModels = businessSystem.dataBaseController.listBristleAnalysisResultModel(businessSystem.analyzeModels[businessSystem.analyzeModels.Count - 1].Id, null);

                init();
                LoadPieChartDataT();
                LoadPieChartDataM1();
                LoadPieChartDataM2();
                LoadPieChartDataM3();
                LoadPieChartDataN();
            }
            catch
            {
            }

            this.Loaded += Report_Loaded;

            //StartLoadingWait(false);
        }

        /// <summary>
        /// StartLoadingWait
        /// </summary>
        /// <param name="start"></param>
        private void StartLoadingWait(bool start)
        {
            //if (start)
            //{
            //    threadLoadingWait = new Thread(() => ThreadMethod());
            //    threadLoadingWait.SetApartmentState(ApartmentState.STA); //Set the thread to STA
            //    //threadLoadingWait.IsBackground = true;
            //    threadLoadingWait.Start();
            //}
            //else
            //{
            //    Thread stopLoading_ = new Thread(() => stopLoading());
            //    stopLoading_.SetApartmentState(ApartmentState.STA); //Set the thread to STA
            //    //wait__.IsBackground = true;
            //    stopLoading_.Start();
            //}
        }

        //private void stopLoading()
        //{
        //    if (loading != null)
        //    {
        //        loading.CloseMe();
        //        loading = null;
        //    }
        //}

        //private void LoadingWait_()
        //{
        //    loading = new Views.LoadingAnimation("other");
        //    loading.ShowDialog();
        //}

        //private void ThreadMethod()
        //{
        //    if (loading == null)
        //    {
        //        //Dispatcher.BeginInvoke((Action)(() => { LoadingWait_(); }));
        //        LoadingWait_();
        //    }
        //    else
        //    {
        //        loading.CloseMe();
        //        loading = null;
        //    }
        //}

        private int shiftComparison(DateTime dateTime)
        {
            businessSystem.shiftsModel = businessSystem.dataBaseController.listShiftsModel();

            if(dateTime >= DateTime.Parse(businessSystem.shiftsModel[0].Shift_start) && dateTime <= DateTime.Parse(businessSystem.shiftsModel[0].Shift_end))
            {
                return 1;
            }
            else if (dateTime >= DateTime.Parse(businessSystem.shiftsModel[1].Shift_start) && dateTime <= DateTime.Parse(businessSystem.shiftsModel[1].Shift_end))
            {
                return 2;
            }
            else if (dateTime >= DateTime.Parse(businessSystem.shiftsModel[2].Shift_start) && dateTime <= DateTime.Parse(businessSystem.shiftsModel[2].Shift_end))
            {
                return 3;
            }
            else
            {
                return 0;
            }
        }

        private void Report_Loaded(object sender, RoutedEventArgs e)
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

        private void init()
        {
            tuftNominalT.Text = ProductionObject.nominalBristle.ToString();
            tuftNominalM1.Text = ProductionObject.nominalBristle.ToString();
            tuftNominalM2.Text = ProductionObject.nominalBristle.ToString();
            tuftNominalM3.Text = ProductionObject.nominalBristle.ToString();
            tuftNominalN.Text = ProductionObject.nominalBristle.ToString();
            nominalTotal.Text = (ProductionObject.nominalBristle * 5).ToString();
        }

        private void LoadPieChartData_result()
        {
            int r1 = ((int.Parse(totalGoodBristles.Text)) * 100) / (noneTotal);
            int r2 = 100 - r1;

            mcChart.LegendTitle = "Total: " + (noneTotal).ToString();  
            mcChart.FontSize = 24;

            //saving final result in skelta's sample log table 
            businessSystem.Endround.fResult = r1;

            ((PieSeries)mcChart.Series[0]).ItemsSource =
                new KeyValuePair<string, int>[]{ 
                new KeyValuePair<string,int>("Ok bristles: " + r1 +"%", r1),
                new KeyValuePair<string,int>("Defect/Not Found: " + r2 +"%",  r2)};

            if(r1 >= 95)
            {
                result.Text = "APPROVED";
                BrushConverter bc = new BrushConverter();
                result.Background = (Brush)bc.ConvertFrom("#FF2EB700");
                resultColorGrid.Background = (Brush)bc.ConvertFrom("#FF2EB700");
                resultColorGridBorderTop.Background = (Brush)bc.ConvertFrom("#FF124A00");
                resultColorGridBorderButton.Background = (Brush)bc.ConvertFrom("#FF124A00");

                //saving final result in skelta's sample log table 
                businessSystem.Endround.iStatus_id = 2;
            }
            else if(r1 >= 90 && r1 < 95)
            {
                result.Text = "ALERT";
                BrushConverter bc = new BrushConverter();
                result.Background = (Brush)bc.ConvertFrom("#FFDADA2D");
                resultColorGrid.Background = (Brush)bc.ConvertFrom("#FFDADA2D");
                resultColorGridBorderTop.Background = (Brush)bc.ConvertFrom("#FFAEAB55");
                resultColorGridBorderButton.Background = (Brush)bc.ConvertFrom("#FFAEAB55");

                //saving final result in skelta's sample log table 
                businessSystem.Endround.iStatus_id = 4;
            }
            else if(r1 < 90)
            {
                result.Text = "NOT APPROVED";
                BrushConverter bc = new BrushConverter();
                result.Background = (Brush)bc.ConvertFrom("#FFFF0000");
                resultColorGrid.Background = (Brush)bc.ConvertFrom("#FFFF0000");
                resultColorGridBorderTop.Background = (Brush)bc.ConvertFrom("#FF8D3D30");
                resultColorGridBorderButton.Background = (Brush)bc.ConvertFrom("#FF8D3D30");

                //saving final result in skelta's sample log table 
                businessSystem.Endround.iStatus_id = 3;
            }

            double nfiosSpec = (int.Parse(totalBristleT.Text) + int.Parse(totalBristleM1.Text) + int.Parse(totalBristleM2.Text) +
                int.Parse(totalBristleM3.Text) + int.Parse(totalBristleN.Text))/5;

            //saving final result in skelta's sample log table
            businessSystem.dataBaseController.updateAI_Sample_log(businessSystem.Endround, null, null);
            businessSystem.NFiosSpec.fResult = nfiosSpec;
            businessSystem.dataBaseController.updateAI_Sample_log(businessSystem.NFiosSpec, null, null);
        }

        private void LoadPieChartDataT()
        {
            int type1 = 0;
            int type2 = 0;
            int type3 = 0;
            int reb = 0;
            int none = 0;
            int discard = 0;
            foreach (var bristleAnalysisResultModel in businessSystem.bristleAnalysisResultModels)
            {
                if(bristleAnalysisResultModel.Position == "T")
                {
                    if(bristleAnalysisResultModel.DefectClassification == "Error1")
                    {
                        type1++;
                    }
                    else if (bristleAnalysisResultModel.DefectClassification == "Error2")
                    {
                        type2++;
                    }
                    else if (bristleAnalysisResultModel.DefectClassification == "Error3")
                    {
                        type3++;
                    }
                    else if (bristleAnalysisResultModel.DefectClassification == "reb")
                    {
                        reb++;
                    }
                    else if (bristleAnalysisResultModel.DefectClassification == "Ok")
                    {
                        none++;
                    }
                    else if (bristleAnalysisResultModel.DefectClassification == "discard")
                    {
                        discard++;
                    }
                }
            }
            int totalType3 = type3 + reb;
            int total = type1 + type2 + totalType3 + discard + none;
            int totalDefact = type1 + type2 + totalType3 + discard;

            if (total == 0) total = 1;

            //mcChartT.LegendTitle = "POSITION T";
            mcChartT.FontSize = 20;

            try
            {
                ((PieSeries)mcChartT.Series[0]).ItemsSource =
              new KeyValuePair<string, int>[]{
                new KeyValuePair<string,int>("Ok: " + ((100 * none) / total)+"%", none),
                new KeyValuePair<string,int>("Erro1: "+ ((100 * type1) / total)+"%", type1),
                new KeyValuePair<string,int>("Erro2: " + ((100 * type2) / total)+"%", type2),
                new KeyValuePair<string,int>("Erro3: " + ((100 * type3) / total)+"%", totalType3),
                //new KeyValuePair<string,int>(((100 * type3) / total)+"%"+" :Undefined", discard),
                //new KeyValuePair<string,int>(((100 * reb) / total)+"%"+" :Reb", reb),
              };
                mcChartT.ApplyTemplate();

                bristleQuantityGoodT_.Text = none.ToString();//((businessSystem.tuffAnalysisResultModels[0].TotalBristleFoundManual + businessSystem.tuffAnalysisResultModels[0].TotalBristlesFoundNN) - totalDefact).ToString();
                totalDefectiveBristlesT_.Text = totalDefact.ToString();// businessSystem.tuffAnalysisResultModels[0].TotalBristlesFoundNN.ToString();            
                totalBristleT.Text = (int.Parse(bristleQuantityGoodT_.Text) + int.Parse(totalDefectiveBristlesT_.Text)).ToString();
                noneTotal += int.Parse(totalBristleT.Text);
            }
            catch (Exception e)
            {
                Console.WriteLine("Report: " + e.Message);
            }
        }

        private void LoadPieChartDataM1()
        {
            int type1 = 0;
            int type2 = 0;
            int type3 = 0;
            int reb = 0;
            int none = 0;
            int discard = 0;
            foreach (var bristleAnalysisResultModel in businessSystem.bristleAnalysisResultModels)
            {
                if (bristleAnalysisResultModel.Position == "M1")
                {
                    if (bristleAnalysisResultModel.DefectClassification == "Error1")
                    {
                        type1++;
                    }
                    else if (bristleAnalysisResultModel.DefectClassification == "Error2")
                    {
                        type2++;
                    }
                    else if (bristleAnalysisResultModel.DefectClassification == "Error3")
                    {
                        type3++;
                    }
                    else if (bristleAnalysisResultModel.DefectClassification == "reb")
                    {
                        reb++;
                    }
                    else if (bristleAnalysisResultModel.DefectClassification == "Ok")
                    {
                        none++;
                    }
                    else if (bristleAnalysisResultModel.DefectClassification == "discard")
                    {
                        discard++;
                    }
                }
            }
            int totalType3 = type3 + reb;
            int total = type1 + type2 + totalType3 + discard + none;
            int totalDefact = type1 + type2 + totalType3 + discard;

            if (total == 0) total = 1;

            //mcChartM1.LegendTitle = "POSITION M1";
            mcChartM1.FontSize = 20;

            try
            {
                ((PieSeries)mcChartM1.Series[0]).ItemsSource =
               new KeyValuePair<string, int>[]{
                new KeyValuePair<string,int>("Ok: " + ((100 * none) / total)+"%", none),
                new KeyValuePair<string,int>("Erro1: " + ((100 * type1) / total)+"%", type1),
                new KeyValuePair<string,int>("Erro2: " + ((100 * type2) / total)+"%", type2),
                new KeyValuePair<string,int>("Erro3: " + ((100 * type3) / total)+"%", totalType3),
                //new KeyValuePair<string,int>(((100 * type3) / total)+"%"+" :Undefined", discard),
                //new KeyValuePair<string,int>(((100 * reb) / total)+"%"+" :Reb", reb),
               };
                mcChartM1.ApplyTemplate();

                bristleQuantityGoodM1_.Text = none.ToString();//((businessSystem.tuffAnalysisResultModels[1].TotalBristleFoundManual + businessSystem.tuffAnalysisResultModels[1].TotalBristlesFoundNN) - totalDefact).ToString();
                totalDefectiveBristlesM1_.Text = totalDefact.ToString();
                totalBristleM1.Text = (int.Parse(bristleQuantityGoodM1_.Text) + int.Parse(totalDefectiveBristlesM1_.Text)).ToString();
                noneTotal += int.Parse(totalBristleM1.Text);
            }
            catch (Exception e)
            {
                Console.WriteLine("Report: " + e.Message);
            }
        }

        private void LoadPieChartDataM2()
        {
            int type1 = 0;
            int type2 = 0;
            int type3 = 0;
            int reb = 0;
            int none = 0;
            int discard = 0;
            foreach (var bristleAnalysisResultModel in businessSystem.bristleAnalysisResultModels)
            {
                if (bristleAnalysisResultModel.Position == "M2")
                {
                    if (bristleAnalysisResultModel.DefectClassification == "Error1")
                    {
                        type1++;
                    }
                    else if (bristleAnalysisResultModel.DefectClassification == "Error2")
                    {
                        type2++;
                    }
                    else if (bristleAnalysisResultModel.DefectClassification == "Error3")
                    {
                        type3++;
                    }
                    else if (bristleAnalysisResultModel.DefectClassification == "reb")
                    {
                        reb++;
                    }
                    else if (bristleAnalysisResultModel.DefectClassification == "Ok")
                    {
                        none++;
                    }
                    else if (bristleAnalysisResultModel.DefectClassification == "discard")
                    {
                        discard++;
                    }
                }
            }
            int totalType3 = type3 + reb;
            int total = type1 + type2 + totalType3 + discard + none;
            int totalDefact = type1 + type2 + totalType3 + discard;

            if (total == 0) total = 1;

            //mcChartM2.LegendTitle = "POSITION M2";
            mcChartM2.FontSize = 20;

            try
            {
                ((PieSeries)mcChartM2.Series[0]).ItemsSource =
                              new KeyValuePair<string, int>[]{
                new KeyValuePair<string,int>("Ok: " + ((100 * none) / total)+"%", none),
                new KeyValuePair<string,int>("Erro1: " + ((100 * type1) / total)+"%", type1),
                new KeyValuePair<string,int>("Erro2: " + ((100 * type2) / total)+"%", type2),
                new KeyValuePair<string,int>("Erro3: " + ((100 * type3) / total)+"%", totalType3),
                //new KeyValuePair<string,int>(((100 * type3) / total)+"%"+" :Undefined", discard),
                                  //new KeyValuePair<string,int>(((100 * reb) / total)+"%"+" :Reb", reb),
                              };
                mcChartM2.ApplyTemplate();

                bristleQuantityGoodM2_.Text = none.ToString();// ((businessSystem.tuffAnalysisResultModels[2].TotalBristleFoundManual + businessSystem.tuffAnalysisResultModels[2].TotalBristlesFoundNN) - totalDefact).ToString();
                totalDefectiveBristlesM2_.Text = totalDefact.ToString();
                totalBristleM2.Text = (int.Parse(bristleQuantityGoodM2_.Text) + int.Parse(totalDefectiveBristlesM2_.Text)).ToString();
                noneTotal += int.Parse(totalBristleM2.Text);
            }
            catch (Exception e)
            {
                Console.WriteLine("Report: " + e.Message);
            }
        }

        private void LoadPieChartDataM3()
        {
            int type1 = 0;
            int type2 = 0;
            int type3 = 0;
            int reb = 0;
            int none = 0;
            int discard = 0;
            foreach (var bristleAnalysisResultModel in businessSystem.bristleAnalysisResultModels)
            {
                if (bristleAnalysisResultModel.Position == "M3")
                {
                    if (bristleAnalysisResultModel.DefectClassification == "Error1")
                    {
                        type1++;
                    }
                    else if (bristleAnalysisResultModel.DefectClassification == "Error2")
                    {
                        type2++;
                    }
                    else if (bristleAnalysisResultModel.DefectClassification == "Error3")
                    {
                        type3++;
                    }
                    else if (bristleAnalysisResultModel.DefectClassification == "reb")
                    {
                        reb++;
                    }
                    else if (bristleAnalysisResultModel.DefectClassification == "Ok")
                    {
                        none++;
                    }
                    else if (bristleAnalysisResultModel.DefectClassification == "discard")
                    {
                        discard++;
                    }
                }
            }
            int totalType3 = type3 + reb;
            int total = type1 + type2 + totalType3 + discard + none;
            int totalDefact = type1 + type2 + totalType3 + discard;

            if (total == 0) total = 1;

            try
            {
                //mcChartM3.LegendTitle = "POSITION M3";
                mcChartM3.FontSize = 20;

                ((PieSeries)mcChartM3.Series[0]).ItemsSource =
                    new KeyValuePair<string, int>[]{
                new KeyValuePair<string,int>("Ok: " + ((100 * none) / total)+"%", none),
                new KeyValuePair<string,int>("Erro1: " + ((100 * type1) / total)+"%", type1),
                new KeyValuePair<string,int>("Erro2: " + ((100 * type2) / total)+"%", type2),
                new KeyValuePair<string,int>("Erro3: " + ((100 * type3) / total)+"%", totalType3),
                //new KeyValuePair<string,int>(((100 * type3) / total)+"%"+" :Undefined", discard),
                        //new KeyValuePair<string,int>(((100 * reb) / total)+"%"+" :Reb", reb),
                    };
                mcChartM3.ApplyTemplate();
                bristleQuantityGoodM3_.Text = none.ToString();
                totalDefectiveBristlesM3_.Text = totalDefact.ToString();// businessSystem.tuffAnalysisResultModels[1].TotalBristlesFoundNN.ToString();
                totalBristleM3.Text = (int.Parse(bristleQuantityGoodM3_.Text) + int.Parse(totalDefectiveBristlesM3_.Text)).ToString();
                noneTotal += int.Parse(totalBristleM3.Text);
            }
            catch (Exception e)
            {
                Console.WriteLine("Report: " + e.Message);
            }
        }

        private void LoadPieChartDataN()
        {
            int type1 = 0;
            int type2 = 0;
            int type3 = 0;
            int reb = 0;
            int none = 0;
            int discard = 0;
            foreach (var bristleAnalysisResultModel in businessSystem.bristleAnalysisResultModels)
            {
                if (bristleAnalysisResultModel.Position == "N")
                {
                    if (bristleAnalysisResultModel.DefectClassification == "Error1")
                    {
                        type1++;
                    }
                    else if (bristleAnalysisResultModel.DefectClassification == "Error2")
                    {
                        type2++;
                    }
                    else if (bristleAnalysisResultModel.DefectClassification == "Error3")
                    {
                        type3++;
                    }
                    else if (bristleAnalysisResultModel.DefectClassification == "reb")
                    {
                        reb++;
                    }
                    else if (bristleAnalysisResultModel.DefectClassification == "Ok")
                    {
                        none++;
                    }
                    else if (bristleAnalysisResultModel.DefectClassification == "discard")
                    {
                        discard++;
                    }
                }
            }
            int totalType3 = type3 + reb;
            int total = type1 + type2 + totalType3 + discard + none;
            int totalDefact = type1 + type2 + totalType3 + discard;

            if (total == 0) total = 1;

            //mcChartN.LegendTitle = "POSITION N";
            mcChartN.FontSize = 20;

            try
            {
                ((PieSeries)mcChartN.Series[0]).ItemsSource =
               new KeyValuePair<string, int>[]{
                new KeyValuePair<string,int>("Ok: " + ((100 * none) / total)+"%", none),
                new KeyValuePair<string,int>("Erro1: " + ((100 * type1) / total)+"%", type1),
                new KeyValuePair<string,int>("Erro2: " + ((100 * type2) / total)+"%", type2),
                new KeyValuePair<string,int>("Erro3: " + ((100 * type3) / total)+"%", totalType3),
                //new KeyValuePair<string,int>(((100 * type3) / total)+"%"+" :Undefined", discard),
                   //new KeyValuePair<string,int>(((100 * reb) / total)+"%"+" :Reb", reb),
               };
                mcChartN.ApplyTemplate();
                bristleQuantityGoodN_.Text = none.ToString();//((businessSystem.tuffAnalysisResultModels[4].TotalBristleFoundManual + businessSystem.tuffAnalysisResultModels[4].TotalBristlesFoundNN) - totalDefact).ToString();
                totalDefectiveBristlesN_.Text = totalDefact.ToString();// businessSystem.tuffAnalysisResultModels[1].TotalBristlesFoundNN.ToString();

                totalBristleN.Text = (int.Parse(bristleQuantityGoodN_.Text) + int.Parse(totalDefectiveBristlesN_.Text)).ToString();
                noneTotal += int.Parse(totalBristleN.Text);

                totalGoodBristles.Text = (int.Parse(bristleQuantityGoodT_.Text) + int.Parse(bristleQuantityGoodM1_.Text) +
                     int.Parse(bristleQuantityGoodM2_.Text) + int.Parse(bristleQuantityGoodM3_.Text) + int.Parse(bristleQuantityGoodN_.Text)).ToString();
                totalBristle.Text = noneTotal.ToString();

                totalDefectiveBristles.Text = (int.Parse(totalDefectiveBristlesT_.Text) + int.Parse(totalDefectiveBristlesM1_.Text) +
                    int.Parse(totalDefectiveBristlesM2_.Text) + int.Parse(totalDefectiveBristlesM3_.Text) + int.Parse(totalDefectiveBristlesN_.Text)).ToString();

                LoadPieChartData_result();
            }
            catch (Exception e)
            {
                Console.WriteLine("Report: " + e.Message);
            }           
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

        private void ButtonGeneralReport_Click(object sender, RoutedEventArgs e)
        {
            Views.GeneralReport generalReport = new GeneralReport(maximized, businessSystem, automaticBristleClassification);
            generalReport.Show();
            this.Close();
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
            if (automaticBristleClassification == null)
            {
                automaticBristleClassification = new Views.AutomaticBristleClassification(maximized, businessSystem);
            }

            automaticBristleClassification.live = true;
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
            Views.UserControl userControl = new UserControl();
            userControl.Show();
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
                if(businessSystem.networkUserModel.NetworkUserGroup != null)
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
                }
                MessageBox.Show("Necessary administrative rights!");
            }
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

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
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
                        if (group == businessSystem.AD_Admin || group == businessSystem.AD_Quality)
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

        private void TextBox_TextChanged_1(object sender, TextChangedEventArgs e)
        {

        }

        private void bristleQuantityGoodNManual_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Views.Help help = new Views.Help(maximized, businessSystem, automaticBristleClassification);
            help.Show();
        }
    }
}
