namespace StudentEnrolmentManagement.Controllers
{
    using DAL;
    using DAL.Models;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    [Authorize]
    public class BookController : ControllerBase
    {
        private readonly BookDataService _bookService;
        public BookController(BookDataService bookService)
        {
            _bookService = bookService;
        }
        [HttpGet]
        [Route("ShowAllBooks")]
        public async Task<IActionResult> ShowAllBooks()
        {
            List<Book> books;
            try
            {
                books = await _bookService.ShowAllBooks();
                return Ok(books);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [HttpGet]
        [Route("ShowBookById/{BookId}")]
        public async Task<IActionResult> ShowBookById(int BookId)
        {
            try
            {
                var book = await _bookService.GetBookById(BookId);
                return Ok(book);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [HttpPost]
        [Route("AddBooks")]
        public async Task<IActionResult> AddBook(Book book)
        {
            try
            {
                var addedBook = await _bookService.AddBook(book);
                return Ok(addedBook);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("UpdateBook")]
        public async Task<IActionResult> UpdateBook([FromBody] Book book)
        {
          
            try
            {
              var  updatedBook = await _bookService.UpdateBook(book);
                return Ok(updatedBook);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
        [HttpDelete("DeleteBook/{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            try
            {
                var book = await _bookService.DeleteBook(id);
                return Ok(book);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
