using System;
using System.Collections.Generic;
using System.Text;

namespace APIVision.Models
{
    public class SocketModel
    {                
        public int x { get; set; }
        public int y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string obj_class { get; set; }
        public double probability { get; set; }
    }
}
