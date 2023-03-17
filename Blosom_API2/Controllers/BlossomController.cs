using Blosom_API2.Data;
using Blossom_API.Data;
using Blossom_API.Models;
using Blossom_API.Models.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blossom_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlossomController : ControllerBase
    {
        private readonly ILogger<BlossomController> _logger;
        private readonly ApplicationDbContext _db;
        public BlossomController(ILogger<BlossomController> logger, ApplicationDbContext db) 
        {
            _logger = logger;
            _db = db;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<BlossomDto>> GetBlossom()
        {
            _logger.LogInformation("See all products");
            return Ok(_db.Blossoms.ToList());
        }

        [HttpGet("id", Name = "GetBlossomProduct")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<BlossomDto> GetBlossom(int id)
        {
            if (id == 0)
            {
                _logger.LogError("error when bringing the product in with the id" + id);
                return BadRequest();
            }

            //var blossom = BlossomStore.blossomList.FirstOrDefault(v => v.Id == id);
            var blossom = _db.Blossoms.FirstOrDefault(v => v.Id == id);

            if (blossom == null)
            {
                return NotFound();
            }

            return Ok(blossom);

        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public ActionResult<BlossomDto> PostProduct([FromBody] BlossomDto blossomDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (_db.Blossoms.FirstOrDefault(v => v.Name.ToLower() == blossomDto.Name.ToLower()) != null)
            {
                ModelState.AddModelError("NameExist", "The product with that name already exists");
                return BadRequest(ModelState);
            }
            if (blossomDto == null)
            {
                return BadRequest(blossomDto);
            }
            if (blossomDto.Id > 0)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            Blossom model = new()
            {
                Name = blossomDto.Name,
                ProductDescrip = blossomDto.ProductDescrip,
                Price = blossomDto.Price,
                Brand = blossomDto.Brand,
                ImageUrl = blossomDto.ImageUrl,

            };

            _db.Blossoms.Add(model);
            _db.SaveChanges();

            return CreatedAtRoute("GetBlossomProduct", new { id = blossomDto.Id }, blossomDto);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult DeleteProduct(int id)
        {
            if (id == 0)
            {
                return BadRequest();
            }
            var blossom = _db.Blossoms.FirstOrDefault(v => v.Id == id);
            if (blossom == null)
            {
                return NotFound();
            }
            _db.Blossoms.Remove(blossom);
            _db.SaveChanges();

            return NoContent();
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult UpdateProduct(int id, [FromBody] BlossomDto blossomDto)
        {
            if (blossomDto == null || id!= blossomDto.Id)
            {
                return BadRequest();
            }
            //var blossom = BlossomStore.blossomList.FirstOrDefault(v => v.Id == id);
            //blossom.Name = blossomDto.Name;
            //blossom.Price = blossomDto.Price;
            //blossom.ProductDescrip = blossomDto.ProductDescrip;
            //blossom.Brand = blossomDto.Brand;

            Blossom model = new()
            {
                Id = blossomDto.Id,
                Name = blossomDto.Name,
                ProductDescrip = blossomDto.ProductDescrip,
                Price = blossomDto.Price,
                Brand = blossomDto.Brand,
                ImageUrl = blossomDto.ImageUrl,
                
            };
            _db.Blossoms.Update(model);
            _db.SaveChanges();

            return NoContent();

        }

        [HttpPatch("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult UpdatePartialProduct(int id, JsonPatchDocument <BlossomDto> patchDto)
        {
            if (patchDto == null || id ==0)
            {
                return BadRequest();
            }
            var blossom = _db.Blossoms.AsNoTracking().FirstOrDefault(v => v.Id == id);

            BlossomDto blossomDto = new()
            {
                Id = blossom.Id,
                Name = blossom.Name,
                ProductDescrip = blossom.ProductDescrip,
                Price = blossom.Price,
                Brand = blossom.Brand,
                ImageUrl = blossom.ImageUrl,
            };

            if (blossom == null) return BadRequest();

            patchDto.ApplyTo(blossomDto, ModelState);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Blossom model = new()
            {
                Id = blossom.Id,
                Name = blossom.Name,
                ProductDescrip = blossom.ProductDescrip,
                Price = blossom.Price,
                Brand = blossom.Brand,
                ImageUrl = blossom.ImageUrl,

            };

            _db.Blossoms.Update(model);
            _db.SaveChanges();
            return NoContent();

        }
    }
}
