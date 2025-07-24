using CRUDUiJWT.Areas.EmployeeManagement.Models;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Security.Policy;
using System.Text;
using System.Text.Json;

namespace CRUDUiJWT.Areas.EmployeeManagement.Controllers
{
    [Area("EmployeeManagement")]
    public class EmployeeController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public EmployeeController(IHttpClientFactory factory, IConfiguration configuration)
        {
            _httpClient = factory.CreateClient();
            _config = configuration;
        }
        public async Task<IActionResult> Index()
        {
            var apiUrl = $"{_config["ApiBaseUrl"]}/api/employees";

            var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);
            request.Headers.Add("Cookie", Request.Headers["Cookie"].ToString());

            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine("Failed to load profile.");
                TempData["ApiError"] = "Failed to load profile.";
                return RedirectToAction("Login", "Account");
            }

            var json = await response.Content.ReadAsStringAsync();
            var profile = JsonSerializer.Deserialize<List<Employee>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return View(profile);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(SignupViewModel data)
        {
            var content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
            var apiUrl = $"{_config["ApiBaseUrl"]}/api/auth/signup";


            var request = new HttpRequestMessage(HttpMethod.Post, apiUrl)
            {
                Content = content
            };
            request.Headers.Add("Cookie", Request.Headers["Cookie"].ToString());

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine("Failed to create profile.");
                TempData["ApiError"] = "Failed to create profile.";
                return RedirectToAction("Index", "Employee");
            }

            TempData["Message"] = "Signup successful! Please log in.";
            return RedirectToAction("Index", "Employee");
        }


        public async Task<IActionResult> Edit(int id)
        {
            Console.WriteLine(id);

            var apiUrl = $"{_config["ApiBaseUrl"]}/api/employees/{id}";

            var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);
            request.Headers.Add("Cookie", Request.Headers["Cookie"].ToString());

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine("Failed to load profile.");
                TempData["ApiError"] = "Failed to load profile.";
                return RedirectToAction("Index", "Employee");
            }

            var json = await response.Content.ReadAsStringAsync();
            var profile = JsonSerializer.Deserialize<Employee>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return View(profile);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(SignupViewModel data) 
        {
            var content = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, "application/json");
            var apiUrl = $"{_config["ApiBaseUrl"]}/api/Employees/{data.EmpId}";


            var request = new HttpRequestMessage(HttpMethod.Put, apiUrl)
            {

                Content = content
            };
            request.Headers.Add("Cookie", Request.Headers["Cookie"].ToString());

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine("Failed to Updation profile.");
                TempData["ApiError"] = "Failed to Updation profile.";
                return RedirectToAction("Index", "Employee");
            }

            TempData["Message"] = "Employee details updated successfully!";
            return RedirectToAction("Index", "Employee");
        }


        public async Task<IActionResult> Delete(int id)
        {
            Console.WriteLine(id);

            var apiUrl = $"{_config["ApiBaseUrl"]}/api/employees/{id}";

            var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);
            request.Headers.Add("Cookie", Request.Headers["Cookie"].ToString());

            var response = await _httpClient.SendAsync(request);
        
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine("Failed to load profile.");
                TempData["ApiError"] = "Failed to load profile.";
                return RedirectToAction("Index", "Employee");
            }

            var json = await response.Content.ReadAsStringAsync();
            var profile = JsonSerializer.Deserialize<Employee>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return View(profile);
        }

        [HttpPost]
        [ActionName("Delete")]
        public async Task<IActionResult> Deletepost(int id)
        {
            var apiUrl = $"{_config["ApiBaseUrl"]}/api/employees/{id}";
            var request = new HttpRequestMessage(HttpMethod.Delete, apiUrl);
            
            request.Headers.Add("Cookie", Request.Headers["Cookie"].ToString());
            
            var response = await _httpClient.SendAsync(request);

            return RedirectToAction("Index", "Employee");
        }
    }
}
