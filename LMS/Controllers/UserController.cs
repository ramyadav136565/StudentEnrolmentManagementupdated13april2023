namespace StudentEnrolmentManagement.Controllers
{
    using DAL;
    using DAL.Models;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Data.SqlClient;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    [Authorize(Roles = "Admin")]
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

        [HttpGet("GetUserById/{userId}")]

        public async Task<IActionResult> GetUserById(int userId)
        {
            try
            {
                var user = await _userService.GetUserById(userId);
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
        [HttpGet("[Action]")]
        public IActionResult UsersExport()
        {
            using (SqlConnection connection = new SqlConnection(@"Data Source=INBLRVM26590142;Initial Catalog=BookStore;Integrated Security=True"))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("select * from dbo.Users", connection))
                {
                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        DataSet ds = new DataSet();
                        adapter.Fill(ds);
                        string csvData = TransformTableToCsv(ds.Tables[0]);



                        var fileByes = Encoding.UTF8.GetBytes(csvData);
                        return File(fileByes, "text/csv", "Userstdata.csv");



                    }
                }
            }
        }
        private string TransformTableToCsv(DataTable dataTable)
        {
            StringBuilder csvBuilder = new StringBuilder();
            IEnumerable<string> columnNames = dataTable.Columns.Cast<DataColumn>()
            .Select(x => x.ColumnName);
            csvBuilder.AppendLine(string.Join(',', columnNames));
            foreach (DataRow row in dataTable.Rows)
            {
                IEnumerable<string> fields = row.ItemArray.Select
                (x => string.Concat("\"", x.ToString().Replace("\"", "\"\""), "\""));
                csvBuilder.AppendLine(string.Join(',', fields));



            }
            return csvBuilder.ToString();
        }
    }
}
