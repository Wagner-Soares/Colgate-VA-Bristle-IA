using APIVision.DataModels;
using Bristle.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bristle.UseCases
{
    public static class FirstTimeConfigurationUseCases
    {
        public static GeneralLocalSettings GetGeneralConfigurationDefaults()
        {
            return new GeneralLocalSettings
            {
                IpPrediction = "127.0.0.1",
                PortPrediction = 8888,
                IpTrain = "127.0.0.1",
                PortTrain = 8888,
                IpInterface = "127.0.0.1",
                PortInterface = 5050,
                EndroundTestName = ConfigurationConstants.EndroundTestName,
                TuftTBristleCountTestName = ConfigurationConstants.TuftTBristleCountTestName,
                TuftM1BristleCountTestName = ConfigurationConstants.TuftM1BristleCountTestName,
                TuftM2BristleCountTestName = ConfigurationConstants.TuftM2BristleCountTestName,
                TuftM3BristleCountTestName = ConfigurationConstants.TuftM3BristleCountTestName,
                TuftNBristleCountTestName = ConfigurationConstants.TuftNBristleCountTestName,
                VideoFormatHeight = 2592,
                VideoFormatWidth = 1944,
                CameraSelect = 0,
                AD_Admin = ConfigurationConstants.AdAdminsGroup,
                AD_Quality = ConfigurationConstants.AdQualityGroup,
                AD_Operator = ConfigurationConstants.AdOperatorsGroup,
                CurrentCameraConfiguration = ConfigurationConstants.DefaultCameraConfigurationName
            };
        }

        public static CameraSettingsModel GetCameraConfigurationDefaults()
        {
            return new CameraSettingsModel
            {
                Name = ConfigurationConstants.DefaultCameraConfigurationName,
                LedsOn = 15,
                LedBrightness = 5,
                Exposure = 500,
                Brightness = 30,
                Contrast = 25,
                Hue = 0,
                WhiteBalance = 0,
                AWBR = 0,
                AWBG = 0,
                AWBB = 0,
                Saturation = 16,
                Sharpness = 1,
                Gamma = 8,
                AutomaticExposure = 0,
                Focus = 0,
                Mirror = 0,
                Negative = 0,
                Monochrome = 0,
                ColorFormat = 15,
                SKU = "",
                Equipment = ""
        };
        }
    }
}
