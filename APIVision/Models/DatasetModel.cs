using Database;
using System;
using System.Collections.Generic;
using System.Text;

namespace APIVision.Models
{
    public class DatasetModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Historic { get; set; }
        public string Type { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Sample_ASet> Sample_ASet { get; set; }
    }
}
