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
    public class QmSpecController
    {
        private readonly IQMSpecDao<QM_Spec> _qm_Spec;

        public QmSpecController(ColgateSkeltaEntities colgateSkeltaEntities)
        {
            _qm_Spec = new QmSpecDao<QM_Spec>(colgateSkeltaEntities);
        }

        public TestSpecificationModel GetQM_SpecByTestId(int testId)
        {
            TestSpecificationModel testSpecificationModel = new TestSpecificationModel();

            try
            {
                var testSpecification = (from n in _qm_Spec.GetAllQM_Specs()
                                select new QmSpecModel
                                {
                                    IID = n.iID,
                                    ITest_id = n.iTest_id,
                                    FTarget = n.fTarget,
                                    FAccept_UpperLimit = n.fSpec_UpperLimit,
                                    FAccept_LowerLimit = n.fSpec_LowerLimit

                                }).FirstOrDefault(testSpec => testSpec.ITest_id == testId);

                testSpecificationModel.TestId = testId;
                testSpecificationModel.TestTarget = testSpecification.FTarget;
                testSpecificationModel.TestSpecUpperLimit = testSpecification.FAccept_UpperLimit;
                testSpecificationModel.TestSpecLowerLimit = testSpecification.FAccept_LowerLimit;

                if (testSpecificationModel != null)
                    return testSpecificationModel;
                else
                    return testSpecificationModel;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }
    }
}
