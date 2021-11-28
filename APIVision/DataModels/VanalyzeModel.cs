using Database;
using System.Collections.Generic;

namespace APIVision.DataModels
{
    public class VanalyzeModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int SKU_Id { get; set; }

        public virtual ValidationDataset ValidationDataset { get; set; }
        public virtual ICollection<VtuftSet> VtuftSets { get; set; }
    }
}
