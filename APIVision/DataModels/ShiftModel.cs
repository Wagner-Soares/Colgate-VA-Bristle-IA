using System;
using System.Collections.Generic;
using System.Text;

namespace APIVision.DataModels
{
    /// <summary>
    /// Shift Record
    /// </summary>
    public class ShiftsModel
    {
        public int Id { get; set; }
        public string Shift_start { get; set; }
        public string Shift_end { get; set; }
    }
}
