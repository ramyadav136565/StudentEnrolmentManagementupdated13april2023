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
    [Authorize]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly StudentDataService _studentService;
        public StudentController(StudentDataService studentService)
        {
            _studentService = studentService;
        }
        [HttpGet]
        [Route("ShowAllStudents")]
        public async Task<IActionResult> ShowAllStudents()
        {
            List<Student> students;
            try
            {
                students = await _studentService.ShowAllStudents();
                return Ok(students);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [HttpGet]
        [Route("ShowStudentById/{studentId}")]
        public async Task<IActionResult> ShowStudentById(int studentId)
        {
            try
            {
                var student = await _studentService.GetStudentById(studentId);
                return Ok(student);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [HttpPost]
        [Route("AddStudent")]
        public async Task<IActionResult> AddStudent(Student student)
        {
            try
            {
                var newStudent = await _studentService.AddStudent(student);
                return Ok(newStudent);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [HttpPut("UpdateStudent")]
        public async Task<IActionResult> UpdateStudent([FromBody] Student student)
        {
            Student updatedStudent;
            try
            {
                updatedStudent = await _studentService.UpdateStudent(student);
                return Ok(updatedStudent);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
       
        
        [HttpDelete("DeleteStudent/{studentId}")]
        public async Task<IActionResult> DeleteStudent(int studentId)
        {
            try
            {
                var result = await _studentService.DeleteStudent(studentId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while deleting the student record.", ex);
            }
        }

        [HttpGet("[Action]")]
        public IActionResult Export()
        {
            using (SqlConnection connection = new SqlConnection(@"Data Source=INBLRVM26590142;Initial Catalog=BookStore;Integrated Security=True"))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("select * from dbo.Student", connection))
                {
                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        DataSet ds = new DataSet();
                        adapter.Fill(ds);
                        string csvData = TransformTableToCsv(ds.Tables[0]);



                        var fileByes = Encoding.UTF8.GetBytes(csvData);
                        return File(fileByes, "text/csv", "Studentdata.csv");



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
