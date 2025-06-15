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
    public class WaiterMenusController : ControllerBase
    {
        private readonly RestaurantContext _context;
        private readonly IMapper _mapper;
        IConfiguration _configuration;

        public WaiterMenusController(RestaurantContext context, IConfiguration configuration, IMapper mapper)
        {
            _context = context;
            _configuration = configuration;
            _mapper = mapper;
        }

        [HttpPost("assignMenu")]
        public async Task<ActionResult> AssignMenuToWaiter(WaiterMenuRequest request)
        {
            WaiterMenu w = await _context.WaiterMenus
                .FirstOrDefaultAsync(wm => wm.WaiterId == request.WaiterId);

            if (w != null)
                return BadRequest("This waiter is already assigned with one menu.");

            WaiterMenu wm = new WaiterMenu
            {
                WaiterId = request.WaiterId,
                MenuId = request.MenuId
            };

            _context.WaiterMenus.Add(wm);
            await _context.SaveChangesAsync();

            return Ok(new { success = true, wm });
        }

        [HttpDelete("removeMenu/{waiterId}")]
        public async Task<ActionResult> RemoveMenuFromWaiter(int waiterId)
        {
            var waiterMenu = await _context.WaiterMenus
                .FirstOrDefaultAsync(wm => wm.WaiterId == waiterId);
            if (waiterMenu == null)
            {
                return NotFound("No menu assigned to this waiter.");
            }
            _context.WaiterMenus.Remove(waiterMenu);
            await _context.SaveChangesAsync();
            return Ok(new { success = true, message = "Menu removed from waiter successfully." });
        }

        [HttpGet]
        public async Task<ActionResult> GetAllWaiterMenus()
        {
            var waiterMenus = await _context.WaiterMenus
                .Include(wm => wm.Menu)
                .Include(wm => wm.Waiter)
                .Select(wm => new
                {
                    wm.WaiterMenuId,
                    wm.MenuId,
                    MenuName = wm.Menu.MenuName,
                    wm.WaiterId,
                    WaiterName = wm.Waiter.FullName
                })
                .ToListAsync();
            return Ok(new { success = true, waiterMenus });
        }

        [HttpGet("restaurant/{restaurantId}")]
        public async Task<ActionResult> GetWaiterMenusByRestaurantId(int restaurantId)
        {
            var waiterMenus = await _context.WaiterMenus
                .Include(wm => wm.Menu)
                .Include(wm => wm.Waiter)
                .Where(wm => wm.Waiter.RestaurantId == restaurantId)
                .Select(wm => new
                {
                    wm.WaiterMenuId,
                    wm.MenuId,
                    MenuName = wm.Menu.MenuName,
                    wm.WaiterId,
                    WaiterName = wm.Waiter.FullName
                })
                .ToListAsync();
            return Ok(new { success = true, waiterMenus });
        }

        [HttpGet("waiter/{waiterId}")]
        public async Task<ActionResult> GetWaiterMenuByWaiterId(int waiterId)
        {
            var waiterMenu = await _context.WaiterMenus
                .Include(wm => wm.Menu)
                .Include(wm => wm.Waiter)
                .Where(wm => wm.WaiterId == waiterId)
                .Select(wm => new
                {
                    wm.WaiterMenuId,
                    wm.MenuId,
                    MenuName = wm.Menu.MenuName,
                    wm.WaiterId,
                    WaiterName = wm.Waiter.Email
                })
                .FirstOrDefaultAsync();
            if (waiterMenu == null)
            {
                return NotFound("No menu assigned to this waiter.");
            }
            return Ok(new { success = true, waiterMenu });
        }
    }
}
