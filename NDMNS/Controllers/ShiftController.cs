using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using NDMNS.Models;

namespace NDMNS.Controllers
{
    public class ShiftController : BaseController
    {
        private readonly HttpClient _httpClient;

        public ShiftController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
        }

        private async Task<List<Shift>> GetShiftsAsync()
        {
            try
            {
                var apiUrl = "https://localhost:44318/api/Shift";
                var response = await _httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonSerializer.Deserialize<DtoResponse<List<Shift>>>(
                        responseContent,
                        new JsonSerializerOptions
                        {
                            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                        }
                    );

                    return apiResponse?.data ?? new List<Shift>();
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return new List<Shift>();
                }
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] =
                    "An unexpected error occurred. Please try again later\n" + ex.Message;
                return new List<Shift>();
            }
            catch (JsonException ex)
            {
                TempData["Error"] =
                    "An unexpected error occurred. Please try again later\n" + ex.Message;
                return new List<Shift>();
            }
        }

        private async Task<List<User>> GetUsersAsync()
        {
            try
            {
                var apiUrl = "https://localhost:44318/api/User";
                var response = await _httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonSerializer.Deserialize<DtoResponse<List<User>>>(
                        responseContent,
                        new JsonSerializerOptions
                        {
                            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                        }
                    );

                    return apiResponse?.data ?? new List<User>();
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return new List<User>();
                }
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] =
                    "An unexpected error occurred. Please try again later\n" + ex.Message;
                return new List<User>();
            }
            catch (JsonException ex)
            {
                TempData["Error"] =
                    "An unexpected error occurred. Please try again later\n" + ex.Message;
                return new List<User>();
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
                var shifts = await GetShiftsAsync();

                if (shifts != null)
                {
                    return View(shifts);
                }
                else
                {
                    TempData["Error"] = "Failed to load Shift data";
                    return View(new List<Shift>());
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] =
                    "An unexpected error occurred. Please try again later\n" + ex.Message;
                return View(new List<Shift>());
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
            if (userRole != "Team Leader")
            {
                TempData["Error"] = "You don't have permission to access this page";
                return RedirectToAction("Index");
            }

            try
            {
                var users = await GetUsersAsync();
                ViewBag.Users = users;

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
        public async Task<IActionResult> Create(Shift shift)
        {
            var userId = HttpContext.Session.GetString("Id");
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Index", "Auth");
            }

            var userRole = HttpContext.Session.GetString("Role");
            if (userRole != "Team Leader")
            {
                TempData["Error"] = "You don't have permission to perform this action";
                return RedirectToAction("Index");
            }

            try
            {
                var jsonContent = JsonSerializer.Serialize(shift);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var apiUrl = "https://localhost:44318/api/Shift";
                var response = await _httpClient.PostAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    var successContent = await response.Content.ReadFromJsonAsync<
                        DtoResponse<Shift>
                    >();

                    TempData["Success"] = successContent.message;
                    return RedirectToAction("Index");
                }
                else
                {
                    var errorContent = await response.Content.ReadFromJsonAsync<
                        DtoResponse<Shift>
                    >();

                    TempData["Error"] = string.IsNullOrEmpty(errorContent.message)
                        ? errorContent.errors.First()
                        : errorContent.message;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                TempData["Error"] = ex.Message;
            }

            try
            {
                var users = await GetUsersAsync();
                ViewBag.Users = users;
            }
            catch (Exception ex)
            {
                TempData["Error"] =
                    "An unexpected error occurred. Please try again later\n" + ex.Message;
            }

            return View(shift);
        }

        public async Task<IActionResult> Update(string id)
        {
            var userId = HttpContext.Session.GetString("Id");
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Index", "Auth");
            }

            var userRole = HttpContext.Session.GetString("Role");
            if (userRole != "Team Leader")
            {
                TempData["Error"] = "You don't have permission to access this page";
                return RedirectToAction("Index");
            }

            try
            {
                var apiUrl = $"https://localhost:44318/api/Shift/{id}";
                var response = await _httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonSerializer.Deserialize<DtoResponse<Shift>>(
                        responseContent,
                        new JsonSerializerOptions
                        {
                            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                        }
                    );

                    if (apiResponse?.data != null)
                    {
                        var users = await GetUsersAsync();
                        ViewBag.Users = users;

                        return View(apiResponse.data);
                    }
                }

                TempData["Error"] = "Shift not found";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error loading Shift data\n" + ex.Message;
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(string id, Shift shift)
        {
            var userId = HttpContext.Session.GetString("Id");
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Index", "Auth");
            }

            var userRole = HttpContext.Session.GetString("Role");
            if (userRole != "Team Leader")
            {
                TempData["Error"] = "You don't have permission to perform this action";
                return RedirectToAction("Index");
            }

            if (id != shift.Id)
            {
                TempData["Error"] = "Invalid Shift data";
                return RedirectToAction("Index");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var jsonContent = JsonSerializer.Serialize(shift);
                    var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                    var apiUrl = $"https://localhost:44318/api/Shift/{id}";
                    var response = await _httpClient.PutAsync(apiUrl, content);

                    if (response.IsSuccessStatusCode)
                    {
                        var successContent = await response.Content.ReadFromJsonAsync<
                            DtoResponse<Shift>
                        >();
                        TempData["Success"] = successContent.message;
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadFromJsonAsync<
                            DtoResponse<Shift>
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
                var users = await GetUsersAsync();
                ViewBag.Users = users;
            }
            catch (Exception ex)
            {
                TempData["Error"] =
                    "An unexpected error occurred. Please try again later\n" + ex.Message;
            }

            return View(shift);
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
            if (userRole != "Team Leader")
            {
                TempData["Error"] = "You don't have permission to perform this action";
                return RedirectToAction("Index");
            }

            try
            {
                var apiUrl = $"https://localhost:44318/api/Shift/{id}";
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
                        DtoResponse<Shift>
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
