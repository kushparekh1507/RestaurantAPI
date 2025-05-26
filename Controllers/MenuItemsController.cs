using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantAPI.DTO;
using RestaurantAPI.Models;
using RestaurantAPI.Services.Interfaces;

namespace RestaurantAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MenuItemsController : ControllerBase
    {
        private readonly RestaurantContext _context;
        private readonly IMapper _mapper;
        IConfiguration _configuration;
        private readonly ICloudinaryService _cloudinaryService;

        public MenuItemsController(RestaurantContext context, IMapper mapper, IConfiguration configuration, ICloudinaryService cloudinaryService)
        {
            _context = context;
            _mapper = mapper;
            _configuration = configuration;
            _cloudinaryService = cloudinaryService;
        }

        // GET: api/MenuItems
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MenuItem>>> GetMenuItem()
        {
            return await _context.MenuItem.ToListAsync();
        }

        // GET: api/MenuItems/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MenuItem>> GetMenuItem(int id)
        {
            var menuItem = await _context.MenuItem.FindAsync(id);

            if (menuItem == null)
            {
                return NotFound();
            }

            return menuItem;
        }

        [HttpGet("restaurant/{id}")]
        public async Task<ActionResult<IEnumerable<MenuItem>>> GetMenuItemsByRestaurant(int id)
        {
            var items = await _context.MenuItem
                .Include(m => m.MenuCategory)
                .Where(m => m.MenuCategory.RestaurantId == id)
                .ToListAsync();

            return Ok(new
            {
                success = true,
                items
            });
        }

        // PUT: api/MenuItems/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMenuItem(int id, [FromForm] MenuItemRequest request, IFormFile? imageFile)
        {
            Console.WriteLine("Id:" + id);
            Console.WriteLine("Id:" + request.MenuItemId);

            if (id != request.MenuItemId)
            {
                return BadRequest();
            }

            MenuItem newItem = await _context.MenuItem.FindAsync(id);

            if (newItem == null)
            {
                return NotFound();
            }
            string imageUrl = newItem.ImageUrl;

            if (imageFile != null && imageFile.Length > 0)
            {
                var uploadResult = await _cloudinaryService.UploadImageAsync(imageFile, "menu_items");
                imageUrl = uploadResult.SecureUrl.ToString();
            }

            newItem.ItemName = request.ItemName;
            newItem.Description = request.Description;
            newItem.MenuCategoryId = request.MenuCategoryId;
            newItem.Price = request.Price;
            newItem.ImageUrl = imageUrl;

            try
            {

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MenuItemExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok("Updated Successfully");
        }

        // POST: api/MenuItems
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<MenuItem>> PostMenuItem([FromForm] MenuItemRequest request, IFormFile imageFile)
        {
            try
            {
                string imageUrl = null;

                if (imageFile != null && imageFile.Length > 0)
                {
                    var uploadResult = await _cloudinaryService.UploadImageAsync(imageFile, "menu_items");
                    imageUrl = uploadResult.SecureUrl.ToString();
                }

                MenuItem newItem = new MenuItem
                {
                    ItemName = request.ItemName,
                    Description = request.Description,
                    Price = request.Price,
                    ImageUrl = imageUrl, // fallback if frontend sends URL directly
                    MenuCategoryId = request.MenuCategoryId
                };

                _context.MenuItem.Add(newItem);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetMenuItem", new { id = newItem.MenuItemId }, newItem);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        // DELETE: api/MenuItems/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMenuItem(int id)
        {
            var menuItem = await _context.MenuItem.FindAsync(id);
            if (menuItem == null)
            {
                return NotFound();
            }

            _context.MenuItem.Remove(menuItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MenuItemExists(int id)
        {
            return _context.MenuItem.Any(e => e.MenuItemId == id);
        }
    }
}
