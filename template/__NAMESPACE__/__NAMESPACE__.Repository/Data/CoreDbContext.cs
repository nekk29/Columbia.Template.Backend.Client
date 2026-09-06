using __NAMESPACE__.Entity;
using Microsoft.EntityFrameworkCore;

namespace __NAMESPACE__.Repository.Data
{
    public class CoreDbContext(DbContextOptions<CoreDbContext> options) : DbContext(options)
    {
        public virtual DbSet<Email> Emails { get; set; } = null!;
        public virtual DbSet<Setting> Settings { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Email>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasIndex(e => new { e.Code, e.Language }, "AK_Emails_Code")
                    .IsUnique();

                entity.Property(e => e.CcEmails).HasMaxLength(1024);

                entity.Property(e => e.Code).HasMaxLength(32);

                entity.Property(e => e.CreationUser)
                    .HasMaxLength(64)
                    .IsUnicode(false);

                entity.Property(e => e.Description).HasMaxLength(256);

                entity.Property(e => e.Language).HasMaxLength(6);

                entity.Property(e => e.Subject).HasMaxLength(256);

                entity.Property(e => e.ToEmails).HasMaxLength(1024);

                entity.Property(e => e.UpdateUser)
                    .HasMaxLength(64)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Setting>(entity =>
            {
                entity.HasKey(e => new { e.Group, e.Code });

                entity.Property(e => e.Group).HasMaxLength(16);

                entity.Property(e => e.Code).HasMaxLength(64);

                entity.Property(e => e.Description).HasMaxLength(255);

                entity.Property(e => e.Value).HasMaxLength(1024);
            });
        }
    }
}
