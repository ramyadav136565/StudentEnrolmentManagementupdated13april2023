namespace StudentEnrolmentManagement.Controllers
{
    using DAL;
    using DAL.Models;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    //[Authorize]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserDataService _userService;
        public UserController(UserDataService userService)
        {
            _userService = userService;
        }

        [HttpGet("ShowAllUsers")]
       public async Task<IActionResult> ShowAllUsers()
        {
            List<User> users;
            try
            {
                users = await _userService.ShowAllUsers();
                return Ok(users);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [HttpGet("ActiveUserById/{userId}")]
      
        public async Task<IActionResult> ShowUserById(int userId)
        {
            try
            {
                var user = await _userService.ActiveUserById(userId);
                return Ok(user);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [HttpPost]
        [Route("AddNewUser")]
        public async Task<IActionResult> AddNewUser(User user)
        {
            try
            {
                var userDetail = await _userService.AddNewUser(user);
                return Ok(userDetail);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [HttpPut("UpdateUser")]
        public async Task<IActionResult> UpdateUser([FromBody] User user)
        {
            User updatedUser;
            try
            {
                updatedUser = await _userService.UpdateUser(user);
                return Ok(updatedUser);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
        [HttpDelete("DeleteUser/{userId}")]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            try
            {
                var user = await _userService.DeleteUser(userId);
                return Ok(user);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
