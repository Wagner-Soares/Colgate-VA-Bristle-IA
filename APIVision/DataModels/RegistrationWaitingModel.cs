using Database;
using System.Collections.Generic;

namespace APIVision.DataModels
{
    public class RegistrationWaitingModel
    {
        public int Id { get; set; }
        public string DataSet_id { get; set; }
        public string Sample_id { get; set; }
        public string AnalyzeSet_id { get; set; }

        public virtual ICollection<TuftTempSetSet> TuftSet1 { get; set; }
    }
}
