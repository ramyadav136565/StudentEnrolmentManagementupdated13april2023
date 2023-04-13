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
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private IConfiguration _config;

        private AuthenticationDataService _authService;
        public AuthenticationController(IConfiguration config, AuthenticationDataService authService)
        {
            _config = config;
            _authService = authService;
        }
        private string CreateToken(string role)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtSettings:Key"]));
            var CredentialInfo = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var clm = new List<Claim>();
            clm.Add(new Claim(ClaimTypes.Role, role));
            var token = new JwtSecurityToken(
            issuer: _config["JwtSettings:Issuer"],
            audience: _config["JwtSettings:Audience"],
            expires: DateTime.Now.AddDays(5),
            claims: clm,
            signingCredentials: CredentialInfo
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        [HttpPost("ValidateUser")]
        public async Task<IActionResult> ValidateUser(string userIdOrEmail, string password)
        {
            TokenDetail tokenDetail = new TokenDetail();
            try
            {
                var userData = await _authService.LogInUser(userIdOrEmail, password);
                if (userData != null)
                {
                    var token = CreateToken(userData.Email);
                    tokenDetail.Token = token;
                    SaveToken(token);
                    return Ok(tokenDetail);
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
        public async Task<IActionResult> RegisterUser(int userId, string password)
        {
            try
            {
                var userData = await _authService.RegisterUser(userId,password);
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
    }
}
