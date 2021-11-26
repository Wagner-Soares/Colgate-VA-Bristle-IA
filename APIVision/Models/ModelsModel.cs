using Database;
using System;
using System.Collections.Generic;
using System.Text;

namespace APIVision.Models
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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Datasets> Datasets { get; set; }
    }
}
