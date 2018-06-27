﻿using RudycommerceData.Entities;
using RudycommerceData.Entities.DesktopUsers;
using RudycommerceData.Entities.Products.Categories;
using RudycommerceData.Entities.Products.Products;
using RudycommerceData.Entities.Products.Specifications;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RudycommerceData.Data
{
    public class RudyDbContext : DbContext
    {
        #region dbSets

        #region Products

        #region Products

        public DbSet<Product> Products { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }

        #endregion

        #region Categories

        public DbSet<Category> Categories { get; set; }
        public DbSet<CategorySpecification> CategorySpecifications { get; set; }
        public DbSet<LocalizedCategory> LocalizedCategories { get; set; }

        #endregion

        #region Specifications

        public DbSet<Specification> Specifications { get; set; }
        public DbSet<SpecificationEnum> SpecificationEnums { get; set; }
        public DbSet<LocalizedEnumValue> LocalizedEnumValues { get; set; }

        #endregion

        #endregion

        public DbSet<Language> Languages { get; set; }

        public DbSet<DesktopUser> DesktopUsers { get; set; }

        #endregion

        public RudyDbContext() : base("name=CProefCS")
        {
            
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            #region Categories (many) to (many) Languages
            modelBuilder.Entity<LocalizedCategory>()
                .HasKey(lpc => new { lpc.CategoryID, lpc.LanguageID });

            modelBuilder.Entity<Category>()
                .HasMany(c => c.LocalizedCategories)
                .WithRequired()
                .HasForeignKey(lpc => lpc.CategoryID);

            modelBuilder.Entity<Language>()
                .HasMany(l => l.LocalizedCategories)
                .WithRequired()
                .HasForeignKey(lpc => lpc.LanguageID);
            #endregion

            #region Categories to Specifications

            modelBuilder.Entity<CategorySpecification>()
                .HasKey(cs => new { cs.CategoryID, cs.SpecificationID });

            modelBuilder.Entity<Category>()
                .HasMany(c => c.CategorySpecifications)
                .WithRequired()
                .HasForeignKey(cs => cs.CategoryID);

            modelBuilder.Entity<Specification>()
                .HasMany(s => s.CategorySpecifications)
                .WithRequired()
                .HasForeignKey(cs => cs.SpecificationID);

            #endregion

            #region SpecificationEnum to Languages

            modelBuilder.Entity<LocalizedEnumValue>()
                .HasKey(lev => new { lev.EnumerationID, lev.LanguageID });

            modelBuilder.Entity<SpecificationEnum>()
                .HasMany(e => e.LocalizedEnumValues)
                .WithRequired()
                .HasForeignKey(lev => lev.EnumerationID);

            modelBuilder.Entity<Language>()
                .HasMany(l => l.LocalizedEnumValues)
                .WithRequired()
                .HasForeignKey(lev => lev.LanguageID);

            #endregion

            #region Products to Languages

            modelBuilder.Entity<LocalizedProduct>()
                .HasKey(lp => new { lp.ProductID, lp.LanguageID });

            modelBuilder.Entity<Product>()
                .HasMany(p => p.LocalizedProducts)
                .WithRequired()
                .HasForeignKey(lp => lp.ProductID);

            modelBuilder.Entity<Language>()
                .HasMany(l => l.LocalizedProducts)
                .WithRequired()
                .HasForeignKey(lp => lp.LanguageID);

            #endregion
        }
    }
}
