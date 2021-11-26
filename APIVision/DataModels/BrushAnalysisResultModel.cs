using Database;
using System;
using System.Collections.Generic;
using System.Text;

namespace APIVision.DataModels
{
    public class BrushAnalysisResultModel
    {
        public int Id { get; set; }
        public int TotalBristles { get; set; }
        public int TotalGoodBristles { get; set; }
        public int TotalBristlesAnalyzed { get; set; }
        public string AnalysisResult { get; set; }
        public bool Hybrid { get; set; }
        public int Signaling_Id { get; set; }

        public virtual AnalyzeSet AnalyzeSet { get; set; }
    }
}
