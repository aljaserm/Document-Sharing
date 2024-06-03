using Microsoft.EntityFrameworkCore;
using Infrastructure.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Document> Documents { get; set; }
        public DbSet<SharedDocument> SharedDocuments { get; set; }
        public DbSet<ShareLink> ShareLinks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Document configuration
            modelBuilder.Entity<Document>(entity =>
            {
                entity.HasKey(d => d.Id);
                entity.Property(d => d.Name)
                      .IsRequired()
                      .HasMaxLength(255);
                entity.Property(d => d.FileType)
                      .IsRequired()
                      .HasMaxLength(50);
                entity.Property(d => d.UploadDate)
                      .IsRequired();
                entity.Property(d => d.DownloadCount)
                      .IsRequired();
                entity.Property(d => d.FilePath)
                      .IsRequired();
                entity.Property(d => d.PreviewImage);
            });

            // SharedDocument configuration
            modelBuilder.Entity<SharedDocument>(entity =>
            {
                entity.HasKey(sd => sd.Id);
                entity.Property(sd => sd.Expiration)
                      .IsRequired();
                entity.HasOne(sd => sd.Document)
                      .WithMany()
                      .HasForeignKey(sd => sd.DocumentId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(sd => sd.ShareLink)
                      .WithMany()
                      .HasForeignKey(sd => sd.ShareLinkId)
                      .OnDelete(DeleteBehavior.NoAction); 
            });

            // ShareLink configuration
            modelBuilder.Entity<ShareLink>(entity =>
            {
                entity.HasKey(sl => sl.Id);
                entity.Property(sl => sl.Link)
                      .IsRequired();
                entity.Property(sl => sl.Expiration)
                      .IsRequired();
                entity.HasOne(sl => sl.Document)
                      .WithMany()
                      .HasForeignKey(sl => sl.DocumentId)
                      .OnDelete(DeleteBehavior.Cascade); 
            });
        }
    }
}
