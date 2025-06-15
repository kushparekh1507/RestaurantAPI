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
    public class MenusController : ControllerBase
    {
        private readonly RestaurantContext _context;
        private readonly IMapper _mapper;
        IConfiguration _configuration;

        public MenusController(RestaurantContext context, IConfiguration configuration, IMapper mapper)
        {
            _context = context;
            _configuration = configuration;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult> GetAllMenus()
        {
            var menus = await _context.Menus
                .Include(m => m.Restaurant)
                .Include(m => m.TableType)
                .Select(m => new
                {
                    m.MenuId,
                    m.MenuName,
                    m.RestaurantId,
                    RestaurantName = m.Restaurant.Name,
                    m.TableTypeId,
                    TableTypeName = m.TableType.TypeName
                })
                .ToListAsync();

            return Ok(new { success = true, menus });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetMenuById(int id)
        {
            var menu = await _context.Menus
                .Include(m => m.Restaurant)
                .Include(m => m.TableType)
                .Select(m => new
                {
                    m.MenuId,
                    m.MenuName,
                    m.RestaurantId,
                    RestaurantName = m.Restaurant.Name,
                    m.TableTypeId,
                    TableTypeName = m.TableType.TypeName
                })
                .FirstOrDefaultAsync(m => m.MenuId == id);

            return Ok(new { success = true, menu });
        }

        [HttpGet("restaurant/{id}")]
        public async Task<ActionResult> GetAllMenusOfRestaurant(int id)
        {
            var menus = await _context.Menus
                .Include(m => m.Restaurant)
                .Include(m => m.TableType)
                .Select(m => new
                {
                    m.MenuId,
                    m.MenuName,
                    m.RestaurantId,
                    RestaurantName = m.Restaurant.Name,
                    m.TableTypeId,
                    TableTypeName = m.TableType.TypeName
                })
                .Where(m => m.RestaurantId == id)
                .ToListAsync();

            return Ok(new { success = true, menus });
        }

        [HttpPost]
        public async Task<ActionResult> CreateMenu(MenuRequest request)
        {
            Menu m = await _context.Menus
                .FirstOrDefaultAsync(me => me.RestaurantId == request.RestaurantId && me.TableTypeId == request.TableTypeId);

            if (m != null)
                return BadRequest("Menu already exists for this Table type and Restaurant");

            var menu = new Menu
            {
                MenuName = request.MenuName,
                RestaurantId = request.RestaurantId,
                TableTypeId = request.TableTypeId
            };
            _context.Menus.Add(menu);
            await _context.SaveChangesAsync();
            return Ok(new { success = true, menu });
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateMenu(int id, MenuRequest request)
        {
            if (id != request.MenuId)
            {
                return BadRequest("Menu ID mismatch.");
            }
            var menu = await _context.Menus.FindAsync(id);
            if (menu == null)
            {
                return NotFound("Menu not found.");
            }
            menu.MenuName = request.MenuName;
            menu.RestaurantId = request.RestaurantId;
            menu.TableTypeId = request.TableTypeId;

            await _context.SaveChangesAsync();
            return Ok(new { success = true, menu });
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteMenu(int id)
        {
            var menu = await _context.Menus.FindAsync(id);
            if (menu == null)
            {
                return NotFound("Menu not found.");
            }
            _context.Menus.Remove(menu);
            await _context.SaveChangesAsync();
            return Ok(new { success = true, message = "Menu deleted successfully." });
        }
    }
}
