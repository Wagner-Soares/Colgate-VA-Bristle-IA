using APIVision.Controllers;
using System;
using System.Drawing;
using System.Collections.Generic;
using APIVision.DataModels;
using APIVision.DataModels.CommandModels;

namespace APIVision
{
    public class BusinessSystem
    {
        #region Parameter
        #endregion
        #region Controllers
        #endregion
        #region Models

        public ComunicationController ComunicationController { get; set; } = new ComunicationController();
        public string Data { get; set; } = "";
        public int ExposureValue { get; set; } = 2500;
        public string LocalRepository { get; set; } = "";
        public int ResizingOfBoundingBoxWeight { get; set; } = 2;        
        public List<ShiftsModel> ShiftsModel { get; set; } = new List<ShiftsModel>();
        public UserSystemModel UserSystemModel_ { get; set; } = new UserSystemModel();
        public NetworkUserModel NetworkUserModel { get; set; } = new NetworkUserModel();
        public CommandS200Model CommandS200Model { get; set; } = new CommandS200Model();
        public CommandS300Model CommandS300Model { get; set; } = new CommandS300Model();
        public Box Box { get; set; } = new Box();
        public List<UserSystemModel> UserSystemModels { get; set; } = new List<UserSystemModel>();
        public UserSystemModel UserSystemCurrent { get; set; } = new UserSystemModel();
        public List<GeneralSettingsModel> GeneralSettingsModels { get; set; } = new List<GeneralSettingsModel>();
        public CameraSettingsModel CameraSettingsModel { get; set; } = new CameraSettingsModel();
        public List<RegistrationWaitingModel> RegistrationWaitingModels { get; set; } = new List<RegistrationWaitingModel>();
        public List<RegistrationWaitingModel> ValidationRegistrationWaitingModels { get; set; } = new List<RegistrationWaitingModel>();
        public List<RegistrationWaitingModel> DatasetRegistrationWaitingModels { get; set; } = new List<RegistrationWaitingModel>();
        public List<SocketModel> BoundingBoxDiscards { get; set; } = new List<SocketModel>();
        public AnalyzeSetModel AnalyzeModel { get; set; } = new AnalyzeSetModel();
        public List<AnalyzeSetModel> AnalyzeModels { get; set; } = new List<AnalyzeSetModel>();
        public BristleAnalysisResultModel BristleAnalysisResultModel { get; set; }
        public BristleModel BristleModel { get; set; } = new BristleModel();
        public BrushAnalysisResultModel BrushAnalysisResultModel { get; set; } = new BrushAnalysisResultModel();
        public CameraModel CameraModel { get; set; } = new CameraModel();
        public DataModel DataModel { get; set; } = new DataModel();
        public DatasetModel DatasetModel { get; set; } = new DatasetModel();
        public EquipmentModel EquipmentModel { get; set; } = new EquipmentModel();
        public ImportDataModel ImportDataModel { get; set; } = new ImportDataModel();
        public ParameterizationModel ParameterizationModel { get; set; } = new ParameterizationModel();
        public SkuModel KU1Model { get; set; } = new SkuModel();
        public AnalyzeSetModel KUModel { get; set; } = new AnalyzeSetModel();
        public TuffAnalysisResultModel TuffAnalysisResultModel { get; set; } = new TuffAnalysisResultModel();
        public UserSystemModel UserSystemModel { get; set; } = new UserSystemModel();
        public BristleTempModel BristleTempModel_ { get; set; }
        public List<BristleTempModel> BristleTempModel { get; set; } = new List<BristleTempModel>();
        public TuftTempModel TuftTempModel { get; set; } = new TuftTempModel();
        public RegistrationWaitingModel RegistrationWaitingModel { get; set; } = new RegistrationWaitingModel();
        public ImageTempModel ImageTempModel { get; set; } = new ImageTempModel();
        #endregion Models      

        /// <summary>
        /// StartComunication
        /// </summary>
        public string StartComunication(int port)
        {
            try
            {
                ComunicationController.EstablishConnection( port);
                this.ComunicationController.NovoArquivoRecebido += new ComunicationController.FileRecievedEventHandler(Form1_NovoArquivoRecebido);

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
            if (Data != null && Data.Length < 10)
            {
                Data = ComunicationController.Command;
            }
        }

        /// <summary>
        /// Send command
        /// </summary>
        /// <param name="command"></param>
        public void SendCommand(string command, Bitmap frame, string ipPrediction, int portPrediction)
        {
            if (frame != null)
            {
                ComunicationController.SendBitmapCommand(command, ipPrediction, portPrediction, frame);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="json"></param>
        public void SendCommand(string command, string json, string ipPrediction, int portPrediction)
        {
            ComunicationController.SendStringJsonCommand(command, ipPrediction, portPrediction, json);
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
            ComunicationController.SendStringJsonCommand(command, ip_, port_, json);
        }
    }
}
