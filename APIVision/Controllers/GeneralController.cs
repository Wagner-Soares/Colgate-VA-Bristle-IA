using APIVision.CommandModels;
using APIVision.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.Script.Serialization;
using System.Windows.Media.Imaging;

namespace APIVision.Controllers
{
    /// <summary>
    /// Class of general methods 
    /// </summary>
    public class GeneralController
    {       
        /// <summary>
        /// Method to save parameters in JSON format.
        /// </summary>
        public string SaveJson(object obj, string path)
        {
            string json;

            if (File.Exists(path))
            {
                File.Delete(path);
                json = JsonConvert.SerializeObject(obj, Formatting.Indented);
                File.AppendAllText(path, json);
            }
            else
            {
                json = JsonConvert.SerializeObject(obj, Formatting.Indented);
                File.AppendAllText(path, json);
            }

            return json;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="comando"></param>
        /// <returns></returns>
        public void CMD(string command)
        {
            Process scriptProc = new Process();
            scriptProc.StartInfo.FileName = @"wscript.exe";
            scriptProc.StartInfo.Arguments = command;
            scriptProc.Start();
            scriptProc.WaitForExit();
            scriptProc.Close();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        public void MountVbs(string name)
        {
            Stream saida = File.Create(name + ".vbs");
            StreamWriter escritor = new StreamWriter(saida);

            escritor.WriteLine("Dim wshShell");
            escritor.WriteLine("Set wshShell = CreateObject(\"WScript.Shell\")");
            escritor.WriteLine("wshShell.Run \""+name + ".bat\", 0, false");
            escritor.Close();
        }

        public void MountBat(string name, string command)
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
        /// <param name="machine"></param>
        /// <param name="path"></param>
        /// <param name="differentFiles"></param>
        public void SaveJson(object machine, string path, bool differentFiles)
        {
            string json = JsonConvert.SerializeObject(machine, Formatting.Indented);

            try
            {
                if (differentFiles)
                {
                    if (!File.Exists(path))
                    {
                        File.AppendAllText(path, "[\n");
                        File.AppendAllText(path, json);
                        File.AppendAllText(path, "]");
                    }
                    else
                    {
                        var lines = File.ReadAllLines(path);
                        lines[lines.Length - 1] = "},";
                        File.WriteAllLines(path, lines);
                        File.AppendAllText(path, json);
                        File.AppendAllText(path, "]");
                    }
                }
                else
                {
                    File.AppendAllText(path, json);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Save file Json {0}", e.Message);
            }
        }

        public string SaveJson2(object obj, string path)
        {
            string json = JsonConvert.SerializeObject(obj, Formatting.Indented);

            if(File.Exists(path))
            {
                File.Delete(path);
                File.AppendAllText(path, json);
            }
            else
            {
                File.AppendAllText(path, json);
            }
           
            return json;
        }

        public string SaveJson(object obj)
        {
            string json = JsonConvert.SerializeObject(obj, Formatting.Indented);

            return json;
        }

        /// <summary>
        /// Deserialization of a JSON file into an object 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public object ReadJson(string path)
        {
            var obj = JsonConvert.DeserializeObject<object>(File.ReadAllText(path));

            return obj;
        }

        /// <summary>
        /// deserialization of a JSON file on a specific object 
        /// </summary>
        /// <param name="json"></param>
        /// <returns>status: OK || NOK</returns>
        public string ReadJsonResponse(string json)
        {
            SocketResponseModel obj = JsonConvert.DeserializeObject<SocketResponseModel>(json);

            return obj.status;
        }


        public void answerJson()
        {

        }

        /// <summary>
        /// Deserialization of a JSON file on a specific object 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public int ReadJson2(string path)
        {
            CommandS300Model obj = JsonConvert.DeserializeObject<CommandS300Model>(File.ReadAllText(path));

            return obj.NewModelId;
        }

        /// <summary>
        /// Deserialization of a JSON file on a specific object 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public GeneralLocalSettings ReadJson3(string path)
        {
            try
            {
                GeneralLocalSettings obj = JsonConvert.DeserializeObject<GeneralLocalSettings>(File.ReadAllText(path));
                return obj;
            }
            catch (Exception ex)
            {
            }

            return null;
        }

        /// <summary>
        /// Deserialization of a JSON file on a specific object 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public CameraSettingsModel ReadJson4(string path)
        {
            try
            {
                CameraSettingsModel obj = JsonConvert.DeserializeObject<CameraSettingsModel>(File.ReadAllText(path));
                return obj;
            }
            catch (Exception ex)
            {
            }

            return null;
        }

        /// <summary>
        /// Deserialization of a JSON file on a specific object 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="businessSystem"></param>
        /// <returns></returns>
        public List<SocketModel> converterJSON(string data, BusinessSystem businessSystem)
       {
            List<SocketModel> socketModel = new List<SocketModel>(); 

            try
            {
                File.WriteAllText(businessSystem.localRepository + @"\data.json", data);
                socketModel = JsonConvert.DeserializeObject<List<SocketModel>>(File.ReadAllText(businessSystem.localRepository + @"\data.json"));
                data = null;

                File.Delete(businessSystem.localRepository + @"\data.json");
            }
            catch 
            {                
            }
           

            return socketModel;
        }

        /// <summary>
        /// Name preformation with timetemp 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="timestemp"></param>
        /// <returns></returns>
        public string formatTimesTemp(string name, string timestemp)
        {
            try
            {
                string[] data_ = timestemp.Split(' ');

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
        /// Conversion of the byte array image 
        /// </summary>
        /// <param name="byteArrayIn"></param>
        /// <returns>Byte[]</returns>
        public Image byteArrayToImage(Byte[] byteArrayIn)
        {
            MemoryStream ms = new MemoryStream(byteArrayIn);
            Image returnImage = Image.FromStream(ms);

            return returnImage;
        }

        /// <summary>
        /// Converter array to image
        /// </summary>
        /// <param name="imageIn"></param>
        /// <returns></returns>
        public byte[] imageToByteArray(Image imageIn)
        {
            MemoryStream ms = new MemoryStream();
            imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Png); //Tiff);// Bmp);//Gif);

            return ms.ToArray();
        }

        /// <summary>
        /// Conversion of the byte array image 
        /// </summary>
        /// <param name="bitmapImage"></param>
        /// <returns></returns>
        public Bitmap BitmapImage2Bitmap(BitmapImage bitmapImage)
        {            
            using (MemoryStream outStream = new MemoryStream())
            {              
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapImage));
                enc.Save(outStream);
                System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(outStream);

                return new Bitmap(bitmap);                
            }
        }

        /// <summary>
        /// Resize the image to the specified width and height.
        /// </summary>
        /// <param name="image">The image to resize.</param>
        /// <param name="width">The width to resize to.</param>
        /// <param name="height">The height to resize to.</param>
        /// <returns>The resized image.</returns>
        public Bitmap ResizeImage(System.Drawing.Image image, int width, int height)
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
        /// Encryptor
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public long Encryptor(string s)
        {
            List<long> value = new List<long>();
            long valueReturn = 0;
            foreach (int var in s)
            {
                int aux = (int)var;
                value.Add((aux * 3) + 32);
            }

            foreach (var v in value)
            {
                valueReturn += v;
            }

            return valueReturn;
        }

        /// <summary>
        /// Decrypt
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public long Decrypt(string s)
        {
            List<long> value = new List<long>();
            long valueReturn = 0;
            foreach (int var in s)
            {
                int aux = (int)var;
                value.Add((aux * 3) + 32);
            }

            foreach (var v in value)
            {
                valueReturn += v;
            }

            return valueReturn;
        }

        /// <summary>
        /// Convert IA name To Default name 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string convertIAToDefault(string name)
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
            else if(name == "Undefined")
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
        public string convertDefaultToIA(string name)
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
    }
}
