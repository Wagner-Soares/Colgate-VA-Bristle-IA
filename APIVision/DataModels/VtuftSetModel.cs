using Database;
using System.Collections.Generic;

namespace APIVision.DataModels
{
    public class VtuftSetModel
    {
        public int Id { get; set; }
        public string Position { get; set; }

        public virtual Vsample_ASet Vsample_ASet { get; set; }
        public virtual ICollection<VbristleSet> VbristleSets { get; set; }
        public virtual ICollection<VimageSet> VimageSets { get; set; }
    }
}
