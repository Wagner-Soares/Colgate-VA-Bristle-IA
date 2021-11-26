using APIVision;
using APIVision.DataModels;
using Bristle.utils;
using Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bristle.UseCases
{
    public static class ScreenNavigationUseCases
    {
        public static GeneralLocalSettings GetGeneralLocalSettings()
        {
            return DataHandlerUseCases.ReadJsonGeneralLocalSettings(ConfigurationConstants.GeneralConfigurationName);
        }

        public static void SaveGeneralLocalSettings(GeneralLocalSettings generalSettings)
        {
            DataHandlerUseCases.SaveJsonIntoSettings(generalSettings, ConfigurationConstants.GeneralConfigurationName);
        }

        public static bool ValidateUserAdministratorPermission(NetworkUserModel networkUserModel)
        {
            if (networkUserModel.NetworkUserGroup == null)
                return false;

            var _generalSettings = ScreenNavigationUseCases.GetGeneralLocalSettings();

            return networkUserModel.NetworkUserGroup.Any(group => group == _generalSettings.AD_Admin);
        }

        public static bool ValidateUserQualityPermission(NetworkUserModel networkUserModel)
        {
            if (networkUserModel.NetworkUserGroup == null)
                return false;

            var _generalSettings = ScreenNavigationUseCases.GetGeneralLocalSettings();

            return networkUserModel.NetworkUserGroup.Any(group => group == _generalSettings.AD_Quality);
        }
    }
}
