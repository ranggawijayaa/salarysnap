using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace SalarySnapApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SalariesController : ControllerBase
    {
        private readonly SalaryContext _context;

        public SalariesController(SalaryContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Salary>>> GetSalaries(
            [FromQuery] int page = 1,
            [FromQuery] int per_page = 20,
            [FromQuery] string sortBy = "",
            [FromQuery] bool sortDesc = false)
        {
            try
            {
                var query = _context.Salaries.AsQueryable();

                //sorting
                if (!string.IsNullOrEmpty(sortBy))
                {
                    if (sortDesc)
                    {
                        query = query.OrderByDescending(e => EF.Property<object>(e, sortBy));
                    }
                    else
                    {
                        query = query.OrderBy(e => EF.Property<object>(e, sortBy));
                    }
                }

                //pagination
                var totalItems = await query.CountAsync();
                var salaries = await query
                    .Skip((page - 1) * per_page)
                    .Take(per_page)
                    .ToListAsync();

                //return the paginated and sorted results
                return Ok(new
                {
                    items = salaries,
                    totalItems = totalItems
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
            
        }

        [HttpPost]
        public async Task<ActionResult<Salary>> PostSalary(Salary salary)
        {
            _context.Salaries.Add(salary);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSalaries), new {id = salary.Id}, salary);
        }
    }
}
