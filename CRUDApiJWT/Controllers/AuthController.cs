using CRUDApiJWT.DTOs;
using CRUDApiJWT.Services;
using Microsoft.AspNetCore.Mvc;

namespace CRUDApiJWT.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _auth;

        public AuthController(AuthService auth)
        {
            _auth = auth;
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh()
        {

            var (newAccessToken, newRefreshToken) = await _auth.RefreshAccessTokenFromCookies(Request);

            Response.Cookies.Append("access_token", newAccessToken, new CookieOptions
            {
                HttpOnly = false,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddMinutes(15)
            });

            Response.Cookies.Append("refresh_token", newRefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(7)
            });

            return Ok(new { message = "Access token refreshed" });
        }


        [HttpPost("signup")]
        public async Task<IActionResult> Signup(SignupRequest req)
        {
            var output = await _auth.Signup(req);
            return Ok(output);
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login(AuthRequest req)
        {
            var (accessToken, refreshToken) = await _auth.Login(req);

            var accessCookie = new CookieOptions
            {
                HttpOnly = false,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddMinutes(15)
            };

            var refreshCookie = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(7)
            };

            Response.Cookies.Append("access_token", accessToken, accessCookie);
            Response.Cookies.Append("refresh_token", refreshToken, refreshCookie);

            return Ok(new { message = $"Logged in successfully" });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var refreshToken = Request.Cookies["refresh_token"];

            if (!string.IsNullOrEmpty(refreshToken))
            {
                // Optional: Revoke the token in DB
                await _auth.InvalidateRefreshToken(refreshToken);
            }

            // Clear cookies
            Response.Cookies.Append("access_token", "", new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddDays(-1),
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Path = "/"
            });

            Response.Cookies.Append("refresh_token", "", new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddDays(-1),
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Path = "/"
            });

            return Ok(new { message = "Logged out and tokens revoked." });
        }


    }
}
