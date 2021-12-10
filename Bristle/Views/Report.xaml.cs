using APIVision;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls.DataVisualization.Charting;
using APIVision.Controllers;
using APIVision.Controllers.DataBaseControllers;
using System.Threading;
using Database;
using APIVision.DataModels;
using Bristle.UseCases;

namespace Bristle.Views
{
    /// <summary>
    /// Lógica interna para Report.xaml
    /// </summary>
    public partial class Report : Window
    {
        private bool maximized = true;
        private readonly BusinessSystem businessSystem;
        private int noneTotal;
        private readonly bool maximized__ = false;

        private Views.AutomaticBristleClassification automaticBristleClassification;

        private readonly AiSampleLogModel endroundTestResult = new AiSampleLogModel();
        private readonly AiSampleLogModel nBristlesTSpecTestResult = new AiSampleLogModel();
        private readonly AiSampleLogModel nBristlesM1SpecTestResult = new AiSampleLogModel();
        private readonly AiSampleLogModel nBristlesM2SpecTestResult = new AiSampleLogModel();
        private readonly AiSampleLogModel nBristlesM3SpecTestResult = new AiSampleLogModel();
        private readonly AiSampleLogModel nBristlesNSpecTestResult = new AiSampleLogModel();

        private readonly ColgateSkeltaEntities _colgateSkeltaEntities;

        private readonly AnalyzeSetController _analyzeSetController;
        private readonly ShiftController _shiftController;
        private readonly BrushAnalysisResultSetController _brushAnalysisResultSet;
        private readonly BristleAnalysisResultSetController _bristleAnalysisResultSetController;
        private readonly AiSampleLogController _aI_Sample_LogController;
        private readonly SampleLogController _Sample_LogController;

        private readonly GeneralLocalSettings _generalSettings;

        public Report(bool maximized_, BusinessSystem businessSystem_, Views.AutomaticBristleClassification automaticBristleClassification_, ColgateSkeltaEntities colgateSkeltaEntities)
        {           
            businessSystem = businessSystem_;
            maximized__ = maximized_;
            automaticBristleClassification = automaticBristleClassification_;

            _colgateSkeltaEntities = colgateSkeltaEntities;

            _analyzeSetController = new AnalyzeSetController(colgateSkeltaEntities);
            _shiftController = new ShiftController(colgateSkeltaEntities);
            _brushAnalysisResultSet = new BrushAnalysisResultSetController(colgateSkeltaEntities);
            _bristleAnalysisResultSetController = new BristleAnalysisResultSetController(colgateSkeltaEntities);
            _aI_Sample_LogController = new AiSampleLogController(colgateSkeltaEntities);
            _Sample_LogController = new SampleLogController(colgateSkeltaEntities);

            _generalSettings = automaticBristleClassification_.GeneralSettings;

            InitializeComponent();

            try
            {
                user_.Text = businessSystem.UserSystemCurrent.Name;

                businessSystem.AnalyzeModels = _analyzeSetController.ListAnalyzeModel(true);
                businessSystem.ShiftsModel = _shiftController.ListShiftsModel();

                SetPropertiesToSaveResult();

                List<BrushAnalysisResultModel> brushAnalysisResultModels;
                List<BristleAnalysisResultModel> bristleAnalysisResultModels;

                brushAnalysisResultModels = _brushAnalysisResultSet.ListBrushAnalysisResultModel(businessSystem.AnalyzeModels[businessSystem.AnalyzeModels.Count - 1].Id, null, null, null);

                totalBristle.Text = brushAnalysisResultModels[brushAnalysisResultModels.Count - 1].TotalBristles.ToString();
                bristleAnalysisResultModels = _bristleAnalysisResultSetController.ListBristleAnalysisResultModel(businessSystem.AnalyzeModels[businessSystem.AnalyzeModels.Count - 1].Id, null);

                Init();
                LoadPieChartDataT(bristleAnalysisResultModels);
                LoadPieChartDataM1(bristleAnalysisResultModels);
                LoadPieChartDataM2(bristleAnalysisResultModels);
                LoadPieChartDataM3(bristleAnalysisResultModels);
                LoadPieChartDataN(bristleAnalysisResultModels);
            }
            catch
            {
                //tryCatch to avoid crash
            }

            this.Loaded += Report_Loaded;
        }        

