using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
    };

    if(productRequest.Tags != null)
    {
        product.Tags = new List<Tag>();
        foreach (var item in productRequest.Tags)
        {
            product.Tags.Add(new Tag { Name = item });            
        }
    }

    context.Products.Add(product);
    context.SaveChanges();

    return Results.Created($"/products/{product.Id}", product.Id);
});

app.MapGet("/products/{id}", ([FromRoute] int id, ApplicationDbContext context) => {

    var savedProduct = context
        .Products
        .Include(x => x.Category)
        .Include(x => x.Tags)
        .FirstOrDefault(x => x.Id == id);

    if (savedProduct != null) 
    {
        return Results.Ok(savedProduct);
    }
    return Results.NotFound();
});

app.MapPut("/products/{id}", ([FromRoute] int id, ProductRequest productRequest, ApplicationDbContext context) => {
    var savedProduct = context
        .Products
        .Include(x => x.Tags)
        .FirstOrDefault(x => x.Id == id);

    var category = context.Categories.FirstOrDefault(x => x.Id == productRequest.CategoryId);

    savedProduct.Code = productRequest.Code;
    savedProduct.Name = productRequest.Name;
    savedProduct.Description = productRequest.Description;
    savedProduct.Category = category;

    if (productRequest.Tags != null)
    {
        savedProduct.Tags = new List<Tag>();
        foreach (var item in productRequest.Tags)
        {
            savedProduct.Tags.Add(new Tag { Name = item });
        }
    }

    context.SaveChanges();
    return Results.Ok();
});

app.MapDelete("/products/{id}", ([FromRoute] int id, ApplicationDbContext context) => {
    var savedProduct = context
        .Products
        .Include(x => x.Tags)
        .FirstOrDefault(x => x.Id == id);

    context.Remove(savedProduct);
    context.SaveChanges();
    return Results.Ok();
});

if (app.Environment.IsStaging()) 
{
    app.MapGet("/configuration/database", (IConfiguration configuration) => {
        return Results.Ok($"{configuration["database:connection"]}/{configuration["database:port"]}");
    });
}

app.Run();
