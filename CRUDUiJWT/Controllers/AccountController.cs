using Microsoft.AspNetCore.Mvc;
using System.Net.Http;

namespace CRUDUiJWT.Controllers
{
    public class AccountController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration configuration;


        public AccountController(IHttpClientFactory factory,IConfiguration configuration)
        {
            _httpClient = factory.CreateClient();
            this.configuration = configuration;
        }

        [HttpGet]
        public IActionResult Login()
        { 
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            var baseUrl = configuration["ApiBaseUrl"];
            var apiUrl = $"{baseUrl}/auth/logout";

            var request = new HttpRequestMessage(HttpMethod.Post, apiUrl);

            // Forward cookies (so API sees the refresh_token/access_token)
            var accessToken = Request.Cookies["access_token"];
            var refreshToken = Request.Cookies["refresh_token"];

            if (!string.IsNullOrEmpty(accessToken))
                request.Headers.Add("Cookie", $"access_token={accessToken}");

            if (!string.IsNullOrEmpty(refreshToken))
                request.Headers.Add("Cookie", $"refresh_token={refreshToken}");

            var response = await _httpClient.SendAsync(request);

            // Optionally: delete cookies client-side (they will expire anyway)
            Response.Cookies.Delete("access_token");
            Response.Cookies.Delete("refresh_token");

            return RedirectToAction("Login", "Account");
        }

    }
}
