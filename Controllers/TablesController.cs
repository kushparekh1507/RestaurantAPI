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
                .Where(t => t.RestaurantId == rid)
                .ToListAsync();


            return Ok(new
            {
                success = true,
                tables
            });
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
            Table table = new Table
            {
                TableNumber = request.TableNumber,
                Capacity = request.Capacity,
                RestaurantId = request.RestaurantId
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
