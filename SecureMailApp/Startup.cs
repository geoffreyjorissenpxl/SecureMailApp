using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SecureMailApp.Entities;
using SecureMailApp.Services;

namespace SecureMailApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //Activates authorization on the whole domain
            services.AddControllersWithViews(o => o.Filters.Add(new AuthorizeFilter()));
            services.AddDbContext<SecureMailDbContext>(options =>
            {
                string connection =
                    @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=SecureMailDb;Integrated Security=True;";
                options.UseSqlServer(connection);
            });

            services.AddScoped<IMessageEncryptionService, MessageEncryptionService>();
            services.AddScoped<IFileEncryptionService, FileEncryptionService>();
            services.AddScoped<IUserStore<User>, UserOnlyStore<User, SecureMailDbContext>>();

            services.AddIdentity<User, IdentityRole>(options =>
            {
                options.Lockout.AllowedForNewUsers = true;
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);

                /*   options.Password.RequireDigit = true;
                   options.Password.RequireNonAlphanumeric = true;
                   options.Password.RequiredLength = 8;*/
                options.Password.RequireDigit = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;


                options.SignIn.RequireConfirmedEmail = true;
            })
                .AddEntityFrameworkStores<SecureMailDbContext>()
                .AddDefaultTokenProviders();

        
            services.Configure<DataProtectionTokenProviderOptions>(options =>
            {
                options.TokenLifespan = TimeSpan.FromDays(2);
            });


            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(20);
                options.Cookie.HttpOnly = true;
            });

            services.AddAuthentication("cookies")
                .AddCookie("cookies", options => options.LoginPath = "/home/Login");

           

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthorization();
            app.UseSession();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
