using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace APIVision.Models
{
    public class DataModel
    {
        public double userIndex { get; set; }

        //public Image frame { get; set; }

        public string name { get; set; }

        public string info { get; set; }

        public bool temperatureAlert { get; set; }

        public bool maskAlert { get; set; }

        public bool crowdingAlert { get; set; }

        public double temperatureValue { get; set; }

        public double userVisible { get; set; }

        public int[] statusIndice { get; set; } = new int[5];

        public int answer { get; set; }
    }
}
