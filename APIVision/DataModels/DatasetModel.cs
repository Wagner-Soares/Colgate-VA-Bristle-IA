using Database;
using System.Collections.Generic;

namespace APIVision.DataModels
{
    public class DatasetModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Historic { get; set; }
        public string Type { get; set; }

        public virtual ICollection<Sample_ASet> Sample_ASet { get; set; }
    }
}
