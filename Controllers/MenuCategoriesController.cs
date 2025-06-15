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
            var categories = await _context.MenuCategories
                .Include(m => m.Restaurant)
                .Include(m => m.MenuItems)
                .Include(m => m.Menu)
                .ThenInclude(m => m.TableType)
                .Select(m => new
                {
                    m.MenuCategoryId,
                    m.CategoryName,
                    m.Description,
                    m.RestaurantId,
                    RestaurantName = m.Restaurant.Name,
                    m.MenuId,
                    MenuName = m.Menu.MenuName,
                    m.CreatedAt,
                    MenuItems = m.MenuItems.Select(mi => new
                    {
                        mi.MenuItemId,
                        mi.ItemName,
                        mi.Price,
                        mi.Description,
                        mi.ImageUrl
                    }).ToList()
                })
                .ToListAsync();

            return Ok(new { success = true, categories });
        }

        // GET: api/MenuCategories/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MenuCategory>> GetMenuCategory(int id)
        {
            var menuCategory = await _context.MenuCategories
                .Include(m => m.Restaurant)
                .Include(m => m.MenuItems)
                .Include(m => m.Menu)
                .ThenInclude(m => m.TableType)
                .Select(m => new
                {
                    m.MenuCategoryId,
                    m.CategoryName,
                    m.Description,
                    m.RestaurantId,
                    RestaurantName = m.Restaurant.Name,
                    m.MenuId,
                    MenuName = m.Menu.MenuName,
                    m.CreatedAt,
                    MenuItems = m.MenuItems.Select(mi => new
                    {
                        mi.MenuItemId,
                        mi.ItemName,
                        mi.Price,
                        mi.Description,
                        mi.ImageUrl
                    }).ToList()
                })
                .FirstOrDefaultAsync(m => m.MenuCategoryId == id);

            if (menuCategory == null)
            {
                return NotFound();
            }

            return Ok(new { success = true, menuCategory });
        }

        [HttpGet("restaurant/{rid}")]
        public async Task<ActionResult<IEnumerable<MenuCategory>>> GetCategoriesOfRestaurant(int rid)
        {
            //var restaurant = await _context.Restaurant.Include(r => r.MenuCategories).FirstOrDefaultAsync(r => r.RestaurantId == rid);
            //if (restaurant == null)
            //{
            //    return NotFound();
            //}

            var categories = await _context.MenuCategories
                .Include(m => m.Restaurant)
                .Include(m => m.MenuItems)
                .Include(m => m.Menu)
                .ThenInclude(m => m.TableType)
                .Select(m => new
                {
                    m.MenuCategoryId,
                    m.CategoryName,
                    m.Description,
                    m.RestaurantId,
                    RestaurantName = m.Restaurant.Name,
                    m.MenuId,
                    MenuName = m.Menu.MenuName,
                    m.CreatedAt,
                    MenuItems = m.MenuItems.Select(mi => new
                    {
                        mi.MenuItemId,
                        mi.ItemName,
                        mi.Price,
                        mi.Description,
                        mi.ImageUrl
                    }).ToList()
                })
                .Where(m => m.RestaurantId == rid)
                .ToListAsync();

            return Ok(new { success = true, categories });
        }

        // PUT: api/MenuCategories/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMenuCategory(int id, MenuCategoryRequest updatedMenu)
        {
            if (id != updatedMenu.MenuCategoryId)
            {
                return BadRequest();
            }

            MenuCategory existingMenu = await _context.MenuCategories.FindAsync(id);

            if (existingMenu == null)
            {
                return NotFound();
            }

            existingMenu.CategoryName = updatedMenu.CategoryName;
            existingMenu.Description = updatedMenu.Description;
            existingMenu.RestaurantId = updatedMenu.RestaurantId;
            existingMenu.MenuId = updatedMenu.MenuId;

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
                MenuCategory newCat = new MenuCategory
                {
                    CategoryName = request.CategoryName,
                    Description = request.Description,
                    RestaurantId = request.RestaurantId,
                    MenuId = request.MenuId
                };

                _context.MenuCategories.Add(newCat);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    Message = "Menu Category Created Successfully",
                    MenuCategory = newCat,
                    success = true
                });
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

            return Ok(new
            {
                Message = "Menu Category Deleted Successfully",
                MenuCategoryId = menuCategory.MenuCategoryId,
                success = true
            });
        }

        private bool MenuCategoryExists(int id)
        {
            return _context.MenuCategories.Any(e => e.MenuCategoryId == id);
        }
    }
}
