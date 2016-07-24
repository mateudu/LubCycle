using Microsoft.Extensions.Configuration;
using System;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;
using LubCycle.Core.Helpers;
using LubCycle.Core.Api.Models.GoogleMaps.DistanceMatrix;

namespace LubCycle.DbSeed
{
    public class Program
    {
        public static string AppDatabaseConnectionString { get; private set; }
        private static string BingMapsApiKey { get; set; }
        private static string CityUids { get; set; }
        private static IConfigurationRoot Configuration { get; set; }

        private static string GoogleMapsApiKey { get; set; }
        private static double MaxSingleDistance { get; set; }
        private static double MaxSingleDuration { get; set; }

        public static void Main(string[] args)
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            Configuration = new ConfigurationBuilder()
                .SetBasePath(System.IO.Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddUserSecrets()
                .Build();
            ConfigureSettings();

            //Task.Run(async () =>
            //{
            //    try
            //    {
            //        var nextBike = new Core.Helpers.NextBikeHelper(CityUids);
            //        var places = await nextBike.GetStationsAsync();
            //        IMapsHelper mapsHelper = new GoogleMapsHelper(GoogleMapsApiKey);
            //        var db = new AppDatabase();
            //        int counter = 0;

            //        var client = new HttpClient();
            //        var obj = new Element();
            //        for (int i = 0; i < places.Count; i++)
            //        {
            //            for (int j = i + 1; j < places.Count; j++)
            //            {
            //                if (Core.Helpers.GeoHelper.CalcDistanceInMeters(
            //                    places[i].Lat,
            //                    places[i].Lng,
            //                    places[j].Lat,
            //                    places[j].Lng) <= MaxSingleDistance)
            //                {
            //                    var dr = await mapsHelper.GetDistanceResponseAsync(
            //                    places[i].Lat,
            //                    places[i].Lng,
            //                    places[j].Lat,
            //                    places[j].Lng);
            //                    if (dr.Distance.HasValue && dr.Duration.HasValue)
            //                    {
            //                        if (dr.Distance.Value <= MaxSingleDistance &&
            //                            dr.Duration.Value <= MaxSingleDuration)
            //                        {
            //                            Console.WriteLine($"{i}:{j} {places[i].Name}<=>{places[j].Name}, {counter++}");
                                        
            //                            // UNCOMMENT THESE LINES BELOW
                                        
            //                            //db.TravelDurations.Add(new TravelDuration()
            //                            //{
            //                            //    Distance = dr.Distance.Value,
            //                            //    Duration = dr.Duration.Value,
            //                            //    Station1Uid = places[i].Uid,
            //                            //    Station2Uid = places[j].Uid
            //                            //});
            //                            if (counter % 50 == 0)
            //                            {
            //                                //Save every 50 entities.

            //                                // UNCOMMENT THESE LINES BELOW

            //                                //await db.SaveChangesAsync();
            //                                //Console.WriteLine("================= SAVED " + counter + " ==================");
            //                            }
            //                        }
            //                    }
            //                }
            //            }
            //        }
            //        Console.WriteLine($"SAVED {counter} ENTITIES.");

            //        // UNCOMMENT THESE LINES BELOW

            //        //await db.SaveChangesAsync();
            //    }
            //    catch (Exception exc)
            //    {
            //        Console.WriteLine(exc.Message);
            //    }
            //});

            IMapsHelper maps = new GoogleMapsHelper(GoogleMapsApiKey);
            var result = maps.GetLocationResponseAsync(@"Lublin Jantarowa 5").Result;
            Console.WriteLine($"{result.Status} {result.Lat} {result.Lng}");
            Console.ReadLine();
        }

        private static void ConfigureSettings()
        {
            string buffer;

            buffer = Configuration["MAX_SINGLE_DISTANCE_METERS"];
            if (String.IsNullOrWhiteSpace(buffer))
                throw new ArgumentNullException("MAX_SINGLE_DISTANCE_METERS");
            else
            {
                MaxSingleDistance = double.Parse(buffer, NumberStyles.AllowDecimalPoint,
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
            BingMapsApiKey = buffer;

            buffer = Configuration["GOOGLE_MAPS_API_KEY"];
            GoogleMapsApiKey = buffer;

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