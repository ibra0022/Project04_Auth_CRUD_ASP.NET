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
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly JwtService _jwtService;

        public AuthController(AppDbContext context, JwtService jwtService)
        {
            _db = context;
            _jwtService = jwtService;
        }
        
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(dto.Password)
            };

            await _db.Users.AddAsync(user);
            await _db.SaveChangesAsync();
            
            var userFromDb = await _db.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            
            return Created("success", userFromDb);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (user == null)
            {
                return BadRequest(new {message = "Invalid Credentials"});
            }

            if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.Password))
            {
                return BadRequest(new {message = "Invalid Credentials"});
            }

            var profile = await _db.Profiles.FirstOrDefaultAsync(p => p.UserId == user.Id);
            
            var jwt = _jwtService.Generate(user.Id);
            
            Response.Cookies.Append("jwt", jwt, new CookieOptions
            {
                HttpOnly = true
            });

            if (profile == null)
            {
                var newProfile = new Profile()
                {
                    UserId = user.Id,
                    User = user,
                    JoinedDateTime = DateTime.Now
                };
                await _db.Profiles.AddAsync(newProfile);
                await _db.SaveChangesAsync();
                
                return Ok(new
                {
                    message = "success",
                    user
                });
            }
            else
            {
                var movies = await _db.Movies.Where(m => m.ProfileId == profile.Id).ToListAsync();
                var tvShow = await _db.TvShows.Where(t => t.ProfileId == profile.Id).ToListAsync();
                var favoriteMovies = await _db.FavoriteMovies.Where(fav => fav.ProfileId == profile.Id).ToListAsync();
                var favoriteTvShows = await _db.FavoriteTvShows.Where(fav => fav.ProfileId == profile.Id).ToListAsync();
            
                return Ok(new
                {
                    message = "success",
                    user
                });
            }
        }

        [HttpGet("user")]
        public async Task<IActionResult> User()
        {
            
            try
            {
                var jwt = Request.Cookies["jwt"];

                var token = _jwtService.Verify(jwt);

                int userId = int.Parse(token.Issuer);

                var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);

                var profile = await _db.Profiles.FirstOrDefaultAsync(p => p.UserId == userId);
                var movies = await _db.Movies.Where(m => m.ProfileId == profile.Id).ToListAsync();
                var tvShow = await _db.TvShows.Where(t => t.ProfileId == profile.Id).ToListAsync();
                var favoriteMovies = await _db.FavoriteMovies.Where(fav => fav.ProfileId == profile.Id).ToListAsync();
                var favoriteTvShows = await _db.FavoriteTvShows.Where(fav => fav.ProfileId == profile.Id).ToListAsync();
                
                return Ok(user);
            }
            catch (Exception e)
            {
                return Unauthorized();
            }
        }
    
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("jwt");
            
            return Ok(new
            {
                message = "success"
            });
        }
    }
}