using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ASP.NET_Core_Web_API_project.Models;
using ASP.NET_Core_Web_API_project.Data;

namespace ASP.NET_Core_Web_API_project.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly ILogger<OrdersController> _logger;
        private readonly MyDbContext _context;

        public OrdersController(ILogger<OrdersController> logger, MyDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        // GET: /orders
        [HttpGet]
        public IEnumerable<Order> Get()
        {
            return _context.Orders.ToList();
        }

        // GET /orders/5
        [HttpGet("{id}")]
        public ActionResult<Order> GetById(int id)
        {
            var order = _context.Orders.Find(id);

            if (order == null)
            {
                return NotFound();
            }

            return order;
        }

        [HttpGet("customerOrders/{customerId}")] //Show orders of a specific Customer by Customer ID
        public IActionResult GetCustomerOrders(int customerId)
        {
            var orders = _context.Orders.Where(o => o.CustomerID == customerId);

            if (!orders.Any())
            {
                return NotFound($"No orders found for customer with ID {customerId}");
            }

            return Ok(orders.ToList());
        }

        // POST /orders
        [HttpPost]
        public ActionResult<Order> Create(Order order)
        {
            _context.Orders.Add(order);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetById), new { id = order.OrderID }, order);
        }

        // PUT /orders/5
        [HttpPut("{id}")]
        public IActionResult Update(int id, Order updatedOrder)
        {
            var order = _context.Orders.Find(id);

            if (order == null)
            {
                return NotFound();
            }

            order.CustomerID = updatedOrder.CustomerID;
            order.OrderDate = updatedOrder.OrderDate;
            order.TotalAmount = updatedOrder.TotalAmount;

            _context.SaveChanges();

            return NoContent();
        }

        // DELETE /orders/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var order = _context.Orders.Find(id);

            if (order == null)
            {
                return NotFound();
            }

            _context.Orders.Remove(order);
            _context.SaveChanges();

            return NoContent();
        }
    }
}
