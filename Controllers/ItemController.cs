using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using my_eshop_api.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace my_eshop_api.Controllers
{
    [Route("api/items")]
    [EnableCors("my_eshop_AllowSpecificOrigins")]
    [ApiController]
    public class ItemController : ControllerBase
    {
        private readonly ItemContext _context;

        public ItemController(ItemContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<ItemPayload>> GetItems([FromQuery] QueryStringParameters qsParameters)
        {
            IQueryable<Item> returnItems = _context.Items
                .Include(it => it.Images)
                .OrderBy(on => on.Id);

            if (qsParameters.Name != null && !qsParameters.Name.Trim().Equals(string.Empty))
                returnItems = returnItems.Where(item => item.Name.ToLower().Contains(qsParameters.Name.Trim().ToLower()));

            if (qsParameters.Category != null && !qsParameters.Category.Trim().Equals(string.Empty))
            {
                string[] categories = qsParameters.Category.Split('#');
            }

            //get total count before paging
            int count = await returnItems.CountAsync();

            returnItems = returnItems
                                .Skip((qsParameters.PageNumber - 1) * qsParameters.PageSize)
                                .Take(qsParameters.PageSize);

            List<Item> list = await returnItems.ToListAsync();

            return new ItemPayload(list, count);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Item>> GetItem(int id)
        {
            var item = await _context.Items
                .Include(it => it.Images)
                .SingleOrDefaultAsync(item => item.Id == id);
            
            if (item == null)
                return NotFound();

            return Ok(item);
        }

        [HttpPost]
        public async Task<ActionResult<Item>> PostItem(Item item)
        {
            await _context.Items.AddAsync(item);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetItem), new { id = item.Id }, item);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutItem(int id, Item item)
        {
            if (id != item.Id)
            {
                return BadRequest();
            }

            _context.Entry(item).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteItem(int id)
        {
            var item = await _context.Items.FindAsync(id);

            if (item == null)
            {
                return NotFound();
            }

            _context.Items.Remove(item);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
