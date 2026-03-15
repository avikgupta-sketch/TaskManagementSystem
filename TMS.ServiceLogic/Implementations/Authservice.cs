using AutoMapper;
using BC = BCrypt.Net.BCrypt;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TMS.Contracts.Response;
using TMS.Contracts.Request;
using TMS.Model.Data;
using TMS.Model.Entities;
using TMS.Model.Enums;
using TMS.ServiceLogic.Interface;

namespace TMS.ServiceLogic.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public AuthService(AppDbContext context, IConfiguration configuration,IMapper mapper)
        {
            _context = context;
            _configuration = configuration;
            _mapper = mapper;
        }

        public async Task<string> RegisterAsync(RegisterRequest request)
        {
            // Check if email already exists
            if (await _context.Users.AnyAsync(u => u.Email == request.Email))
                return "AlreadyRegistered";

            // Mapping Username,Email from DTO to User
            var user = _mapper.Map<User>(request);
            user.PasswordHash = BC.HashPassword(request.Password);
            

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return "Success";
        }

        public async Task<AuthResponse?> LoginAsync(LoginRequest request)
        {
            // Find user by email
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == request.Email);

            if (user == null || !BC.Verify(request.Password, user.PasswordHash))
                return null;

            return GenerateToken(user);
        }

        public async Task<string> DeleteUserAsync(int userId)
        {
            var user = await _context.Users
                .Include(u => u.AssignedTasks)
                .Include(u => u.CreatedTasks)
                .FirstOrDefaultAsync(u => u.Id == userId ); 

            if (user == null) return "User not found";

            //  Check InProgress tasks for any user and assigned task in progress or deleted
            
            bool hasInProgressTasks = user.AssignedTasks
                .Any(t=>t.Status == TMS.Model.Enums.TaskStatus.InProgress);

            if (hasInProgressTasks)
                return "Cannot delete user: They have tasks currently 'In Progress'.";

            //  Admin specific rule
            if (user.Role == UserRole.Admin)
            {
                // Check if they created any tasks that aren't deleted
                bool hasActiveCreatedTasks = user.CreatedTasks.Any();
                if (hasActiveCreatedTasks)
                    return "Cannot delete Admin: This admin has active tasks in the system.";
            }


           
            user.IsDeleted = true;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return "Success";
        }

        private AuthResponse GenerateToken(User user)
        {
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["JwtSettings:Key"]!));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            var expiry = DateTime.UtcNow.AddDays(
                int.Parse(_configuration["JwtSettings:ExpiryInDays"]!));

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                expires: expiry,
                signingCredentials: credentials
            );

            var response = _mapper.Map<AuthResponse>(user);
            response.Token = new JwtSecurityTokenHandler().WriteToken(token);
            response.ExpiresAt = expiry;

            return response;
        }
    }
}


