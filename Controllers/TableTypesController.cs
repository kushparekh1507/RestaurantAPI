using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantAPI.DTO;
using RestaurantAPI.Models;

namespace RestaurantAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TableTypesController : ControllerBase
    {
        private readonly RestaurantContext _context;

        public TableTypesController(RestaurantContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TableType>>> GetTableTypes()
        {
            return await _context.TableTypes.Include(t => t.Tables).ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TableType>> GetTableYype(int id)
        {
            var tableType = await _context.TableTypes.Include(t => t.Tables).FirstOrDefaultAsync(t => t.TableTypeId == id);

            if (tableType == null)
            {
                return NotFound();
            }

            return tableType;
        }

        [HttpPost]
        public async Task<ActionResult> CreateTableType(TableTypeRequest request)
        {
            TableType type = new TableType
            {
                TypeName = request.TypeName,
            };

            _context.TableTypes.Add(type);
            await _context.SaveChangesAsync();

            return Ok(new { success = true, type });
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateTableType(int id, TableTypeRequest request)
        {
            var tableType = await _context.TableTypes.FindAsync(id);
            tableType.TypeName = request.TypeName;

            await _context.SaveChangesAsync();

            return Ok(new { success = true, tableType });
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteTableType(int id)
        {
            var tableType = await _context.TableTypes.FindAsync(id);

            _context.TableTypes.Remove(tableType);
            await _context.SaveChangesAsync();

            return Ok(new { success = true });
        }
    }
}
