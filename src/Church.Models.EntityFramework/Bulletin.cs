//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Church.Models.EntityFramework
{
    using System;
    using System.Collections.Generic;
    
    public partial class Bulletin
    {
        public System.Guid Id { get; set; }
        public System.DateTime Date { get; set; }
        public string PlainText { get; set; }
        public string FileUrl { get; set; }
        public string Culture { get; set; }
    }
}