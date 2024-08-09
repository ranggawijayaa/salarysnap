namespace SalarySnapApi
{
    public class Salary
    {
        public int Id { get; set; }
        public string Company { get; set; }
        public string Role { get; set; }
        public int YearsOfExperience { get; set; }
        public decimal Salaries { get; set; }
        public decimal? YearlyBonus { get; set; }
    }
}
