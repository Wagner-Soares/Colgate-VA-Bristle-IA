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
    
    public partial class CheckList
    {
        public int iCheckList { get; set; }
        public string sDescription { get; set; }
        public Nullable<int> iSKU_id { get; set; }
        public Nullable<int> iTest_id { get; set; }
        public string sCreated_by { get; set; }
        public Nullable<System.DateTime> dtCreated_at { get; set; }
    }
}