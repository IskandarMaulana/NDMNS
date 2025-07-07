using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using NDMNS.Models;

namespace NDMNS.Controllers
{
    public class WhatsAppClientController : BaseController
    {
        private readonly HttpClient _httpClient;

        public WhatsAppClientController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
        }

        private async Task<WhatsAppClient> GetWhatsAppStatusAsync()
        {
            try
            {
                var apiUrl = "https://localhost:44318/api/WhatsApp/status";
                var response = await _httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonSerializer.Deserialize<WhatsAppClient>(
                        responseContent,
                        new JsonSerializerOptions
                        {
                            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                        }
                    );

                    return apiResponse ?? new WhatsAppClient();
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.Session.GetString("Id");
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Index", "Auth");
            }

            try
            {
                var whatsAppClient = await GetWhatsAppStatusAsync();

                ViewBag.UserName = HttpContext.Session.GetString("Name");
                ViewBag.UserRole = HttpContext.Session.GetString("Role");
                ViewBag.IsNOC = HttpContext.Session.GetString("Role") == "Network Operation Center";

                ViewBag.WhatsAppStatus = whatsAppClient;

                return View();
            }
            catch (Exception ex)
            {
                TempData["Error"] =
                    "An unexpected error occurred while loading dashboard data: " + ex.Message;
                return View();
            }
        }
    }
}
