using Microsoft.EntityFrameworkCore;
using WorkRequestService.Dtos;
using WorkRequestService.Endpoints;
using WorkRequestService.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "myAllowSpecificOrigins",
                      policy =>
                      {
                          policy.WithOrigins("https://localhost:5173",
                                              "http://localhost:5173")
                                .AllowAnyHeader()
                                .AllowAnyMethod();
                      });
});

builder.Services.AddDbContext<WorkRequestDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("WorkRequestDb") ?? "Data Source=workrequests.db"));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<WorkRequestDbContext>();
    db.Database.EnsureCreated();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors("myAllowSpecificOrigins");

app.UseHttpsRedirection();

// Map API endpoints
app.MapWorkRequestEndpoints();

app.Run();
