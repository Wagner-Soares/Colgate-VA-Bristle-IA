using System;
using System.Collections.Generic;
using System.Text;

namespace APIVision.Models
{
    /// <summary>
    /// Record of tests performed
    /// </summary>
    public class TestModel
    {
        public int Id { get; set; }
        public string sDescription { get; set; }
        public Nullable<int> iSKU { get; set; }
        public string sCreated_by { get; set; }
        public System.DateTime dtCreated_at { get; set; }
    }
}
