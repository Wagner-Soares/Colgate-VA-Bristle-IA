using Database;
using System;
using System.Collections.Generic;
using System.Text;

namespace APIVision.Models
{
    public class ImageTempModel
    {
        public int Id { get; set; }
        public string Path { get; set; }
        public virtual TuftTempSetSet TuftSet { get; set; }
    }
}
