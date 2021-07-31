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
    public class TvShowsController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly JwtService _jwtService;

        public TvShowsController(AppDbContext context, JwtService jwtService)
        {
            _db = context;
            _jwtService = jwtService;
        }
        
        
        // GET: api/TvShows
        [HttpGet]
        public async Task<IActionResult> GetTvShows()
        {
            return Ok(new
            {
                List = await _db.TvShows.ToListAsync()
            });
        }

        // GET: api/TvShows/5
        [HttpGet("{id}")]
        public async Task<ActionResult> Get(int id)
        {
            var tvShow = await _db.TvShows.SingleOrDefaultAsync(a => a.Id == id);

            if (tvShow == null)
            {
                return BadRequest();
            }
            IEnumerable<TvShowReview> tvReviews = await _db.TvShowReview.Where(r => r.TvShowId == id).Include(p => p.Profile).Include(n => n.Profile.User).ToListAsync();
            List<ReviewDto> reviewDto = new List<ReviewDto>();
            foreach (var review in tvReviews)
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
                tvShow,
                reviewDto
            });
        }
        
        // POST: api/TvShows
        [HttpPost]
        public async Task<ActionResult> Post(TvShow tvShow)
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

                var tvShowInDatabase =  await _db.TvShows.Where(t => t.Title == tvShow.Title).ToListAsync();
                if (tvShowInDatabase.Count > 0)
                {
                    return BadRequest(new
                    {
                        message = "Already Exist in Database"
                    });
                }

                tvShow.ProfileId = profile.Id;
                
               await _db.TvShows.AddAsync(tvShow);
               await _db.SaveChangesAsync();
                
                return Ok(tvShow);
            }
            catch (Exception e)
            {
                return Unauthorized();
            }
        }
        
        // PUT: api/TvShows/5
        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, TvShow tvShow)
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
                
                var editTvShow = await _db.TvShows.SingleOrDefaultAsync(a => a.Id == id);
                
                if (editTvShow == null)
                {
                    return BadRequest();
                }

                if (editTvShow.ProfileId != profile.Id && profile.Id != 1)
                {
                    return Unauthorized();
                }
                
                editTvShow.Title = tvShow.Title;
                editTvShow.Date = tvShow.Date;
                editTvShow.Summary = tvShow.Summary;
                editTvShow.Rating = tvShow.Rating;
                editTvShow.Poster = tvShow.Poster;
                
                await _db.SaveChangesAsync();
                
                return Ok(tvShow);
            }
            catch (Exception e)
            {
                return Unauthorized();
            }
        }
    }
}
