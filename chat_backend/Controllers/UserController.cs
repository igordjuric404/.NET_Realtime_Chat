using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using _NET_Realtime_Chat.Data;
using _NET_Realtime_Chat.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace _NET_Realtime_Chat.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<User>> CreateUser(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            // Use Created instead of CreatedAtAction
            return Created($"/api/users/{user.Id}", user);
        }
    }
}
