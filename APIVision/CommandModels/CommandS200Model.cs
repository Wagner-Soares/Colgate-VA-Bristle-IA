using System;
using System.Collections.Generic;
using System.Text;

namespace APIVision.CommandModels
{
    /// <summary>
    /// Data model of the Command S200: Command for retraining.
    /// </summary>
    public class CommandS200Model
    {
        public int[] TrainDatasetId { get; set; }

        public int[] ValidationDatasetId { get; set; }
    }
}
