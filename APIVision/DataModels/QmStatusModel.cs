using System;

namespace APIVision.DataModels
{
    /// <summary>
    /// Sample Status Record
    /// </summary>
    public class QmStatusModel
    {
        public int Id { get; set; }
        public int IStatus_id { get; set; }
        public string SDescription { get; set; }
        public string SCreated_by { get; set; }
        public DateTime DtCreated_at { get; set; }
    }
}
