using BookStore_API.Contracts;
using BookStore_API.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BookStore_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : Controller
    {
        // Dependancys
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userInManager;
        private readonly ILoggerService _logger;
        private readonly IConfiguration _config;


   public UsersController(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userInManager, ILoggerService logger, IConfiguration config)
        {
            _signInManager = signInManager;
            _userInManager = userInManager;
            _logger = logger;
            _config = config;
        }

        /// <summary>
        ///    registor endpoint
        /// </summary>
        /// <param name="userDTO"></param>
        /// <returns></returns>
        [Route("registor")]
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Registor([FromBody] UserDTO userDTO)
        {
            var location = GetControloerActionNames();

            try
            {
                // Create new user
                var user = new IdentityUser {Email =  userDTO.EmailAddress, UserName = userDTO.EmailAddress };
                var result = await _userInManager.CreateAsync(user, userDTO.Password);
         

                if (!result.Succeeded)
                {
                  

                    foreach (var item in result.Errors)
                    {
                        _logger.Logerror($"{location}- {item.Code} {item.Description}");
                    }

                    return InterError($"{location}- {user.UserName} registration failed");

                }

                // return new geniric object
                return Ok(new { result.Succeeded});
            }
            catch (Exception ex)
            {
                _logger.Logerror(ex.Message);
                return InterError($"{location}- {ex.InnerException} - {ex.Message}");
            }

        }


        /// <summary>
        ///    userlogin endpoint
        /// </summary>
        /// <param name="userDTO"></param>
        /// <returns></returns>
        [Route("login")]
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] UserDTO userDTO)
        {
            var location = GetControloerActionNames();

            try
            {
                var result = await _signInManager.PasswordSignInAsync(userDTO.EmailAddress, userDTO.Password, false, false);

                if (result.Succeeded)
                {
                    var user = await _userInManager.FindByNameAsync(userDTO.EmailAddress);
                    var tokenString = GenerateJSONWebToken(user);
                    return Ok(new { tokenString });
                }
                return Unauthorized(userDTO);
            }
            catch (Exception ex)
            {
                _logger.Logerror(ex.Message);
                return InterError($"{location}- {ex.InnerException} - {ex.Message}");
            }

        }

        private async Task<string> GenerateJSONWebToken(IdentityUser user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                 new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                  new Claim(ClaimTypes.NameIdentifier, user.Id)
             };
            var roles = await _userInManager.GetRolesAsync(user);
            claims.AddRange(roles.Select(r => new Claim(ClaimsIdentity.DefaultRoleClaimType, r)));

            var token = new JwtSecurityToken(_config["Jwt:Issuer"], _config["Jwt:Issuer"], claims, null, expires: DateTime.Now.AddMinutes(5), signingCredentials:credentials);
            return  new JwtSecurityTokenHandler().WriteToken(token);
        }





        private ObjectResult InterError(string message)
        {
            _logger.Logerror(message);
            return StatusCode(500, "Something went wrong");
        }

        private string GetControloerActionNames()
        {
            var controllor = ControllerContext.ActionDescriptor.ControllerName;
            var action = ControllerContext.ActionDescriptor.ActionName;

            return $"{controllor} - {action}";
        }
    }
}
