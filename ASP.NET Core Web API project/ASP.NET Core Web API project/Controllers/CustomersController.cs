using System.Collections.Generic;
using System.Linq;
using ASP.NET_Core_Web_API_project.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ASP.NET_Core_Web_API_project.Models;

namespace ASP.NET_Core_Web_API_project.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CustomersController : ControllerBase
    {
        private readonly ILogger<CustomersController> _logger;
        private readonly MyDbContext _context;

        public CustomersController(ILogger<CustomersController> logger, MyDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet]
        public IEnumerable<Customer> Get()
        {
            return _context.Customers.ToList();
        }

        [HttpGet("{id}")]
        public Customer GetById(int id)
        {
            return _context.Customers.FirstOrDefault(c => c.CustomerID == id);
        }


        [HttpGet("allData")] //A METHOD FOR INNER JOIN QUERY 
        public IActionResult GetAllData()
        {
            var query = from c in _context.Customers
                        join o in _context.Orders on c.CustomerID equals o.CustomerID
                        select new { c.CustomerID, c.Name, c.Address, c.Phone, o.OrderID, o.OrderDate, o.TotalAmount };

            return Ok(query.ToList());
        }


        [HttpPost]
        public IActionResult Create(Customer customer)
        {
            _context.Customers.Add(customer);
            _context.SaveChanges();
            return CreatedAtAction(nameof(GetById), new { id = customer.CustomerID }, customer);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, Customer customer)
        {
            var existingCustomer = _context.Customers.FirstOrDefault(c => c.CustomerID == id);
            if (existingCustomer == null)
            {
                return NotFound();
            }
            existingCustomer.Name = customer.Name;
            existingCustomer.Address = customer.Address;
            existingCustomer.Phone = customer.Phone;
            _context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var customer = _context.Customers.FirstOrDefault(c => c.CustomerID == id);
            if (customer == null)
            {
                return NotFound();
            }
            _context.Customers.Remove(customer);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
