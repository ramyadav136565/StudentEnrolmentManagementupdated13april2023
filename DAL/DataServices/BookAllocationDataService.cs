namespace DAL
{
    using DAL.Models;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class BookAllocationDataService
    {
        private BookStoreContext _dbContext;
        public BookAllocationDataService(BookStoreContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<BookAllocation>> ShowAllocatedBooks()
        {
            List<BookAllocation> books;
            try
            {
                books = await _dbContext.BookAllocations.ToListAsync();
                return books;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<List<object>> GetAllocatedBooskByUniversityIdAndStudentId(int universityId, int studentId)
        {
            var university = await _dbContext.Universities.FindAsync(universityId);
            if (university == null)
            {
                throw new Exception("Please enter correct universityid");
            }
            var student = await _dbContext.Students.FindAsync(studentId);
            if (student == null || student.UniversityId != universityId)
            {
                throw new Exception("Please enter correct studentId and universityId");
            }
            var books = await _dbContext.BookAllocations
                .Where(ba => ba.StudentId == studentId && ba.UniversityId == universityId)
                .Join(_dbContext.Books,
                    ba => ba.BookId,
                    b => b.BookId,
                    (ba, b) => new { ba.StudentId, b.BookId, b.BookName, b.BookAuthor })
                .ToListAsync();

            if (!books.Any())
            {
                throw new Exception("No book allocation found for the given university and student");
            }

            return books.Cast<object>().ToList();
        }

        public async Task<List<object>> GetAllocatedBooksToUniversity(string universityIdOrUniversityName, int term)
        {
            var result = await _dbContext.Universities
                .Where(u => u.UniversityId.ToString() == universityIdOrUniversityName || u.Name == universityIdOrUniversityName)
                .SelectMany(u => u.Students.Where(s => s.Term == term), (u, s) => new { University = u, Student = s })
                .SelectMany(us => us.Student.BookAllocations, (us, ba) => new { us.University, us.Student, BookAllocation = ba })
                .Select(ba => new
                {
                    StudentName = ba.Student.FullName,
                    StudentEmail = ba.Student.Email,
                    BookName = ba.BookAllocation.Book.BookName,
                    BookPrice = ba.BookAllocation.Book.BookPrice,
                    Course = ba.Student.Course
                })
                .ToListAsync();

            return result.Cast<object>().ToList();
        }

        public async Task<BookAllocation> GetBookAllocationById(int serialNO)
        {
            try
            {
                var Bookllocation = await _dbContext.BookAllocations.FindAsync(serialNO);
                if (Bookllocation == null)
                {
                    throw new Exception($"Book  Allocation not found.");
                }
                return Bookllocation;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<BookAllocation> AllocateBookToStudent(int studentId, int bookId, int universityId)
        {
            var university = await _dbContext.Universities.FindAsync(universityId);
            if (university == null)
            {
                throw new Exception("University record not found.");
            }


            var student = await _dbContext.Students.FirstOrDefaultAsync(s => s.StudentId == studentId && s.UniversityId == universityId);
            if (student == null)
            {
                throw new ArgumentException($"Student with {studentId} is not enrolled in {university.Name}.");
            }


            var book = await _dbContext.Books.FindAsync(bookId);
            if (book == null)
            {
                throw new Exception($"Book with id {bookId} does not exist.");
            }
            else if (book.IsDeleted)
            {
                throw new Exception($"{book.BookName} book  is not available.");
            }

           



            var allocation = await _dbContext.BookAllocations.FirstOrDefaultAsync(a => a.BookId == bookId);
            if (allocation != null && allocation.StudentId == studentId)
            {
                throw new Exception($"{book.BookName} book is already allocated to the student {student.FullName}.");
            }


            allocation = new BookAllocation
            {   
                BookId = bookId,
                StudentId = studentId,
                UniversityId = universityId
            };

            _dbContext.BookAllocations.Add(allocation);
            await _dbContext.SaveChangesAsync();

            return allocation;
        }
        public async Task<BookAllocation> UpdateAllocatedBooksToStudent(BookAllocation allocatedBook)
        {
            try
            {
                var existingBook = await _dbContext.Books.FindAsync(allocatedBook.BookId);
                if (existingBook != null)
                {
                    _dbContext.Entry(existingBook).CurrentValues.SetValues(allocatedBook);
                    await _dbContext.SaveChangesAsync();
                    return allocatedBook;
                }
                else
                {
                    throw new Exception("Book not found.");
                }
            }
            catch (DbUpdateException ex)
            {
                throw new Exception("An error occurred while updating the book.", ex);
            }
        }

        public async Task<string> DeleteBookAllocation(int serialNo)
        {
            try
            {

                var bookAllocation = await _dbContext.BookAllocations.FindAsync(serialNo);
                if (bookAllocation != null)
                {
                    _dbContext.BookAllocations.Remove(bookAllocation);
                    await _dbContext.SaveChangesAsync();
                    return "The Book deallocation has been successfully completed";

                }
                else
                {
                    return $"BookAllocation record not found.";
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
