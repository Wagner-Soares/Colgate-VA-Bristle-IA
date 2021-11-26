using System;
using System.Collections.Generic;
using System.Text;

namespace APIVision.Models
{
    /// <summary>
    /// Data model of a bristle 
    /// </summary>
    public class BristleModel
{
        public int Id { get; set; }
        public string Classification { get; set; }
        public string Name { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public double Probability { get; set; }
        public virtual DatasetModel Dataset { get; set; }

        public int DatasetId { get; set; }
    }
}
