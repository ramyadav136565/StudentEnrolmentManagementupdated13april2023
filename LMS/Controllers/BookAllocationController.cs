namespace StudentEnrolmentManagement.Controllers
{
    using DAL;
    using DAL.Models;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    [ApiController]
    public class BookAllocationController : ControllerBase
    {
        private readonly BookAllocationDataService _bookAllocationService;
        public BookAllocationController(BookAllocationDataService bookAllocationService)
        {
            _bookAllocationService = bookAllocationService;
        }

        [HttpGet]
        [Route("ShowAllocatedBooks")]
        public async Task<IActionResult> ShowAllocatedBooks()
        {
            List<BookAllocation> books;
            try
            {
                books = await _bookAllocationService.ShowAllocatedBooks();
                return Ok(books);

            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }


        [HttpGet]
        [Route("GetAllocatedBooskByUniversityIdAndStudentId/{universityid}/{studentId}")]
        public async Task<IActionResult> GetAllocatedBooskByUniversityIdAndStudentId(int universityid, int studentId)
        {
            try
            {
                var books = await _bookAllocationService.GetAllocatedBooskByUniversityIdAndStudentId(universityid, studentId);
                return Ok(books);

            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }


        [HttpGet("GetAllocatedBooksToUniversity/{universityIdOrUniversityName}/{term}")]
         public async Task<IActionResult> GetAllocatedBooksToUniversity(string universityIdOrUniversityName,int term)
        {
            try
            {
                var books = await _bookAllocationService.GetAllocatedBooksToUniversity(universityIdOrUniversityName,term);
                return Ok(books);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


            [HttpPost]
        [Route("AllocateBookToStudent/{studentId}/{bookId}/{universityId}")]
        public async Task<IActionResult> AllocateBookToStudent(int studentId, int bookId, int universityId)
        {
            try
            {
                var books = await _bookAllocationService.AllocateBookToStudent(studentId, bookId, universityId);
                return Ok(books);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }



        [HttpPut("UpdateAllocatedBooksToStudent")]
        public async Task<IActionResult> UpdateAllocatedBooksToStudent([FromBody] BookAllocation allocatedBook)
        {
            BookAllocation updateAllocatedBook;
            try
            {


                updateAllocatedBook = await _bookAllocationService.UpdateAllocatedBooksToStudent(allocatedBook);
                return Ok(updateAllocatedBook);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }


        [HttpDelete("DeleteAllocatedBook/{universityId}/{studentId}/{bookId}")]
        public async Task<IActionResult> DeleteAllocatedBook(int universityId, int studentId,int bookId)
        {
            try
            {
                var Deallocatebook = await _bookAllocationService.DeleteAllocatedBook(universityId,studentId, bookId);
                return Ok(Deallocatebook);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
