using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using NDMNS.Models;

namespace NDMNS.Controllers
{
    public class HelpdeskController : BaseController
    {
        private readonly HttpClient _httpClient;

        public HelpdeskController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
        }

        private async Task<List<Helpdesk>> GetHelpdeskAsync()
        {
            try
            {
                var apiUrl = "https://localhost:44318/api/Helpdesk";
                var response = await _httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonSerializer.Deserialize<DtoResponse<List<Helpdesk>>>(
                        responseContent,
                        new JsonSerializerOptions
                        {
                            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                        }
                    );

                    return apiResponse?.data ?? new List<Helpdesk>();
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return new List<Helpdesk>();
                }
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] =
                    "An unexpected error occurred. Please try again later\n" + ex.Message;
                return new List<Helpdesk>();
            }
            catch (JsonException ex)
            {
                TempData["Error"] =
                    "An unexpected error occurred. Please try again later\n" + ex.Message;
                return new List<Helpdesk>();
            }
        }

        private async Task<List<Isp>> GetIspAsync()
        {
            try
            {
                var apiUrl = "https://localhost:44318/api/Isp";
                var response = await _httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonSerializer.Deserialize<DtoResponse<List<Isp>>>(
                        responseContent,
                        new JsonSerializerOptions
                        {
                            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                        }
                    );

                    return apiResponse?.data ?? new List<Isp>();
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return new List<Isp>();
                }
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] =
                    "An unexpected error occurred. Please try again later\n" + ex.Message;
                return new List<Isp>();
            }
            catch (JsonException ex)
            {
                TempData["Error"] =
                    "An unexpected error occurred. Please try again later\n" + ex.Message;
                return new List<Isp>();
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
                var helpdesks = await GetHelpdeskAsync();

                if (helpdesks != null)
                {
                    ViewBag.UserName = HttpContext.Session.GetString("Name");
                    ViewBag.UserRole = HttpContext.Session.GetString("Role");
                    ViewBag.IsNOC =
                        HttpContext.Session.GetString("Role") == "Network Operation Center";

                    return View(helpdesks);
                }
                else
                {
                    TempData["Error"] = "Failed to load Helpdesk data";
                    return View(new List<Helpdesk>());
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] =
                    "An unexpected error occurred. Please try again later\n" + ex.Message;
                return View(new List<Helpdesk>());
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
                var isps = await GetIspAsync();
                ViewBag.Isps = isps;

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
        public async Task<IActionResult> Create(Helpdesk helpdesk)
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
                var jsonContent = JsonSerializer.Serialize(helpdesk);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var apiUrl = "https://localhost:44318/api/Helpdesk";
                var response = await _httpClient.PostAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    var successContent = await response.Content.ReadFromJsonAsync<
                        DtoResponse<Helpdesk>
                    >();
                    TempData["Success"] = successContent.message;
                    return RedirectToAction("Index");
                }
                else
                {
                    var errorContent = await response.Content.ReadFromJsonAsync<
                        DtoResponse<Helpdesk>
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
                var isps = await GetIspAsync();
                ViewBag.Isps = isps;
            }
            catch (Exception ex)
            {
                TempData["Error"] =
                    "An unexpected error occurred. Please try again later\n" + ex.Message;
            }

            return View(helpdesk);
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
                var apiUrl = $"https://localhost:44318/api/Helpdesk/{id}";
                var response = await _httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonSerializer.Deserialize<DtoResponse<Helpdesk>>(
                        responseContent,
                        new JsonSerializerOptions
                        {
                            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                        }
                    );

                    if (apiResponse?.data != null)
                    {
                        var isps = await GetIspAsync();
                        ViewBag.Isps = isps;

                        return View(apiResponse.data);
                    }
                }

                TempData["Error"] = "Helpdesk not found";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error loading Helpdesk data\n" + ex.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(string id, Helpdesk helpdesk)
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

            if (id != helpdesk.Id)
            {
                TempData["Error"] = "Invalid Helpdesk data";
                return RedirectToAction("Index");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var jsonContent = JsonSerializer.Serialize(helpdesk);
                    var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                    var apiUrl = $"https://localhost:44318/api/Helpdesk/{id}";
                    var response = await _httpClient.PutAsync(apiUrl, content);

                    if (response.IsSuccessStatusCode)
                    {
                        var successContent = await response.Content.ReadFromJsonAsync<
                            DtoResponse<Helpdesk>
                        >();
                        TempData["Success"] = successContent.message;
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadFromJsonAsync<
                            DtoResponse<Helpdesk>
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
                var isps = await GetIspAsync();
                ViewBag.Isps = isps;
            }
            catch (Exception ex)
            {
                TempData["Error"] =
                    "An unexpected error occurred. Please try again later\n" + ex.Message;
            }

            return View(helpdesk);
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
                var apiUrl = $"https://localhost:44318/api/Helpdesk/{id}";
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
                        DtoResponse<Helpdesk>
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
