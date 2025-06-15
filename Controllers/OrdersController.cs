using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging.Signing;
using RestaurantAPI.DTO;
using RestaurantAPI.ENUM;
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
            var orders = await _context.Order
                .Include(o => o.CustomerUser)
                .Include(o => o.Table)
                .Include(o => o.OrderItems)
                .ThenInclude(o => o.MenuItem)
                .Select(o => new
                {
                    o.OrderId,
                    o.Status,
                    o.TotalAmount,
                    o.OrderDate,
                    o.TableId,
                    o.Table.TableNumber,
                    o.CustomerUserId,
                    WaiterName = o.CustomerUser.FullName,
                    OrderItems = o.OrderItems.Select(oi => new
                    {
                        oi.OrderItemId,
                        oi.Quantity,
                        oi.Price,
                        oi.TotalPrice,
                        oi.Status,
                        oi.ItemId,
                        oi.MenuItem.ItemName,
                        oi.MenuItem.ImageUrl
                    })
                })
                .ToListAsync();

            return Ok(new { success = true, orders });
        }

        // GET: api/Orders/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrder(int id)
        {
            var order = await _context.Order
                .Include(o => o.CustomerUser)
                .Include(o => o.Table)
                .Include(o => o.OrderItems)
                .ThenInclude(o => o.MenuItem)
                .Where(o => o.OrderId == id)
                .Select(o => new
                {
                    o.OrderId,
                    o.Status,
                    o.TotalAmount,
                    o.OrderDate,
                    o.TableId,
                    o.Table.TableNumber,
                    o.CustomerUserId,
                    WaiterName = o.CustomerUser.FullName,
                    OrderItems = o.OrderItems.Select(oi => new
                    {
                        oi.OrderItemId,
                        oi.Quantity,
                        oi.Price,
                        oi.TotalPrice,
                        oi.Status,
                        oi.ItemId,
                        oi.MenuItem.ItemName,
                        oi.MenuItem.ImageUrl
                    })
                })
                .FirstOrDefaultAsync();

            if (order == null)
            {
                return NotFound();
            }

            return Ok(new { success = true, order });
        }

        [HttpGet("items/{id}")]
        public async Task<ActionResult<IEnumerable<OrderItem>>> GetOrderItems(int id)
        {
            var order = await _context.Order.Include(o => o.OrderItems).FirstOrDefaultAsync(o => o.OrderId == id);

            return Ok(order.OrderItems.ToList());
        }

        [HttpGet("restaurant/{rid}")]
        public async Task<ActionResult> GetAllOrdersOfRestaurant(int rid)
        {
            var orders = await _context.Order
                .Include(o => o.CustomerUser)
                .Include(o => o.Table)
                .ThenInclude(o => o.TableType)
                .Include(o => o.Restaurant)
                .Include(o => o.OrderItems)
                .ThenInclude(o => o.MenuItem)
                .Where(o => o.RestaurantId == rid && o.Status == OrderStatus.pending)
                .Select(o => new
                {
                    o.OrderId,
                    o.Status,
                    o.TotalAmount,
                    o.OrderDate,
                    o.CustomerUserId,
                    WaiterName = o.CustomerUser.FullName,
                    o.TableId,
                    o.Table.TableNumber,
                    o.Table.TableType.TypeName,
                    o.RestaurantId,
                    RestaurantName = o.Restaurant.Name,
                    OrderItems = o.OrderItems.Select(oi => new
                    {
                        oi.OrderItemId,
                        oi.Quantity,
                        oi.Price,
                        oi.TotalPrice,
                        oi.Status,
                        oi.ItemId,
                        oi.MenuItem.ItemName,
                        oi.MenuItem.ImageUrl
                    })
                })
                .ToListAsync();


            return Ok(new { success = true, orders });
        }

        [HttpPatch("UpdateStatus/{orderId}")]
        public async Task<ActionResult> UpdateOrderStatus(int orderId,[FromBody] OrderStatus status)
        {
            var order = await _context.Order
                .Include(o => o.OrderItems) // Include related order items
                .FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (order == null)
            {
                return NotFound();
            }

            // Ensure all items are served before allowing completion
            foreach (var item in order.OrderItems)
            {
                if (item.Status != OrderItemStatus.served)
                {
                    return BadRequest("Cannot complete order while there are pending items.");
                }
            }

            order.Status = status;

            // Free the table from active order
            var table = await _context.Tables.FindAsync(order.TableId);
            if (table != null)
            {
                table.HasActiveOrder = false;
                _context.Tables.Update(table);
            }

            _context.Order.Update(order);
            await _context.SaveChangesAsync();

            return Ok(new { success = true });
        }


        [HttpGet("waiter/{waiterId}/status/{st}")]
        public async Task<ActionResult> GetWaitersOrderByStatus(int waiterId, OrderStatus st)
        {
            var orders = await _context.Order
                .Include(o => o.CustomerUser)
                .Include(o => o.Table)
                .ThenInclude(o => o.TableType)
                .Include(o => o.Restaurant)
                .Include(o => o.OrderItems)
                .ThenInclude(o => o.MenuItem)
                .Where(o => o.CustomerUserId == waiterId && o.Status == st)
                .Select(o => new
                {
                    o.OrderId,
                    o.Status,
                    o.TotalAmount,
                    o.OrderDate,
                    o.CustomerUserId,
                    WaiterName = o.CustomerUser.FullName,
                    o.TableId,
                    o.Table.TableNumber,
                    o.Table.TableType.TypeName,
                    o.RestaurantId,
                    RestaurantName = o.Restaurant.Name,
                    OrderItems = o.OrderItems.Select(oi => new
                    {
                        oi.OrderItemId,
                        oi.Quantity,
                        oi.Price,
                        oi.TotalPrice,
                        oi.Status,
                        oi.ItemId,
                        oi.MenuItem.ItemName,
                        oi.MenuItem.ImageUrl
                    })
                })
                .ToListAsync();


            return Ok(new { success = true, orders });
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
                Status = OrderStatus.pending,
                OrderDate = DateTime.Now,
                TableId = o.TableId,
                CustomerUserId = o.CustomerUserId
            };

            _context.Order.Add(order);
            await _context.SaveChangesAsync();

            Table t = await _context.Tables.FindAsync(o.TableId);

            double totalAmount = 0;
            foreach (var item in o.Items)
            {
                MenuItem i = await _context.MenuItem.FindAsync(item.ItemId);
                var oItem = new OrderItem
                {
                    OrderId = order.OrderId,
                    ItemId = item.ItemId,
                    Price = i.Price,
                    Status = OrderItemStatus.pending,
                    Quantity = item.Quantity,
                    TotalPrice = i.Price * item.Quantity
                };
                totalAmount += oItem.TotalPrice;
                _context.OrderItem.Add(oItem);
            }
            t.HasActiveOrder = true;
            order.TotalAmount = totalAmount;
            _context.Order.Update(order);
            _context.Tables.Update(t);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetOrder", new { id = order.OrderId }, order);
        }

        [HttpPost("upsert")]
        public async Task<IActionResult> UpsertOrder(OrderRequest dto)
        {
            var order = await _context.Order
                .Include(o => o.OrderItems)
                .Include(o => o.Table)
                .FirstOrDefaultAsync(o => o.TableId == dto.TableId && o.Table.HasActiveOrder);

            Table t = await _context.Tables.FindAsync(dto.TableId);

            if (order != null)
            {
                foreach (var incoming in dto.Items)
                {
                    var pendingItem = order.OrderItems
                        .FirstOrDefault(i => i.ItemId == incoming.ItemId && i.Status == (int)OrderItemStatus.pending);

                    if (pendingItem != null)
                    {
                        pendingItem.Quantity += incoming.Quantity;
                        pendingItem.TotalPrice = pendingItem.Price * pendingItem.Quantity;
                    }
                    else
                    {
                        var menuItem = await _context.MenuItem.FindAsync(incoming.ItemId);
                        if (menuItem == null)
                            return BadRequest($"Invalid menu item ID: {incoming.ItemId}");

                        var newOrderItem = new OrderItem
                        {
                            OrderId = order.OrderId,
                            ItemId = incoming.ItemId,
                            Price = menuItem.Price,
                            Status = OrderItemStatus.pending,
                            Quantity = incoming.Quantity,
                            TotalPrice = menuItem.Price * incoming.Quantity
                        };

                        _context.OrderItem.Add(newOrderItem);
                    }
                }

                // Recalculate total amount
                order.TotalAmount = order.OrderItems.Sum(i => i.TotalPrice);
                t.HasActiveOrder = true;
                _context.Tables.Update(t);

                await _context.SaveChangesAsync();
                return Ok(new { success = true, order = order });
            }

            // Create new order
            var newOrderItems = new List<OrderItem>();
            double totalAmount = 0;

            foreach (var incoming in dto.Items)
            {
                var menuItem = await _context.MenuItem.FindAsync(incoming.ItemId);
                if (menuItem == null)
                    return BadRequest($"Invalid menu item ID: {incoming.ItemId}");

                var totalPrice = menuItem.Price * incoming.Quantity;
                totalAmount += totalPrice;

                newOrderItems.Add(new OrderItem
                {
                    ItemId = incoming.ItemId,
                    Price = menuItem.Price,
                    Quantity = incoming.Quantity,
                    Status = OrderItemStatus.pending,
                    TotalPrice = totalPrice
                });
            }

            var newOrder = new Order
            {
                TableId = dto.TableId,
                CustomerUserId = dto.CustomerUserId,
                OrderItems = newOrderItems,
                TotalAmount = totalAmount,
                RestaurantId = dto.RestaurantId,
                Status = OrderStatus.pending
            };

            _context.Order.Add(newOrder);
            t.HasActiveOrder = true;
            _context.Tables.Update(t);
            await _context.SaveChangesAsync();

            return Ok(new { success = true, order = newOrder });
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
