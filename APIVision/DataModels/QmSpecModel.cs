using System;

namespace APIVision.DataModels
{
    /// <summary>
    /// Sample Status Record
    /// </summary>
    public partial class QmSpecModel
    {
        public int IID { get; set; }
        public int ITest_id { get; set; }
        public double FSpec_UpperLimit { get; set; }
        public double FSpec_LowerLimit { get; set; }
        public double FControl_UpperLimit { get; set; }
        public double FControl_LowerLimit { get; set; }
        public double FAccept_UpperLimit { get; set; }
        public double FAccept_LowerLimit { get; set; }
        public double FTarget { get; set; }
        public string SSpare { get; set; }
        public string SCreated_by { get; set; }
        public DateTime DtCreated_at { get; set; }
    }
}
