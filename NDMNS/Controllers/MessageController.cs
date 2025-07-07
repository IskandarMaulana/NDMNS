using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using NDMNS.Models;

namespace NDMNS.Controllers
{
    public class MessageController : BaseController
    {
        private readonly HttpClient _httpClient;

        public MessageController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
        }

        private async Task<List<Message>> GetMessagesAsync()
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

                    return apiResponse?.data ?? new List<Message>();
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return new List<Message>();
                }
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] =
                    "An unexpected error occurred. Please try again later\n" + ex.Message;
                return new List<Message>();
            }
            catch (JsonException ex)
            {
                TempData["Error"] =
                    "An unexpected error occurred. Please try again later\n" + ex.Message;
                return new List<Message>();
            }
        }

        private async Task<List<Downtime>> GetDowntimesAsync()
        {
            try
            {
                var apiUrl = "https://localhost:44318/api/Downtime";
                var response = await _httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonSerializer.Deserialize<DtoResponse<List<Downtime>>>(
                        responseContent,
                        new JsonSerializerOptions
                        {
                            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                        }
                    );

                    return apiResponse?.data ?? new List<Downtime>();
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return new List<Downtime>();
                }
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] =
                    "An unexpected error occurred. Please try again later\n" + ex.Message;
                return new List<Downtime>();
            }
            catch (JsonException ex)
            {
                TempData["Error"] =
                    "An unexpected error occurred. Please try again later\n" + ex.Message;
                return new List<Downtime>();
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
                var messages = await GetMessagesAsync();

                if (messages != null)
                {
                    ViewBag.UserName = HttpContext.Session.GetString("Name");
                    ViewBag.UserRole = HttpContext.Session.GetString("Role");
                    ViewBag.IsNOC =
                        HttpContext.Session.GetString("Role") == "Network Operation Center";

                    return View(messages);
                }
                else
                {
                    TempData["Error"] = "Failed to load Message data";
                    return View(new List<Message>());
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] =
                    "An unexpected error occurred. Please try again later\n" + ex.Message;
                return View(new List<Message>());
            }
        }

        public async Task<IActionResult> Create()
        {
            var userId = HttpContext.Session.GetString("Id");
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Index", "Auth");
            }

            var userRole = HttpContext.Session.GetString("Role");
            if (userRole != "Network Operation Center" && userRole != "Team Leader")
            {
                TempData["Error"] = "You don't have permission to access this page";
                return RedirectToAction("Index");
            }

            try
            {
                var downtimes = await GetDowntimesAsync();
                ViewBag.Downtimes = downtimes;

                return View();
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error loading page\n" + ex.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Message message)
        {
            var userId = HttpContext.Session.GetString("Id");
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Index", "Auth");
            }

            var userRole = HttpContext.Session.GetString("Role");
            if (userRole != "Network Operation Center" && userRole != "Team Leader")
            {
                TempData["Error"] = "You don't have permission to perform this action";
                return RedirectToAction("Index");
            }

            try
            {
                var jsonContent = JsonSerializer.Serialize(message);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var apiUrl = "https://localhost:44318/api/Message";
                var response = await _httpClient.PostAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    var successContent = await response.Content.ReadFromJsonAsync<
                        DtoResponse<Message>
                    >();
                    TempData["Success"] = successContent.message;
                    return RedirectToAction("Index");
                }
                else
                {
                    var errorContent = await response.Content.ReadFromJsonAsync<
                        DtoResponse<Message>
                    >();
                    TempData["Error"] = errorContent.message;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                TempData["Error"] = ex.Message;
            }

            try
            {
                var downtimes = await GetDowntimesAsync();
                ViewBag.Downtimes = downtimes;
            }
            catch (Exception ex)
            {
                TempData["Error"] =
                    "An unexpected error occurred. Please try again later\n" + ex.Message;
            }

            return View(message);
        }

        public async Task<IActionResult> Update(string id)
        {
            var userId = HttpContext.Session.GetString("Id");
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Index", "Auth");
            }

            var userRole = HttpContext.Session.GetString("Role");
            if (userRole != "Network Operation Center" && userRole != "Team Leader")
            {
                TempData["Error"] = "You don't have permission to access this page";
                return RedirectToAction("Index");
            }

            try
            {
                var apiUrl = $"https://localhost:44318/api/Message/{id}";
                var response = await _httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonSerializer.Deserialize<DtoResponse<Message>>(
                        responseContent,
                        new JsonSerializerOptions
                        {
                            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                        }
                    );

                    if (apiResponse?.data != null)
                    {
                        var downtimes = await GetDowntimesAsync();
                        ViewBag.Downtimes = downtimes;

                        return View(apiResponse.data);
                    }
                }

                TempData["Error"] = "Message not found";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error loading message data\n" + ex.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(string id, Message message)
        {
            var userId = HttpContext.Session.GetString("Id");
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Index", "Auth");
            }

            var userRole = HttpContext.Session.GetString("Role");
            if (userRole != "Network Operation Center" && userRole != "Team Leader")
            {
                TempData["Error"] = "You don't have permission to perform this action";
                return RedirectToAction("Index");
            }

            if (id != message.Id)
            {
                TempData["Error"] = "Invalid message data";
                return RedirectToAction("Index");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var jsonContent = JsonSerializer.Serialize(message);
                    var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                    var apiUrl = $"https://localhost:44318/api/Message/{id}";
                    var response = await _httpClient.PutAsync(apiUrl, content);

                    if (response.IsSuccessStatusCode)
                    {
                        var successContent = await response.Content.ReadFromJsonAsync<
                            DtoResponse<Message>
                        >();
                        TempData["Success"] = successContent.message;
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadFromJsonAsync<
                            DtoResponse<Message>
                        >();
                        TempData["Error"] = errorContent.message;
                    }
                }
                catch (Exception ex)
                {
                    TempData["Error"] = ex.Message;
                }
            }

            try
            {
                var downtimes = await GetDowntimesAsync();
                ViewBag.Downtimes = downtimes;
            }
            catch (Exception ex)
            {
                TempData["Error"] =
                    "An unexpected error occurred. Please try again later\n" + ex.Message;
            }

            return View(message);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            var userId = HttpContext.Session.GetString("Id");
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Index", "Auth");
            }

            var userRole = HttpContext.Session.GetString("Role");
            if (userRole != "Network Operation Center" && userRole != "Team Leader")
            {
                TempData["Error"] = "You don't have permission to perform this action";
                return RedirectToAction("Index");
            }

            try
            {
                var apiUrl = $"https://localhost:44318/api/Message/{id}";
                var response = await _httpClient.DeleteAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    var successContent = await response.Content.ReadFromJsonAsync<
                        DtoResponse<object>
                    >();
                    TempData["Success"] = successContent.message;
                }
                else
                {
                    var errorContent = await response.Content.ReadFromJsonAsync<
                        DtoResponse<Message>
                    >();
                    TempData["Error"] = errorContent.message;
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] =
                    "An unexpected error occurred. Please try again later\n" + ex.Message;
            }

            return RedirectToAction("Index");
        }
    }
}
