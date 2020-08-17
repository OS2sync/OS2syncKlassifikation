using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using StsKlassifikation.Model;
using System;

namespace StsKlassifikation.DBContext
{
    public class ClassificationContext : DbContext
    {
        public ClassificationContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Klassifikation> Klassifikation { get; set; }
        public DbSet<Facet> Facet { get; set; }
        public DbSet<KlasseRedaktoer> KlasseRedaktoer { get; set; }
        public DbSet<KlasseLovligeKombinationer> KlasseLovligeKombination { get; set; }
        public DbSet<KlasseTilfoejelse> KlasseTilfoejelse { get; set; }
        public DbSet<KlasseSideordnet> KlasseSideordnet { get; set; }
        public DbSet<KlasseErstatter> KlasseErstatter { get; set; }
        public DbSet<Klasse> Klasse { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.ConfigureWarnings(c => c.Log((RelationalEventId.CommandExecuting, LogLevel.Debug)));

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Klassifikation

            modelBuilder.Entity<Klassifikation>()
                .HasKey(c => c.UUID);

            modelBuilder.Entity<Klassifikation>()
                .HasMany(c => c.Facetter)
                .WithOne(f => f.Klassifikation)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Klassifikation>()
                .Property(c => c.Livscykluskode)
                .HasConversion(
                    v => v.ToString(),
                    v => (KlassifikationSystem.LivscyklusKodeType) Enum.Parse(typeof(KlassifikationSystem.LivscyklusKodeType), v));

            // Facet

            modelBuilder.Entity<Facet>()
                .HasKey(f => f.UUID);

            modelBuilder.Entity<Facet>()
                .HasMany(f => f.Redaktoerer);

            modelBuilder.Entity<Facet>()
                .Property(c => c.Livscykluskode)
                .HasConversion(
                    v => v.ToString(),
                    v => (FacetWebService.LivscyklusKodeType)Enum.Parse(typeof(FacetWebService.LivscyklusKodeType), v));

            // Klasse

            modelBuilder.Entity<Klasse>()
                .HasKey(k => k.UUID);

            modelBuilder.Entity<Klasse>()
                .HasMany(k => k.Redaktoerer).WithOne(r => r.Klasse);

            modelBuilder.Entity<Klasse>()
                .HasMany(k => k.Erstatter).WithOne(r => r.Klasse);

            modelBuilder.Entity<Klasse>()
                .HasMany(k => k.Sideordnet).WithOne(r => r.Klasse);

            modelBuilder.Entity<Klasse>()
                .HasMany(k => k.Tilfoejelser).WithOne(r => r.Klasse);

            modelBuilder.Entity<Klasse>()
                .HasMany(k => k.LovligeKombinationer).WithOne(r => r.Klasse);

            modelBuilder.Entity<Klasse>()
                .Property(c => c.Livscykluskode)
                .HasConversion(
                    v => v.ToString(),
                    v => (KlasseWebService.LivscyklusKodeType)Enum.Parse(typeof(KlasseWebService.LivscyklusKodeType), v));

            modelBuilder.Entity<KlasseRedaktoer>()
                .HasKey(r => r.Id);
            modelBuilder.Entity<KlasseLovligeKombinationer>()
                .HasKey(r => r.Id);
            modelBuilder.Entity<KlasseTilfoejelse>()
                .HasKey(r => r.Id);
            modelBuilder.Entity<KlasseSideordnet>()
                .HasKey(r => r.Id);
            modelBuilder.Entity<KlasseErstatter>()
                .HasKey(r => r.Id);
        }
    }
}
