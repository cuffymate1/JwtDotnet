using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using WebStoreApi.Authentication;

namespace WebStoreApi.Controllers
{
    
    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

        public AuthenticationController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var user = await _userManager.FindByNameAsync(model.Username);
            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }

                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

                var token = new JwtSecurityToken(
                    issuer: _configuration["JWT:ValidIssuer"],
                    audience: _configuration["JWT:ValidAudience"],
                    expires: DateTime.Now.AddHours(3),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo
                }); 
            }
            return Unauthorized();
        }
        // Register 
        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model){
            var userExists = await _userManager.FindByNameAsync(model.Username); // Check if user exists
            if(userExists != null){
                return StatusCode(500, new Response { Status = "Error", Message = "User already exists!" });
            }

            ApplicationUser user = new ApplicationUser() // Create new user
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(), // Generate unique identifier
                UserName = model.Username
            };

            var result = await _userManager.CreateAsync(user, model.Password); // Create user
            if(!result.Succeeded){
                return StatusCode(500, new Response { 
                    Status = "Error", 
                    Message = "User creation failed! Please check user details and try again." 
                });
            }

            return Ok(new Response { 
                Status = "Success", 
                Message = "User created successfully!" 
            });
        }

        // Register Admin
        [HttpPost]
        [Route("register-admin")]
        public async Task<IActionResult> RegisterAdmin([FromBody] RegisterModel model){
            var userExists = await _userManager.FindByNameAsync(model.Username); // Check if user exists
            if(userExists != null){
                return StatusCode(500, new Response { 
                    Status = "Error", 
                    Message = "User already exists!" 
                });
            }

            ApplicationUser user = new ApplicationUser() // Create new user
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(), // Generate unique identifier
                UserName = model.Username
            };

            var result = await _userManager.CreateAsync(user, model.Password); // Create user
            if(!result.Succeeded){
                return StatusCode(500, new Response { 
                    Status = "Error", 
                    Message = "User creation failed! Please check user details and try again." 
                });
            }

            if(!await _roleManager.RoleExistsAsync(UserRoles.Admin)){ // Check if admin role exists
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.Admin)); // Create admin role
            }

            if (!await _roleManager.RoleExistsAsync(UserRoles.User)){ // Check if user role exists
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.User)); // Create user role
            }

            if(await _roleManager.RoleExistsAsync(UserRoles.Admin)){ // Check if admin role exists
                await _userManager.AddToRoleAsync(user, UserRoles.Admin); // Add user to admin role
            }

            return Ok(new Response { 
                Status = "Success", 
                Message = "User created successfully!" 
            });
        }
    }
}