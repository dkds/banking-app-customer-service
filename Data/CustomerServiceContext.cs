using CustomerService.Models;
using Microsoft.EntityFrameworkCore;

namespace CustomerService.Data
{
    public class CustomerServiceContext : DbContext
    {
        public CustomerServiceContext(DbContextOptions<CustomerServiceContext> options)
            : base(options)
        {
        }

        public DbSet<Customer> Customer { get; set; } = default!;

        public DbSet<Account> Account { get; set; } = default!;

    }
}
