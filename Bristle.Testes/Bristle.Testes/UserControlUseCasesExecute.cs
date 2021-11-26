using Bristle.UseCases;
using Bristle.Views;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Bristle.Testes
{
    [TestClass]
    public class UserControlUseCasesExecute
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
    }
}
