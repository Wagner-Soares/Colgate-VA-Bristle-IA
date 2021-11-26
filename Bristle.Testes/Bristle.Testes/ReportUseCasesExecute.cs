using Bristle.UseCases;
using Bristle.Views;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Bristle.Testes
{
    [TestClass]
    public class ReportUseCasesExecute
    {
        [TestMethod]
        public void DadoValorDoTesteDentroDaEspecificao_RetornaStatusDePassed()
        {
            //arrange

            //Act - método sob teste
            var statusToValidate = ReportUseCase.EvaluateTestAndReturnStatus(30, 20, 40);

            //Assert   
            Assert.AreEqual(2,statusToValidate);
        }

        [TestMethod]
        public void DadoValorDoTesteAcimaDaEspecificao_RetornaStatusDeRejected()
        {
            //arrange

            //Act - método sob teste
            var statusToValidate = ReportUseCase.EvaluateTestAndReturnStatus(30, 20, 25);

            //Assert   
            Assert.AreEqual(3, statusToValidate);
        }

        [TestMethod]
        public void DadoValorDoTesteAbaixoDaEspecificao_RetornaStatusDeRejected()
        {
            //arrange

            //Act - método sob teste
            var statusToValidate = ReportUseCase.EvaluateTestAndReturnStatus(30, 35, 40);

            //Assert   
            Assert.AreEqual(3, statusToValidate);
        }
    }
}
