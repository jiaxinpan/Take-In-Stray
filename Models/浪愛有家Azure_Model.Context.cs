﻿//------------------------------------------------------------------------------
// <auto-generated>
//     這個程式碼是由範本產生。
//
//     對這個檔案進行手動變更可能導致您的應用程式產生未預期的行為。
//     如果重新產生程式碼，將會覆寫對這個檔案的手動變更。
// </auto-generated>
//------------------------------------------------------------------------------

namespace 浪愛有家.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class 浪愛有家Azure_Entities : DbContext
    {
        public 浪愛有家Azure_Entities()
            : base("name=浪愛有家Azure_Entities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Adoption> Adoption { get; set; }
        public virtual DbSet<City> City { get; set; }
        public virtual DbSet<Country> Country { get; set; }
        public virtual DbSet<Manager> Manager { get; set; }
        public virtual DbSet<Member> Member { get; set; }
        public virtual DbSet<Missing> Missing { get; set; }
        public virtual DbSet<Rescue> Rescue { get; set; }
    }
}
