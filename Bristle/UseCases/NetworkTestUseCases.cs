using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Bristle.UseCases
{
    public static class NetworkTestUseCases
    {
        public static bool TestIpPingConnection(string ipAdress)
        {
            if (ipAdress.Split('.').Length == 4)
            {
                try
                {
                    Ping pinger = new Ping();
                    PingReply resposta = pinger.Send(ipAdress, 100);

                    if (resposta.Status == IPStatus.Success)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch
                {
                    //Prevent Ping Crash for wrong formatting

                    return false;
                }
            }

            return false;
        }

        public static bool TestIpPortConnection(string ipAdress, int port)
        {
            try
            {
                TcpClient clientSocket = new TcpClient();
                
                try
                {
                    var task = Task.Run(() => clientSocket = new TcpClient(ipAdress, port));
                    if (!task.Wait(TimeSpan.FromSeconds(1)))
                        throw new TimeoutException("Timed out");
                }
                catch
                {
                    clientSocket.Dispose();
                    return false;
                }

                if (clientSocket.Connected)
                {
                    clientSocket.Dispose();
                    return true;
                }
                else
                {
                    clientSocket.Dispose();
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        public static bool TestSharedFolderAcess(string folderPath)
        {
            bool haveAccess = false;

            if (folderPath != null)
            {
                DirectoryInfo di = new DirectoryInfo(folderPath);
                if (di.Exists)
                {
                    try
                    {
                        // you could also call GetDirectories or GetFiles
                        // to test them individually
                        // this will throw an exception if you don't have 
                        // rights to the directory, though
                        var acl = di.GetAccessControl();
                        if(acl != null)
                            haveAccess = true;
                    }
                    catch (UnauthorizedAccessException uae)
                    {
                        if (uae.Message.Contains("read-only"))
                        {
                            // seems like it is just read-only
                            haveAccess = true;
                        }
                        // no access, sorry
                        // do something else...
                    }
                }
            }

            return haveAccess;
        }
    }
}
