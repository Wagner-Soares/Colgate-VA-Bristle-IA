using APIVision.DAO;
using APIVision.DataModels;
using APIVision.Interfaces;
using Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace APIVision.Controllers.DataBaseControllers
{
    public class TestController
    {
        private readonly ITestDao<Test> _testRepo;

        public TestController(ColgateSkeltaEntities colgateSkeltaEntities)
        {
            _testRepo = new TestDao<Test>(colgateSkeltaEntities);
        }

        public List<TestModel> ListTestModelBySKUId(int SKU)
        {
            try
            {
                var query = (from n in _testRepo.GetAllTests()
                                select new TestModel
                                {
                                    Id = n.iTest_id,
                                    ISKU = n.iSKU,
                                    SDescription = n.sDescription,
                                    DtCreated_at = n.dtCreated_at,
                                    SCreated_by = n.sCreated_by
                                }).ToList();

                List<TestModel> result = new List<TestModel>(query);
                List<TestModel> resultFilter = new List<TestModel>();

                if (SKU != -1)
                {
                    foreach (var item in result)
                    {
                        if (item.ISKU == SKU)
                        {
                            resultFilter.Add(item);
                        }

                    }

                    return resultFilter;
                }
                else
                {
                    return result;
                }                
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return new List<TestModel>();
            }
        }
    }
}
