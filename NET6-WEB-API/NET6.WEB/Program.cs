using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ApplicationDbContext>();

var app = builder.Build();
var configuration = app.Configuration;
ProductRepository.Init(configuration);

app.MapPost("/products", (Product product) => {
    ProductRepository.Add(product);
    return Results.Created($"/products/{product.Code}", product.Code);
});

app.MapGet("/products/{code}", ([FromRoute] string code) => {
    var savedProduct = ProductRepository.GetBy(code);
    if(savedProduct != null) 
    {
        Console.WriteLine("Product found");
    }
    return savedProduct;
});

app.MapPut("/products", (Product product) => {
    var savedProduct = ProductRepository.GetBy(product.Code);
    savedProduct.Name = product.Name;
    return Results.Ok();
});

app.MapDelete("/products/{code}", ([FromRoute] string code) => {
    var savedProduct = ProductRepository.GetBy(code);
    ProductRepository.Remove(savedProduct);
    return Results.Ok();
});

if (app.Environment.IsStaging()) 
{
    app.MapGet("/configuration/database", (IConfiguration configuration) => {
        return Results.Ok($"{configuration["database:connection"]}/{configuration["database:port"]}");
    });
}

app.Run();

public static class ProductRepository
{
    public static List<Product> Products { get; set; } = new List<Product>();

    public static void Init(IConfiguration configuration) 
    {
        var products = configuration.GetSection("Products").Get<List<Product>>();
        Products = products;
    }

    public static void Add(Product product) { Products.Add(product); }

    public static Product GetBy(string code)
    {
        return Products.FirstOrDefault(x => x.Code == code);
    }

    public static void Remove(Product product)
    {
        Products.Remove(product);
    }
}

public class Product
{
    public int Id { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
}

public class ApplicationDbContext : DbContext 
{
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

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) 
        => optionsBuilder.UseSqlServer("Server=DESKTOP-PKH0UOM\\SQLEXPRESS;Initial Catalog=Products;Trusted_Connection=True; TrustServerCertificate=True;");
    //"Server=localhost;Database=Products;User Id=sa;Password=@sql2019;MiltipleActiveResultSets=true;Encrypt=YES;TrustServerCertificate=YES"
}
