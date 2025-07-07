using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using NDMNS.Models;

namespace NDMNS.Controllers
{
    public class PicController : BaseController
    {
        private readonly HttpClient _httpClient;

        public PicController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
        }

        private async Task<List<Pic>> GetPicsAsync()
        {
            try
            {
                var apiUrl = "https://localhost:44318/api/Pic";
                var response = await _httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonSerializer.Deserialize<DtoResponse<List<Pic>>>(
                        responseContent,
                        new JsonSerializerOptions
                        {
                            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                        }
                    );

                    return apiResponse?.data ?? new List<Pic>();
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return new List<Pic>();
                }
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] =
                    "An unexpected error occurred. Please try again later\n" + ex.Message;
                return new List<Pic>();
            }
            catch (JsonException ex)
            {
                TempData["Error"] =
                    "An unexpected error occurred. Please try again later\n" + ex.Message;
                return new List<Pic>();
            }
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

        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.Session.GetString("Id");
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Index", "Auth");
            }

            try
            {
                var pics = await GetPicsAsync();

                if (pics != null)
                {
                    ViewBag.UserName = HttpContext.Session.GetString("Name");
                    ViewBag.UserRole = HttpContext.Session.GetString("Role");
                    ViewBag.IsNOC =
                        HttpContext.Session.GetString("Role") == "Network Operation Center";

                    return View(pics);
                }
                else
                {
                    TempData["Error"] = "Failed to load PIC data";
                    return View(new List<Pic>());
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] =
                    "An unexpected error occurred. Please try again later\n" + ex.Message;
                return View(new List<Pic>());
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
                var sites = await GetSitesAsync();
                ViewBag.Sites = sites;

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
        public async Task<IActionResult> Create(Pic pic)
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
                var jsonContent = JsonSerializer.Serialize(pic);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var apiUrl = "https://localhost:44318/api/Pic";
                var response = await _httpClient.PostAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    var successContent = await response.Content.ReadFromJsonAsync<
                        DtoResponse<Pic>
                    >();

                    TempData["Success"] = successContent.message;
                    return RedirectToAction("Index");
                }
                else
                {
                    var errorContent = await response.Content.ReadFromJsonAsync<DtoResponse<Pic>>();
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
                var sites = await GetSitesAsync();
                ViewBag.Sites = sites;
            }
            catch (Exception ex)
            {
                TempData["Error"] =
                    "An unexpected error occurred. Please try again later\n" + ex.Message;
            }

            return View(pic);
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
                var apiUrl = $"https://localhost:44318/api/Pic/{id}";
                var response = await _httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonSerializer.Deserialize<DtoResponse<Pic>>(
                        responseContent,
                        new JsonSerializerOptions
                        {
                            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                        }
                    );

                    if (apiResponse?.data != null)
                    {
                        var sites = await GetSitesAsync();
                        ViewBag.Sites = sites;

                        return View(apiResponse.data);
                    }
                }

                TempData["Error"] = "PIC not found";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error loading PIC data\n" + ex.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(string id, Pic pic)
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

            if (id != pic.Id)
            {
                TempData["Error"] = "Invalid PIC data";
                return RedirectToAction("Index");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var jsonContent = JsonSerializer.Serialize(pic);
                    var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                    var apiUrl = $"https://localhost:44318/api/Pic/{id}";
                    var response = await _httpClient.PutAsync(apiUrl, content);

                    if (response.IsSuccessStatusCode)
                    {
                        var successContent = await response.Content.ReadFromJsonAsync<
                            DtoResponse<Pic>
                        >();
                        TempData["Success"] = successContent.message;
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadFromJsonAsync<
                            DtoResponse<Pic>
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
                var sites = await GetSitesAsync();
                ViewBag.Sites = sites;
            }
            catch (Exception ex)
            {
                TempData["Error"] =
                    "An unexpected error occurred. Please try again later\n" + ex.Message;
            }

            return View(pic);
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
                var apiUrl = $"https://localhost:44318/api/Pic/{id}";
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
                    var errorContent = await response.Content.ReadFromJsonAsync<DtoResponse<Pic>>();
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
