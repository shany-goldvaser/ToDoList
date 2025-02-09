using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using TodoApi;

var builder = WebApplication.CreateBuilder(args);

// Set up the in-memory database
// builder.Services.AddDbContext<ToDoDbContext>(options =>
//     options.UseMySql(builder.Configuration.GetConnectionString("tododb"), 
//     new MySqlServerVersion(new Version(8, 0, 33))));
builder.Services.AddDbContext<ToDoDbContext>(options =>
    options.UseMySql("name=tododb",ServerVersion.Parse("8.0.33-mysql")
    ));
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder => builder
            .AllowAnyOrigin() 
            .AllowAnyMethod() 
            .AllowAnyHeader()); 
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
});
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseCors("AllowAllOrigins");

app.UseSwagger();

app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
    c.RoutePrefix = string.Empty; 
});

app.UseAuthorization();
app.MapControllers();
app.MapGet("/items", async (ToDoDbContext context) =>
    await context.Items.ToListAsync());

app.MapPost("/items", async (ToDoDbContext context, Item item) =>
{
    context.Items.Add(item);
    await context.SaveChangesAsync();
    return Results.Created($"/items/{item.Id}", item);
});

app.MapPut("/items/{id}", async (ToDoDbContext context, int id, Item updatedItem) =>
{
    var item = await context.Items.FindAsync(id);
    if (item is null) return Results.NotFound();

    item.Name = item.Name;
    item.IsComplete = updatedItem.IsComplete;
    await context.SaveChangesAsync();
    return Results.NoContent();
});

app.MapDelete("/items/{id}", async (ToDoDbContext context, int id) =>
{
    var item = await context.Items.FindAsync(id);
    if (item is null) return Results.NotFound();

    context.Items.Remove(item);
    await context.SaveChangesAsync();
    return Results.NoContent();
});

app.Run();
