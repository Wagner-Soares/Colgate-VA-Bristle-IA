using System;
using System.Collections.Generic;
using System.Text;

namespace APIVision.DataModels
{
    public class TestSpecificationModel
    {
        public int TestId { get; set; } = -1;
        public double TestTarget { get; set; } = 1;
        public double TestSpecLowerLimit { get; set; } = 0;
        public double TestSpecUpperLimit { get; set; } = 2;
    }
}
