using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SocialNetworkOKO.Controllers;
using SocialNetworkOKO.DbContext;
using SocialNetworkOKO.Mapper;
using SocialNetworkOKO.Models;
using SocialNetworkOKO.Repositories;

var builder = WebApplication.CreateBuilder(args);

SQLitePCL.Batteries.Init();

builder.Services.AddControllersWithViews();

string connection = builder.Configuration.GetConnectionString("DefaultConnection")!;

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite(connection))
   .AddUnitOfWork()
   .AddCustomRepository<Friend, FriendsRepository>()
   .AddCustomRepository<Message, MessageRepository>();

builder.Services.AddIdentity<User, IdentityRole>(opts => {
     opts.Password.RequiredLength = 5;
     opts.Password.RequireNonAlphanumeric = false;
     opts.Password.RequireLowercase = false;
     opts.Password.RequireUppercase = false;
     opts.Password.RequireDigit = false;
 }).AddEntityFrameworkStores<ApplicationDbContext>()
   .AddRoles<IdentityRole>();


var mapperConfig = new MapperConfiguration((v) =>
{
    v.AddProfile(new MappingProfile());
});

IMapper mapper = mapperConfig.CreateMapper();

builder.Services.AddSingleton(mapper);

var app = builder.Build();

await InitializeRoles(app.Services);

    app.UseAuthentication();
    app.UseAuthorization();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

async Task InitializeRoles(IServiceProvider serviceProvider)
{
    // Создаем область для получения RoleManager
    using (var scope = serviceProvider.CreateScope())
    {
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        string[] roleNames = { "Admin", "User", "Manager" };

        foreach (var roleName in roleNames)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }
    }
}
