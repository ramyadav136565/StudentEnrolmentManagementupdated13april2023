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
    public class UniversityController : ControllerBase
    {
        private readonly UniversityDataService _universityService;
        public UniversityController(UniversityDataService universityService)
        {
            _universityService = universityService;
        }
        [HttpGet]
        [Route("ShowAllUniversities")]
        public async Task<IActionResult> ShowAllUniversities()
        {
            List<University> universities;
            try
            {
                universities = await _universityService.ShowAllUniversities();
                return Ok(universities);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [HttpGet]
        [Route("SearchUniversityById/{universityId}")]
        public async Task<IActionResult> SearchUniversityById(int universityId)
        {
            try
            {
                var university = await _universityService.SearchUniversityById(universityId);
                return Ok(university);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }






        [HttpPost]
        [Route("AddUniversity")]
        public async Task<IActionResult> AddUniversity(University university)
        {
            try
            {
                var newUniversity = await _universityService.AddUniversity(university);
                return Ok(newUniversity);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [HttpPut("UpdateUniversity")]
        public async Task<IActionResult> UpdateUniversity([FromBody] University university)
        {
            try
            {
                var updateduniversity = await _universityService.UpdateUniversity(university);
                return Ok(updateduniversity);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
        [HttpDelete("DeleteUniversity/{universityId}")]
        public async Task<IActionResult> DeleteUniversity(int universityId)
        {
            try
            {
                var university = await _universityService.DeleteUniversity(universityId);
                return Ok(university);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
        [HttpGet("[Action]")]
        public IActionResult UniversityExport()
        {
            using (SqlConnection connection = new SqlConnection(@"Data Source=INBLRVM26590142;Initial Catalog=BookStore;Integrated Security=True"))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("select * from dbo.University", connection))
                {
                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        DataSet ds = new DataSet();
                        adapter.Fill(ds);
                        string csvData = TransformTableToCsv(ds.Tables[0]);



                        var fileByes = Encoding.UTF8.GetBytes(csvData);
                        return File(fileByes, "text/csv", "Universitytdata.csv");



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
