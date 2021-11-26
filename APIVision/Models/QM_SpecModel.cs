using System;
using System.Collections.Generic;
using System.Text;

namespace APIVision.Models
{
    /// <summary>
    /// Sample Status Record
    /// </summary>
    public partial class QM_SpecModel
    {
        public int iID { get; set; }
        public int iTest_id { get; set; }
        public double fSpec_UpperLimit { get; set; }
        public double fSpec_LowerLimit { get; set; }
        public double fControl_UpperLimit { get; set; }
        public double fControl_LowerLimit { get; set; }
        public double fAccept_UpperLimit { get; set; }
        public double fAccept_LowerLimit { get; set; }
        public double fTarget { get; set; }
        public string sSpare { get; set; }
        public string sCreated_by { get; set; }
        public System.DateTime dtCreated_at { get; set; }
    }
}
