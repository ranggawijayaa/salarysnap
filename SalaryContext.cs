using Microsoft.EntityFrameworkCore;

namespace SalarySnapApi
{
    public class SalaryContext : DbContext
    {
        public SalaryContext(DbContextOptions<SalaryContext> options) : base(options) { }

        public DbSet<Salary> Salaries { get; set; }
    }
}
