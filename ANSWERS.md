# C# Proovitöö Küsimused ja Vastused

## 1. Mis on vahe Task ja Thread-il C#-is ning millal võiks ühte või teist kasutada?

Tegelikult on ka kolmas variant olemas, mis on sünkroonne.  
* **Sünkroonne** meetod kasutab põhilõime `lõim on protsessori osa [inglese keeles thread]`, et teostada mingit toimingut või protsessi. Seda kasutatakse siis, kui on kindlalt vaja teha mingid protsessid järjest ja järjekord on oluline. Sünkroonne meetod ei lase enne ühelgi teisel protsessil alata, kuni eelnev protsess on läbi saanud. 

* **Task** on asünkroonne meetod. Task kasutab samamoodi põhilõime, kuid erinevalt sünkroonsest meetodist ei oota see tegevuse lõpetamist, et alustada järgmist protsessi. Task sobib näiteks serveritesse, kus on palju kasutajaid, kes teevad päringuid, mille täitmine võtab natuke rohkem aega (nt. andmebaasipäringud). Serverit ei hoita seega kinni, kuni üks kasutaja saab oma andmed andmebaasist kätte.

* **Thread** on kolmas võimalus, mis kasutab hoopis mingit muud lõime kui põhilõim. Kasutatakse seda reeglina siis, kui on vaja jooksutada mingit protsessi, mis nõuab rohkem protsessori töömahtu ja eraldi mälu.

## 2. Mis vahe on dependency injection (DI) transient, scoped ja singleton elutsüklitel ASP.NET Core’is?

Vahe on elueas. **Transient** tähendab seda, et igakord, kui mingi teenuse poole pöördutakse, luuakse sellest teenusest uus eksemplar, mida asutakse kasutama. **Scoped** on seotud HTTP päringutega. Iga HTTP päringu kohta tehakse uus eksemplar sellest instantsist, kus seda Scoped elutsüklit kasutatakse. **Singleton** tähendab seda, et luuakse kogu programmi jooksutamise peale ainult üks eksemplar - programmi alguses ning sellest programmi jooksutamise ajal rohkem uusi eksemplare ei looda.

## 3. Miks ja millal peaks kasutama AsNoTracking() meetodit Entity Frameworkis?

**AsNoTracking** tähendab seda, et DbContext ei jälgi uuendusi andmetes. Kasutatakse seda siis, kui loetakse andmeid andmebaasist, kuid neid ei kirjutata sinna. See säästab protsessori töömahtu ja kiirendab protsessi.

## 4. Mida teeb lock märksõna C#-is ja kuidas see erineb SemaphoreSlim-ist?

**Lock** annab võimaluse piirata mingile koodilõigule juurdepääsu, lubades juurdepääsu korraga vaid ühele lõimele. **SemaphoreSlim** võimaldab aga paindlikumalt sättida mingile koodilõigule juurdepääsu saavate lõimede arvu. Juurdepääsu piiratakse vahel seetõttu, et vältida olukorda, kus mitu protsessi üritavad samaaegselt muuta sama resurssi (nt. kirjet andmebaasis).

## 5. Kuidas sa teeksid asünkroonse OpenAI API päringu .NET-is, kasutades HttpClient-i?

* Samm 1: Initsialiseeri **HTTPClient**  
* Samm 2: Võta keskkonnamuutujate hulgast **API võti**  
* Samm 3: Määra attribuudid (prompt, API päringu attribuudid)  
* Samm 4: Sisesta **API võti** päringu **header**-isse  
* Samm 5: Teosta päring  
* Samm 6: Vastavalt päringu tulemusele määra edasine meetodi käitumine/väljund

### Väljavõte mu proovitööst:

```csharp
private readonly HttpClient _httpClient = new();
private readonly string _apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY") 
                                      ?? throw new Exception("OpenAI API Key is missing");

public async Task<string> GetSalaryComment(decimal netSalary)
{
    var prompt = $"Kirjuta mulle lühike hinnang sellele palgale Eestis: {netSalary} eurot netopalka kuus.";

    var requestData = new
    {
        model = "gpt-3.5-turbo",
        messages = new[] { new { role = "user", content = prompt } },
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
```
