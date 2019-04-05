using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace WasherDAL.Models
{
    public partial class washerContext : DbContext
    {
        public washerContext()
        {
        }

        public washerContext(DbContextOptions<washerContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Users> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Data Source =(localdb)\\MSSQLLocalDB;Initial Catalog=washer;Integrated Security=true");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Users>(entity =>
            {
                entity.HasKey(e => e.Userid);

                entity.ToTable("USERS");

                entity.HasIndex(e => e.Useremail)
                    .HasName("const_UK")
                    .IsUnique();

                entity.HasIndex(e => e.Usermobile)
                    .HasName("const_mb_UK")
                    .IsUnique();

                entity.Property(e => e.Userid)
                    .HasColumnName("USERID")
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .ValueGeneratedNever();

                entity.Property(e => e.Latitude)
                    .IsRequired()
                    .HasColumnName("LATITUDE")
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Longitude)
                    .IsRequired()
                    .HasColumnName("LONGITUDE")
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Useremail)
                    .IsRequired()
                    .HasColumnName("USEREMAIL")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Usermobile)
                    .IsRequired()
                    .HasColumnName("USERMOBILE")
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasColumnName("USERNAME")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Userpassword)
                    .IsRequired()
                    .HasColumnName("USERPASSWORD")
                    .HasMaxLength(64);

                entity.Property(e => e.Washing).HasColumnName("WASHING");
            });
        }
    }
}
