using Database;
using System;
using System.Collections.Generic;

namespace APIVision.DataModels
{
    /// <summary>
    /// Brush scan data model 
    /// </summary>
    public class AnalyzeSetModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int ISKU_id { get; set; }
        public DateTime Timestamp { get; set; }
        public string Equipament { get; set; }

        public virtual ICollection<BristleAnalysisResultSet> BristleAnalysisResultSets { get; set; }
        public virtual ICollection<BrushAnalysisResultSet> BrushAnalysisResultSets { get; set; }
        public virtual ICollection<TuffAnalysisResultSet> TuffAnalysisResultSets { get; set; }
    }
}
