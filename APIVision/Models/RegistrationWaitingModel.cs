using Database;
using System;
using System.Collections.Generic;
using System.Text;

namespace APIVision.Models
{
    public class RegistrationWaitingModel
    {
        public int Id { get; set; }
        public string DataSet_id { get; set; }
        public string Sample_id { get; set; }
        public string AnalyzeSet_id { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TuftTempSetSet> TuftSet1 { get; set; }
    }
}
