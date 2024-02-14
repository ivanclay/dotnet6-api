using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : DbContext 
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options):base(options)
    {
        
    }
    public DbSet<Product> Products { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>()
            .Property(x => x.Description).HasMaxLength(500).IsRequired(false);
        modelBuilder.Entity<Product>()
            .Property(x => x.Name).HasMaxLength(120).IsRequired();
        modelBuilder.Entity<Product>()
            .Property(x => x.Code).HasMaxLength(20).IsRequired(false);
    }

    //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) 
    //    => optionsBuilder.UseSqlServer();
    //"Server=localhost;Database=Products;User Id=sa;Password=@sql2019;MiltipleActiveResultSets=true;Encrypt=YES;TrustServerCertificate=YES"
}
