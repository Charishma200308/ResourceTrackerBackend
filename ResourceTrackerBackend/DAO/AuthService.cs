using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ResourceTrackerBackend.Models;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Data.SqlClient;

namespace ResourceTrackerBackend.DAO
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _config;
        private readonly string _connectionString;

        public AuthService(IConfiguration config)
        {
            _config = config;
            _connectionString = config.GetConnectionString("EmployeeDb");
        }

        public User? GetUserByEmail(string email)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand("SELECT * FROM Users WHERE Email = @Email", conn);
            cmd.Parameters.AddWithValue("@Email", email);

            conn.Open();
            using var reader = cmd.ExecuteReader();
            if (!reader.Read()) return null;

            return new User
            {
                Id = (int)reader["Id"],
                Username = reader["Username"].ToString()!,
                Email = reader["Email"].ToString()!,
                PasswordHash = reader["PasswordHash"].ToString()!
            };
        }

        public bool ValidatePassword(User user, string password)
        {
            var hasher = new PasswordHasher<User>();
            return hasher.VerifyHashedPassword(user, user.PasswordHash, password) == PasswordVerificationResult.Success;
        }

        public string GenerateJwtToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("UserId", user.Id.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public bool RegisterUser(User user, string password)
        {
            var hasher = new PasswordHasher<User>();
            user.PasswordHash = hasher.HashPassword(user, password);

            using var conn = new SqlConnection(_connectionString);
            using var cmd = new SqlCommand(
                "INSERT INTO Users (Username, Email, PasswordHash) VALUES (@Username, @Email, @PasswordHash)", conn);

            cmd.Parameters.AddWithValue("@Username", user.Username);
            cmd.Parameters.AddWithValue("@Email", user.Email);
            cmd.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);

            conn.Open();
            return cmd.ExecuteNonQuery() > 0;
        }
    }
}
