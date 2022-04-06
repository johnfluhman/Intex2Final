using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using CollisionsDB.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.ML.OnnxRuntime;

namespace CollisionsDB
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
            services.AddControllersWithViews();

            //This will connect to MySQL
            services.AddDbContext<CollisionDBContext>(options =>
            {
                options.UseMySql(Configuration["ConnectionStrings:CollisionConnection"]);
            });

            services.AddScoped<ICollisionRepository, EFCollisionRepository>();

            services.AddDistributedMemoryCache();
            services.AddSession();

            services.AddSingleton<InferenceSession>(
                new InferenceSession("Models/trained_model_hgboost.onnx")
            );

            //services.AddIdentity<IdentityUser, IdentityRole>()
            //    .AddEntityFrameworkStores<AppIdentityDBContext>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseSession();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "countypage",
                    pattern: "{county}/Page{pageNum}",
                    defaults: new { Controller = "Home", action = "Summary"});

                endpoints.MapControllerRoute(
                    name: "Paging",
                    pattern: "Page{pageNum}",
                    defaults: new { Controller = "Home", action = "Summary", pageNum = 1 });

                endpoints.MapControllerRoute(
                    name: "county",
                    pattern: "{county}",
                    defaults: new { Controller = "Home", action = "Summary", pageNum = 1 });

                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
