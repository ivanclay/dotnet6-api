using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapPost("/products", (Product product) => {
    ProductRepository.Add(product);
});

app.MapGet("/products/{code}", ([FromRoute] string code) => {
    var savedProduct = ProductRepository.GetBy(code);
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

app.MapGet("/configuration/database", (IConfiguration configuration) => {
    return Results.Ok($"{configuration["database:connection"]}/{configuration["database:port"]}");
});

app.Run();

public static class ProductRepository
{
    public static List<Product> Products { get; set; } = new List<Product>();

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
    public string Code { get; set; }
    public string Name { get; set; }
}
