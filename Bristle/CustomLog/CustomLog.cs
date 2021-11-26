using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Bristle.Log
{
    public static class CustomLog
    {
        private static void ArchieveLog()
        {
            string fileName = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Colgate Vision\Log\Log.txt";
            FileInfo fileInfo = new FileInfo(fileName);

            try
            {
                //Verify if the file has the maximum allowed size (1MB)
                if (fileInfo.Length > 1000000)
                {
                    string backupFileName = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Colgate Vision\Log\" +
                                            "Log_" + DateTime.Now.ToString("yyyy-MM-dd--HH-mm") + ".txt";

                    File.Move(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Colgate Vision\Log\Log.txt", backupFileName);

                    //Verify if it is necessary to delete obsolete logs
                    DeleteObsoleteLogs();
                }
            }
            catch
            {
                //If some error occurs, prevent the app to crash
            }
        }

        private static void CreateLogDir()
        {
            string dirPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Colgate Vision\Log";

            try
            {
                // Determine whether the directory exists.
                if (!Directory.Exists(dirPath))
                {
                    Directory.CreateDirectory(dirPath);
                }
            }
            catch
            {
                //If some error occurs, prevent the app to crash
            }
        }

        private static void DeleteObsoleteLogs()
        {
            try
            {
                string[] files = Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Colgate Vision\Log");

                foreach (string file in files)
                {
                    FileInfo fi = new FileInfo(file);
                    if (fi.LastAccessTime < DateTime.Now.AddMonths(-3))
                        fi.Delete();
                }
            }
            catch 
            {
                //If some error occurs, prevent the app to crash
            }
        }

        public static void LogMessage(string message)
        {
            try
            {
                //Verify if it is necessary to create the Log dir
                CreateLogDir();

                string logFileToAppend = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Colgate Vision\Log\Log.txt";

                // Appending the given texts
                using (StreamWriter sw = File.AppendText(logFileToAppend))
                {
                    sw.WriteLine("Date: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm") + "------ Message: " + message);
                }

                //Verify if it is necessary to backup the log file and create a new one
                ArchieveLog();
            }
            catch
            {
                //Try to prevent crash
            }
        }
    }
}
