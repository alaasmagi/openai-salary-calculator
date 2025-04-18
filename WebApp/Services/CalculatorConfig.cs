namespace WebApp.Services;

public class CalculatorConfig
{
    static CalculatorConfig()
    {
        DotNetEnv.Env.Load();
    }

    public static string Currency => GetString("CURRENCY");
    public static decimal SocialTax => GetDecimal("SOCIAL_TAX");
    public static decimal IncomeTax => GetDecimal("INCOME_TAX");
    public static decimal UnemploymentEmployee => GetDecimal("UNEMPLOYMENT_INSURANCE_EMPLOYEE");
    public static decimal UnemploymentEmployer => GetDecimal("UNEMPLOYMENT_INSURANCE_EMPLOYER");
    public static decimal MandatoryPension => GetDecimal("MANDATORY_PENSION_PERCENT");
    public static decimal MinimumTaxFreeGrossIncome => GetDecimal("MINIMUM_TAX_FREE_GROSS_INCOME");
    public static decimal MaximumTaxFreeGrossIncome => GetDecimal("MAXIMUM_TAX_FREE_GROSS_INCOME");
    public static decimal BaseTaxFreeIncome => GetDecimal("BASE_TAX_FREE_INCOME");

    private static decimal GetDecimal(string key)
    {
        var value = Environment.GetEnvironmentVariable(key);
        return decimal.TryParse(value, out var result) ? result : 0;
    }
    
    private static string GetString(string key)
    {
        var value = Environment.GetEnvironmentVariable(key);
        return value ?? string.Empty;
    }
}