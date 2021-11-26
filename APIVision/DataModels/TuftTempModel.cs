using Database;
using System.Collections.Generic;

namespace APIVision.DataModels
{
    public class TuftTempModel
    {
        public int Id { get; set; }
        public string Position { get; set; }

        public virtual ICollection<BristleTempSetSet> BristleSet { get; set; }
        public virtual ICollection<ImageTempSetSet> ImageSet { get; set; }
        public virtual RegistrationWaitingSet RegistrationWaiting { get; set; }
    }
}
