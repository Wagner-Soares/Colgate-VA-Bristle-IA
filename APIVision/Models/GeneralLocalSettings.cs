using System;
using System.Collections.Generic;
using System.Text;

namespace APIVision.Models
{
    public class GeneralLocalSettings
    {
        public string NamePrefix { get; set; }
        public string IpPrediction { get; set; }
        public int PortPrediction { get; set; }
        public string IpTrain { get; set; }
        public int PortTrain { get; set; }
        public string IpInterface { get; set; }
        public int PortInterface { get; set; }
        public string CurrentCameraConfiguration { get; set; }
        public int VideoFormatHeight { get; set; }
        public int VideoFormatWidth { get; set; }
        public int CameraSelect { get; set; }
        public string Test { get; set; }
        public int IdTest_1 { get; set; }
        public int IdTest_2 { get; set; }
        public string Area { get; set; }        
        public string BatchLote { get; set; }
        public string Model { get; set; }
        public bool MissingCamera { get; set; }
    }
}
