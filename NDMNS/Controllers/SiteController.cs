using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using NDMNS.Models;

namespace NDMNS.Controllers
{
    public class SiteController : BaseController
    {
        private readonly HttpClient _httpClient;

        public SiteController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
        }

        private async Task<List<Site>> GetSitesAsync()
        {
            try
            {
                var apiUrl = "https://localhost:44318/api/Site";
                var response = await _httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonSerializer.Deserialize<DtoResponse<List<Site>>>(
                        responseContent,
                        new JsonSerializerOptions
                        {
                            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                        }
                    );

                    return apiResponse?.data ?? new List<Site>();
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return new List<Site>();
                }
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] =
                    "An unexpected error occurred. Please try again later\n" + ex.Message;
                return new List<Site>();
            }
            catch (JsonException ex)
            {
                TempData["Error"] =
                    "An unexpected error occurred. Please try again later\n" + ex.Message;
                return new List<Site>();
            }
        }

        private async Task<List<WhatsAppGroup>> GetWhatsAppGroupsAsync()
        {
            try
            {
                var apiUrl = "https://localhost:44318/api/WhatsApp/groups";
                var response = await _httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonSerializer.Deserialize<DtoResponse<List<WhatsAppGroup>>>(
                        responseContent,
                        new JsonSerializerOptions
                        {
                            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                        }
                    );

                    return apiResponse?.data ?? new List<WhatsAppGroup>();
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return new List<WhatsAppGroup>();
                }
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] =
                    "An unexpected error occurred. Please try again later\n" + ex.Message;
                return new List<WhatsAppGroup>();
            }
            catch (JsonException ex)
            {
                TempData["Error"] =
                    "An unexpected error occurred. Please try again later\n" + ex.Message;
                return new List<WhatsAppGroup>();
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
                var sites = await GetSitesAsync();

                if (sites != null)
                {
                    ViewBag.UserName = HttpContext.Session.GetString("Name");
                    ViewBag.UserRole = HttpContext.Session.GetString("Role");
                    ViewBag.IsNOC =
                        HttpContext.Session.GetString("Role") == "Network Operation Center";

                    return View(sites);
                }
                else
                {
                    TempData["Error"] = "Failed to load Site data";
                    return View(new List<Site>());
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] =
                    "An unexpected error occurred. Please try again later\n" + ex.Message;
                return View(new List<Site>());
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
                var groups = await GetWhatsAppGroupsAsync();
                ViewBag.WhatsAppGroups = groups;

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
        public async Task<IActionResult> Create(Site site)
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
                var jsonContent = JsonSerializer.Serialize(site);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var apiUrl = $"https://localhost:44318/api/Site";
                var response = await _httpClient.PostAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    var successContent = await response.Content.ReadFromJsonAsync<
                        DtoResponse<Site>
                    >();
                    TempData["Success"] = successContent.message;
                    return RedirectToAction("Index");
                }
                else
                {
                    var errorContent = await response.Content.ReadFromJsonAsync<
                        DtoResponse<Site>
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
                var groups = await GetWhatsAppGroupsAsync();
                ViewBag.WhatsAppGroups = groups;
            }
            catch (Exception ex)
            {
                TempData["Error"] =
                    "An unexpected error occurred. Please try again later\n" + ex.Message;
            }

            return View(site);
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
                var apiUrl = $"https://localhost:44318/api/Site/{id}";
                var response = await _httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonSerializer.Deserialize<DtoResponse<Site>>(
                        responseContent,
                        new JsonSerializerOptions
                        {
                            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                        }
                    );

                    if (apiResponse?.data != null)
                    {
                        var groups = await GetWhatsAppGroupsAsync();
                        ViewBag.WhatsAppGroups = groups;

                        return View(apiResponse.data);
                    }
                }

                TempData["Error"] = "Site not found";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error loading site data\n" + ex.Message;

                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(string id, Site site)
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

            if (id != site.Id)
            {
                TempData["Error"] = "Invalid site data";
                return RedirectToAction("Index");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var jsonContent = JsonSerializer.Serialize(site);
                    var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                    var apiUrl = $"https://localhost:44318/api/Site/{id}";
                    var response = await _httpClient.PutAsync(apiUrl, content);

                    if (response.IsSuccessStatusCode)
                    {
                        var successContent = await response.Content.ReadFromJsonAsync<
                            DtoResponse<Site>
                        >();
                        TempData["Success"] = successContent.message;
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadFromJsonAsync<
                            DtoResponse<Site>
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
                var groups = await GetWhatsAppGroupsAsync();
                ViewBag.WhatsAppGroups = groups;
            }
            catch (Exception ex)
            {
                TempData["Error"] =
                    "An unexpected error occurred. Please try again later\n" + ex.Message;
            }

            return View(site);
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
                var apiUrl = $"https://localhost:44318/api/Site/{id}";
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
                        DtoResponse<Site>
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
