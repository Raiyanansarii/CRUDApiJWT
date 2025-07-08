using BCrypt.Net;
using CRUDApiJWT.Data;
using CRUDApiJWT.DTOs;
using CRUDApiJWT.Helpers;
using Npgsql;

namespace CRUDApiJWT.Services
{
    public class AuthService
    {
        private readonly DbHelper _db;
        private readonly JwtHelper _jwt;

        public AuthService(DbHelper db, JwtHelper jwt)
        {
            _db = db;
            _jwt = jwt;
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

            // NOTE: ExecuteNonQueryAsync returns rows affected (not ID), you may want to use a separate SP to return inserted userId.
            await _db.ExecuteNonQueryAsync("sp_createuser", pars);

            // Generate token for user after signup
            return _jwt.GenerateToken(0, req.Username, req.Role); // 0 = placeholder, use actual userId if available
        }

        public async Task<string> Login(AuthRequest req)
        {
            var pars = new[] {
                new NpgsqlParameter("@username", req.Username)
            };

            var sql = "SELECT userid, username, passwordhash, role FROM users WHERE username = @username";

            var user = await _db.ExecuteRawItemAsync(sql,
                r => new
                {
                    UserId = (int)r["userid"],
                    Username = (string)r["username"],
                    PasswordHash = (string)r["passwordhash"],
                    Role = (string)r["role"]
                }, pars);

            if (user == null || !BCrypt.Net.BCrypt.Verify(req.Password, user.PasswordHash))
                throw new UnauthorizedAccessException("Invalid credentials");

            return _jwt.GenerateToken(user.UserId, user.Username, user.Role);
        }


    }
}
