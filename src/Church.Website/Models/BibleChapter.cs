//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Church.Website.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class BibleChapter
    {
        public System.Guid BookId { get; set; }
        public int Order { get; set; }
        public string VerseStrings { get; set; }
    
        public virtual BibleBook BibleBook { get; set; }
    }
}
