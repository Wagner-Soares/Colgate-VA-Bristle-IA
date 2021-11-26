using System;
using System.Collections.Generic;
using System.Text;

namespace APIVision.Models
{
    public class CameraModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int F { get; set; }
        public int ISO { get; set; }
        public int FPS { get; set; }
    }
}
