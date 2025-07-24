using CRUDUiJWT.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Net.Http.Headers;


namespace CRUDUiJWT.Controllers
{
    public class DashboardController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public DashboardController(IHttpClientFactory factory, IConfiguration config)
        {
            _httpClient = factory.CreateClient();
            _config = config;
        }

        public async Task<IActionResult> Index()
        {
            var token = Request.Cookies["access_token"];
            if (string.IsNullOrEmpty(token))
            {
                Console.WriteLine("failed To Get Token");
                return RedirectToAction("Login", "Account");
            }

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            var empIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "userId");

            //var empIdClaim2 = User.FindFirst("userId")?.Value;
            //if (string.IsNullOrEmpty(empIdClaim))
            //{
            //    Console.WriteLine("failed");
            //    return RedirectToAction("Login", "Account");
            //}

            if (empIdClaim == null)
            {
                Console.WriteLine("Invalid token or User Not Found");
                return Unauthorized("Invalid token or User Not Found");
            }

            var empId = int.Parse(empIdClaim.Value);
            var apiUrl = $"{_config["ApiBaseUrl"]}/api/employees/{empId}";

            var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);
            request.Headers.Add("Cookie", Request.Headers["Cookie"].ToString());
            //request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            //int empId = int.Parse(empIdClaim);
            //Console.WriteLine(empId);
            //var apiBaseUrl = _config["ApiBaseUrl"];
            //var client = _httpClientFactory.CreateClient();

            //var request = new HttpRequestMessage(HttpMethod.Get, $"{apiBaseUrl}/api/employees/{empId}");
            //request.Headers.Add("Cookie", Request.Headers["Cookie"].ToString()); // Forward cookies

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine("Failed to load profile.");
                TempData["ApiError"] = "Failed to load profile.";
                return RedirectToAction("Login", "Account");
            }

            var json = await response.Content.ReadAsStringAsync();
            var profile = JsonSerializer.Deserialize<UserProfileViewModel>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return View(profile);
        }

    }
}
