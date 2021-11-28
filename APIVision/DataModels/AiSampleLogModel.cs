using System;

namespace APIVision.DataModels
{
    public class AiSampleLogModel
    {
        public int Id { get; set; }
        public int IStatus_id { get; set; }
        public Nullable<bool> BActive { get; set; }
        public int IShift { get; set; }
        public int ITest_id { get; set; }
        public string SEquipament { get; set; }
        public string SArea { get; set; }
        public string SBatchLote { get; set; }
        public DateTime? DtSample { get; set; }
        public double FResult { get; set; }
        public string SOperator { get; set; }
        public DateTime DtPublished_at { get; set; }
        public string SComments { get; set; }
        public string SCreated_by { get; set; }
        public DateTime DtCreated_at { get; set; }
    }
}