        private void SetPropertiesToSaveResult()
        {
            string userName;

            if (businessSystem.UserSystemCurrent.Name.Contains(@"\"))
                userName = businessSystem.UserSystemCurrent.Name.Substring(businessSystem.UserSystemCurrent.Name.IndexOf(@"\")+1);
            else
                userName = businessSystem.UserSystemCurrent.Name;

            //saving final result in skelta's sample log table
            endroundTestResult.SOperator = endroundTestResult.SCreated_by =
                nBristlesTSpecTestResult.SOperator = nBristlesTSpecTestResult.SCreated_by =
                nBristlesM1SpecTestResult.SOperator = nBristlesM1SpecTestResult.SCreated_by =
                nBristlesM2SpecTestResult.SOperator = nBristlesM2SpecTestResult.SCreated_by =
                nBristlesM3SpecTestResult.SOperator = nBristlesM3SpecTestResult.SCreated_by =
                nBristlesNSpecTestResult.SOperator = nBristlesNSpecTestResult.SCreated_by = userName;

            equipament.Text = businessSystem.AnalyzeModels[businessSystem.AnalyzeModels.Count - 1].Equipament;
            sku.Text = businessSystem.AnalyzeModels[businessSystem.AnalyzeModels.Count - 1].Name;

            //saving final result in skelta's sample log table 
            endroundTestResult.DtSample = nBristlesTSpecTestResult.DtSample = nBristlesM1SpecTestResult.DtSample = nBristlesM2SpecTestResult.DtSample = nBristlesM3SpecTestResult.DtSample =
                nBristlesNSpecTestResult.DtSample = businessSystem.AnalyzeModels[businessSystem.AnalyzeModels.Count - 1].Timestamp;

            endroundTestResult.DtPublished_at = nBristlesTSpecTestResult.DtPublished_at = nBristlesM1SpecTestResult.DtPublished_at = nBristlesM2SpecTestResult.DtPublished_at =
                nBristlesM3SpecTestResult.DtPublished_at = nBristlesNSpecTestResult.DtPublished_at = businessSystem.AnalyzeModels[businessSystem.AnalyzeModels.Count - 1].Timestamp;

            endroundTestResult.DtCreated_at = nBristlesTSpecTestResult.DtCreated_at = nBristlesM1SpecTestResult.DtCreated_at = nBristlesM2SpecTestResult.DtCreated_at =
                nBristlesM3SpecTestResult.DtCreated_at = nBristlesNSpecTestResult.DtCreated_at = businessSystem.AnalyzeModels[businessSystem.AnalyzeModels.Count - 1].Timestamp;

            endroundTestResult.SEquipament = nBristlesTSpecTestResult.SEquipament = nBristlesM1SpecTestResult.SEquipament = nBristlesM2SpecTestResult.SEquipament =
                nBristlesM3SpecTestResult.SEquipament = nBristlesNSpecTestResult.SEquipament = businessSystem.AnalyzeModels[businessSystem.AnalyzeModels.Count - 1].Equipament;

            endroundTestResult.IShift = nBristlesTSpecTestResult.IShift = nBristlesM1SpecTestResult.IShift = nBristlesM2SpecTestResult.IShift = nBristlesM3SpecTestResult.IShift =
                nBristlesNSpecTestResult.IShift = ShiftComparison(businessSystem.AnalyzeModels[businessSystem.AnalyzeModels.Count - 1].Timestamp);

            endroundTestResult.SArea = nBristlesTSpecTestResult.SArea = nBristlesM1SpecTestResult.SArea = nBristlesM2SpecTestResult.SArea = nBristlesM3SpecTestResult.SArea =
                nBristlesNSpecTestResult.SArea = _generalSettings.Area;

            endroundTestResult.SBatchLote = nBristlesTSpecTestResult.SBatchLote = nBristlesM1SpecTestResult.SBatchLote = nBristlesM2SpecTestResult.SBatchLote = nBristlesM3SpecTestResult.SBatchLote =
                nBristlesNSpecTestResult.SBatchLote = _generalSettings.BatchLote;

            endroundTestResult.BActive = nBristlesTSpecTestResult.BActive = nBristlesM1SpecTestResult.BActive = nBristlesM2SpecTestResult.BActive = nBristlesM3SpecTestResult.BActive =
                nBristlesNSpecTestResult.BActive = true;

            endroundTestResult.SComments = nBristlesTSpecTestResult.SComments = nBristlesM1SpecTestResult.SComments = nBristlesM2SpecTestResult.SComments = nBristlesM3SpecTestResult.SComments =
                nBristlesNSpecTestResult.SComments = "";

            endroundTestResult.ITest_id = _generalSettings.EndroundSpecTest.TestId;
            nBristlesTSpecTestResult.ITest_id = _generalSettings.TuftTBristleCountSpecTest.TestId;
            nBristlesM1SpecTestResult.ITest_id = _generalSettings.TuftM1BristleCountSpecTest.TestId;
            nBristlesM2SpecTestResult.ITest_id = _generalSettings.TuftM2BristleCountSpecTest.TestId;
            nBristlesM3SpecTestResult.ITest_id = _generalSettings.TuftM3BristleCountSpecTest.TestId;
            nBristlesNSpecTestResult.ITest_id = _generalSettings.TuftNBristleCountSpecTest.TestId;
        }

        private int ShiftComparison(DateTime dateTime)
        {
            businessSystem.ShiftsModel = _shiftController.ListShiftsModel();

            if(dateTime.AddHours(-3) >= DateTime.Parse(businessSystem.ShiftsModel[0].Shift_start) && dateTime.AddHours(-3) <= DateTime.Parse(businessSystem.ShiftsModel[0].Shift_end))
            {
                return 1;
            }
            else if (dateTime.AddHours(-3) >= DateTime.Parse(businessSystem.ShiftsModel[1].Shift_start) && dateTime.AddHours(-3) <= DateTime.Parse(businessSystem.ShiftsModel[1].Shift_end))
            {
                return 2;
            }
            else if (dateTime.AddHours(-3) >= DateTime.Parse(businessSystem.ShiftsModel[2].Shift_start) && dateTime.AddHours(-3) <= DateTime.Parse(businessSystem.ShiftsModel[2].Shift_end))
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

        private void Init()
        {
            tuftNominalT.Text = "/" + _generalSettings.TuftTBristleCountSpecTest.TestTarget.ToString();
            tuftNominalM1.Text = "/" + _generalSettings.TuftM1BristleCountSpecTest.TestTarget.ToString();
            tuftNominalM2.Text = "/" + _generalSettings.TuftM2BristleCountSpecTest.TestTarget.ToString();
            tuftNominalM3.Text = "/" + _generalSettings.TuftM3BristleCountSpecTest.TestTarget.ToString();
            tuftNominalN.Text = "/" + _generalSettings.TuftNBristleCountSpecTest.TestTarget.ToString();
            nominalTotal.Text = "/" + (Convert.ToInt16(tuftNominalT.Text.Replace("/",""))
                                + Convert.ToInt16(tuftNominalM1.Text.Replace("/", ""))
                                + Convert.ToInt16(tuftNominalM2.Text.Replace("/", ""))
                                + Convert.ToInt16(tuftNominalM3.Text.Replace("/", ""))
                                + Convert.ToInt16(tuftNominalN.Text.Replace("/", ""))).ToString();
        }

        private void LoadPieChartData_result()
        {
            int endroundTestResultStatus;

            int r1 = ((int.Parse(totalGoodBristles.Text)) * 100) / (noneTotal);
            int r2 = 100 - r1;

            mcChart.LegendTitle = "Total: " + (noneTotal).ToString();
            mcChart.FontSize = 24;

            //saving final result in skelta's sample log table 
            endroundTestResult.FResult = r1;

            ((PieSeries)mcChart.Series[0]).ItemsSource =
                new KeyValuePair<string, int>[]{
                new KeyValuePair<string,int>("Ok bristles: " + r1 +"%", r1),
                new KeyValuePair<string,int>("Defect/Not Found: " + r2 +"%",  r2)};

            endroundTestResultStatus = ReportUseCase.EvaluateTestAndReturnStatus(r1, _generalSettings.EndroundSpecTest.TestSpecLowerLimit, _generalSettings.EndroundSpecTest.TestSpecUpperLimit);

            BrushConverter bc = new BrushConverter();
            switch (endroundTestResultStatus)
            {
                case 2:
                    result.Text = "APPROVED";
                    result.Background = (Brush)bc.ConvertFrom("#FF2EB700");
                    resultColorGrid.Background = (Brush)bc.ConvertFrom("#FF2EB700");
                    resultColorGridBorderTop.Background = (Brush)bc.ConvertFrom("#FF124A00");
                    resultColorGridBorderButton.Background = (Brush)bc.ConvertFrom("#FF124A00");

                    //saving final result in skelta's sample log table 
                    endroundTestResult.IStatus_id = endroundTestResultStatus;
                    break;
                case 3:
                    result.Text = "NOT APPROVED";
                    result.Background = (Brush)bc.ConvertFrom("#FFFF0000");
                    resultColorGrid.Background = (Brush)bc.ConvertFrom("#FFFF0000");
                    resultColorGridBorderTop.Background = (Brush)bc.ConvertFrom("#FF8D3D30");
                    resultColorGridBorderButton.Background = (Brush)bc.ConvertFrom("#FF8D3D30");

                    //saving final result in skelta's sample log table 
                    endroundTestResult.IStatus_id = endroundTestResultStatus;
                    break;
            }

            //saving final result in skelta's sample log table
            if (endroundTestResult.ITest_id > 0)
            {
                _aI_Sample_LogController.UpdateAI_Sample_log(endroundTestResult);
                _Sample_LogController.UpdateSample_log(DataHandlerUseCases.ConvertAI_SampleLogToSampleLog(endroundTestResult));
            }

            nBristlesTSpecTestResult.FResult = int.Parse(totalBristleT.Text);
            nBristlesTSpecTestResult.IStatus_id = ReportUseCase.EvaluateTestAndReturnStatus(nBristlesTSpecTestResult.FResult, _generalSettings.TuftTBristleCountSpecTest.TestSpecLowerLimit, _generalSettings.TuftTBristleCountSpecTest.TestSpecUpperLimit);

            if (nBristlesTSpecTestResult.ITest_id > 0)
            {
                _aI_Sample_LogController.UpdateAI_Sample_log(nBristlesTSpecTestResult);
                _Sample_LogController.UpdateSample_log(DataHandlerUseCases.ConvertAI_SampleLogToSampleLog(nBristlesTSpecTestResult));
            }
            nBristlesM1SpecTestResult.FResult = int.Parse(totalBristleM1.Text);
            nBristlesM1SpecTestResult.IStatus_id = ReportUseCase.EvaluateTestAndReturnStatus(nBristlesM1SpecTestResult.FResult, _generalSettings.TuftM1BristleCountSpecTest.TestSpecLowerLimit, _generalSettings.TuftM1BristleCountSpecTest.TestSpecUpperLimit);

            if (nBristlesM1SpecTestResult.ITest_id > 0)
            {
                _aI_Sample_LogController.UpdateAI_Sample_log(nBristlesM1SpecTestResult);
                _Sample_LogController.UpdateSample_log(DataHandlerUseCases.ConvertAI_SampleLogToSampleLog(nBristlesM1SpecTestResult));
            }

            nBristlesM2SpecTestResult.FResult = int.Parse(totalBristleM2.Text);
            nBristlesM2SpecTestResult.IStatus_id = ReportUseCase.EvaluateTestAndReturnStatus(nBristlesM2SpecTestResult.FResult, _generalSettings.TuftM2BristleCountSpecTest.TestSpecLowerLimit, _generalSettings.TuftM2BristleCountSpecTest.TestSpecUpperLimit);

            if (nBristlesM2SpecTestResult.ITest_id > 0)
            {
                _aI_Sample_LogController.UpdateAI_Sample_log(nBristlesM2SpecTestResult);
                _Sample_LogController.UpdateSample_log(DataHandlerUseCases.ConvertAI_SampleLogToSampleLog(nBristlesM2SpecTestResult));
            }

            nBristlesM3SpecTestResult.FResult = int.Parse(totalBristleM3.Text);
            nBristlesM3SpecTestResult.IStatus_id = ReportUseCase.EvaluateTestAndReturnStatus(nBristlesM3SpecTestResult.FResult, _generalSettings.TuftM3BristleCountSpecTest.TestSpecLowerLimit, _generalSettings.TuftM3BristleCountSpecTest.TestSpecUpperLimit);

            if (nBristlesM3SpecTestResult.ITest_id > 0)
            {
                _aI_Sample_LogController.UpdateAI_Sample_log(nBristlesM3SpecTestResult);
                _Sample_LogController.UpdateSample_log(DataHandlerUseCases.ConvertAI_SampleLogToSampleLog(nBristlesM3SpecTestResult));
            }

            nBristlesNSpecTestResult.FResult = int.Parse(totalBristleN.Text);
            nBristlesNSpecTestResult.IStatus_id = ReportUseCase.EvaluateTestAndReturnStatus(nBristlesNSpecTestResult.FResult, _generalSettings.TuftNBristleCountSpecTest.TestSpecLowerLimit, _generalSettings.TuftNBristleCountSpecTest.TestSpecUpperLimit);

            if (nBristlesNSpecTestResult.ITest_id > 0)
            {
                _aI_Sample_LogController.UpdateAI_Sample_log(nBristlesNSpecTestResult);
                _Sample_LogController.UpdateSample_log(DataHandlerUseCases.ConvertAI_SampleLogToSampleLog(nBristlesNSpecTestResult));
            }
        }

        private void LoadPieChartDataT(List<BristleAnalysisResultModel> bristleAnalysisResultModels)
        {
            int type1 = 0;
            int type2 = 0;
            int type3 = 0;
            int reb = 0;
            int none = 0;
            int discard = 0;
            foreach (var bristleAnalysisResultModel in bristleAnalysisResultModels)
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

            mcChartT.FontSize = 20;

            try
            {
                ((PieSeries)mcChartT.Series[0]).ItemsSource =
              new KeyValuePair<string, int>[]{
                new KeyValuePair<string,int>("Ok: " + ((100 * none) / total)+"%", none),
                new KeyValuePair<string,int>("Erro1: "+ ((100 * type1) / total)+"%", type1),
                new KeyValuePair<string,int>("Erro2: " + ((100 * type2) / total)+"%", type2),
                new KeyValuePair<string,int>("Erro3: " + ((100 * type3) / total)+"%", totalType3),
              };
                mcChartT.ApplyTemplate();

                bristleQuantityGoodT_.Text = none.ToString();
                totalDefectiveBristlesT_.Text = totalDefact.ToString();         
                totalBristleT.Text = (int.Parse(bristleQuantityGoodT_.Text) + int.Parse(totalDefectiveBristlesT_.Text)).ToString();
                noneTotal += int.Parse(totalBristleT.Text);
            }
            catch (Exception e)
            {
                Console.WriteLine("Report: " + e.Message);
            }
        }

        private void LoadPieChartDataM1(List<BristleAnalysisResultModel> bristleAnalysisResultModels)
        {
            int type1 = 0;
            int type2 = 0;
            int type3 = 0;
            int reb = 0;
            int none = 0;
            int discard = 0;
            foreach (var bristleAnalysisResultModel in bristleAnalysisResultModels)
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

            mcChartM1.FontSize = 20;

            try
            {
                ((PieSeries)mcChartM1.Series[0]).ItemsSource =
               new KeyValuePair<string, int>[]{
                new KeyValuePair<string,int>("Ok: " + ((100 * none) / total)+"%", none),
                new KeyValuePair<string,int>("Erro1: " + ((100 * type1) / total)+"%", type1),
                new KeyValuePair<string,int>("Erro2: " + ((100 * type2) / total)+"%", type2),
                new KeyValuePair<string,int>("Erro3: " + ((100 * type3) / total)+"%", totalType3),
               };
                mcChartM1.ApplyTemplate();

                bristleQuantityGoodM1_.Text = none.ToString();
                totalDefectiveBristlesM1_.Text = totalDefact.ToString();
                totalBristleM1.Text = (int.Parse(bristleQuantityGoodM1_.Text) + int.Parse(totalDefectiveBristlesM1_.Text)).ToString();
                noneTotal += int.Parse(totalBristleM1.Text);
            }
            catch (Exception e)
            {
                Console.WriteLine("Report: " + e.Message);
            }
        }

        private void LoadPieChartDataM2(List<BristleAnalysisResultModel> bristleAnalysisResultModels)
        {
            int type1 = 0;
            int type2 = 0;
            int type3 = 0;
            int reb = 0;
            int none = 0;
            int discard = 0;
            foreach (var bristleAnalysisResultModel in bristleAnalysisResultModels)
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

            mcChartM2.FontSize = 20;

            try
            {
                ((PieSeries)mcChartM2.Series[0]).ItemsSource =
                              new KeyValuePair<string, int>[]{
                new KeyValuePair<string,int>("Ok: " + ((100 * none) / total)+"%", none),
                new KeyValuePair<string,int>("Erro1: " + ((100 * type1) / total)+"%", type1),
                new KeyValuePair<string,int>("Erro2: " + ((100 * type2) / total)+"%", type2),
                new KeyValuePair<string,int>("Erro3: " + ((100 * type3) / total)+"%", totalType3),
                              };
                mcChartM2.ApplyTemplate();

                bristleQuantityGoodM2_.Text = none.ToString();
                totalDefectiveBristlesM2_.Text = totalDefact.ToString();
                totalBristleM2.Text = (int.Parse(bristleQuantityGoodM2_.Text) + int.Parse(totalDefectiveBristlesM2_.Text)).ToString();
                noneTotal += int.Parse(totalBristleM2.Text);
            }
            catch (Exception e)
            {
                Console.WriteLine("Report: " + e.Message);
            }
        }

        private void LoadPieChartDataM3(List<BristleAnalysisResultModel> bristleAnalysisResultModels)
        {
            int type1 = 0;
            int type2 = 0;
            int type3 = 0;
            int reb = 0;
            int none = 0;
            int discard = 0;
            foreach (var bristleAnalysisResultModel in bristleAnalysisResultModels)
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
                mcChartM3.FontSize = 20;

                ((PieSeries)mcChartM3.Series[0]).ItemsSource =
                    new KeyValuePair<string, int>[]{
                new KeyValuePair<string,int>("Ok: " + ((100 * none) / total)+"%", none),
                new KeyValuePair<string,int>("Erro1: " + ((100 * type1) / total)+"%", type1),
                new KeyValuePair<string,int>("Erro2: " + ((100 * type2) / total)+"%", type2),
                new KeyValuePair<string,int>("Erro3: " + ((100 * type3) / total)+"%", totalType3),
                    };
                mcChartM3.ApplyTemplate();
                bristleQuantityGoodM3_.Text = none.ToString();
                totalDefectiveBristlesM3_.Text = totalDefact.ToString();
                totalBristleM3.Text = (int.Parse(bristleQuantityGoodM3_.Text) + int.Parse(totalDefectiveBristlesM3_.Text)).ToString();
                noneTotal += int.Parse(totalBristleM3.Text);
            }
            catch (Exception e)
            {
                Console.WriteLine("Report: " + e.Message);
            }
        }

        private void LoadPieChartDataN(List<BristleAnalysisResultModel> bristleAnalysisResultModels)
        {
            int type1 = 0;
            int type2 = 0;
            int type3 = 0;
            int reb = 0;
            int none = 0;
            int discard = 0;
            foreach (var bristleAnalysisResultModel in bristleAnalysisResultModels)
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

            mcChartN.FontSize = 20;

            try
            {
                ((PieSeries)mcChartN.Series[0]).ItemsSource =
               new KeyValuePair<string, int>[]{
                new KeyValuePair<string,int>("Ok: " + ((100 * none) / total)+"%", none),
                new KeyValuePair<string,int>("Erro1: " + ((100 * type1) / total)+"%", type1),
                new KeyValuePair<string,int>("Erro2: " + ((100 * type2) / total)+"%", type2),
                new KeyValuePair<string,int>("Erro3: " + ((100 * type3) / total)+"%", totalType3),
               };
                mcChartN.ApplyTemplate();
                bristleQuantityGoodN_.Text = none.ToString();
                totalDefectiveBristlesN_.Text = totalDefact.ToString();

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
            CameraObject.DinoLiteSDK.MicroTouchPressed -= V_MicroTouchPress;
            CameraObject.DinoLiteSDK.Connected = false;
            Thread.Sleep(280);
            Views.UserControl userControl = new UserControl();
            userControl.Show();
            automaticBristleClassification.Close();
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

        private void ButtonGeneralReport_Click(object sender, RoutedEventArgs e)
        {
            if (ScreenNavigationUseCases.OpenGeneralReportScreen(businessSystem.UserSystemCurrent, businessSystem.NetworkUserModel, _generalSettings, businessSystem, _colgateSkeltaEntities, maximized))
            {
                automaticBristleClassification.Close();
                this.Close();
            }
            else
            {
                MessageBox.Show("Necessary administrative rights!");
            }
        }

        private void ButtonHome_Click(object sender, RoutedEventArgs e)
        {
            if (ScreenNavigationUseCases.OpenMainScreen(_generalSettings, businessSystem, _colgateSkeltaEntities, maximized))
            {
                automaticBristleClassification.Close();
                this.Close();
            }
        }

        private void ButtonAutomaticBristleClassification_Click(object sender, RoutedEventArgs e)
        {
            automaticBristleClassification.Close();

            var automaticBristleClassificationNew = new Views.AutomaticBristleClassification(maximized, businessSystem, _colgateSkeltaEntities);

            automaticBristleClassificationNew.Show();
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
            if (ScreenNavigationUseCases.OpenNeuralNetworkRetrainingScreen(businessSystem.UserSystemCurrent, businessSystem.NetworkUserModel, _generalSettings, businessSystem, _colgateSkeltaEntities, maximized))
            {
                automaticBristleClassification.Close();
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
                automaticBristleClassification.Close();
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
                automaticBristleClassification.Close();
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
                automaticBristleClassification.Close();
                this.Close();
            }
            else
            {
                MessageBox.Show("Necessary administrative rights!");
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
