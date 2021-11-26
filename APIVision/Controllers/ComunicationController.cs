using APIVision.DataModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace APIVision.Controllers
{
    /// <summary>
    /// Class that handles system communication 
    /// </summary>
    public class ComunicationController
    {
        public delegate void FileRecievedEventHandler(object fonte, string nomeArquivo);
        public event FileRecievedEventHandler NovoArquivoRecebido;

        private BinaryWriter dataWrite;
        private BinaryReader dataRead;
        private Socket socket;
        private NetworkStream socketStream;

        public DataModel FrameModel { get; set; } = new DataModel() {
            TemperatureAlert = false,
            MaskAlert = false,
            Name = "name",
            TemperatureValue = 0};

        public string Command { get; set; } = "";
        public string Name { get; set; }

        /// <summary>
        /// Start Server TCP
        /// </summary>
        /// <param name="port"></param>
        public void EstablishConnection( int port)
        {
            try
            {
                Task.Factory.StartNew(() => TCPServer(port));
                System.Console.WriteLine("Escutando na porta...: " + port);
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("Erro : " + ex.Message);
            }
        }

        /// <summary>
        /// Server TCP
        /// </summary>
        /// <param name="porta"></param>
        public void TCPServer(int porta)
        {
            try
            {
                IPAddress ip;
                TcpListener tcpListener;
                ip = IPAddress.Parse("0.0.0.0");
                tcpListener = new TcpListener(ip, porta);
                tcpListener.Start();
#pragma warning disable S4423 // Weak SSL/TLS protocols should not be used
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;
#pragma warning restore S4423 // Weak SSL/TLS protocols should not be used

                while (true)
                {
                    try
                    {
                        socket = tcpListener.AcceptSocket();
                        StringBuilder stringBuilder = new StringBuilder();
                        socketStream = new NetworkStream(socket);
                        dataWrite = new BinaryWriter(socketStream);
                        dataRead = new BinaryReader(socketStream);

                        string resp_ = string.Empty;
                        char resp;

                        do
                        {
                            try
                            {
                                resp = dataRead.ReadChar();

                                resp_ = stringBuilder.Append(resp).ToString();
                            }
                            catch (Exception e)
                            {
                                dataWrite.Close();
                                dataRead.Close();
                                socketStream.Close();
                                socket.Close();
                                resp_ = "#";
                                Console.WriteLine("[3] Unexpected exception : {0}", e.ToString());
                                break;
                            }

                        } while (resp != '\n' && socket.Connected);

                        Thread.Sleep(100);

                        dataWrite.Close();
                        dataRead.Close();
                        socketStream.Close();
                        socket.Close();

                        Thread t = new Thread(() => UploadResultFileMain(resp_))
                        {
                            IsBackground = true
                        };
                        t.Start();
                    }
                    catch (Exception ex)
                    {
                        dataWrite.Close();
                        dataRead.Close();
                        socketStream.Close();
                        socket.Close();
                        Console.WriteLine("[6] Unexpected exception : {0}", ex.ToString());
                    }
                }
            }            
            catch
            {
                //tryCatch to avoid crash
            }
        }

        public void UploadResultFileMain(string data)
        {
            Thread t_ = new Thread(() => UploadResultFile(data))
            {
                Name = "t",
                IsBackground = true
            };
            t_.Start();
            t_.Join();
        }

        public void UploadResultFile(string data)
        {
            try
            {
                Command = "";

                string[] data_1 = data.Split('[');

                if(data_1.Count() > 1)
                {
                    Command = "[" + data_1[1];

                    if (Command.Length < 10)
                    {
                        Command = "*";
                    }
                }
                else
                {
                    string[] data_2 = data.Split('{');

                    if (data_2.Count() > 1)
                    {
                        Command = "{" + data_2[1];

                        if (Command.Length < 10)
                        {
                            Command = "*";
                        }
                        else
                        {
                            Command = ReadJsonResponseFromPredict(Command);
                        }
                    }
                }               

                NovoArquivoRecebido?.Invoke(this, "data");
            }
            catch
            {
                NovoArquivoRecebido?.Invoke(this, "data");
            }
           
        }

        public void SendBitmapCommand(string command, string address, int port, Bitmap frame)
        {
            Thread t = new Thread(() => SendCommand_(command, address, port, frame))
            {
                Name = "t",
                IsBackground = true
            };
            t.Start();        
        }

        public void SendStringJsonCommand(string command, string address, int port, string json)
        {
            Thread t2 = new Thread(() => SendCommand_2(command, address, port, json))
            {
                Name = "t2",
                IsBackground = true
            };
            t2.Start();
        }

        private void SendCommand_(string command, string address, int port, Bitmap frame)
        {
            try
            {
                if (!string.IsNullOrEmpty(address))
                {
                    byte[] fileNameByte = Encoding.ASCII.GetBytes(command); //COMMDS100
                    byte[] fileData = ImageToByteArray(frame);
                    byte[] clientData = new byte[4 + fileNameByte.Length + fileData.Length];
                    byte[] fileNameLen = BitConverter.GetBytes(fileNameByte.Length);

                    fileNameLen.CopyTo(clientData, 0);
                    fileNameByte.CopyTo(clientData, 4);
                    fileData.CopyTo(clientData, 4 + fileNameByte.Length);

                    TcpClient clientSocket = new TcpClient(address, port);
                    NetworkStream networkStream = clientSocket.GetStream();

                    networkStream.Write(clientData, 0, clientData.GetLength(0));
                    networkStream.Close();
                    clientSocket.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public void SendCommand_2(string command, string address, int port, string json)
        {
            try
            {
                if (!string.IsNullOrEmpty(address))
                {
                    byte[] fileNameByte = Encoding.ASCII.GetBytes(command);
                    byte[] fileData = Encoding.ASCII.GetBytes(json);
                    byte[] clientData = new byte[4 + fileNameByte.Length + fileData.Length];
                    byte[] fileNameLen = BitConverter.GetBytes(fileNameByte.Length);

                    fileNameLen.CopyTo(clientData, 0);
                    fileNameByte.CopyTo(clientData, 4);
                    fileData.CopyTo(clientData, 4 + fileNameByte.Length);

                    TcpClient clientSocket = new TcpClient(address, port);
                    NetworkStream networkStream = clientSocket.GetStream();

                    networkStream.Write(clientData, 0, clientData.GetLength(0));
                    networkStream.Close();
                    clientSocket.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        /// <summary>
        /// Converter array to image
        /// </summary>
        /// <param name="imageIn"></param>
        /// <returns></returns>
        private byte[] ImageToByteArray(Image imageIn)
        {
            MemoryStream ms = new MemoryStream();
            imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Png);

            return ms.ToArray();
        }

        /// <summary>
        /// deserialization of a JSON file on a specific object 
        /// </summary>
        /// <param name="json"></param>
        /// <returns>status: OK || NOK</returns>
        private string ReadJsonResponseFromPredict(string json)
        {
            SocketResponseModel obj = JsonConvert
                                        .DeserializeObject<SocketResponseModel>(json);

            return obj.Status;
        }
    }
}
