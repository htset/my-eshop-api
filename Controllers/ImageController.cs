using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using my_eshop_api.Models;
using System;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace my_eshop_api.Controllers
{
    [Route("api/[controller]")]
    [EnableCors("my_eshop_AllowSpecificOrigins")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly ItemContext Context;

        public ImageController(ItemContext context)
        {
            Context = context;
        }

        [HttpPost, DisableRequestSizeLimit]
        public async Task<ActionResult> UploadImage()
        {
            try
            {
                var formCollection = await Request.ReadFormAsync();
                var file = formCollection.Files.First();

                var folderName = Path.Combine("wwwroot", "images");
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);

                if (file.Length > 0)
                {
                    var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                    var fullPath = Path.Combine(pathToSave, fileName);
                    var dbPath = Path.Combine(folderName, fileName);

                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }

                    var itemId = int.Parse(fileName.Substring(0, fileName.IndexOf('.')));
                    var fileType = fileName.Substring(fileName.IndexOf('.')+1, fileName.Length - fileName.IndexOf('.') -1);
                    var image = await Context.Images.FirstOrDefaultAsync(img => img.ItemId == itemId);
                    if(image == null)
                    {
                        image = new Image()
                        {
                            ItemId = itemId,
                            FileName = fileName,
                            FileType = fileType
                        };
                        await Context.Images.AddAsync(image);
                    }
                    else
                    {
                        image.FileName = fileName;
                        image.FileType = fileType;
                    }

                    await Context.SaveChangesAsync();

                    return Ok(new { dbPath });
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }
        }
    }
}
