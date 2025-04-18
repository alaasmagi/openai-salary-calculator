using DotNetEnv;
using WebApp.Services;

Env.Load();
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddSingleton<OpenAiService>();
builder.Services.AddSingleton<SalaryCalculatorService>();
builder.Services.AddSingleton<CalculatorConfig>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Salary}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();