using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace WebApp.Services;

public class OpenAiService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public OpenAiService(IConfiguration configuration)
    {
        _httpClient = new HttpClient();
        _apiKey = configuration["OpenAI:ApiKey"] ?? throw new Exception("OpenAI API Key is missing");
    }

    public async Task<string> GetSalaryComment(decimal netSalary)
    {
        var prompt = $"Kirjuta mulle lühike hinnang sellele palgale Eestis: {netSalary} eurot netopalka kuus.";

        var requestData = new
        {
            model = "gpt-3.5-turbo",
            messages = new[]
            {
                new { role = "user", content = prompt }
            },
            max_tokens = 100
        };

        var requestJson = JsonSerializer.Serialize(requestData);
        var request = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/chat/completions");

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
        request.Content = new StringContent(requestJson, Encoding.UTF8, "application/json");

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var jsonResponse = await response.Content.ReadAsStringAsync();

        using var doc = JsonDocument.Parse(jsonResponse);
        var content = doc.RootElement
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString();

        return content?.Trim() ?? "Tekkis tõrge, AI ei vastanud.";
    }
}