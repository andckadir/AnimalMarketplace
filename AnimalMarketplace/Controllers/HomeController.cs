using AnimalMarketplace.Database.DbContexts;
using AnimalMarketplace.Models;
using AnimalMarketplace.Repositories.Implementations;
using AnimalMarketplace.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AnimalMarketplace.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController(IUserRepository userRepository) : ControllerBase
    {
        [HttpGet("GetAllUsers")]
        public async Task<IActionResult> Index()
        {
            var users = await userRepository.GetAllUsersAsync();   
            return Ok(users);
        }
        
        [HttpGet("privacy")]
        public IActionResult Privacy()
        {
            var data = new
            {
                Privacy = "Privacy"
            };
            return Ok(data);
        }
    }
}
