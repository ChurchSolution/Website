﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class FrameworkEntities : DbContext
    {
        public FrameworkEntities()
            : base("name=FrameworkEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Bulletin> Bulletins { get; set; }
        public virtual DbSet<Event> Events { get; set; }
        public virtual DbSet<Hymn> Hymns { get; set; }
        public virtual DbSet<Incident> Incidents { get; set; }
        public virtual DbSet<Material> Materials { get; set; }
        public virtual DbSet<Person> People { get; set; }
        public virtual DbSet<Sermon> Sermons { get; set; }
    }
}
