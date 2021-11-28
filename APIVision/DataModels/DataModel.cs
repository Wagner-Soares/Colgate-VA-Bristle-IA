using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace APIVision.DataModels
{
    public class DataModel
    {
        public double UserIndex { get; set; }

        public string Name { get; set; }

        public string Info { get; set; }

        public bool TemperatureAlert { get; set; }

        public bool MaskAlert { get; set; }

        public bool CrowdingAlert { get; set; }

        public double TemperatureValue { get; set; }

        public double UserVisible { get; set; }

        public int[] StatusIndice { get; set; } = new int[5];

        public int Answer { get; set; }
    }
}
