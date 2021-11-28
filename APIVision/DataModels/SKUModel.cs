using System;

namespace APIVision.DataModels
{
    /// <summary>
    /// Skelta
    /// </summary>
    public class SkuModel
    {
        public int IID { get; set; }
        public string SSKU { get; set; }
        public string SDescription { get; set; }
        public string IArea_id { get; set; }
        public string SCreated_by { get; set; }
        public DateTime DtCreated_at { get; set; }
    }
}
