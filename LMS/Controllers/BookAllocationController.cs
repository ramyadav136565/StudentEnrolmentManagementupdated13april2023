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
        [Route("ShowAllAllocatedBooks")]
        public async Task<IActionResult> ShowAllAllocatedBooks()
        {
            List<BookAllocation> books;
            try
            {
                books = await _bookAllocationService.ShowAllAllocatedBooks();
                return Ok(books);

            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [HttpGet]
        [Route("GetAllocatedBooksToStudent/{studentId}")]
        public async Task<IActionResult> GetAllocatedBookByStudentId(int studentId)
        {
            try
            {
                var books = await _bookAllocationService.GetAllocatedBookByStudentId(studentId);
                return Ok(books);

            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [HttpGet("ShowAllAllocatedBooksTouniversity/{universityIdOrUniversityName}/{term}")]
        public async Task<IActionResult> GetAllocatedBookByUniversity(string universityIdOrUniversityName,int term)
        {
            try
            {
                var books = await _bookAllocationService.GetAllocatedBookByUniversity(universityIdOrUniversityName,term);
                return Ok(books);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
            [HttpPost]
        [Route("BookAllocateToStudent/{studentId}/{bookId}/{universityIdOrUniversityName}")]
        public async Task<IActionResult> BookAllocateToStudent(int studentId, int bookId, string universityIdOrUniversityName)
        {
            try
            {
                var books = await _bookAllocationService.BookAllocateToStudent(studentId, bookId, universityIdOrUniversityName);
                return Ok(books);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("UpdateAllocatedBookToStudent")]
        public async Task<IActionResult> UpdateAllocatedBookToStudent([FromBody] BookAllocation allocatedBook)
        {
            BookAllocation updateAllocatedBook;
            try
            {


                updateAllocatedBook = await _bookAllocationService.UpdateAllocatedBookToStudent(allocatedBook);
                return Ok(updateAllocatedBook);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
        [HttpDelete("DeleteAllocatedBook/{studentId}/{bookId}")]
        public async Task<IActionResult> DeleteAllocatedBook(int studentId,int bookId)
        {
            try
            {
                var Deallocatebook = await _bookAllocationService.DeleteAllocatedBook(studentId,bookId);
                return Ok(Deallocatebook);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
