using APIVision.Controllers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Bristle.UseCases
{
    public static class CameraUseCases
    {
        public static Bitmap SaveEDOFBitmap()
        {
            string filePath = @"C:\Dino-LiteEDOF\EdofImage.bmp";

            CameraObject.DinoLiteSDK.SaveEDOF(1, 5, filePath);

            while (!File.Exists(filePath))
            {
                Thread.Sleep(1000);
            }

            long length;

            do
            {
                length = new FileInfo(filePath).Length;
                Thread.Sleep(1000);
            } while (length != 0);

            do
            {
                length = new FileInfo(filePath).Length;
                Thread.Sleep(1000);
            } while (length == 0);


            System.Drawing.Bitmap img;

            using (Bitmap bm = new Bitmap(filePath))
            {
                img = new Bitmap(bm);
            }

            return img;
        }
    }
}
