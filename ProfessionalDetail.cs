//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Demo
{
    using System;
    using System.Collections.Generic;
    
    public partial class ProfessionalDetail
    {
        public int Id { get; set; }
        public Nullable<int> Year { get; set; }
        public Nullable<int> Month { get; set; }
        public string SkillIds { get; set; }
        public int DetailId { get; set; }
        public Nullable<int> FileId { get; set; }
    
        public virtual Detail Detail { get; set; }
        public virtual File File { get; set; }
    }
}
