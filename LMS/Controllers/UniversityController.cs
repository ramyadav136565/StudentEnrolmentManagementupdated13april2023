namespace StudentEnrolmentManagement.Controllers
{
    using DAL;
    using DAL.Models;
    using Microsoft.AspNetCore.Mvc;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
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
        [Route("SearchUniversityById/{UniversityId}")]
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
        [HttpDelete("DeleteUniversity/{id}")]
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
    }
}
