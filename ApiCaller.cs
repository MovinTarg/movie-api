using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace movie_api
{
    public class WebRequest
    {
        // The second parameter is a function that returns a Dictionary of string keys to object values.
        // If an API returned an array as its top level collection the parameter type would be "Action>"
        public static async Task GetMovieDataAsync(string movie_search, Action<List<Movie>> Callback)
        {
            // Create a temporary HttpClient connection.
            using (var Client = new HttpClient())
            {
                try
                {
                    Client.BaseAddress = new Uri($"https://api.themoviedb.org/3/search/movie?api_key=a00fbdea0010d4ba50be402a4ef2d237&query={movie_search}");
                    // API Key = ?api_key=a00fbdea0010d4ba50be402a4ef2d237
                    HttpResponseMessage Response = await Client.GetAsync(""); // Make the actual API call.
                    Response.EnsureSuccessStatusCode(); // Throw error if not successful.
                    string StringResponse = await Response.Content.ReadAsStringAsync(); // Read in the response as a string.
                     
                    // Then parse the result into JSON and convert to a dictionary that we can use.
                    // DeserializeObject will only parse the top level object, depending on the API we may need to dig deeper and continue deserializing
                    Dictionary<string, object> JsonResponse = JsonConvert.DeserializeObject<Dictionary<string, object>>(StringResponse);
                    var ListResults = (JsonResponse["results"] as JToken).Value<JArray>().ToList();
                    var MovieList = new List<Movie>();
                    foreach(var movie in ListResults)
                    {
                        // bool NoReleaseDate = (movie["release_date"].Value<string>() == "");

                        // DateTime theReleaseDate = 
                        //     (NoReleaseDate) 
                        //     ? default(DateTime)
                        //     : Convert.ToDateTime(movie["release_date"].Value<string>());

                        MovieList.Add(new Movie()
                        {
                            Title = movie["title"].Value<string>(),
                            Rating = movie["vote_average"].Value<float>(),
                            ReleaseDate = movie["release_date"].Value<string>()
                        });
                    }
                    // Finally, execute our callback, passing it the response we got.
                    Callback(MovieList);
                }
                catch (HttpRequestException e)
                {
                    // If something went wrong, display the error.
                    Console.WriteLine($"Request exception: {e.Message}");
                }
            }
        }
    }
}