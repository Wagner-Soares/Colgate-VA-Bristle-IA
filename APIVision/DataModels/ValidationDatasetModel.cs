using Database;
using System.Collections.Generic;

namespace APIVision.DataModels
{
    public class ValidationDatasetModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Historic { get; set; }

        public virtual ICollection<Vsample_ASet> Vsample_ASet { get; set; }
    }
}
