using CharitySaleBackEnd.Models.Context;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using CharitySaleBackEnd.Models;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using System.Diagnostics;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Reflection;

namespace CharitySaleBackEnd
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            // Register PostgreSQL context configuration
            services.AddDbContext<AppDbContext>(options => options.UseNpgsql(Configuration.GetConnectionString("PostgreSqlConnection")));
            services.AddCors();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Latest);
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "CharitySaleBackEnd", Version = "v1" });
                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
            services.AddControllers()
                .ConfigureApiBehaviorOptions(options => { options.SuppressMapClientErrors = true; });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "CharitySaleBackEnd v1"));
            }

            app.UseRouting();

            // Global cors policy
            app.UseCors(x => x
                .AllowAnyMethod()
                .AllowAnyHeader()
                .SetIsOriginAllowed(origin => true) // allow any origin
                .AllowCredentials()); // allow credentials

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            // Share folder for uploading product images
            var uploadsDir = env.ContentRootPath + "/uploads";
            try
            {
                if (!Directory.Exists(uploadsDir))
                {
                    DirectoryInfo folder = Directory.CreateDirectory(uploadsDir);
                }
            }
            catch (Exception) { }

            app.UseFileServer(new FileServerOptions()
            {
                FileProvider = new PhysicalFileProvider(uploadsDir),
                RequestPath = new PathString("/uploads"),
                EnableDirectoryBrowsing = true
            });

            //-------------------------------------------------------------------------------------
            // Run once on start app:
            // On startup check db, is the first run?
            // And fill Demo data for Exercise presentation
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                var services = serviceScope.ServiceProvider;
                var db = services.GetService<AppDbContext>();
                var isDbExists = (db.Database.GetService<IDatabaseCreator>() as RelationalDatabaseCreator).Exists();
                db.Database.EnsureCreated();

                if (!isDbExists)
                {
                    CategoryItem wear = new CategoryItem() { Name = "Wear" };
                    CategoryItem bake = new CategoryItem() { Name = "Bake" };
                    db.Categories.AddRange(wear, bake);
                    db.SaveChanges();

                    Debug.WriteLine($"CategoryItem wear: {wear.Id}.{wear.Name}");
                    Debug.WriteLine($"CategoryItem bake: {bake.Id}.{bake.Name}");

                    var wearCatId = db.Categories.Select(id => id).Where(cat => cat.Name == "Wear").FirstOrDefault();
                    var bakeCatId = db.Categories.Select(id => id).Where(cat => cat.Name == "Bake").FirstOrDefault();

                    List<ProductItem> bakeProducts = new List<ProductItem>()
                {
                    new ProductItem(){ CategoryId = bakeCatId.Id, Name="Brownie", Price=0.65, Count=48, PreviewImageFileName="Brownie.jpg" },
                    new ProductItem(){ CategoryId = bakeCatId.Id, Name="Muffin", Price=1, Count=36, PreviewImageFileName="muffin.jpg" },
                    new ProductItem(){ CategoryId = bakeCatId.Id, Name="Cake Pop", Price=1.35, Count=24, PreviewImageFileName="cake-pop.jpg" },
                    new ProductItem(){ CategoryId = bakeCatId.Id, Name="Apple tart", Price=1.5, Count=60, PreviewImageFileName="apple-tart.jpg" },
                    new ProductItem(){ CategoryId = bakeCatId.Id, Name="Water", Price=1.5, Count=30, PreviewImageFileName="water.png"}
                };

                    List<ProductItem> wearProducts = new List<ProductItem>()
                {
                     new ProductItem(){ CategoryId = wearCatId.Id, Name="Shirt", Price=2, Count=1, PreviewImageFileName="shirt.jpg"},
                     new ProductItem(){ CategoryId = wearCatId.Id, Name="Pants", Price=3, Count=1, PreviewImageFileName="pants.jpg" },
                     new ProductItem(){ CategoryId = wearCatId.Id, Name="Jacket", Price=4, Count=1, PreviewImageFileName="jacket.png" },
                     new ProductItem(){ CategoryId = wearCatId.Id, Name="Toy", Price=1, Count=1, PreviewImageFileName="toy.png"}
                };

                    db.Products.AddRange(bakeProducts);
                    db.Products.AddRange(wearProducts);
                    db.SaveChanges();
                }
                //-------------------------------------------------------------------------------------
            }
        }
    }
}
