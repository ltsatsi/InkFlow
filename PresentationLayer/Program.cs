using DomainLayer.Data;
using DomainLayer.Models;
using DotNetEnv;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RepositoryLayer.IRepository;
using RepositoryLayer.Repository;
using ServiceLayer.IService;
using ServiceLayer.Service;
using Supabase;
using System.Diagnostics;

namespace PresentationLayer
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Env.Load();
            var builder = WebApplication.CreateBuilder(args);

            #region Environment Variables
            builder.Configuration["Supabase:Url"] = Environment.GetEnvironmentVariable("URL");
            builder.Configuration["Supabase:AnonKey"] = Environment.GetEnvironmentVariable("ANON_KEY");
            builder.Configuration["ConnectionStrings:DefaultConnection"] = Environment.GetEnvironmentVariable("CONNECTION_STRING");
            #endregion

            // Supabase
            var url = builder.Configuration["Supabase:Url"]!;
            var key = builder.Configuration["Supabase:AnonKey"]!;

            var options = new SupabaseOptions { AutoConnectRealtime = false }; 
            var supabase = new Client(url, key, options);

            await supabase.InitializeAsync();
            builder.Services.AddSingleton(supabase);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            // DB
            //builder.Services.AddDbContext<AppDbContext>(options =>
            //options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

            // DI
            builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            builder.Services.AddScoped<ICustomService<Blog>, BlogService>();
            builder.Services.AddScoped<ICustomService<Comment>, CommentService>();
            builder.Services.AddScoped<IImageService, ImageService>();
            builder.Services.AddScoped<IStorageService, SupabaseStorageService>();

            // Auth
            builder.Services.AddIdentity<AppUser, AppRole>(options =>
            {

                options.SignIn.RequireConfirmedEmail = false;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 8;
            }).AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Account/SignIn";
            });

            var app = builder.Build();

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

            //app.MapControllerRoute(
            //    name: "default",
            //    pattern: "{controller=Home}/{action=Index}/{id?}");

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Blog}/{action=Index}/{id?}");

            try
            {
                using IServiceScope scope = app.Services.CreateScope();

                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<AppRole>>();
                var roles = new string[2] { "User", "Admin" };
                var userRoleDesc = "User role has limited permissions to manage their own posts.";
                var adminRoleDesc = "Admin role has all the permissions to manage the system.";

                foreach (string role in roles)
                {
                    if (!await roleManager.RoleExistsAsync(role))
                    {
                        await roleManager.CreateAsync(new AppRole
                        {
                            Name = role,
                            IsActive = true,
                            Description = role == "Admin" ? adminRoleDesc : userRoleDesc,
                        });
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception($"An error occurred: {e.Message}");
            }

            try
            {

                using (var scope = app.Services.CreateScope())
                {
                    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();

                    string email = "admin@inkflow.com";

                    var user = await userManager.FindByEmailAsync(email);

                    if (user == null)
                    {
                        user = new AppUser
                        {
                            FullName = "System Administrator",
                            RoleName = "Admin",

                            UserName = email,
                            Email = email,
                            EmailConfirmed = true,

                            CreatedOn = DateTimeOffset.UtcNow
                        };

                        await userManager.CreateAsync(user, "AdminPassword#1");
                        await userManager.AddToRoleAsync(user, "Admin");
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception($"An error occurred: {e.Message}");
            }

            await app.RunAsync();
        }
    }
}
