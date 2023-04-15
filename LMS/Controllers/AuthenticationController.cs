namespace UILayer.Controllers
{
    using DAL.Models;
    using DALayer;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.IdentityModel.Tokens;
    using System;
    using System.Collections.Generic;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Text;
    using System.Threading.Tasks;
    using System.Linq;
    using DAL;

    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private IConfiguration _config;
        private BookStoreContext _dbContext;
      
        private BookStoreContext service;
        private AuthenticationDataService _authService;
        public AuthenticationController(IConfiguration config, AuthenticationDataService authService, BookStoreContext dbContext)
        {
            _dbContext = dbContext;
            _config = config;
            _authService = authService;
        }
        private string CreateToken(string userIdOrEmail)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtSettings:Key"]));
            var CredentialInfo = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var role = (from user in _dbContext.Users
                            join roles in _dbContext.Roles on user.RoleId equals roles.RoleId
                            where user.UserId.ToString() == userIdOrEmail || user.Email == userIdOrEmail
                            select roles.UserRole).FirstOrDefault().ToString();

            var claim = new List<Claim>();
            claim.Add(new Claim(ClaimTypes.Role, role));
            var token = new JwtSecurityToken(
            issuer: _config["JwtSettings:Issuer"],
            audience: _config["JwtSettings:Audience"],
            expires: DateTime.Now.AddDays(5),
            claims: claim,
            signingCredentials: CredentialInfo
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        [HttpPost("ValidateUser")]
        public async Task<IActionResult> ValidateUser(string userIdOrEmail, string password)
        {
           
            try
            {
                var userData = await _authService.ValidateUser(userIdOrEmail, password);
                if (userData != null)
                {
                    var token = CreateToken(userData.Email);
                    SaveToken(token);
                    return Ok(new { Token = token });
                }
                else
                {
                    return BadRequest("Invalid Credentials");
                }
            }
            catch (Exception e)
            {

                throw new Exception(e.Message);
            }
           
        }
        [HttpPost("RegisterUser")]
        public async Task<IActionResult> RegisterUser(string userIdOrEmail, string password)
        {
            try
            {
                var userData = await _authService.RegisterUser(userIdOrEmail, password);
                if (userData != null)
                {
                    return Ok(userData);
                }
                else
                {
                    return BadRequest("Registration Failed");
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        private void SaveToken(string token)
        {
            HttpContext.Response.Cookies.Append("Jwt", token);
        }
        [HttpPost("Logout")]
        public IActionResult Logout()
        {
            HttpContext.Response.Cookies.Delete("Jwt");
            return Ok();
        }
    }
}


/*
 
 private IConfiguration _config;
        private BookStoreContext _dbContext;
        private AuthenticationDataService _authService;
        public AuthenticationController(IConfiguration config, AuthenticationDataService authService, BookStoreContext dbContext)
        {
            _config = config;
            _authService = authService;
            _dbContext = dbContext;
        }

       
        private string CreateToken(string userIdOrEmail)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtSettings:Key"]));
            var CredentialInfo = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var userRole = (from user in _dbContext.Users
                            join roles in _dbContext.Roles on user.RoleId equals roles.RoleId
                            where user.UserId.ToString() == userIdOrEmail || user.Email == userIdOrEmail
                            select roles).FirstOrDefault();


            string role = String.Join(" ", userRole);


            var claim = new List<Claim>();
            claim.Add(new Claim(ClaimTypes.Role, role));
 
 */