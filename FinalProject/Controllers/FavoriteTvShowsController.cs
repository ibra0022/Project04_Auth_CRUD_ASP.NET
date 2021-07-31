using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinalProject.Data;
using FinalProject.Helpers;
using FinalProject.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinalProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FavoriteTvShowsController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly JwtService _jwtService;

        public FavoriteTvShowsController(AppDbContext context, JwtService jwtService)
        {
            _db = context;
            _jwtService = jwtService;
        }
        
        
        // GET: api/FavoriteTvShow
        [HttpGet]
        public async Task<IActionResult> GetFavTvShow()
        {
            try
            {
                var jwt = Request.Cookies["jwt"];

                var token = _jwtService.Verify(jwt);

                int userId = int.Parse(token.Issuer);

                var profile = await _db.Profiles.FirstOrDefaultAsync(p => p.UserId == userId);

                var favTvShow = await _db.FavoriteTvShows.Where(f => f.ProfileId == profile.Id).Include(f => f.TvShow).ToListAsync();
                
                if (favTvShow.Count < 1)
                {
                    return NotFound();
                }
                
                return Ok(favTvShow[0].Profile.FavoriteTvShow);
            }
            catch (Exception e)
            {
                return Unauthorized();
            }
        }
        
        // // GET: api/FavoriteTvShow/5
        // [HttpGet("{id}", Name = "Get")]
        // public string Get(int id)
        // {
        //     return "value";
        // }

        // POST: api/FavoriteTvShow
        [HttpPost("{id}")]
        public async Task<IActionResult> Post(int id)
        {
            try
            {
                var jwt = Request.Cookies["jwt"];

                var token = _jwtService.Verify(jwt);

                int userId = int.Parse(token.Issuer);

                var profile = await _db.Profiles.FirstOrDefaultAsync(p => p.UserId == userId);
                
                if (await _db.FavoriteTvShows.FirstOrDefaultAsync(f => f.ProfileId == profile.Id && f.TvShowId == id) != null)
                {
                    return BadRequest();
                }

                var favTvShow = new FavoriteTvShow
                {
                    ProfileId = profile.Id,
                    TvShowId = id
                };
                await _db.FavoriteTvShows.AddAsync(favTvShow);
                await _db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return Unauthorized();
            }
            return Ok();
        }

        // // PUT: api/FavoriteTvShow/5
        // [HttpPut("{id}")]
        // public void Put(int id, [FromBody] string value)
        // {
        // }

        // DELETE: api/FavoriteTvShow/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var jwt = Request.Cookies["jwt"];

                var token = _jwtService.Verify(jwt);

                int userId = int.Parse(token.Issuer);

                var profile = await _db.Profiles.FirstOrDefaultAsync(p => p.UserId == userId);
                
                var favTvShow = await _db.FavoriteTvShows.FirstOrDefaultAsync(f => f.ProfileId == profile.Id && f.TvShowId == id);
                
                if (favTvShow == null)
                {
                    return NotFound();
                }
                
                _db.FavoriteTvShows.Remove(favTvShow);
                await _db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return Unauthorized();
            }
            return Ok();
        }
    }
}
