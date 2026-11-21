using AutoMapper;
using GymManagementSystem_BLL;
using GymManagementSystem_DAL;
using GymManagementSystem_DAL.Data;
using GymManagementSystem_DAL.Data.Seed;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services
    .AddGymDataAccess(builder.Configuration)
    .AddGymBusinessServices();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    await using var scope = app.Services.CreateAsyncScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<GymDbContext>();
    await dbContext.Database.MigrateAsync();
    await DatabaseSeeder.SeedAllAsync(dbContext);
    scope.ServiceProvider.GetRequiredService<IMapper>().ConfigurationProvider.AssertConfigurationIsValid();
}

app.UseRouting();
app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
