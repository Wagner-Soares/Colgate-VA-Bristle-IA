using System;
using System.Collections.Generic;
using System.Text;

namespace APIVision.DataModels
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
        public string EndroundTestName { get; set; }
        public string TuftTBristleCountTestName { get; set; }
        public string TuftM1BristleCountTestName { get; set; }
        public string TuftM2BristleCountTestName { get; set; }
        public string TuftM3BristleCountTestName { get; set; }
        public string TuftNBristleCountTestName { get; set; }
        public TestSpecificationModel EndroundSpecTest { get; set; }
        public TestSpecificationModel TuftTBristleCountSpecTest { get; set; }
        public TestSpecificationModel TuftM1BristleCountSpecTest { get; set; }
        public TestSpecificationModel TuftM2BristleCountSpecTest { get; set; }
        public TestSpecificationModel TuftM3BristleCountSpecTest { get; set; }
        public TestSpecificationModel TuftNBristleCountSpecTest { get; set; }
        public string Area { get; set; }        
        public string BatchLote { get; set; }
        public string Model { get; set; }
        public bool MissingCamera { get; set; }
        public string AD_Operator { get; set; }
        public string AD_Quality { get; set; }
        public string AD_Admin { get; set; }
    }
}
