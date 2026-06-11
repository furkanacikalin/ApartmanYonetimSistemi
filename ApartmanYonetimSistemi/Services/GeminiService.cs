using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace ApartmanYonetimSistemi.Services
{
    public class GeminiService
    {
        private readonly string _apiKey;
        private readonly string _model;
        private readonly HttpClient _httpClient;

        // IHttpClientFactory kullanarak soket sızıntılarını (Socket Exhaustion) önlüyoruz
        public GeminiService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient();
            _apiKey = configuration["AiSettings:GroqApiKey"] ?? string.Empty;

            // Model adını configuration'dan çekiyoruz, yoksa fallback olarak llama kullanıyoruz
            _model = configuration["AiSettings:ModelName"] ?? "llama-3.3-70b-versatile";
        }

        public async Task<(int Score, string Reason)> AnalyzeRequestPriority(string title, string description)
        {
            try
            {
                var url = "https://api.groq.com/openai/v1/chat/completions";

                // İstek başlığını temiz tutmak ve güvenliği sağlamak için ekliyoruz
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

                // Kullanıcı girdilerini tırnak işareti vb. hatalardan korumak için string interpolation yerine 
                // prompt metnini güvenli bir yapıda kurguluyoruz.
                string userPrompt = $"Aşağıdaki apartman yönetim talebini 1 ile 5 arasında puanla.\n" +
                                    $"Kriterler:\n" +
                                    $"5: CAN VE MAL GÜVENLİĞİ (Su baskını, yangın, gaz kaçağı, asansörde mahsur kalma).\n" +
                                    $"4: GÜVENLİK VE ACİL ONARIM (Dış kapı kilidi bozuk, ana aydınlatma yok).\n" +
                                    $"3: KONFOR VE RUTİN ARIZA (Asansör lambası, gürültü şikayeti).\n" +
                                    $"2: ÖNERİ VE DİLEK (Bahçe düzenlemesi, boya badana).\n" +
                                    $"1: DÜŞÜK ÖNCELİK (Genel ricalar).\n\n" +
                                    $"Başlık: {title}\n" +
                                    $"Açıklama: {description}\n\n" +
                                    $"Yanıtı sadece şu JSON formatında ver:\n" +
                                    $"{{\"score\": puan, \"reason\": \"kısa açıklama\"}}";

                var requestBody = new
                {
                    model = _model,
                    messages = new[]
                    {
                        new { role = "system", content = "Sen bir apartman yönetim asistanısın. Görevin gelen talepleri aciliyetine göre 1-5 arası puanlamak ve kısa bir neden sunmaktır. Sadece geçerli bir JSON döndür." },
                        new { role = "user", content = userPrompt }
                    },
                    temperature = 0.1,
                    response_format = new { type = "json_object" }
                };

                // PostAsJsonAsync metodu arka planda tüm karakter kaçışlarını (escape characters) otomatik yönetir
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
                    .GetProperty("content").GetString() ?? string.Empty;

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
            if (t.Contains("acil") || t.Contains("arıza") || t.Contains("su") || t.Contains("yangın") || t.Contains("kaçak"))
                return (5, "Otomatik yüksek öncelik (Yedek Sistem).");

            return (2, "Standart öncelik (Yedek Sistem).");
        }
    }
}