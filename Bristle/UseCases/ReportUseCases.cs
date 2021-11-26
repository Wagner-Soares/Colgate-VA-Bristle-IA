using Bristle.utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bristle.UseCases
{
    public static class ReportUseCase
    {
        public static int EvaluateTestAndReturnStatus(double testResult, double testLowerSpec, double testHigherSpec)
        {
            //2 = Passed, 3 = Rejected
            int testStatus;

            if (testResult > testHigherSpec)
            {
                testStatus = ConfigurationConstants.StatusRejectedTest;
            }
            else if (testResult < testLowerSpec)
            {
                testStatus = ConfigurationConstants.StatusRejectedTest;
            }
            else
            {
                testStatus = ConfigurationConstants.StatusPassedTest;
            }
            return testStatus;
        }
    }
}
