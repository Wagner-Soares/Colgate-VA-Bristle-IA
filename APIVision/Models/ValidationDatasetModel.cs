using Database;
using System;
using System.Collections.Generic;
using System.Text;

namespace APIVision.Models
{
    public class ValidationDatasetModel
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Historic { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Vsample_ASet> Vsample_ASet { get; set; }
    }
}
