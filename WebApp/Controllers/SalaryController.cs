using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Services;

namespace WebApp.Controllers;

public class SalaryController : Controller
{
    private readonly SalaryCalculatorService _calculator;
    private readonly OpenAiService _aiService;

    public SalaryController(SalaryCalculatorService calculator, OpenAiService aiService)
    {
        _calculator = calculator;
        _aiService = aiService;
    }

    [HttpGet]
    public IActionResult Index() => View(new SalaryInputModel());

    [HttpPost]
    public async Task<IActionResult> Index(SalaryInputModel input)
    {
        var result = _calculator.Calculate(input);
        
        try
        {
            result.AiComment = await _aiService.GetSalaryComment(input.OpenAiApiKey, result.NetSalary);
        }
        catch
        {
            result.AiComment = "AI hinnangut ei õnnestunud hetkel saada.";
        }
        
        return View("Result", result);
    }
}