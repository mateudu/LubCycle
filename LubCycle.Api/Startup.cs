﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using LubCycle.Api.Data;
using LubCycle.Api.Models;
using LubCycle.Api.Services;
using LubCycle.Core;
using LubCycle.Core.Helpers;
using LubCycle.Core.Models.Navigation;
using Swashbuckle.SwaggerGen.Generator;

namespace LubCycle.Api
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsDevelopment())
            {
                // For more details on using the user secret store see http://go.microsoft.com/fwlink/?LinkID=532709
                //builder.AddUserSecrets();
            }
            builder.AddUserSecrets();
            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public static IConfigurationRoot Configuration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddDbContext<ApplicationDbContext>(options =>
                //options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
                //options.UseSqlServer(Configuration.GetConnectionString("AzureSQL")));
                options.UseSqlServer(Configuration["APP_DATABASE_CONNECTION_STRING"]));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddMvc();

            // Add application services.
            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();

            // LubCycle Dependency Injection
            services.AddSingleton<Core.Helpers.NextBikeHelper>(
                provider => new NextBikeHelper(Configuration["CITY_UIDS"]));
            services.AddSingleton<Core.Helpers.BingHelper>(
                provider => new BingHelper(Configuration["BING_MAPS_API_KEY"]));
            services.AddSingleton<Core.Helpers.GoogleMapsHelper>(
                provider => new GoogleMapsHelper(Configuration["GOOGLE_MAPS_API_KEY"]));

            double buffer;
            services.AddSingleton<Core.Helpers.NavigationHelper>(
                provider =>
                    new NavigationHelper(
                        new NavigationHelperSettings(
                            provider.GetService<ApplicationDbContext>().TravelDurations.Cast<RouteStatistic>().ToList(),
                            provider.GetService<NextBikeHelper>().GetStationsAsync().Result)
                        {
                            MaximalSingleDuration = double.TryParse(
                                Configuration["MAX_SINGLE_DURATION"],
                                out buffer)?buffer:2400.0,
                            MaximalSingleDistance = double.TryParse(
                                Configuration["MAX_DISTANCE_SQRT"],
                                out buffer) ? buffer*1000.0 : 7000.0
                        }));
            
            // Inject an implementation of ISwaggerProvider with defaulted settings applied
            services.AddSwaggerGen();
            services.ConfigureSwaggerGen(options =>
            {
                options.SingleApiVersion(new Info()
                {
                    Contact = new Contact()
                    {
                        Name = "LubCycle",
                        Email = "kontakt@lubcycle.pl"
                    },
                    Version = "v1",
                    Title = "LubCycle"
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseIdentity();

            // Add external authentication middleware below. To configure them please see http://go.microsoft.com/fwlink/?LinkID=532715

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            // Enable middleware to serve generated Swagger as a JSON endpoint
            app.UseSwaggerGen();

            // Enable middleware to serve swagger-ui assets (HTML, JS, CSS etc.)
            app.UseSwaggerUi();
        }
    }
}
