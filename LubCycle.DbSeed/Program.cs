using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using LubCycle.Core;
using LubCycle.Core.Models;
using LubCycle.Core.Models.Navigation;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace LubCycle.DbSeed
{
    public class Program
    {
        public static IConfigurationRoot Configuration { get; set; }

        public static double MaxDistanceSqrt { get; private set; }
        public static double MaxSingleDuration { get; private set; }
        public static string AppDatabaseConnectionString { get; private set; }
        public static string BingMapsApiKey { get; private set; }
        public static string CityUids { get; private set; }

        public static void Main(string[] args)
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            Configuration = new ConfigurationBuilder()
                .SetBasePath(System.IO.Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddUserSecrets()
                .Build();
            ConfigureSettings();

            Task.Run(async () =>
            {
                try
                {
                    var nextBike = new Core.Helpers.NextBikeHelper(CityUids);
                    var places = await nextBike.GetStationsAsync();
                    var bing = new LubCycle.Core.Helpers.BingHelper(BingMapsApiKey);
                    var db = new AppDatabase();
                    int counter = 0;

                    var client = new HttpClient();
                    var resource = new LubCycle.Core.Models.BingMaps.Resource();
                    for (int i = 0; i < places.Count; i++)
                    {
                        for (int j = i + 1; j < places.Count; j++)
                        {
                            if (Core.Helpers.GeoHelper.CalcDistance(places[i], places[j]) < MaxDistanceSqrt)
                            {
                                var response = await bing.GetDirectionsAsync(places[i], places[j]);
                                resource = response.ResourceSets.First().Resources.First();
                                if (resource.TravelDuration < MaxSingleDuration)
                                {
                                    //Console.WriteLine(places[i].Name + " " + places[j].Name + " " + resource.TravelDistance);
                                    db.TravelDurations.Add(new TravelDuration()
                                    {
                                        Distance = resource.TravelDistance,
                                        Duration = (int)resource.TravelDuration,
                                        Station1Uid = places[i].Uid,
                                        Station2Uid = places[j].Uid
                                    });
                                    if (counter % 50 == 0)
                                    {
                                        //Save every 50 entities.
                                        //await db.SaveChangesAsync();
                                        //Console.WriteLine("================= SAVED " + counter + " ==================");
                                    }
                                }

                            }
                        }
                    }
                    //await db.SaveChangesAsync();
                }
                catch (Exception exc)
                {
                    Console.WriteLine(exc.Message);
                }
            });

            Console.ReadLine();
        }
        private static void ConfigureSettings()
        {
            string buffer;

            buffer = Configuration["MAX_DISTANCE_SQRT"];
            if (String.IsNullOrWhiteSpace(buffer))
                throw new ArgumentNullException("MAX_DISTANCE_SQRT");
            else
            {
                MaxDistanceSqrt = double.Parse(buffer, NumberStyles.AllowDecimalPoint,
                    System.Globalization.CultureInfo.InvariantCulture);
            }

            buffer = Configuration["MAX_SINGLE_DURATION"];
            if (String.IsNullOrWhiteSpace(buffer))
                throw new ArgumentNullException("MAX_SINGLE_DURATION");
            else
            {
                MaxSingleDuration = double.Parse(buffer, NumberStyles.AllowDecimalPoint,
                    System.Globalization.CultureInfo.InvariantCulture);
            }

            buffer = Configuration["APP_DATABASE_CONNECTION_STRING"];
            if (String.IsNullOrWhiteSpace(buffer))
                throw new ArgumentNullException("APP_DATABASE_CONNECTION_STRING");
            else
            {
                AppDatabaseConnectionString = buffer;
            }

            buffer = Configuration["BING_MAPS_API_KEY"];
            if (String.IsNullOrWhiteSpace(buffer))
                throw new ArgumentNullException("BING_MAPS_API_KEY");
            else
            {
                BingMapsApiKey = buffer;
            }

            buffer = Configuration["CITY_UIDS"];
            if (String.IsNullOrWhiteSpace(buffer))
                throw new ArgumentNullException("CITY_UIDS");
            else
            {
                CityUids = buffer;
            }
        }
    }
}
