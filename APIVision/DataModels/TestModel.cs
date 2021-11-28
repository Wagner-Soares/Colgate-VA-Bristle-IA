using System;

namespace APIVision.DataModels
{
    /// <summary>
    /// Record of tests performed
    /// </summary>
    public class TestModel
    {
        public int Id { get; set; }
        public string SDescription { get; set; }
        public int? ISKU { get; set; }
        public string SCreated_by { get; set; }
        public DateTime DtCreated_at { get; set; }
    }
}
