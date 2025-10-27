using DocumentManager.Domain;
using Microsoft.EntityFrameworkCore;

namespace DocumentManager.Data
{
    public class DocumentDbContext : DbContext
    {
        public DocumentDbContext(DbContextOptions<DocumentDbContext> options)
            : base(options)
        {
        }

        public DbSet<Document> Documents { get; set; }
        public DbSet<DocumentItems> DocumentItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Document>()
                .HasMany(d => d.Items)
                .WithOne(i => i.Document)
                .HasForeignKey(i => i.DocumentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DocumentItems>()
                .Property(d => d.Price)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<DocumentItems>()
                .Property(d => d.Quantity)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<DocumentItems>()
                .Property(d => d.TaxRate)
                .HasColumnType("decimal(5,2)");
        }
    }
}
