namespace WebApp.Models;

public class SalaryInputModel
{
    public decimal? NetSalary { get; set; }
    public decimal? GrossSalary { get; set; }
    public decimal? EmployerCost { get; set; }

    public int PensionPercent { get; set; } = 2;
    public bool IncludeUnemploymentInsurance { get; set; } = true;
    public bool UseTaxFreeIncome { get; set; } = true;
}