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
    public class MenuItemsController : ControllerBase
    {
        private readonly RestaurantContext _context;
        private readonly IMapper _mapper;
        IConfiguration _configuration;

        public MenuItemsController(RestaurantContext context, IMapper mapper,IConfiguration configuration)
        {
            _context = context;
            _mapper = mapper;
            _configuration = configuration;
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

        // PUT: api/MenuItems/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMenuItem(int id, MenuItemRequest menuItem)
        {
            if (id != menuItem.MenuItemId)
            {
                return BadRequest();
            }

            var newItem=await _context.MenuItem.FindAsync(menuItem.MenuItemId);

            if (newItem == null)
            {
                return NotFound();
            }

            try
            {
                _mapper.Map(menuItem, newItem);
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
        public async Task<ActionResult<MenuItem>> PostMenuItem(MenuItemRequest request)
        {
            try
            {
                var newItem = _mapper.Map<MenuItemRequest, MenuItem>(request);
                _context.MenuItem.Add(newItem);
                await _context.SaveChangesAsync();
                return CreatedAtAction("GetMenuItem", new { id = newItem.MenuItemId }, newItem);
            }
            catch(Exception ex)
            {
                return BadRequest(ex);
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
