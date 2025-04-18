using WebApp.Models;

namespace WebApp.Services;

public class SalaryCalculatorService
{
    public SalaryResultModel Calculate(SalaryInputModel input)
    {
        decimal gross = 0;
        decimal net = 0;
        decimal employer = 0;

        decimal pension = input.PensionPercent / 100m;
        decimal unemploymentEmployee = input.IncludeUnemploymentInsurance ? CalculatorConfig.UnemploymentEmployee : 0m;
        decimal unemploymentEmployer = input.IncludeUnemploymentInsurance ? CalculatorConfig.UnemploymentEmployer : 0m;
        decimal socialTax = CalculatorConfig.SocialTax;
        decimal incomeTaxRate = CalculatorConfig.IncomeTax;

        if (input.NetSalary.HasValue)
        {
            net = input.NetSalary.Value;

            decimal deductions = pension + unemploymentEmployee + incomeTaxRate;
            gross = net / (1 - deductions);

            employer = gross * (1 + socialTax + unemploymentEmployer);
        }
        else if (input.GrossSalary.HasValue)
        {
            gross = input.GrossSalary.Value;

            decimal taxFree = input.UseTaxFreeIncome ? CalculateMonthlyTaxFree(gross) : 0m;

            decimal taxableIncome = Math.Max(0, gross - taxFree);
            decimal incomeTax = taxableIncome * incomeTaxRate;

            decimal netPercent = 1 - pension - unemploymentEmployee;
            net = gross * netPercent - incomeTax;

            employer = gross * (1 + socialTax + unemploymentEmployer);
        }
        else if (input.EmployerCost.HasValue)
        {
            employer = input.EmployerCost.Value;
            gross = employer / (1 + socialTax + unemploymentEmployer);

            decimal taxFree = input.UseTaxFreeIncome ? CalculateMonthlyTaxFree(gross) : 0m;

            decimal taxableIncome = Math.Max(0, gross - taxFree);
            decimal incomeTax = taxableIncome * incomeTaxRate;

            decimal netPercent = 1 - pension - unemploymentEmployee;
            net = gross * netPercent - incomeTax;
        }

        return new SalaryResultModel
        {
            NetSalary = Math.Round(net, 2),
            GrossSalary = Math.Round(gross, 2),
            EmployerCost = Math.Round(employer, 2)
        };
    }

    private decimal CalculateMonthlyTaxFree(decimal gross)
    {
        var minTaxFreeGrossIncome = CalculatorConfig.MinimumTaxFreeGrossIncome;
        var maxTaxFreeGrossIncome = CalculatorConfig.MaximumTaxFreeGrossIncome;
        var baseTaxFreeIncome = CalculatorConfig.BaseTaxFreeIncome;

        if (gross <= minTaxFreeGrossIncome)
            return baseTaxFreeIncome;
        if (gross >= maxTaxFreeGrossIncome)
            return 0;

        var reductionRate = baseTaxFreeIncome / (maxTaxFreeGrossIncome - minTaxFreeGrossIncome);
        return baseTaxFreeIncome - reductionRate * (gross - minTaxFreeGrossIncome);
    }
}