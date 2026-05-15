using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Net.Http.Json;
using System.Net.Http.Headers;
using Microsoft.Extensions.Configuration;

namespace ApartmanYonetimSistemi.Services
{
    public class GeminiService 
    {

        private readonly string _apiKey;
        private readonly HttpClient _httpClient;
        private const string _model = "llama-3.3-70b-versatile";

        public GeminiService(IConfiguration configuration)
        {
            _httpClient = new HttpClient();
            _apiKey = configuration["AiSettings:GroqApiKey"];
        }

        public async Task<(int Score, string Reason)> AnalyzeRequestPriority(string title, string description)
        {
            try
            {
                var url = "https://api.groq.com/openai/v1/chat/completions";

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

                var requestBody = new
                {
                    model = _model,
                    messages = new[]
                    {
                        new { role = "system", content = "Sen bir apartman yönetim asistanısın. Görevin gelen talepleri aciliyetine göre 1-5 arası puanlamak ve kısa bir neden sunmaktır. Sadece geçerli bir JSON döndür." },
                        new { role = "user", content = $@"
                            Aşağıdaki apartman yönetim talebini 1 ile 5 arasında puanla. 
                            Kriterler:
                            5: CAN VE MAL GÜVENLİĞİ (Su baskını, yangın, gaz kaçağı, asansörde mahsur kalma).
                            4: GÜVENLİK VE ACİL ONARIM (Dış kapı kilidi bozuk, ana aydınlatma yok).
                            3: KONFOR VE RUTİN ARIZA (Asansör lambası, gürültü şikayeti).
                            2: ÖNERİ VE DİLEK (Bahçe düzenlemesi, boya badana).
                            1: DÜŞÜK ÖNCELİK (Genel ricalar).

                            Başlık: {title}
                            Açıklama: {description}

                            Yanıtı sadece şu JSON formatında ver:
                            {{ ""score"": puan, ""reason"": ""kısa açıklama"" }}" }
                    },
                   
                    temperature = 0.1,
                    response_format = new { type = "json_object" }
                };

                var response = await _httpClient.PostAsJsonAsync(url, requestBody);
                var responseString = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Groq API Hatası ({response.StatusCode}): {responseString}");
                    return GetFallbackPriority(title);
                }

               
                using var doc = JsonDocument.Parse(responseString);
                string rawAiResponse = doc.RootElement
                    .GetProperty("choices")[0]
                    .GetProperty("message")
                    .GetProperty("content").GetString();

                
                using var resultDoc = JsonDocument.Parse(rawAiResponse);
                int score = resultDoc.RootElement.GetProperty("score").GetInt32();
                string reason = resultDoc.RootElement.GetProperty("reason").GetString() ?? "Analiz tamamlandı.";

                return (score, reason);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Groq Kritik Hata: {ex.Message}");
                return GetFallbackPriority(title);
            }
        }

        private (int Score, string Reason) GetFallbackPriority(string title)
        {
            string t = title.ToLower();
            if (t.Contains("acil") || t.Contains("arıza") || t.Contains("su") || t.Contains("yangın"))
                return (5, "Otomatik yüksek öncelik (Yedek Sistem).");

            return (2, "Standart öncelik (Yedek Sistem).");
        }
    }
}