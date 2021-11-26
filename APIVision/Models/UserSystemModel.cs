using System;
using System.Collections.Generic;
using System.Text;

namespace APIVision.Models
{
    public class UserSystemModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Salt { get; set; }
        public string Key { get; set; }
        public string Type { get; set; }

        //public virtual GeneralSystemSettings GeneralSystemSettings { get; set; }

        public int GeneralSystemSettingsId { get; set; }
    }
}
