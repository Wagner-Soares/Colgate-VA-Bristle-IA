using Database;
using System;
using System.Collections.Generic;
using System.Text;

namespace APIVision.Models
{
    public class TuftTempModel
    {
        public int Id { get; set; }
        public string Position { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BristleTempSetSet> BristleSet { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ImageTempSetSet> ImageSet { get; set; }
        public virtual RegistrationWaitingSet RegistrationWaiting { get; set; }
    }
}
