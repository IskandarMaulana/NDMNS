using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using NDMNS.Models;

namespace NDMNS.Controllers
{
    public class AuthController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public AuthController(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(LoginRequest loginRequest)
        {
            try
            {
                var jsonContent = JsonSerializer.Serialize(loginRequest);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var apiUrl = "https://localhost:44318/api/User/login";
                var response = await _httpClient.PostAsync(apiUrl, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var loginResponse = JsonSerializer.Deserialize<DtoResponse<User>>(
                        responseContent,
                        new JsonSerializerOptions
                        {
                            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                        }
                    );

                    if (loginResponse?.data != null)
                    {
                        HttpContext.Session.SetString("Id", loginResponse.data.Id);
                        HttpContext.Session.SetString("Name", loginResponse.data.Name ?? "");
                        HttpContext.Session.SetString("Nrp", loginResponse.data.Nrp);
                        HttpContext.Session.SetString("Role", loginResponse.data.Role ?? "");
                        HttpContext.Session.SetString("Email", loginResponse.data.Email ?? "");
                        HttpContext.Session.SetString(
                            "WhatsApp",
                            loginResponse.data.WhatsApp ?? ""
                        );

                        return RedirectToAction("Index", "Dashboard");
                    }
                }
                else
                {
                    var errorContent = await response.Content.ReadFromJsonAsync<
                        DtoResponse<User>
                    >();

                    TempData["Error"] = errorContent.message;

                    ViewBag.Nrp = loginRequest.Nrp;
                    return View(loginRequest);
                }
            }
            catch (HttpRequestException ex)
            {
                TempData["Error"] = "Unable to connect to server. Please try again later.";
                ViewBag.Nrp = loginRequest.Nrp;
                return View(loginRequest);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                TempData["Error"] = "An unexpected error occurred. Please try again.";
                ViewBag.Nrp = loginRequest.Nrp;
                return View(loginRequest);
            }

            TempData["Error"] = "Login failed. Please check your credentials.";
            ViewBag.Nrp = loginRequest.Nrp;
            return View(loginRequest);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return View("Index");
        }
    }
}
