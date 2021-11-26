using System;

namespace APIVision.DataModels
{
    public class EquipmentModel
    {
        public int IID { get; set; }
        public string IEquipment_id { get; set; }
        public string SDescription { get; set; }
        public string IArea_id { get; set; }
        public string SCreated_by { get; set; }
        public DateTime DtCreated_at { get; set; }
    }
}
