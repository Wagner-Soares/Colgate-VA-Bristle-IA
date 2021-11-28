using APIVision.Controllers.DataBaseControllers;
using APIVision;
using Database;
using System;
using Xunit;

namespace Bristle.Testes
{
    
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            //arrange
            var dbContext = new ColgateSkeltaEntities("Data Source=10.140.1.230;Initial Catalog=ColgateSkelta;Persist Security Info=True;User ID=sa;Password=C0lg@teBRAV;multipleactiveresultsets=True;application name=EntityFramework");


            var controller = new SkuController(dbContext);

            //Act
            var teste = controller.ListSKUsModel();

            //Assert
            Assert.NotNull(teste);
        }
    }
}
