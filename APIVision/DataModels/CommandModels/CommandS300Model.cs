using System;
using System.Collections.Generic;
using System.Text;

namespace APIVision.DataModels.CommandModels
{
    /// <summary>
    /// Data model of theCommand S300: To change the current model 
    /// </summary>
    public class CommandS300Model
    {
        public int NewModelId { get; set; }

        public int OldModelId { get; set; }
    }
}
