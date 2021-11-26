using System;
using System.Collections.Generic;
using System.Text;

namespace APIVision.Models
{
    public class EquipmentModel
    {
        public int iID { get; set; }
        public string iEquipment_id { get; set; }
        public string sDescription { get; set; }
        public string iArea_id { get; set; }
        public string sCreated_by { get; set; }
        public System.DateTime dtCreated_at { get; set; }
    }
}
