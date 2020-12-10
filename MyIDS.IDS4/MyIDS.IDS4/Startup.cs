using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MyIDS.IDS4.Data;
using MyIDS.IDS4.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MyIDS.IDS4
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            string connStr = Configuration.GetConnectionString("AppDb");

            Action<DbContextOptionsBuilder> dbCtx = (ctx => ctx.UseSqlServer(connStr));

            var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

            services.AddDbContext<ApplicationDbContext>(options => 
                options
                .UseSqlServer(connStr)
                .ConfigureWarnings(warnings => warnings.Throw(RelationalEventId.MultipleCollectionIncludeWarning)));

            services.AddIdentity<ApplicationUser, IdentityRole>(options => {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            services.Configure<CookiePolicyOptions>(options =>
            {
                options.HttpOnly = Microsoft.AspNetCore.CookiePolicy.HttpOnlyPolicy.Always;
            });

            services.AddIdentityServer(option =>
                option.UserInteraction.LoginUrl = "~/account/login2"
            )
            // services.AddIdentityServer()
            // .AddTestUsers(Config.IDSConfiguration.TestUsers)
            // .AddAspNetIdentity<ApplicationUser>()
            .AddDeveloperSigningCredential()
            .AddConfigurationStore(o =>
            {
                o.ConfigureDbContext = dbCtx =>
                    dbCtx.UseSqlServer(connStr, sqlOptions => sqlOptions.MigrationsAssembly(migrationsAssembly));
            })
            .AddOperationalStore(o =>
            {
                o.ConfigureDbContext = dbCtx =>
                    dbCtx.UseSqlServer(connStr, sqlOptions => sqlOptions.MigrationsAssembly(migrationsAssembly));
            })
            .AddAspNetIdentity<ApplicationUser>();

            services.AddCors(option => option.AddPolicy("MyPolicy", builder =>
            {
                builder.AllowAnyOrigin();
            }));

            //services.ConfigureApplicationCookie((obj) =>
            //{
            //    obj.LoginPath = "/account/login2";
            //});

            services.ConfigureApplicationCookie(options =>
            {
                options.Events.OnSigningOut += signingOutContext =>
                {
                    signingOutContext.CookieOptions.SameSite = SameSiteMode.Lax;
                    return Task.CompletedTask;
                };
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            SeedIdentityServerData(app);
            SeedIdentityUserData(app).Wait();

            app.UseStaticFiles();
            app.UseRouting();

            app.UseIdentityServer();
            app.UseAuthentication();
            app.UseAuthorization();
            // app.UseCookiePolicy(new CookiePolicyOptions { MinimumSameSitePolicy = SameSiteMode.Lax });

            app.UseEndpoints(endpoints => endpoints.MapDefaultControllerRoute());
        }

        private async Task SeedIdentityUserData(IApplicationBuilder app)
        {
            var scope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope();

            var usermng = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var rolemng = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            bool adminRole = await rolemng.RoleExistsAsync("Admin");
            if (!adminRole)
            {
                var role = new IdentityRole { Name = "Admin" };
                await rolemng.CreateAsync(role);
            }

            bool operatorRole = await rolemng.RoleExistsAsync("Operator");
            if (!operatorRole)
            {
                var role = new IdentityRole { Name = "Operator" };
                await rolemng.CreateAsync(role);
            }

            bool userRole = await rolemng.RoleExistsAsync("User");
            if (!userRole)
            {
                var role = new IdentityRole { Name = "User" };
                await rolemng.CreateAsync(role);
            }

            var users = await usermng.Users.CountAsync();
            if (users > 0) return;

            foreach (var u in Config.IDSConfiguration.TestUsers)
            {
                ApplicationUser user = new ApplicationUser
                {
                    UserName = u.Username,
                    Email = u.Username,
                    EmailConfirmed = true,
                    PhoneNumber = "1234567890",
                    PhoneNumberConfirmed = true
                };
                var result = await usermng.CreateAsync(user, u.Password);
                if (result.Succeeded)
                {
                    var claim = u.Claims.FirstOrDefault(c => c.Type == "role");
                    var r = await usermng.AddToRoleAsync(user, claim.Value);
                }
            }
        }

        private void SeedIdentityServerData(IApplicationBuilder app)
        {
            var scope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope();

            var configDbCtx = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();

            configDbCtx.IdentityResources.RemoveRange(configDbCtx.IdentityResources.ToList());
            foreach (var r in Config.IDSConfiguration.IdentityResources)
            {
                configDbCtx.IdentityResources.Add(r.ToEntity());
            }

            configDbCtx.ApiResources.RemoveRange(configDbCtx.ApiResources.ToList());
            foreach (var r in Config.IDSConfiguration.ApiResources)
            {
                configDbCtx.ApiResources.Add(r.ToEntity());
            }

            configDbCtx.ApiScopes.RemoveRange(configDbCtx.ApiScopes.ToList());
            foreach (var s in Config.IDSConfiguration.ApiScopes)
            {
                configDbCtx.ApiScopes.Add(s.ToEntity());
            }

            configDbCtx.Clients.RemoveRange(configDbCtx.Clients.ToList());
            foreach (var c in Config.IDSConfiguration.Clients)
            {
                configDbCtx.Clients.Add(c.ToEntity());
            }

            configDbCtx.SaveChanges();

            //if (!configDbCtx.IdentityResources.Any())
            //{
            //    foreach (var r in Config.IDSConfiguration.IdentityResources)
            //    {
            //        configDbCtx.IdentityResources.Add(r.ToEntity());
            //    }
            //    configDbCtx.SaveChanges();
            //}

            //if (!configDbCtx.ApiResources.Any())
            //{
            //    foreach (var r in Config.IDSConfiguration.ApiResources)
            //    {
            //        configDbCtx.ApiResources.Add(r.ToEntity());
            //    }
            //    configDbCtx.SaveChanges();
            //}

            //if (!configDbCtx.ApiScopes.Any())
            //{
            //    foreach (var s in Config.IDSConfiguration.ApiScopes)
            //    {
            //        configDbCtx.ApiScopes.Add(s.ToEntity());
            //    }
            //    configDbCtx.SaveChanges();
            //}

            //if (!configDbCtx.Clients.Any())
            //{
            //    foreach (var c in Config.IDSConfiguration.Clients)
            //    {
            //        configDbCtx.Clients.Add(c.ToEntity());
            //    }
            //    configDbCtx.SaveChanges();
            //}
        }
    }
}
