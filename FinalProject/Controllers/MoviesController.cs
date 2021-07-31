using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinalProject.Data;
using FinalProject.Dtos;
using FinalProject.Helpers;
using FinalProject.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinalProject.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        
        private readonly AppDbContext _db;
        private readonly JwtService _jwtService;

        public MoviesController(AppDbContext context, JwtService jwtService)
        {
            _db = context;
            _jwtService = jwtService;
        }
        
        // GET: api/Movies
        [HttpGet]
        public async Task<ActionResult> GetMovies()
        {
            return Ok(new
            {
                List = await _db.Movies.ToListAsync()
            });
        }

        // GET: api/Movies/5
        [HttpGet("{id}")]
        public async Task<ActionResult> Get(int id)
        {
            var movie = await _db.Movies.SingleOrDefaultAsync(a => a.Id == id);
            
            if (movie == null)
            {
                return BadRequest();
            }
            
            IEnumerable<MovieReview> movieReviews = await _db.MovieReview.Where(r => r.MovieId == id).Include(p => p.Profile).Include(n => n.Profile.User).ToListAsync();
            List<ReviewDto> reviewDto = new List<ReviewDto>();
            foreach (var review in movieReviews)
            {
                reviewDto.Add(new ReviewDto()
                {
                    UserName = review.Profile.User.Name,
                    Text = review.Text,
                    Spoiler = review.Spoiler
                });
            }
            
            return Ok(new
            {
                movie,
                reviewDto
            });
            
        }
        
        // POST: api/Movies
        [HttpPost]
        public async Task<ActionResult> Post( Movie movie )
        {
            try
            {
                var jwt = Request.Cookies["jwt"];

                var token = _jwtService.Verify(jwt);

                int userId = int.Parse(token.Issuer);


                var profile = await _db.Profiles.FirstOrDefaultAsync(p => p.UserId == userId);

                if (profile == null)
                {
                    return Unauthorized();
                }

                var movieInDatabase =  await _db.Movies.Where(m => m.Title == movie.Title).ToListAsync();
                if (movieInDatabase.Count > 0)
                {
                    return BadRequest(new
                    {
                        message = "Already Exist in Database"
                    });
                }

                movie.ProfileId = profile.Id;
                
                await _db.Movies.AddAsync(movie);
                await _db.SaveChangesAsync();
                
                return Ok(movie);
            }
            catch (Exception e)
            {
                return Unauthorized();
            }
            
        }
        
        // PUT: api/Movies/5
        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, Movie movie)
        {
            try
            {
                var jwt = Request.Cookies["jwt"];

                var token = _jwtService.Verify(jwt);

                int userId = int.Parse(token.Issuer);

                var profile = await _db.Profiles.FirstOrDefaultAsync(p => p.UserId == userId);

                if (profile == null)
                {
                    return Unauthorized();
                }
                
                var editMovie = await _db.Movies.SingleOrDefaultAsync(a => a.Id == id);
                
                if (editMovie == null)
                {
                    return BadRequest();
                }

                if (editMovie.ProfileId != profile.Id && profile.Id != 1)
                {
                    return Unauthorized();
                }
                
                editMovie.Title = movie.Title;
                editMovie.Date = movie.Date;
                editMovie.Summary = movie.Summary;
                editMovie.Rating = movie.Rating;
                editMovie.Poster = movie.Poster;
                
               await _db.SaveChangesAsync();
                
                return Ok(movie);
            }
            catch (Exception e)
            {
                return Unauthorized();
            }
           
        }

    }
}
