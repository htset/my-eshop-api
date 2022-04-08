using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using my_eshop_api.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace my_eshop_api.Controllers
{
    [Route("api/[controller]")]
    [EnableCors("my_eshop_AllowSpecificOrigins")]
    [ApiController]
    public class AddressController : ControllerBase
    {
        private readonly ItemContext _context;

        public AddressController(ItemContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Address>>> Get()
        {
            return await _context.Addresses.ToListAsync();
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<IEnumerable<Address>>> GetByUserId(int userId)
        {
            return await _context.Addresses.Where((addr) => addr.UserId == userId).ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<Address>> Post([FromBody] Address value)
        {
            if (value.Id == 0)
            {
                await _context.Addresses.AddAsync(value);
            }
            else
            {
                _context.Addresses.Update(value);
            }
            await _context.SaveChangesAsync();
            return value;
        }

        [HttpDelete("{id}")]
        public async Task<int> Delete(int id)
        {
            var addr = await _context.Addresses.FindAsync(id);
            _context.Addresses.Remove(addr);
            await _context.SaveChangesAsync();
            return id;
        }

    }
}
