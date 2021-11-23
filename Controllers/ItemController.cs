using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using my_eshop_api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace my_eshop_api.Controllers
{
    [Route("api/items")]
    [ApiController]
    public class ItemController : ControllerBase
    {
        private readonly ItemContext _context;

        public ItemController(ItemContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<ItemPayload>> GetItems()
        {
            int count = await _context.Items.CountAsync();
            List<Item> list = await _context.Items.ToListAsync();
            return new ItemPayload(list, count);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Item>> GetItem(int id)
        {
            var Item = await _context.Items.FindAsync(id);
            if (Item == null)
            {
                return NotFound();
            }
            return Item;
        }
    }
}
