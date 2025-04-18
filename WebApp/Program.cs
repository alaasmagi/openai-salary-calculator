using DotNetEnv;
using WebApp.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

Env.Load();
var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
builder.Services.AddSingleton<OpenAiService>();

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