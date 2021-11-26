using Database;
using System;
using System.Collections.Generic;
using System.Text;

namespace APIVision.Models
{
    public class VimageSetModel
    {
        public int Id { get; set; }
        public string Path { get; set; }
        public virtual VtuftSet VtuftSet { get; set; }
    }
}
