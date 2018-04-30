using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using movie_api.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace movie_api.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        [Route("/")]
        public IActionResult Index()
        {
            List<Dictionary<string, object>> SearchedMovies = DbConnector.Query("SELECT * FROM movies");
            ViewBag.movies = SearchedMovies;
            return View();
        }
        [Route("/{movie_search}")]
        public IActionResult searchMovies(string movie_search)
        {
            WebRequest.GetMovieDataAsync(movie_search, ApiResponse =>
            {
                // ViewBag.Movies = ApiResponse;
                string query = $"INSERT INTO movies (title, rating, release_date, created_at, updated_at) VALUES ('{ApiResponse[0].Title}', {ApiResponse[0].Rating}, '{ApiResponse[0].ReleaseDate}', NOW(), NOW());";
                DbConnector.Execute(query);
            }
            ).Wait();

            return RedirectToAction("Index");
        }

    }
}
