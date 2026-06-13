using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using HospitalManagementSystem.API.DTOs;
using HospitalManagementSystem.API.Models;
using HospitalManagementSystem.API.Repositories.Interfaces;
using HospitalManagementSystem.API.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace HospitalManagementSystem.API.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public AuthService(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        public async Task<AuthResponseDto?> LoginAsync(LoginDto dto)
        {
            var user = await _userRepository.GetByUsernameAsync(dto.Username);
            if (user == null) return null;
            if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash)) return null;

            return GenerateToken(user);
        }

        public async Task<(AuthResponseDto? result, string? error)> RegisterAsync(RegisterDto dto)
        {
            var validRoles = new[] { "Admin", "Doctor", "Patient" };
            if (!validRoles.Contains(dto.Role))
                return (null, "Role must be Admin, Doctor, or Patient.");

            if (await _userRepository.UsernameExistsAsync(dto.Username))
                return (null, "Username already taken.");

            var user = new User
            {
                Username = dto.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Role = dto.Role,
                Email = dto.Email
            };

            var created = await _userRepository.CreateAsync(user);
            return (GenerateToken(created), null);
        }

        private AuthResponseDto GenerateToken(User user)
        {
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.AddHours(8);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: expires,
                signingCredentials: creds);

            return new AuthResponseDto
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Username = user.Username,
                Role = user.Role,
                ExpiresAt = expires
            };
        }
    }
}
