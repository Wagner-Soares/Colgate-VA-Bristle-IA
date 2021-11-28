using System.Collections.Generic;

namespace APIVision.DataModels
{
    public class ModelsModel
    {
        public int Id { get; set; }       
        public string Status { get; set; }
        public string Path { get; set; }
        public double PerformanceType1 { get; set; }
        public double PerformanceType2 { get; set; }
        public double PerformanceType3 { get; set; }
        public double PerformanceNone { get; set; }
        public double PerformanceLocalization { get; set; }

        public virtual ICollection<Datasets> Datasets { get; set; }
    }
}
