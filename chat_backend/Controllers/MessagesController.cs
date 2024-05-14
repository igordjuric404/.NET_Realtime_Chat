using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using _NET_Realtime_Chat.Data;
using _NET_Realtime_Chat.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace _NET_Realtime_Chat.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MessagesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public MessagesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{user1}/{user2}")]
        public async Task<ActionResult<IEnumerable<Message>>> GetMessages(string user1, string user2)
        {
            var messages = await _context.Messages
                .Where(m => (m.Sender == user1 && m.Receiver == user2) || (m.Sender == user2 && m.Receiver == user1))
                .OrderBy(m => m.Timestamp)
                .ToListAsync();

            return Ok(messages);
        }

        [HttpPost]
        public async Task<ActionResult<Message>> CreateMessage(Message message)
        {
            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            // Use the name of the action and the correct route values
            return CreatedAtAction(nameof(GetMessages), new { user1 = message.Sender, user2 = message.Receiver }, message);
        }
    }
}
