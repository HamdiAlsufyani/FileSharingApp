using FileSharingPractice.Data;
using FileSharingPractice.Helpers.Mail;
using FileSharingPractice.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FileSharingPractice
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
            services.AddControllersWithViews().AddViewLocalization(op=>
            {
                op.ResourcesPath = "Resources";
            });
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnextion"));
            });
            services.AddAuthentication().
                AddFacebook(option =>
                {
                    option.AppId  = Configuration["Authentication:Facebook:AppId"];
                    option.AppSecret = Configuration["Authentication:Facebook:AppSecret"];
                }).AddGoogle(option=>
                {
                    option.ClientId = Configuration["Authentication:Google:ClientId"];
                    option.ClientSecret = Configuration["Authentication:Google:ClientSecret"];
                });
               
            services.AddIdentity<ApplicationUser, IdentityRole>(option=>
            {
                option.SignIn.RequireConfirmedEmail = true;
            }).AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

            services.Configure<DataProtectionTokenProviderOptions>(option =>
            {
                option.TokenLifespan = TimeSpan.FromDays(3);

            });
            services.AddLocalization();
            services.AddTransient<IMailService, Mailhelper>();
            services.AddTransient<IUploadservice, Uploadservice>();
            services.AddAutoMapper(typeof(Startup));
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
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();

            app.UseAuthorization();
            var supportedCulture = new[] { "ar-SA", "en-US" };
            app.UseRequestLocalization(r =>{
                r.AddSupportedUICultures(supportedCulture);
                r.AddSupportedCultures(supportedCulture);
                 r.SetDefaultCulture("en-Us");
            });

       
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
