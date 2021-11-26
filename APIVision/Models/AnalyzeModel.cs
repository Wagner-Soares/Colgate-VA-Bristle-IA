using Database;
using System;
using System.Collections.Generic;
using System.Text;

namespace APIVision.Models
{
    /// <summary>
    /// Brush scan data model 
    /// </summary>
    public class AnalyzeModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int iSKU_id { get; set; }
        public System.DateTime Timestamp { get; set; }
        public string Equipament { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BristleAnalysisResultSet> BristleAnalysisResultSets { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BrushAnalysisResultSet> BrushAnalysisResultSets { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TuffAnalysisResultSet> TuffAnalysisResultSets { get; set; }
    }
}
