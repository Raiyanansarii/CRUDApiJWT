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
        public AuthController(AuthService auth) => _auth = auth;

        [HttpPost("signup")]
        public async Task<IActionResult> Signup(SignupRequest req)
        {
            var token = await _auth.Signup(req);
            return Ok(new AuthResponse { Token = token });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(AuthRequest req)
        {
            var token = await _auth.Login(req);
            return Ok(new AuthResponse { Token = token });
        }
    }
}
