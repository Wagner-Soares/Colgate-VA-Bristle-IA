using System;
using System.Collections.Generic;
using System.Text;

namespace APIVision.DataModels
{
    public class TestSpecificationModel
    {
        public int TestId { get; set; }
        public double TestTarget { get; set; }
        public double TestSpecLowerLimit { get; set; }
        public double TestSpecUpperLimit { get; set; }
    }
}
