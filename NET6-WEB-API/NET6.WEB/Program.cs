using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSqlServer<ApplicationDbContext>(builder.Configuration["Database:SqlServerConnection"]);

var app = builder.Build();
var configuration = app.Configuration;
ProductRepository.Init(configuration);

app.MapPost("/products", (ProductRequest productRequest, ApplicationDbContext context) => {

    var category = context.Categories.FirstOrDefault(x => x.Id == productRequest.CategoryId);
    var product = new Product 
    {
        Code = productRequest.Code,
        Name = productRequest.Name,
        Category = category,
        CategoryId = productRequest.CategoryId,
        Description = productRequest.Description,
        //Id = productRequest.Id
        //Tags = productRequest.Tags
    };

    context.Products.Add(product);
    context.SaveChanges();

    return Results.Created($"/products/{product.Id}", product.Id);
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
