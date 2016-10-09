using System;
using System.Collections.Generic;
using System.IO;
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
using LubCycle.Core.Api.Models.Navigation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json.Serialization;
using Swashbuckle.Swagger.Model;
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

            double buffer;


            // Add API response caching.
            services
                .AddMvc(options =>
                {
                    options.CacheProfiles.Add("AddressCaching",
                        new CacheProfile()
                        {
                            Duration = 60
                        });
                    options.CacheProfiles.Add("StationsCaching",
                        new CacheProfile()
                        {
                            Duration = (int)(double.TryParse(Configuration["NEXTBIKE_UPDATE_FREQUENCY"], out buffer) ? buffer : 15.0)
                        });
                })
                .AddJsonOptions(opt =>
                {
                    var resolver = opt.SerializerSettings.ContractResolver;
                    if (resolver != null)
                    {
                        var res = resolver as DefaultContractResolver;
                        res.NamingStrategy = null;  // <<!-- this removes the camelcasing
                    }
                });

            // Add application services.
            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();

            // LubCycle Dependency Injection

            services.AddSingleton<Core.Helpers.NextBikeHelper>(
                provider => 
                    new NextBikeHelper(
                            Configuration["CITY_UIDS"], 
                            double.TryParse( Configuration["NEXTBIKE_UPDATE_FREQUENCY"], out buffer) ? buffer : 15.0
                        )
                        );

            services.AddSingleton<Core.Helpers.IMapsHelper>(
                provider => new GoogleMapsHelper(Configuration["GOOGLE_MAPS_API_KEY"]));
            services.AddSingleton<Core.Helpers.NavigationHelper>(
                provider =>
                    new NavigationHelper(
                        new NavigationHelperSettings(
                            provider.GetService<ApplicationDbContext>().TravelDurations.Cast<RouteStatistic>().ToList(),
                            provider.GetService<NextBikeHelper>().GetStationsAsync().Result)
                        {
                            MaximalSingleDuration = double.TryParse(
                                Configuration["MAX_SINGLE_DURATION"],
                                out buffer) ? buffer : 2400.0,
                            MaximalSingleDistance = double.TryParse(
                                Configuration["MAX_SINGLE_DISTANCE_METERS"],
                                out buffer) ? buffer : 7000.0
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
                //options.IncludeXmlComments(GetXmlCommentsPath());
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

            app.UseStaticFiles(new StaticFileOptions()
            {
                OnPrepareResponse = context =>
                {
                    var headers = context.Context.Response.GetTypedHeaders();
                    headers.CacheControl = new CacheControlHeaderValue()
                    {
                        MaxAge = TimeSpan.FromMinutes(60)
                    };
                }
            });

            app.UseIdentity();

            // Add external authentication middleware below. To configure them please see http://go.microsoft.com/fwlink/?LinkID=532715

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            //// Enable middleware to serve generated Swagger as a JSON endpoint
            //app.UseSwaggerGen();
            app.UseSwagger();

            //// Enable middleware to serve swagger-ui assets (HTML, JS, CSS etc.)
            app.UseSwaggerUi();
        }

        private string GetXmlCommentsPath()
        {
            string s = Path.Combine(System.AppContext.BaseDirectory, @"LubCycle.Api.xml");
            return s;
        }
    }
}
