using AxDNVideoXLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace APIVision.Controllers
{
    /// <summary>
    /// Definition class of some camera parameters 
    /// </summary>

    /*
     *  resolution , resolution_size>> Resolution Number 0{Width=1280, Height=960}
        resolution , resolution_size>> Resolution Number 1{Width=640, Height=480}
        resolution , resolution_size>> Resolution Number 2{Width=1600, Height=1200}
        resolution , resolution_size>> Resolution Number 3{Width=2048, Height=1536}
        resolution , resolution_size>> Resolution Number 4{Width=2592, Height=1944}
        FPS: {Width=1280, Height=960} at 20FPS
        FPS: {Width=640, Height=480} at 30FPS
        FPS: {Width=1600, Height=1200} at 8FPS
        FPS: {Width=2048, Height=1536} at 2FPS
        FPS: {Width=2592, Height=1944} at 2FPS
        Codec:  0 WMVideo8 Encoder DMO
        Codec:  1 WMVideo9 Encoder DMO
        Codec:  2 MSScreen 9 encoder DMO
        Codec:  3 DV Video Encoder
        Codec:  4 MJPEG Compressor
        Codec:  5 Cinepak Codec by Radius
        Codec:  6 Codec IYUV Intel
        Codec:  7 Codec IYUV Intel
        Codec:  8 Microsoft RLE
        Codec:  9 Microsoft Video 1
        */

    public static class CameraObject 
    {
        private static readonly AxDNVideoX axDNVideoX = new AxDNVideoX(); //SDK
        private static readonly int ResolutionWidthInitialValue = 2592;
        private static readonly int ResolutionHeightInitialValue = 1944;
        private static readonly int VideoDeviceIndexInitialValue = 0;
        private static readonly int nominalBristleInitialValue = 0;
        private static readonly string dataSInitialValue = "";

        public static AxDNVideoX DinoLiteSDK { get; set; } = axDNVideoX;
        public static int ResolutionWidth { get; set; } = ResolutionWidthInitialValue; //2592; //1600 //1280
        public static int ResolutionHeight { get; set; } = ResolutionHeightInitialValue; //1944; //1200 //960
        public static int VideoDeviceIndex { get; set; } = VideoDeviceIndexInitialValue;
        public static int NominalBristle { get; set; } = nominalBristleInitialValue;
        public static string DataS { get; set; } = dataSInitialValue;

    }
}
