//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Database
{
    using System;
    using System.Collections.Generic;
    
    public partial class Audit_Trail
    {
        public int iID { get; set; }
        public string sAction { get; set; }
        public int iOld_value { get; set; }
        public int iNew_value { get; set; }
        public Nullable<int> spare1 { get; set; }
        public Nullable<int> spare2 { get; set; }
        public Nullable<int> spare3 { get; set; }
        public string spare4 { get; set; }
        public string spare5 { get; set; }
        public string sModified_by { get; set; }
        public string dtModified_at { get; set; }
    }
}
