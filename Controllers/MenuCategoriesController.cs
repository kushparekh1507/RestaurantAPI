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

namespace RestaurantAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MenuCategoriesController : ControllerBase
    {
        private readonly RestaurantContext _context;
        private readonly IMapper _mapper;
        IConfiguration _configuration;

        public MenuCategoriesController(RestaurantContext context, IConfiguration configuration, IMapper mapper)
        {
            _context = context;
            _configuration = configuration;
            _mapper = mapper;
        }

        // GET: api/MenuCategories
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MenuCategory>>> GetMenuCategories()
        {
            return await _context.MenuCategories.ToListAsync();
        }

        // GET: api/MenuCategories/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MenuCategory>> GetMenuCategory(int id)
        {
            var menuCategory = await _context.MenuCategories.Include(r => r.Restaurant).FirstOrDefaultAsync(r => r.MenuCategoryId == id);

            if (menuCategory == null)
            {
                return NotFound();
            }

            return menuCategory;
        }

        // PUT: api/MenuCategories/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMenuCategory(int id, MenuCategory menuCategory)
        {
            if (id != menuCategory.MenuCategoryId)
            {
                return BadRequest();
            }

            _context.Entry(menuCategory).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MenuCategoryExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/MenuCategories
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<MenuCategory>> PostMenuCategory(MenuCategoryRequest request)
        {
            try
            {
                var newCat = _mapper.Map<MenuCategoryRequest, MenuCategory>(request);

                _context.MenuCategories.Add(newCat);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetMenuCategory", new { id = newCat.MenuCategoryId }, newCat);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        // DELETE: api/MenuCategories/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMenuCategory(int id)
        {
            var menuCategory = await _context.MenuCategories.FindAsync(id);
            if (menuCategory == null)
            {
                return NotFound();
            }

            _context.MenuCategories.Remove(menuCategory);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MenuCategoryExists(int id)
        {
            return _context.MenuCategories.Any(e => e.MenuCategoryId == id);
        }
    }
}
