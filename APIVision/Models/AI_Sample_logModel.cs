using System;
using System.Collections.Generic;
using System.Text;

namespace APIVision.Models
{
    public class AI_Sample_logModel
    {
        public int Id { get; set; }
        public int iStatus_id { get; set; }
        public Nullable<bool> bActive { get; set; }
        public int iShift { get; set; }
        public int iTest_id { get; set; }
        public string sEquipament { get; set; }
        public string sArea { get; set; }
        public string sBatchLote { get; set; }
        public Nullable<System.DateTime> dtSample { get; set; }
        public double fResult { get; set; }
        public string sOperator { get; set; }
        public System.DateTime dtPublished_at { get; set; }
        public string sComments { get; set; }
        public string sCreated_by { get; set; }
        public System.DateTime dtCreated_at { get; set; }
    }
}
