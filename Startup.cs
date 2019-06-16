using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Soat.Masterclass.Labs.Models.CosmosDB;
using Soat.Masterclass.Labs.Repositories;

namespace Soat.Masterclass.Labs
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
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });


            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddSingleton<IDocumentDBRepository<Item>>(c =>
            {
                string endpoint = Configuration["CosmosDB:endpoint"];
                string key = Configuration["CosmosDB:key"];
                string databaseId = Configuration["CosmosDB:databaseId"];
                string collectionId = Configuration["CosmosDB:collectionId"];
                return new DocumentDBRepository<Item>(endpoint, key, databaseId, collectionId);
            });

            services.AddSingleton<IStorageBlobHelper>(c =>
            {
                string accountName = Configuration["Storage:accountName"];
                string accountKey = Configuration["Storage:accountKey"];
                string container = Configuration["Storage:container"];

                return new StorageBlobHelper(accountName, accountKey, container);
            });

            services.AddDbContext<SqlDBContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DbConnection")));

            // Automatically perform database migration
            services.BuildServiceProvider().GetService<SqlDBContext>().Database.Migrate();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
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
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
