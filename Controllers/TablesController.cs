using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantAPI.DTO;
using RestaurantAPI.Models;

namespace RestaurantAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TablesController : ControllerBase
    {
        private readonly RestaurantContext _context;

        public TablesController(RestaurantContext context)
        {
            _context = context;
        }

        // GET: api/Tables
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Table>>> GetTables()
        {
            return await _context.Tables.Include(t => t.Restaurant).ToListAsync();
        }

        // GET: api/Tables/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Table>> GetTable(int id)
        {
            var table = await _context.Tables.Include(t => t.Restaurant).FirstOrDefaultAsync(t => t.TableId == id);

            if (table == null)
            {
                return NotFound();
            }

            return table;
        }

        [HttpGet("restaurant/{rid}")]
        public async Task<ActionResult<IEnumerable<Table>>> GetTablesByRestaurant(int rid)
        {
            var tables = await _context.Tables
                .Include(t => t.Restaurant)
                .Include(t => t.TableType)
                .Where(t => t.RestaurantId == rid)
                .Select(t => new
                {
                    t.TableId,
                    t.TableNumber,
                    t.Capacity,
                    t.RestaurantId,
                    t.TableTypeId,
                    TableTypeName = t.TableType.TypeName,
                    CreatedAt = t.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"),
                    t.HasActiveOrder
                })
                .ToListAsync();


            return Ok(new
            {
                success = true,
                tables
            });
        }

        [HttpGet("restaurant/hasOrder/{rid}")]
        public async Task<ActionResult<IEnumerable<Table>>> GetOrdersTablesByRestaurant(int rid)
        {
            var tables = await _context.Tables
                .Include(t => t.Restaurant)
                .Include(t => t.TableType)
                .Where(t => t.RestaurantId == rid && t.HasActiveOrder == true)
                .Select(t => new
                {
                    t.TableId,
                    t.TableNumber,
                    t.Capacity,
                    t.RestaurantId,
                    t.TableTypeId,
                    TableTypeName = t.TableType.TypeName,
                    CreatedAt = t.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"),
                    t.HasActiveOrder
                })
                .ToListAsync();


            return Ok(new
            {
                success = true,
                tables
            });
        }

        [HttpGet("waiter/{waiterId}")]
        public async Task<ActionResult<IEnumerable<Table>>> GetTablesByWaiter(int waiterId)
        {
            var waiterMenu = await _context.WaiterMenus
                .Include(wm => wm.Menu)
                .FirstOrDefaultAsync(wm => wm.WaiterId == waiterId);

            if (waiterMenu == null)
                return NotFound(new { message = "No menu assigned to this waiter." });

            var tableTypeId = waiterMenu.Menu.TableTypeId;

            var tables = await _context.Tables
                .Where(t => t.TableTypeId == tableTypeId)
                .Select(t => new
                {
                    t.TableId,
                    t.TableNumber,
                    t.Capacity,
                    t.RestaurantId,
                    t.TableTypeId,
                    TableTypeName = t.TableType.TypeName,
                    RestaurantName = t.Restaurant.Name,
                    CreatedAt = t.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"),
                    t.HasActiveOrder
                })
                .ToListAsync();

            return Ok(new { tables });
        }

        // PUT: api/Tables/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        // PUT: api/Tables/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTable(int id, TableRequest request)
        {
            if (id != request.TableId)
            {
                return BadRequest("Table ID mismatch.");
            }

            var existingTable = await _context.Tables.FindAsync(id);
            if (existingTable == null)
            {
                return NotFound();
            }

            existingTable.TableNumber = request.TableNumber;
            existingTable.Capacity = request.Capacity;
            existingTable.RestaurantId = request.RestaurantId;
            existingTable.TableTypeId = request.TableTypeId;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TableExists(id))
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


        // POST: api/Tables
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Table>> PostTable(TableRequest request)
        {
            Table t = await _context.Tables.FirstOrDefaultAsync(ta => ta.TableNumber == request.TableNumber && ta.TableTypeId == request.TableTypeId);
            if (t != null)
                return BadRequest("Table Number already exists");

            Table table = new Table
            {
                TableNumber = request.TableNumber,
                Capacity = request.Capacity,
                RestaurantId = request.RestaurantId,
                TableTypeId = request.TableTypeId,
                HasActiveOrder = false,
            };
            _context.Tables.Add(table);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTable", new { id = table.TableId }, table);
        }

        // DELETE: api/Tables/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTable(int id)
        {
            var table = await _context.Tables.FindAsync(id);
            if (table == null)
            {
                return NotFound();
            }

            _context.Tables.Remove(table);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TableExists(int id)
        {
            return _context.Tables.Any(e => e.TableId == id);
        }
    }
}
