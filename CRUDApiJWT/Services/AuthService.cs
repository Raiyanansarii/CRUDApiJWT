using BCrypt.Net;
using CRUDApiJWT.Data;
using CRUDApiJWT.DTOs;
using CRUDApiJWT.Helpers;
using Microsoft.AspNetCore.Http.HttpResults;
using Npgsql;
using System.Security.Claims;

namespace CRUDApiJWT.Services
{
    public class AuthService
    {
        private readonly DbHelper _db;
        private readonly JwtHelper _jwt;
        private readonly ILogger<AuthService> logger;

        public AuthService(DbHelper db, JwtHelper jwt, ILogger<AuthService> logger)
        {
            _db = db;
            _jwt = jwt;
            this.logger = logger;
        }

        public async Task<string> Signup(SignupRequest req)
        {
            var hash = BCrypt.Net.BCrypt.HashPassword(req.Password);

            var pars = new[]
            {
                new NpgsqlParameter("p_username", req.Username),
                new NpgsqlParameter("p_passwordhash", hash),
                new NpgsqlParameter("p_role", req.Role),
                new NpgsqlParameter("p_empid", req.EmpId ?? (object)DBNull.Value),
                new NpgsqlParameter("p_name", req.Name ?? (object)DBNull.Value),
                new NpgsqlParameter("p_joindate", req.JoinDate ?? (object)DBNull.Value),
                new NpgsqlParameter("p_dob", req.DOB ?? (object)DBNull.Value),
                new NpgsqlParameter("p_gender", req.Gender ?? (object)DBNull.Value),
                new NpgsqlParameter("p_dept", req.Department ?? (object)DBNull.Value),
                new NpgsqlParameter("p_desig", req.Designation ?? (object)DBNull.Value)
            };

            await _db.ExecuteNonQueryAsync("sp_createuser", pars);

            return $"{req.Username} user Created";

            // Generate token for user after signup
            //return _jwt.GenerateAccessToken(0, req.Username, req.Role); // 0 = placeholder, use actual userId if available
        }

 
        public async Task<(string accessToken, string refreshToken)> Login(AuthRequest req)
        {
            var pars = new[] {
                new NpgsqlParameter("@username", req.Username)
            };

            var sql = "SELECT empid, userid, username, passwordhash, role FROM users WHERE username = @username";

            var user = await _db.ExecuteRawItemAsync(sql,
                r => new
                {
                    EmpId = (int)r["empid"],
                    UserId = (int)r["userid"],
                    Username = (string)r["username"],
                    PasswordHash = (string)r["passwordhash"],
                    Role = (string)r["role"]
                }, pars);

            if (user == null || !BCrypt.Net.BCrypt.Verify(req.Password, user.PasswordHash))
                throw new UnauthorizedAccessException("Invalid credentials");

            var accessToken = _jwt.GenerateAccessToken(user.EmpId, user.Username, user.Role);
            var refreshToken = _jwt.GenerateRefreshToken();

            await StoreRefreshToken(user.EmpId, refreshToken);

            // Store refreshToken securely in DB if needed (not covered here yet)
            return (accessToken, refreshToken);
        }

        public async Task<(string newAccessToken, string newRefreshToken)> RefreshAccessTokenFromCookies(HttpRequest request)
        {
            var refreshToken = request.Cookies["refresh_token"];

            var sql = "SELECT  u.empid, u.username, u.role FROM refresh_tokens rt JOIN users u ON u.empid = rt.emp_id WHERE rt.token = @token AND rt.expires_at > NOW() AND rt.revoked_at IS NULL LIMIT 1";
            var pars = new[] {
                new NpgsqlParameter("@token", refreshToken)
            };

            var user = await _db.ExecuteRawItemAsync(sql, r => new
            {
                //UserId = (int)r["userid"],
                EmpId = (int)r["empid"],
                Username = (string)r["username"],
                Role = (string)r["role"]
            }, pars);

            // Invalidate old refresh token (optional for rotation)
            await InvalidateRefreshToken(refreshToken);

            // Issue new tokens
            var newAccessToken = _jwt.GenerateAccessToken(user.EmpId, user.Username, user.Role);
            var newRefreshToken = _jwt.GenerateRefreshToken();
            logger.LogInformation("new refresh token generated");

            await StoreRefreshToken(user.EmpId, newRefreshToken);


            return (newAccessToken, newRefreshToken);
        }


        private async Task StoreRefreshToken(int empId, string refreshToken)
        {
            var pars = new[] {
                new NpgsqlParameter("p_emp_id", empId),
                new NpgsqlParameter("p_token", refreshToken),
                new NpgsqlParameter("p_expires_at", DateTime.UtcNow.AddDays(7))

            };
            await _db.ExecuteNonQueryAsync("sp_store_refresh_token", pars);
        }

        public async Task InvalidateRefreshToken(string token)
        {
            var pars = new[] {new NpgsqlParameter("@p_token", token)};
            await _db.ExecuteNonQueryAsync("sp_revoke_refresh_token", pars);
        }


    }
}
