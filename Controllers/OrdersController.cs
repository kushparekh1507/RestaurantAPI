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
    public class OrdersController : ControllerBase
    {
        private readonly RestaurantContext _context;

        public OrdersController(RestaurantContext context)
        {
            _context = context;
        }

        // GET: api/Orders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrder()
        {
            return await _context.Order.Include(o => o.CustomerUser).Include(o => o.Table).Include(o => o.OrderItems).ToListAsync();
        }

        // GET: api/Orders/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrder(int id)
        {
            var order = await _context.Order.Include(o => o.CustomerUser).Include(o => o.Table).Include(o=>o.OrderItems).FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null)
            {
                return NotFound();
            }

            return order;
        }

        [HttpGet("items/{id}")]
        public async Task<ActionResult<IEnumerable<OrderItem>>> GetOrderItems(int id)
        {
            var order = await _context.Order.Include(o => o.OrderItems).FirstOrDefaultAsync(o => o.OrderId == id);

            return Ok(order.OrderItems.ToList());
        }

        // PUT: api/Orders/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrder(int id, Order order)
        {
            if (id != order.OrderId)
            {
                return BadRequest();
            }

            _context.Entry(order).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(id))
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

        // POST: api/Orders
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Order>> PostOrder(OrderRequest o)
        {
            Order order = new Order
            {
                Status = "Pending",
                OrderDate = DateTime.Now,
                TableId = o.TableId,
                CustomerUserId = o.CustomerUserId
            };

            _context.Order.Add(order);
            await _context.SaveChangesAsync();

            double totalAmount = 0;
            foreach(var item in o.Items)
            {
                MenuItem i = await _context.MenuItem.FindAsync(item.ItemId);
                var oItem = new OrderItem
                {
                    OrderId = order.OrderId,
                    ItemId = item.ItemId,
                    Price = i.Price,
                    Status = "Pending",
                    Quantity = item.Quantity,
                    TotalPrice = i.Price * item.Quantity
                };
                totalAmount += oItem.TotalPrice;
                _context.OrderItem.Add(oItem);
            }
            order.TotalAmount = totalAmount;
            _context.Order.Update(order);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetOrder", new { id = order.OrderId }, order);
        }

        // DELETE: api/Orders/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var order = await _context.Order.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            _context.Order.Remove(order);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool OrderExists(int id)
        {
            return _context.Order.Any(e => e.OrderId == id);
        }
    }
}
