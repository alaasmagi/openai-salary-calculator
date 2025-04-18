namespace WebApp.Models;

public class SalaryResultModel
{
    public decimal NetSalary { get; set; }
    public decimal GrossSalary { get; set; }
    public decimal EmployerCost { get; set; }

    public string? AiComment { get; set; } 
}