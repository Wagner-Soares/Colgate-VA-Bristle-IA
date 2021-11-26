using Database;
using System;
using System.Collections.Generic;
using System.Text;

namespace APIVision.Models
{
    public class TuffAnalysisResultModel
    {
        public int Id { get; set; }
        public string Position { get; set; }
        public int TotalBristlesFoundNN { get; set; }
        public int TotalBristleFoundManual { get; set; }
        public bool SelectedManual { get; set; }
        public string Probability { get; set; }

        public virtual AnalyzeSet AnalyzeSet { get; set; }
    }
}
