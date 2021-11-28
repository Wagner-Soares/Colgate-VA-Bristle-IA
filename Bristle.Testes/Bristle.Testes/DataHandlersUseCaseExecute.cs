using APIVision.DataModels;
using APIVision.DataModels.CommandModels;
using Bristle.UseCases;
using Bristle.utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Bristle.Testes
{
    [TestClass]
    public class DataHandlersUseCaseExecute
    {
        [TestMethod]
        public void DadoValorDaChaveParaVerificar_RetornaSeExiste()
        {
            //arrange
            var cameraConfigDefault = FirstTimeConfigurationUseCases.GetCameraConfigurationDefaults();

            DataHandlerUseCases.SaveJsonIntoSettings(cameraConfigDefault, ConfigurationConstants.CameraConfigurationName);

            //Act - método sob teste
            var settingsExistTest = DataHandlerUseCases.SettingExists(ConfigurationConstants.CameraConfigurationName);

            //Assert   
            Assert.AreEqual(true, settingsExistTest);
        }

        [TestMethod]
        public void DadoValorDaChaveExistente_DeletaChave()
        {
            //arrange
            var cameraConfigDefault = FirstTimeConfigurationUseCases.GetCameraConfigurationDefaults();

            DataHandlerUseCases.SaveJsonIntoSettings(cameraConfigDefault, ConfigurationConstants.CameraConfigurationName);

            //Act - método sob teste
            DataHandlerUseCases.EraseSetting(ConfigurationConstants.CameraConfigurationName);

            //Assert   
            var settingsExistTest = DataHandlerUseCases.SettingExists(ConfigurationConstants.CameraConfigurationName);
            Assert.AreEqual(false, settingsExistTest);
        }

        [TestMethod]
        public void DadoValorDeDoisJSON_SalvaAmbosNaMesmaChave()
        {
            //arrange
            var generalConfigDummy1 = FirstTimeConfigurationUseCases.GetCameraConfigurationDefaults();
            var generalConfigDummy2 = FirstTimeConfigurationUseCases.GetCameraConfigurationDefaults();

            DataHandlerUseCases.EraseSetting(ConfigurationConstants.CameraConfigurationName);

            //Act - método sob teste
            DataHandlerUseCases.SaveOrAppendToSettings(generalConfigDummy1, ConfigurationConstants.CameraConfigurationName);
            DataHandlerUseCases.SaveOrAppendToSettings(generalConfigDummy2, ConfigurationConstants.CameraConfigurationName);

            //Assert   
            List<CameraSettingsModel> settingsExistTest = DataHandlerUseCases.ReadJsonCameraSettings(ConfigurationConstants.CameraConfigurationName);
            Assert.AreEqual(2,settingsExistTest.Count);
        }

        [TestMethod]
        public void DadoValorDeJSON_SalvaERetornaStringDeConteudo()
        {
            //arrange
            var generalConfigDummy1 = FirstTimeConfigurationUseCases.GetGeneralConfigurationDefaults();

            DataHandlerUseCases.EraseSetting(ConfigurationConstants.GeneralConfigurationName);

            //Act - método sob teste
            var stringTest = DataHandlerUseCases.SaveJsonWithStringReturn(generalConfigDummy1, ConfigurationConstants.GeneralConfigurationName);

            //Assert   
            Assert.IsNotNull(stringTest);
        }

        [TestMethod]
        public void DadoNomeDaChave_RetornaValor()
        {
            //arrange
            var generalConfigDummy1 = FirstTimeConfigurationUseCases.GetGeneralConfigurationDefaults();

            DataHandlerUseCases.SaveJsonIntoSettings(generalConfigDummy1, ConfigurationConstants.GeneralConfigurationName);

            //Act - método sob teste
            var objReturn = DataHandlerUseCases.ReadJsonFromSettingsKey(ConfigurationConstants.GeneralConfigurationName);

            //Assert   
            Assert.IsNotNull(objReturn);
        }

        [TestMethod]
        public void DadoObjeto_RetornaEmFormatoJSON()
        {
            //arrange
            var generalConfigDummy1 = FirstTimeConfigurationUseCases.GetGeneralConfigurationDefaults();

            //Act - método sob teste
            var jsonTest = DataHandlerUseCases.ConvertObjectToJson(generalConfigDummy1);

            //Assert   
            Assert.IsNotNull(jsonTest);
        }

        [TestMethod]
        public void RetornaDadosDoComando300()
        {
            //arrange
            CommandS300Model cmd300Model = new CommandS300Model
            {
                NewModelId = 10
            };

            DataHandlerUseCases.EraseSetting("commandS300");

            DataHandlerUseCases.SaveJsonIntoSettings(cmd300Model, "commandS300");

            //Act - método sob teste
            var cmd300ModelId = DataHandlerUseCases.ReadJsonCommand300("commandS300");

            //Assert   
            Assert.AreEqual(10,cmd300ModelId);
        }

        [TestMethod]
        public void RetornaDadosDasConfiguracoesGerais()
        {
            //arrange

            var generalConfigDummy1 = FirstTimeConfigurationUseCases.GetGeneralConfigurationDefaults();

            DataHandlerUseCases.EraseSetting(ConfigurationConstants.GeneralConfigurationName);

            DataHandlerUseCases.SaveJsonIntoSettings(generalConfigDummy1, ConfigurationConstants.GeneralConfigurationName);

            //Act - método sob teste
            var generalSettingsTest = DataHandlerUseCases.ReadJsonGeneralLocalSettings(ConfigurationConstants.GeneralConfigurationName);

            //Assert   
            Assert.IsNotNull(generalSettingsTest);
        }

        [TestMethod]
        public void RetornaTimestampFormat()
        {
            //arrange

            string name = "dummyName";

            string timestamp = DateTime.Now.ToString();

            //Act - método sob teste
            var testReturned = DataHandlerUseCases.FormatTimestamp(name, timestamp);

            //Assert   
            Assert.IsNotNull(testReturned);
        }

        [TestMethod]
        public void DadoBitmapETamanhoRequerido_RedimensionaBitmap()
        {
            //arrange

            var bitmapDummy = new Bitmap(100,100);

            //Act - método sob teste
            var testReturned = DataHandlerUseCases.ResizeImage(bitmapDummy, 200,200);

            //Assert   
            Assert.AreEqual(200, testReturned.Height);
        }

        [TestMethod]
        public void DadoNome_Error1_EmFormatoDaAI_RetornaDefault()
        {
            //arrange
            string classDummy = "Error1";

            //Act - método sob teste
            var testReturned = DataHandlerUseCases.ConvertIAToDefault(classDummy);

            //Assert   
            Assert.AreEqual("type1", testReturned);
        }

        [TestMethod]
        public void DadoNome_Error2_EmFormatoDaAI_RetornaDefault()
        {
            //arrange
            string classDummy = "Error2";

            //Act - método sob teste
            var testReturned = DataHandlerUseCases.ConvertIAToDefault(classDummy);

            //Assert   
            Assert.AreEqual("type2", testReturned);
        }

        [TestMethod]
        public void DadoNome_Error3_EmFormatoDaAI_RetornaDefault()
        {
            //arrange
            string classDummy = "Error3";

            //Act - método sob teste
            var testReturned = DataHandlerUseCases.ConvertIAToDefault(classDummy);

            //Assert   
            Assert.AreEqual("type3", testReturned);
        }

        [TestMethod]
        public void DadoNome_Ok_EmFormatoDaAI_RetornaDefault()
        {
            //arrange
            string classDummy = "Ok";

            //Act - método sob teste
            var testReturned = DataHandlerUseCases.ConvertIAToDefault(classDummy);

            //Assert   
            Assert.AreEqual("none", testReturned);
        }

        [TestMethod]
        public void DadoNome_Undefined_EmFormatoDaAI_RetornaDefault()
        {
            //arrange
            string classDummy = "Undefined";

            //Act - método sob teste
            var testReturned = DataHandlerUseCases.ConvertIAToDefault(classDummy);

            //Assert   
            Assert.AreEqual("discard", testReturned);
        }

        [TestMethod]
        public void DadoNome_Qualquer_EmFormatoDaAI_RetornaIgual()
        {
            //arrange
            string classDummy = "dummyValue";

            //Act - método sob teste
            var testReturned = DataHandlerUseCases.ConvertIAToDefault(classDummy);

            //Assert   
            Assert.AreEqual("dummyValue", testReturned);
        }

        [TestMethod]
        public void DadoNome_Error1_EmFormatoDefault_RetornaAI()
        {
            //arrange
            string classDummy = "type1";

            //Act - método sob teste
            var testReturned = DataHandlerUseCases.ConvertDefaultToIA(classDummy);

            //Assert   
            Assert.AreEqual("Error1", testReturned);
        }

        [TestMethod]
        public void DadoNome_Error2_EmFormatoDefault_RetornaAI()
        {
            //arrange
            string classDummy = "type2";

            //Act - método sob teste
            var testReturned = DataHandlerUseCases.ConvertDefaultToIA(classDummy);

            //Assert   
            Assert.AreEqual("Error2", testReturned);
        }

        [TestMethod]
        public void DadoNome_Error3_EmFormatoDefault_RetornaAI()
        {
            //arrange
            string classDummy = "type3";

            //Act - método sob teste
            var testReturned = DataHandlerUseCases.ConvertDefaultToIA(classDummy);

            //Assert   
            Assert.AreEqual("Error3", testReturned);
        }

        [TestMethod]
        public void DadoNome_Ok_EmFormatoDefault_RetornaAI()
        {
            //arrange
            string classDummy = "none";

            //Act - método sob teste
            var testReturned = DataHandlerUseCases.ConvertDefaultToIA(classDummy);

            //Assert   
            Assert.AreEqual("Ok", testReturned);
        }

        [TestMethod]
        public void DadoNome_Undefined_EmFormatoDefault_RetornaAI()
        {
            //arrange
            string classDummy = "discard";

            //Act - método sob teste
            var testReturned = DataHandlerUseCases.ConvertDefaultToIA(classDummy);

            //Assert   
            Assert.AreEqual("Undefined", testReturned);
        }

        [TestMethod]
        public void DadoNome_Qualquer_EmFormatoDefault_RetornaAI()
        {
            //arrange
            string classDummy = "dummyValue";

            //Act - método sob teste
            var testReturned = DataHandlerUseCases.ConvertDefaultToIA(classDummy);

            //Assert   
            Assert.AreEqual("dummyValue", testReturned);
        }
    }
}
