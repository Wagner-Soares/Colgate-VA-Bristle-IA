using APIVision.DataModels;
using Bristle.UseCases;
using Bristle.utils;
using Bristle.Views;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Bristle.Testes
{
    [TestClass]
    public class ScreenNavigationUseCasesExecute
    {
        [TestMethod]
        public void SalvaConfiguracoesGerais()
        {
            //arrange
            var generalConfigDummy1 = FirstTimeConfigurationUseCases.GetGeneralConfigurationDefaults();

            DataHandlerUseCases.EraseSetting(ConfigurationConstants.GeneralConfigurationName);

            //Act - método sob teste
            ScreenNavigationUseCases.SaveGeneralLocalSettings(generalConfigDummy1);

            //Assert   
            var objReturn = DataHandlerUseCases.ReadJsonFromSettingsKey(ConfigurationConstants.GeneralConfigurationName);
            Assert.IsNotNull(objReturn);
        }

        [TestMethod]
        public void RetornaConfiguracoesGerais()
        {
            //arrange
            var generalConfigDummy1 = FirstTimeConfigurationUseCases.GetGeneralConfigurationDefaults();

            DataHandlerUseCases.EraseSetting(ConfigurationConstants.GeneralConfigurationName);

            ScreenNavigationUseCases.SaveGeneralLocalSettings(generalConfigDummy1);
            //Act - método sob teste
            var objReturn = ScreenNavigationUseCases.GetGeneralLocalSettings();

            //Assert   
            Assert.IsNotNull(objReturn);
        }

        [TestMethod]
        public void DadoObjetoDeUsuarioLogado_RetornaSeTemPermissaoDeAdministrador()
        {
            //arrange
            var generalConfigDummy1 = FirstTimeConfigurationUseCases.GetGeneralConfigurationDefaults();
            var networkUserModelDummy = new NetworkUserModel
            {
                NetworkUserGroup = new string[] { ConfigurationConstants.AdAdminsGroup }
            };

            DataHandlerUseCases.EraseSetting(ConfigurationConstants.GeneralConfigurationName);
            ScreenNavigationUseCases.SaveGeneralLocalSettings(generalConfigDummy1);

            //Act - método sob teste
            var objReturn = ScreenNavigationUseCases.ValidateUserAdministratorPermission(networkUserModelDummy);

            //Assert   
            Assert.IsTrue(objReturn);
        }

        [TestMethod]
        public void DadoObjetoDeUsuarioLogado_RetornaSeTemPermissaoDeQualidade()
        {
            //arrange
            var generalConfigDummy1 = FirstTimeConfigurationUseCases.GetGeneralConfigurationDefaults();
            var networkUserModelDummy = new NetworkUserModel
            {
                NetworkUserGroup = new string[] { ConfigurationConstants.AdQualityGroup }
            };

            DataHandlerUseCases.EraseSetting(ConfigurationConstants.GeneralConfigurationName);
            ScreenNavigationUseCases.SaveGeneralLocalSettings(generalConfigDummy1);

            //Act - método sob teste
            var objReturn = ScreenNavigationUseCases.ValidateUserQualityPermission(networkUserModelDummy);

            //Assert   
            Assert.IsTrue(objReturn);
        }
    }
}
