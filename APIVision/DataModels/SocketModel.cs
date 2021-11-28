using System;
using System.Collections.Generic;
using System.Text;

namespace APIVision.DataModels
{
    public class SocketModel
    {                
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string Obj_class { get; set; }
        public double Probability { get; set; }
    }
}
