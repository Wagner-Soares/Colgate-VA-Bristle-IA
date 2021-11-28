using APIVision;
using APIVision.DataModels;
using Bristle.utils;
using Bristle.Views;
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

        public static bool OpenGeneralSettingsScreen(UserSystemModel userSystemModel, NetworkUserModel networkUserModel, GeneralLocalSettings generalLocalSettings, BusinessSystem businessSystem, ColgateSkeltaEntities colgateSkeltaEntities, bool maximized)
        {
            if (userSystemModel.Type == "administrator")
            {
                SaveGeneralLocalSettings(generalLocalSettings);
                Views.GeneralSettings generalSettings = new Views.GeneralSettings(maximized, businessSystem, colgateSkeltaEntities);
                generalSettings.Show();
                return true;
            }
            else
            {
                if (ValidateUserAdministratorPermission(networkUserModel) || ValidateUserQualityPermission(networkUserModel))
                {
                    SaveGeneralLocalSettings(generalLocalSettings);
                    Views.GeneralSettings generalSettings = new Views.GeneralSettings(maximized, businessSystem, colgateSkeltaEntities);
                    generalSettings.Show();
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public static bool OpenNeuralNetworkRetrainingScreen(UserSystemModel userSystemModel, NetworkUserModel networkUserModel, GeneralLocalSettings generalLocalSettings, BusinessSystem businessSystem, ColgateSkeltaEntities colgateSkeltaEntities, bool maximized)
        {
            if (userSystemModel.Type == "administrator")
            {
                SaveGeneralLocalSettings(generalLocalSettings);
                NeuralNetworkRetraining neuralNetworkRetraining = new NeuralNetworkRetraining(maximized, businessSystem, colgateSkeltaEntities);
                neuralNetworkRetraining.Show();
                return true;
            }
            else
            {
                if (ValidateUserAdministratorPermission(networkUserModel) || ValidateUserQualityPermission(networkUserModel))
                {
                    SaveGeneralLocalSettings(generalLocalSettings);
                    Views.NeuralNetworkRetraining neuralNetworkRetraining = new NeuralNetworkRetraining(maximized, businessSystem, colgateSkeltaEntities);
                    neuralNetworkRetraining.Show();
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public static bool OpenAutomaticBristleClassificationScreen(GeneralLocalSettings generalLocalSettings, BusinessSystem businessSystem, ColgateSkeltaEntities colgateSkeltaEntities, bool maximized)
        {
            try
            {
                ScreenNavigationUseCases.SaveGeneralLocalSettings(generalLocalSettings);
                Views.AutomaticBristleClassification automaticBristleClassification = new Views.AutomaticBristleClassification(maximized, businessSystem, colgateSkeltaEntities);
                automaticBristleClassification.Show();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool OpenGeneralReportScreen(UserSystemModel userSystemModel, NetworkUserModel networkUserModel, GeneralLocalSettings generalLocalSettings, BusinessSystem businessSystem, ColgateSkeltaEntities colgateSkeltaEntities, bool maximized)
        {
            if (userSystemModel.Type == "administrator")
            {
                ScreenNavigationUseCases.SaveGeneralLocalSettings(generalLocalSettings);
                GeneralReport generalReport = new GeneralReport(maximized, businessSystem, colgateSkeltaEntities);
                generalReport.Show();
                return true;
            }
            else
            {
                if (ValidateUserAdministratorPermission(networkUserModel) || ValidateUserQualityPermission(networkUserModel))
                {
                    ScreenNavigationUseCases.SaveGeneralLocalSettings(generalLocalSettings);
                    Views.GeneralReport generalReport = new GeneralReport(maximized, businessSystem, colgateSkeltaEntities);
                    generalReport.Show();
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public static bool OpenPasswordScreen(UserSystemModel userSystemModel, NetworkUserModel networkUserModel, GeneralLocalSettings generalLocalSettings, BusinessSystem businessSystem, ColgateSkeltaEntities colgateSkeltaEntities, bool maximized)
        {
            if (userSystemModel.Type == "administrator")
            {
                SaveGeneralLocalSettings(generalLocalSettings);
                Views.Password passwordView = new Views.Password(maximized, businessSystem, colgateSkeltaEntities);
                passwordView.Show();
                return true;
            }
            else
            {
                if (ValidateUserAdministratorPermission(networkUserModel) || ValidateUserQualityPermission(networkUserModel))
                {
                    SaveGeneralLocalSettings(generalLocalSettings);
                    Views.Password passwordView = new Password(maximized, businessSystem, colgateSkeltaEntities);
                    passwordView.Show();
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public static bool OpenUserScreen(UserSystemModel userSystemModel, NetworkUserModel networkUserModel, GeneralLocalSettings generalLocalSettings, BusinessSystem businessSystem, ColgateSkeltaEntities colgateSkeltaEntities, bool maximized)
        {
            if (userSystemModel.Type == "administrator")
            {
                SaveGeneralLocalSettings(generalLocalSettings);
                User userView = new User(maximized, businessSystem, colgateSkeltaEntities);
                userView.Show();
                return true;
            }
            else
            {
                if (ValidateUserAdministratorPermission(networkUserModel) || ValidateUserQualityPermission(networkUserModel))
                {
                    SaveGeneralLocalSettings(generalLocalSettings);
                    User userView = new User(maximized, businessSystem, colgateSkeltaEntities);
                    userView.Show();
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public static bool OpenMainScreen(GeneralLocalSettings generalLocalSettings, BusinessSystem businessSystem, ColgateSkeltaEntities colgateSkeltaEntities, bool maximized)
        {
            try
            {
                SaveGeneralLocalSettings(generalLocalSettings);
                MainWindow mainWindow = new MainWindow(maximized, businessSystem, colgateSkeltaEntities);
                mainWindow.Show();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
