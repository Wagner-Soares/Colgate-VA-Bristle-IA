using APIVision.Models;
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
        public DataModel FrameModel = new DataModel();
        public string command = "";
        public string Name;
        private GeneralController generalController = new GeneralController();
        private NetworkStream networkStream;
        private BinaryWriter dataWrite;
        private BinaryReader dataRead;
        private Socket socket;
        private NetworkStream socketStream;
        private SocketError errorCode;

        public ComunicationController()
        {
            FrameModel.temperatureAlert = false;
            FrameModel.maskAlert = false;
            FrameModel.name = "name";
            FrameModel.temperatureValue = 0;
        }

        /// <summary>
        /// Start Server TCP
        /// </summary>
        /// <param name="address"></param>
        /// <param name="port"></param>
        public void EstablishConnection(string address, int port)
        {
            try
            {
                Task.Factory.StartNew(() => TCPServer(port, address));
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
        /// <param name="enderecoIp"></param>
        public void TCPServer(int porta, string enderecoIp)
        {
            try
            {
                IPAddress ip;
                TcpListener tcpListener;
                Int64 cont = 0;
                ip = IPAddress.Parse("0.0.0.0");
                tcpListener = new TcpListener(ip, porta);
                tcpListener.Start();
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;
                                   
                while (true)
                {
                    try
                    {
                        socket = tcpListener.AcceptSocket();
                        socketStream = new NetworkStream(socket);
                        dataWrite = new BinaryWriter(socketStream);
                        dataRead = new BinaryReader(socketStream);

                        string resp_ = "";
                        char resp = '#';

                        do
                        {
                            try
                            {
                                resp = dataRead.ReadChar();
                                resp_ += resp;
                            }
                            catch (ArgumentNullException ane)
                            {
                                dataWrite.Close();
                                dataRead.Close();
                                socketStream.Close();
                                socket.Close();
                                resp_ = "#";
                                Console.WriteLine("[1] ArgumentNullException : {0}", ane.ToString());
                                break;
                            }
                            catch (SocketException se)
                            {
                                dataWrite.Close();
                                dataRead.Close();
                                socketStream.Close();
                                socket.Close();
                                resp_ = "#";
                                Console.WriteLine("[2] SocketException : {0}", se.ToString());
                                break;
                            }
                            catch (Exception e)
                            {
                                dataWrite.Close();
                                dataRead.Close();
                                socketStream.Close();
                                socket.Close();
                                resp_ = "#";
                                Console.WriteLine("[3] Unexpected exception : {0}", e.ToString());
                                ////server.BeginAcceptTcpClient(new AsyncCallback(AcceptMessage), server);
                                //throw new SocketException((int)socket.GetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Error));
                                break;
                            }

                        } while (resp != '\n' && socket.Connected); //"}"

                        Thread.Sleep(100);

                        dataWrite.Close();
                        dataRead.Close();
                        socketStream.Close();
                        socket.Close();

                        //if (cont >= 4000)
                        //{
                        //    cont = 0;
                        //}

                        //Threads[cont] = new Thread(() => uploadResultFileMain(resp_));       
                        //Threads[cont].IsBackground = true;
                        //Threads[cont].Start();
                        //cont++;         

                        Thread t = new Thread(() => uploadResultFileMain(resp_));
                        t.IsBackground = true;
                        t.Start();
                    }
                    catch (ArgumentNullException ane)
                    {
                        dataWrite.Close();
                        dataRead.Close();
                        socketStream.Close();
                        socket.Close();
                        Console.WriteLine("[4] ArgumentNullException : {0}", ane.ToString());
                    }
                    catch (SocketException se)
                    {
                        dataWrite.Close();
                        dataRead.Close();
                        socketStream.Close();
                        socket.Close();
                        Console.WriteLine("[5] SocketException : {0}", se.ToString());
                    }
                    catch (Exception e)
                    {
                        dataWrite.Close();
                        dataRead.Close();
                        socketStream.Close();
                        socket.Close();
                        Console.WriteLine("[6] Unexpected exception : {0}", e.ToString());
                    }
                }
            }            
            catch
            { 
            }
        }

        public void uploadResultFileMain(string data)
        {
            Thread t_ = new Thread(() => uploadResultFile(data));
            t_.Name = "t";
            t_.IsBackground = true;
            t_.Start();
            t_.Join();
        }

        public void uploadResultFile(string data)
        {
            try
            {
                command = "";

                string[] data_1 = data.Split('[');

                if(data_1.Count() > 1)
                {
                    command = "[" + data_1[1];

                    if (command.Length < 10)
                    {
                        command = "*";
                    }
                }
                else
                {
                    string[] data_2 = data.Split('{');

                    if (data_2.Count() > 1)
                    {
                        command = "{" + data_2[1];

                        if (command.Length < 10)
                        {
                            command = "*";
                        }
                        else
                        {
                            command = generalController.ReadJsonResponse(command);
                        }
                    }
                }               

                NovoArquivoRecebido?.Invoke(this, "data");
            }
            catch (Exception e_)
            {
                NovoArquivoRecebido?.Invoke(this, "data");

            }
           
        }

        /// <summary>
        /// Command sending via TCP 
        /// </summary>
        /// <param name="command">command</param>
        /// <param name="address">IP</param>
        /// <param name="port">PORT</param>
        public void sendCommand(string command, string address, int port)
        {
            Thread t3 = new Thread(() => sendCommand_3(command, address, port));
            t3.Name = "t3";
            t3.IsBackground = true;
            t3.Start();
            try
            {
                if (!string.IsNullOrEmpty(address))
                {
                    byte[] fileNameByte = Encoding.ASCII.GetBytes("COMMD");
                    byte[] fileData = Encoding.ASCII.GetBytes(command);
                    byte[] clientData = new byte[4 + fileNameByte.Length + fileData.Length];
                    byte[] fileNameLen = BitConverter.GetBytes(fileNameByte.Length);

                    fileNameLen.CopyTo(clientData, 0);
                    fileNameByte.CopyTo(clientData, 4);
                    fileData.CopyTo(clientData, 4 + fileNameByte.Length);

                    TcpClient clientSocket = new TcpClient(address, port);
                    NetworkStream networkStream = clientSocket.GetStream();

                    networkStream.Write(clientData, 0, clientData.GetLength(0));
                    networkStream.Close();
                }
            }
            catch
            {
            }
        }

        public void sendCommand_3(string command, string address, int port)
        {

        }

        public void sendCommand(string command, string address, int port, Bitmap frame)
        {
            Thread t = new Thread(() => sendCommand_(command, address, port, frame));
            t.Name = "t";
            t.IsBackground = true;
            t.Start();        
        }

        private void sendCommand_(string command, string address, int port, Bitmap frame)
        {
            try
            {
                if (!string.IsNullOrEmpty(address))
                {
                    byte[] fileNameByte = Encoding.ASCII.GetBytes(command); //COMMDS100
                    byte[] fileData = generalController.imageToByteArray(frame);
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

        public void sendCommand(string command, string address, int port, string json)
        {
            Thread t2 = new Thread(() => sendCommand_2(command, address, port, json));
            t2.Name = "t2";
            t2.IsBackground = true;
            t2.Start();
        }

        public void sendCommand_2(string command, string address, int port, string json)
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
    }
}
