using CB_WebApiTask.Models;
using Microsoft.EntityFrameworkCore;

namespace CB_WebApiTask.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        public DbSet<Customer> Customers { get; set; }

    }
}
