using System;
using System.Collections.Generic;
using System.Text;

namespace APIVision.Models
{
    /// <summary>
    /// Sample Status Record
    /// </summary>
    public class QM_StatusModel
    {
        public int Id { get; set; }
        public int iStatus_id { get; set; }
        public string sDescription { get; set; }
        public string sCreated_by { get; set; }
        public System.DateTime dtCreated_at { get; set; }
    }
}
