using System;
using System.Collections.Generic;
using System.Text;

namespace APIVision.DataModels
{
    public class CameraSettingsModel
    {
        public string Name { get; set; }       
        public int LedsOn { get; set; }
        public int LedBrightness { get; set; }
        public double Exposure { get; set; }
        public int Brightness { get; set; }
        public int Contrast { get; set; }
        public int Hue { get; set; }
        public int Saturation { get; set; }
        public int Sharpness { get; set; }
        public int AutomaticExposure { get; set; }
        public double Focus { get; set; }
        public int Mirror { get; set; }
        public int Negative { get; set; }
        public int WhiteBalance { get; set; }
        public int Monochrome { get; set; }
        public int Gamma { get; set; }
        public int AWBR { get; set; }
        public int AWBG { get; set; }
        public int AWBB { get; set; }
        public int ColorFormat { get; set; }

        public string SKU { get; set; }
        public string Equipment { get; set; }
    }
}
