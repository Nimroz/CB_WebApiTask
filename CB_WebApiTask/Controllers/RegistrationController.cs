using CB_WebApiTask.Data;
using CB_WebApiTask.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CB_WebApiTask.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegistrationController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        public RegistrationController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        //Only For Testing
        [HttpGet]
        public IActionResult GetAllCstomers()
        {
            var customers = _dbContext.Customers.ToList();
            return Ok(customers);
        }
    }
}
