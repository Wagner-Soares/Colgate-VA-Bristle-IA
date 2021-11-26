using System;
using System.Collections.Generic;
using System.Text;

namespace APIVision.DataModels
{
    public class Datasets
    {
        public int Id { get; set; }
        public int Dataset_id { get; set; }
        public virtual Database.Models Models { get; set; }
    }
}
