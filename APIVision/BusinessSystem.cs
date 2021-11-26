using APIVision.Controllers;
using System;
using System.IO;
using System.Threading;
using System.Drawing;
using System.Windows.Media.Imaging;
using System.Threading;
using System.Windows.Threading;
using System.Collections.Generic;
using APIVision.Models;
using Newtonsoft.Json;

namespace APIVision
{
    public class BusinessSystem
    {
        #region Parameter
        public string data = "";
        public int exposureValue = 2500;
        public string localRepository = "";
        public int resizingOfBoundingBoxWeight = 2;
        public string AD_Operator = "BRVA_TB_OperatorU";
        public string AD_Quality = "BRVA_TB_QualityU";
        public string AD_Admin = "BRAV_WonderwareAdminsU";
        #endregion
        #region Controllers
        public DataBaseController dataBaseController = new DataBaseController();
        public GeneralController generalController = new GeneralController();
        public ComunicationController comunicationController = new ComunicationController();
        public BristleInitController bristleInitController = new BristleInitController();
        public DeviceController deviceController = new DeviceController();
        public ImagePreProcessingController imagePreProcessingController = new ImagePreProcessingController();
        public NeuralNetworkController neuralNetworkController = new NeuralNetworkController();
        public NeuralNetworkRetrainingController neuralNetworkRetrainingController = new NeuralNetworkRetrainingController();
        public ReportController reportController = new ReportController();
        public SeedersController SeedersController = new SeedersController();
        public UserControlController userControlController = new UserControlController();
        #endregion    
        #region Models
        public AI_Sample_logModel Endround = new AI_Sample_logModel();
        public AI_Sample_logModel NFiosSpec = new AI_Sample_logModel();
        public List<ShiftsModel> shiftsModel = new List<ShiftsModel>();
        public UserSystemModel userSystemModel_ = new UserSystemModel();
        public NetworkUserModel networkUserModel = new NetworkUserModel();
        public List<TestModel> testModels = new List<TestModel>();
        public CommandModels.CommandS200Model commandS200Model = new CommandModels.CommandS200Model();
        public CommandModels.CommandS300Model commandS300Model = new CommandModels.CommandS300Model();
        public Box box = new Box();
        public List<UserSystemModel> userSystemModels = new List<UserSystemModel>();
        public UserSystemModel userSystemCurrent = new UserSystemModel();
        public List<GeneralSettingsModel> generalSettingsModels = new List<GeneralSettingsModel>();
        public CameraSettingsModel cameraSettingsModel = new CameraSettingsModel();
        public List<CameraSettingsModel> cameraSettingsModels = new List<CameraSettingsModel>();
        public GeneralSettingsModel generalSettingsModel = new GeneralSettingsModel();
        public GeneralLocalSettings generalSettings = new GeneralLocalSettings();
        public List<RegistrationWaitingModel> registrationWaitingModels = new List<RegistrationWaitingModel>();
        public List<RegistrationWaitingModel> validationRegistrationWaitingModels = new List<RegistrationWaitingModel>();
        public List<RegistrationWaitingModel> datasetRegistrationWaitingModels = new List<RegistrationWaitingModel>();
        public SocketModel socketModel = new SocketModel();
        public List<SocketModel> socketModels = new List<SocketModel>();
        public List<SocketModel> boundingBoxDiscards = new List<SocketModel>();
        public List<SKU1Model> sKU1Model = new List<SKU1Model>();
        public AnalyzeModel analyzeModel = new AnalyzeModel();
        public List<AnalyzeModel> analyzeModels = new List<AnalyzeModel>();
        public BristleAnalysisResultModel bristleAnalysisResultModel;
        public List<BristleAnalysisResultModel> bristleAnalysisResultModels = new List<BristleAnalysisResultModel>();
        public BristleModel bristleModel = new BristleModel();
        public BrushAnalysisResultModel brushAnalysisResultModel = new BrushAnalysisResultModel();
        public List<BrushAnalysisResultModel> brushAnalysisResultModels = new List<BrushAnalysisResultModel>();
        public CameraModel cameraModel = new CameraModel();
        public DataModel dataModel = new DataModel();
        public DatasetModel datasetModel = new DatasetModel();
        public EquipmentModel equipmentModel = new EquipmentModel();
        public ImportDataModel importDataModel = new ImportDataModel();
        public ParameterizationModel parameterizationModel = new ParameterizationModel();
        public SKU1Model kU1Model = new SKU1Model();
        public AnalyzeModel kUModel = new AnalyzeModel();
        public TuffAnalysisResultModel tuffAnalysisResultModel = new TuffAnalysisResultModel();
        public List<TuffAnalysisResultModel> tuffAnalysisResultModels = new List<TuffAnalysisResultModel>();
        public UserSystemModel userSystemModel = new UserSystemModel();
        public BristleTempModel bristleTempModel_;
        public List<BristleTempModel> bristleTempModel = new List<BristleTempModel>();
        public TuftTempModel tuftTempModel = new TuftTempModel();
        public RegistrationWaitingModel registrationWaitingModel = new RegistrationWaitingModel();
        public ImageTempModel imageTempModel = new ImageTempModel();
        #endregion Models      

        /// <summary>
        /// StartComunication
        /// </summary>
        public string StartComunication(string IP, int port)
        {
            try
            {
                comunicationController.EstablishConnection(IP, port);
                //this.comunicationController.NovoArquivoRecebido -= new ComunicationController.FileRecievedEventHandler(Form1_NovoArquivoRecebido);
                this.comunicationController.NovoArquivoRecebido += new ComunicationController.FileRecievedEventHandler(Form1_NovoArquivoRecebido);

                return "Start Comunication succeeded.";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// Form1_NovoArquivoRecebido
        /// </summary>
        /// <param name="fonte"></param>
        /// <param name="nomeArquivo"></param>
        private void Form1_NovoArquivoRecebido(object fonte, string nomeArquivo)
        {
            if(data != null)
            {
                if (data.Length < 10)
                {
                    data = comunicationController.command;
                }
            }
           
            //data = "*";                  
            //Console.WriteLine("Recebi!");
        }

        /// <summary>
        /// Send command
        /// </summary>
        /// <param name="command"></param>
        public void SendCommand(string command, Bitmap frame)
        {
            if(frame == null)
            {
              //  comunicationController.sendCommand(command, "10.167.1.199", 6666);
            }
            else
            {
                ///host: UO631M4067452
                ///IP: 10.167.1.159
                //comunicationController.sendCommand(command, "10.167.1.159", 8888, frame);
                //comunicationController.sendCommand(command, ip, 8888, frame);businessSystem.generalSettings
                comunicationController.sendCommand(command, generalSettings.IpPrediction, generalSettings.PortPrediction, frame); 
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        public void SendCommand(string command)
        {
            comunicationController.sendCommand(command, generalSettings.IpPrediction, generalSettings.PortPrediction);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="json"></param>
        public void SendCommand(string command, string json)
        {
            comunicationController.sendCommand(command, generalSettings.IpPrediction, generalSettings.PortPrediction, json);
        }

        /// <summary>
        /// Socket
        /// </summary>
        /// <param name="command"></param>
        /// <param name="ip_"></param>
        /// <param name="port_"></param>
        /// <param name="json"></param>
        public void SendCommand(string command,string ip_, int port_, string json)
        {
            comunicationController.sendCommand(command, ip_, port_, json);
        }
    }
}
