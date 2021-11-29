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
            IQueryable<Item> returnItems = _context.Items.OrderBy(on => on.Id);

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
            var Item = await _context.Items.FindAsync(id);
            if (Item == null)
                return NotFound();

            return Item;
        }
    }
}
