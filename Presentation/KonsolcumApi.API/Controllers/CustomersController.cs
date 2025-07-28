using KonsolcumApi.Application.Repositories;
using KonsolcumApi.Domain.Entities;
using KonsolcumApi.Persistence.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KonsolcumApi.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerRepository _customerRepository;

        public CustomersController(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        // GET: api/Customers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Customer>>> GetAllCustomers()
        {
            var customers = await _customerRepository.GetAllAsync();
            return Ok(customers);
        }

        // GET: api/Customers/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Customer>> GetCustomerById(Guid id)
        {
            var customer = await _customerRepository.GetByIdAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            return Ok(customer);
        }

        // GET: api/Customers/by-email/{email}
        [HttpGet("by-email/{email}")]
        public async Task<ActionResult<Customer>> GetCustomerByEmail(string email)
        {
            var customer = await _customerRepository.GetByEmailAsync(email);
            if (customer == null)
            {
                return NotFound($"Customer with email '{email}' not found.");
            }
            return Ok(customer);
        }

        // GET: api/Customers/email-exists/{email}
        [HttpGet("email-exists/{email}")]
        public async Task<ActionResult<bool>> EmailExists(string email)
        {
            var exists = await _customerRepository.EmailExistsAsync(email);
            return Ok(exists);
        }

        // POST: api/Customers
        [HttpPost]
        public async Task<ActionResult<Customer>> CreateCustomer(Customer customer)
        {
            var createdCustomer = await _customerRepository.CreateAsync(customer);
            return CreatedAtAction(nameof(GetCustomerById), new { id = createdCustomer.Id }, createdCustomer);
        }

        // PUT: api/Customers/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCustomer(Guid id, Customer customer)
        {
            if (id != customer.Id)
            {
                return BadRequest("Customer ID mismatch.");
            }

            var existingCustomer = await _customerRepository.GetByIdAsync(id);
            if (existingCustomer == null)
            {
                return NotFound();
            }

            var updatedCustomer = await _customerRepository.UpdateAsync(customer);
            return Ok(updatedCustomer);
        }

        // DELETE: api/Customers/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(Guid id)
        {
            var success = await _customerRepository.DeleteAsync(id);
            if (!success)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
