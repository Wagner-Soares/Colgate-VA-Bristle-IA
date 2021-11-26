using System;
using System.Collections.Generic;
using System.Text;

namespace APIVision.DataModels
{
    /// <summary>
    /// Data model of a new bounding box  
    /// </summary>
    public class Box
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }
}
