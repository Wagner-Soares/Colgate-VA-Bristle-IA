using APIVision;
using APIVision.DataModels;
using APIVision.DataModels.CommandModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bristle.UseCases
{
    public static class DataHandlerUseCases
    {
        private static Configuration _configuration = ConfigurationManager
                                                        .OpenExeConfiguration(ConfigurationUserLevel.None);

        public static bool SettingExists(string appSettingKeyName)
        {
            ReloadConfigFile();
            var exists = string.Empty;

            try
            {
                exists = _configuration
                        .AppSettings
                        .Settings[appSettingKeyName].Value;
            }
            catch
            {
                //tryCatch to avoid crash
            }

            ReloadConfigFile();

            return exists != string.Empty;
        }

        private static void ReloadConfigFile()
        {
            Properties.Settings.Default.Reload();

            _configuration = null;

            _configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        }

        public static void EraseSetting(string appSettingKeyName)
        {
            try
            {
                _configuration
                    .AppSettings
                    .Settings.Remove(appSettingKeyName);

                _configuration
                        .Save(ConfigurationSaveMode.Modified);

                ConfigurationManager
                    .RefreshSection("appSettings");

                ReloadConfigFile();
            }
            catch
            {
                //catch to avoid crash
            }
        }

        private static string SettingsReadAllLines(string appSettingKeyName)
        {
            ReloadConfigFile();

            try
            {
                return _configuration
                        .AppSettings
                        .Settings[appSettingKeyName].Value;
            }
            catch
            {
                return string.Empty;
            }
        }

        public static void SaveJsonIntoSettings(object objToSave, string appSettingKeyName)
        {
            var objInJsonFormat = JsonConvert
                                    .SerializeObject(objToSave, Formatting.Indented);

            if (SettingExists(appSettingKeyName))
            { 
                _configuration
                .AppSettings
                .Settings[appSettingKeyName].Value = objInJsonFormat; 
            }
            else
            {
                _configuration
                    .AppSettings
                    .Settings
                    .Add(appSettingKeyName, "Initial Value");

                _configuration
                .AppSettings
                .Settings[appSettingKeyName].Value = objInJsonFormat;
            }

            _configuration
                    .Save(ConfigurationSaveMode.Modified);

            ConfigurationManager
                .RefreshSection("appSettings");

            ReloadConfigFile();
        }

        /// <summary>
        /// Method to save parameters in JSON format.
        /// </summary>
        public static string SaveJsonWithStringReturn(object obj, string path)
        {
            string json;

            json = JsonConvert.SerializeObject(obj, Formatting.Indented);
            SaveJsonIntoSettings(obj, path);

            return json;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="comando"></param>
        /// <returns></returns>
        public static void CMD(string command)
        {
            Process scriptProc = new Process();
            scriptProc.StartInfo.FileName = @"C:\Windows\System32\wscript.exe";
            scriptProc.StartInfo.Arguments = command;
            scriptProc.Start();
            scriptProc.WaitForExit();
            scriptProc.Close();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        public static void MountVbs(string name)
        {
            Stream saida = File.Create(name + ".vbs");
            StreamWriter escritor = new StreamWriter(saida);

            escritor.WriteLine("Dim wshShell");
            escritor.WriteLine("Set wshShell = CreateObject(\"WScript.Shell\")");
            escritor.WriteLine("wshShell.Run \"" + name + ".bat\", 0, false");
            escritor.Close();
        }

        public static void MountBat(string name, string command)
        {
            Stream saida = File.Create(name);
            StreamWriter escritor = new StreamWriter(saida);

            escritor.WriteLine(command);
            escritor.Close();
        }

        /// <summary>
        /// Method to save parameters in JSON format. This method can accumulate,
        /// that is, it can add more than one set of data 
        /// </summary>
        /// <param name="objectToSaveOrAppend"></param>
        /// <param name="appSettingKeyName"></param>
        /// <param name="differentFiles"></param>
        public static void SaveOrAppendToSettings(object objectToSaveOrAppend, string appSettingKeyName)
        {
            string content = string.Empty;
            string json = JsonConvert
                            .SerializeObject(objectToSaveOrAppend);

            _configuration
                    .Save(ConfigurationSaveMode.Modified);

            ConfigurationManager
                .RefreshSection("appSettings");

            ReloadConfigFile();

            _configuration
                    .AppSettings
                    .Settings
                    .Add("temp", "Initial Value");

            _configuration
            .AppSettings
            .Settings["temp"].Value = json;

            _configuration
                    .Save(ConfigurationSaveMode.Modified);

            ConfigurationManager
                .RefreshSection("appSettings");

            ReloadConfigFile();

            var readjson = SettingsReadAllLines("temp");

            try
            {
                if (!SettingExists(appSettingKeyName))
                {
                    content += "[";
                    content += json;
                    content += "]";
                }
                else
                {
                    content = SettingsReadAllLines(appSettingKeyName).Replace("\n","").Replace("\r","").Replace("\\","").Replace("rn  ","").Replace("rn}","}");

                    content = content.Replace("]","," + readjson + "]");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Save file Json {0}", e.Message);
            }

            SaveJsonIntoSettings(content, appSettingKeyName);
        }

        public static string ConvertObjectToJson(object obj)
        {
            string json = JsonConvert.SerializeObject(obj, Formatting.Indented);

            return json;
        }

        /// <summary>
        /// Deserialization of a JSON file into an object 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static object ReadJsonFromSettingsKey(string appSettingKeyName)
        {
            var obj = JsonConvert
                        .DeserializeObject<object>(SettingsReadAllLines(appSettingKeyName));

            return obj;
        }

        /// <summary>
        /// Deserialization of a JSON file on a specific object 
        /// </summary>
        /// <param name="appSettingKeyName"></param>
        /// <returns></returns>
        public static int ReadJsonCommand300(string appSettingKeyName)
        {
            CommandS300Model obj = JsonConvert
                                       .DeserializeObject<CommandS300Model>(SettingsReadAllLines(appSettingKeyName));

            return obj.NewModelId;
        }

        /// <summary>
        /// Deserialization of a JSON file on a specific object 
        /// </summary>
        /// <param name="appSettingKeyName"></param>
        /// <returns></returns>
        public static GeneralLocalSettings ReadJsonGeneralLocalSettings(string appSettingKeyName)
        {
            try
            {
                GeneralLocalSettings obj = JsonConvert
                    .DeserializeObject<GeneralLocalSettings>(SettingsReadAllLines(appSettingKeyName));
                return obj;
            }
            catch
            {
                //tryCatch to avoid crash
            }

            return null;
        }

        public static List<CameraSettingsModel> ReadJsonCameraSettings(string appSettingKeyName)
        {
            try
            {
                var temp = SettingsReadAllLines(appSettingKeyName);
                var temp2 = temp;

                while (temp2.Contains("\\\"["))
                {
                    temp2 = temp2.Replace("\\\"[", "[").Replace("]\\\"", "]");
                }
                
                var readAux = JsonConvert
                    .DeserializeObject<object>(temp2);

                List<CameraSettingsModel> obj = JsonConvert
                    .DeserializeObject<List<CameraSettingsModel>>(readAux.ToString());
                return obj;
            }
            catch
            {
                //tryCatch to avoid crash
            }

            return new List<CameraSettingsModel>();
        }

        /// <summary>
        /// Deserialization of a JSON file on a specific object 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="businessSystem"></param>
        /// <returns></returns>
        public static List<SocketModel> GetSocketModelsReturned(string data, BusinessSystem businessSystem)
        {
            List<SocketModel> socketModel = new List<SocketModel>(); 

            try
            {
                File.WriteAllText(businessSystem.LocalRepository + @"\data.json", data);
                socketModel = JsonConvert.DeserializeObject<List<SocketModel>>(File.ReadAllText(businessSystem.LocalRepository + @"\data.json"));

                File.Delete(businessSystem.LocalRepository + @"\data.json");
            }
            catch
            {
                //tryCatch to avoid crash
            }


            return socketModel;
        }

        /// <summary>
        /// Name preformation with timetemp 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="givenTimestamp"></param>
        /// <returns></returns>
        public static string FormatTimestamp(string name, string givenTimestamp)
        {
            try
            {
                string[] data_ = givenTimestamp.Split(' ');

                string[] data_day = data_[0].Split('/');

                string[] timestamp = data_[1].Split(':');

                string result = name + "_" + data_day[2] + data_day[1] + data_day[0] + "T" + timestamp[0] + timestamp[1] + timestamp[2];

                return result;
            }
            catch
            {
                return "0";
            }
        }       

        /// <summary>
        /// Resize the image to the specified width and height.
        /// </summary>
        /// <param name="image">The image to resize.</param>
        /// <param name="width">The width to resize to.</param>
        /// <param name="height">The height to resize to.</param>
        /// <returns>The resized image.</returns>
        public static Bitmap ResizeImage(System.Drawing.Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

        /// <summary>
        /// Convert IA name To Default name 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string ConvertIAToDefault(string name)
        {
            string name_;

            if (name == "Error1")
            {
                name_ = "type1";
            }
            else if (name == "Error2")
            {
                name_ = "type2";
            }
            else if (name == "Error3" || name == "reb")
            {
                name_ = "type3";
            }
            else if (name == "Ok")
            {
                name_ = "none";
            }
            else if (name == "Undefined")
            {
                name_ = "discard";
            }
            else
            {
                name_ = name;
            }

            return name_;
        }

        /// <summary>
        /// Convert Default name To IA name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string ConvertDefaultToIA(string name)
        {
            string name_;

            if (name == "type1")
            {
                name_ = "Error1";
            }
            else if (name == "type2")
            {
                name_ = "Error2";
            }
            else if (name == "type3" || name == "reb")
            {
                name_ = "Error3";
            }
            else if (name == "none")
            {
                name_ = "Ok";
            }
            else if (name == "discard")
            {
                name_ = "Undefined";
            }
            else
            {
                name_ = name;
            }

            return name_;
        }

        public static SampleLogModel ConvertAI_SampleLogToSampleLog(AiSampleLogModel aiSampleLogModel)
        {
            return new SampleLogModel
            {
                IStatus_id = aiSampleLogModel.IStatus_id,
                BActive = aiSampleLogModel.BActive,
                IShift = aiSampleLogModel.IShift,
                ITest_id = aiSampleLogModel.ITest_id,
                SEquipament = aiSampleLogModel.SEquipament,
                SArea = aiSampleLogModel.SArea,
                SBatchLote = aiSampleLogModel.SBatchLote,
                DtSample = aiSampleLogModel.DtSample,
                FResult = aiSampleLogModel.FResult,
                SOperator = aiSampleLogModel.SOperator,
                DtPublished_at = aiSampleLogModel.DtPublished_at,
                SComments = aiSampleLogModel.SComments,
                SCreated_by = aiSampleLogModel.SCreated_by,
                DtCreated_at = aiSampleLogModel.DtCreated_at
            };
        }
    }
}
