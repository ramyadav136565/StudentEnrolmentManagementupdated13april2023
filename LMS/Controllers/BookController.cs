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
        [HttpGet("[Action]")]
        public IActionResult BookExport()
        {
            using (SqlConnection connection = new SqlConnection(@"Data Source=INBLRVM26590142;Initial Catalog=BookStore;Integrated Security=True"))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("select * from dbo.Book", connection))
                {
                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        DataSet ds = new DataSet();
                        adapter.Fill(ds);
                        string csvData = TransformTableToCsv(ds.Tables[0]);



                        var fileByes = Encoding.UTF8.GetBytes(csvData);
                        return File(fileByes, "text/csv", "Bookdata.csv");



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
