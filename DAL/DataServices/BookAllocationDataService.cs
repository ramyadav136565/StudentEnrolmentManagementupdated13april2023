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

            if (term < 1 || term > 12)
            {
                throw new ArgumentException("Please enter valid term");
            }

            var university = await _dbContext.Universities.FirstOrDefaultAsync(u => u.UniversityId.ToString() == universityIdOrUniversityName || u.Name == universityIdOrUniversityName);
            if (university == null)
            {
                throw new ArgumentException("University does not exist");
            }

            var books = await _dbContext.BookAllocations
                .Join(_dbContext.Students,
                    ba => ba.StudentId,
                    s => s.StudentId,
                    (ba, s) => new { BookAllocation = ba, Student = s })
                .Join(_dbContext.Universities,
                    bs => bs.Student.UniversityId,
                    u => u.UniversityId,
                    (bs, u) => new { bs.Student, University = u, bs.BookAllocation })
                .Join(_dbContext.Books,
                    bsu => bsu.BookAllocation.BookId,
                    b => b.BookId,
                    (bsu, b) => new { bsu.University, bsu.Student, Book = b })
                .Where(bsb => (bsb.University.UniversityId.ToString() == universityIdOrUniversityName || bsb.University.Name == universityIdOrUniversityName)
                    && bsb.Student.Term == term)
                .Select(bsb => new
                {
                    bsb.University.Name,
                    bsb.Student.StudentId,
                    bsb.Student.Term,
                    bsb.Book.BookId,
                    bsb.Book.BookName,
                    bsb.Book.BookAuthor
                })
                .ToListAsync();

            if (books.Count == 0)
            {
                throw new ArgumentException($"No book allocation found for university '{universityIdOrUniversityName}' in term '{term}'");
            }

            return books.Cast<object>().ToList();
        }
        
        public async Task<BookAllocation> AllocateBookToStudent(int studentId, int bookId, int universityId)
        {
            var book = await _dbContext.Books.FindAsync(bookId);
            if (book == null)
            {
                throw new ArgumentException($"Book with id {bookId} does not exist.");
            }
            else if (book.IsDeleted)
            {
                throw new Exception($"Book with id {bookId} is not available. Please contact to admin.");
            }

            var allocation = await _dbContext.BookAllocations.FirstOrDefaultAsync(a => a.BookId == bookId);
            if (allocation != null && allocation.StudentId == studentId)
            {
                throw new Exception($"This book is already allocated to the student .");
            }


            var student = await _dbContext.Students.FirstOrDefaultAsync(s => s.StudentId == studentId && s.UniversityId == universityId);
            if (student == null)
            {
                throw new ArgumentException($"Student with id {studentId} does not exist in university with id {universityId}.");
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
        
        public async Task<BookAllocation> UpdateAllocatedBooksToStudent(BookAllocation allocatedBook)    //see
        {
            try
            {
                var book = await _dbContext.Books.FindAsync(allocatedBook.BookId);
                if (book != null && !book.IsDeleted)
                {
                    var existingAllocation = await _dbContext.BookAllocations
                        .FirstOrDefaultAsync(ba => ba.StudentId == allocatedBook.StudentId && ba.BookId == allocatedBook.BookId);

                    if (existingAllocation != null)
                    {
                        throw new Exception("Book is already allocated to the student.");
                    }
                    else
                    {
                        await _dbContext.BookAllocations.AddAsync(allocatedBook);
                        await _dbContext.SaveChangesAsync();
                        return allocatedBook;
                    }
                }
                else if (book == null)
                {
                    throw new Exception("Book not found.");
                }
                else
                {
                    throw new Exception("Book is marked as available.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Same book can not be allocated to one student", ex);
            }
        }
        
        public async Task<string> DeleteAllocatedBook(int universityId, int studentId, int bookId)
        {
            try
            {
                var university = await _dbContext.Universities.FindAsync(universityId);
                if (university == null)
                {
                    return "Please enter correct universityId or universityName";
                }
                var student = await _dbContext.Students.FindAsync(studentId);
                if (student == null)
                {
                    return "Please enter correct studentId";
                }
                var book = await _dbContext.Books.FindAsync(bookId);
                if (book == null)
                {
                    return "Please enter correct BookId or BookName";
                }
                var studentBook = await _dbContext.BookAllocations
                    .FirstOrDefaultAsync(ba => ba.StudentId == studentId && ba.BookId == bookId && ba.UniversityId == universityId);

                if (studentBook != null)
                {
                    _dbContext.BookAllocations.Remove(studentBook);
                    await _dbContext.SaveChangesAsync();
                    return "The BookDeAllocation has been successfully completed";

                }
                else
                {
                    return $"BookAllocation record not found for student {student.FullName} in the  {university.Name} University .";
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
