using System.ComponentModel.DataAnnotations;

namespace SalarySnapApi
{
    public class Salary
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(100)]
        public string Company { get; set; }
        [Required]
        [MaxLength(100)]
        public string Role { get; set; }
        [Required]
        [Range(0, 50)]
        public int YearsOfExperience { get; set; }
        [Required]
        [Range(0, 1000000)]
        public decimal Salaries { get; set; }
        [Range(0, 1000000)]
        public decimal? YearlyBonus { get; set; }
    }
}
