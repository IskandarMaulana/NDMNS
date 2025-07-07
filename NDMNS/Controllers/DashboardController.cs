using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using NDMNS.Models;

namespace NDMNS.Controllers
{
    public class DashboardController : BaseController
    {
        private readonly HttpClient _httpClient;

        public DashboardController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
        }

        private async Task<List<Network>> GetDownNetworksAsync()
        {
            try
            {
                var apiUrl = "https://localhost:44318/api/Network";
                var response = await _httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonSerializer.Deserialize<DtoResponse<List<Network>>>(
                        responseContent,
                        new JsonSerializerOptions
                        {
                            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                        }
                    );

                    var allNetworks = apiResponse?.data ?? new List<Network>();
                    return allNetworks.Where(s => s.Status.Equals(1)).ToList();
                }
                return new List<Network>();
            }
            catch (Exception)
            {
                return new List<Network>();
            }
        }

        private async Task<Message> GetLatestMessageAsync()
        {
            try
            {
                var apiUrl = "https://localhost:44318/api/Message";
                var response = await _httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonSerializer.Deserialize<DtoResponse<List<Message>>>(
                        responseContent,
                        new JsonSerializerOptions
                        {
                            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                        }
                    );

                    var messages = apiResponse?.data ?? new List<Message>();
                    return messages.OrderByDescending(m => m.Date).FirstOrDefault()!;
                }
                return null!;
            }
            catch (Exception)
            {
                return null!;
            }
        }

        public async Task<List<Message>> GetMessagesAsync()
        {
            try
            {
                var apiUrl = "https://localhost:44318/api/Message";
                var response = await _httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonSerializer.Deserialize<DtoResponse<List<Message>>>(
                        responseContent,
                        new JsonSerializerOptions
                        {
                            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                        }
                    );

                    var messages = apiResponse?.data ?? new List<Message>();
                    return messages;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private async Task<Email> GetLatestEmailAsync()
        {
            try
            {
                var apiUrl = "https://localhost:44318/api/Email";
                var response = await _httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonSerializer.Deserialize<DtoResponse<List<Email>>>(
                        responseContent,
                        new JsonSerializerOptions
                        {
                            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                        }
                    );

                    var emails = apiResponse?.data ?? new List<Email>();
                    return emails.OrderByDescending(e => e.Date).FirstOrDefault()!;
                }
                return null!;
            }
            catch (Exception ex)
            {
                return null!;
            }
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

        private async Task<int> GetMessageCountAsync()
        {
            try
            {
                var apiUrl = "https://localhost:44318/api/Message/count";
                var response = await _httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonSerializer.Deserialize<DtoResponse<int>>(
                        responseContent,
                        new JsonSerializerOptions
                        {
                            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                        }
                    );

                    return apiResponse?.data ?? 0;
                }
                return 0;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        private async Task<SystemHealthResult> GetSystemHealth()
        {
            try
            {
                var apiUrl = "https://localhost:44318/api/Setting/health";
                var response = await _httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonSerializer.Deserialize<DtoResponse<SystemHealthResult>>(
                        responseContent,
                        new JsonSerializerOptions
                        {
                            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                        }
                    );

                    return apiResponse?.data ?? new SystemHealthResult();
                }
                return null!;
            }
            catch (Exception)
            {
                return null!;
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
                var downNetworksTask = GetDownNetworksAsync();
                var latestMessageTask = GetLatestMessageAsync();
                var latestEmailTask = GetLatestEmailAsync();
                var whatsAppStatusTask = GetWhatsAppStatusAsync();
                var messageCountTask = GetMessageCountAsync();
                var messagesDataTask = GetMessagesAsync();
                var systemHealthDataTask = GetSystemHealth();

                await Task.WhenAll(
                    downNetworksTask,
                    latestMessageTask,
                    latestEmailTask,
                    whatsAppStatusTask,
                    messageCountTask,
                    systemHealthDataTask
                );

                ViewBag.DownNetworks = downNetworksTask.Result;
                ViewBag.LatestMessage = latestMessageTask.Result;
                ViewBag.LatestEmail = latestEmailTask.Result;
                ViewBag.WhatsAppStatus = whatsAppStatusTask.Result;
                ViewBag.MessageCount = messageCountTask.Result;
                ViewBag.MessagesData = messagesDataTask.Result;
                ViewBag.SystemHealth = systemHealthDataTask.Result;

                return View();
            }
            catch (Exception ex)
            {
                TempData["Error"] =
                    "An unexpected error occurred while loading dashboard data: " + ex.Message;
                return View();
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetNetworkMessages(string networkId)
        {
            try
            {
                var apiUrl = $"https://localhost:44318/api/Message/network/{networkId}";
                var response = await _httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonSerializer.Deserialize<DtoResponse<List<Message>>>(
                        responseContent,
                        new JsonSerializerOptions
                        {
                            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                        }
                    );

                    var messages = apiResponse?.data ?? new List<Message>();
                    return Json(new { success = true, data = messages });
                }

                return Json(new { success = false, message = "Failed to fetch network messages" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
