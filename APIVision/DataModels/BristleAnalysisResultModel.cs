using Database;
using System;
using System.Collections.Generic;
using System.Text;

namespace APIVision.DataModels
{
    /// <summary>
    /// Brush scan result data model 
    /// </summary>
    public class BristleAnalysisResultModel
    {
        public int Id { get; set; }
        public string DefectClassification { get; set; }
        public string DefectIdentified { get; set; }
        public int Probability { get; set; }
        public bool SelectedManual { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string Position { get; set; }

        public virtual AnalyzeSet AnalyzeSet { get; set; }
    }
}
