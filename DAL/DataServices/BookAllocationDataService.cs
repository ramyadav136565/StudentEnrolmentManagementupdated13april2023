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
        public async Task<List<BookAllocation>> ShowAllAllocatedBooks()
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
        public async Task<List<object>> GetAllocatedBookByStudentId(int studentId)
        {
            var books = await _dbContext.BookAllocations
                .Where(ba => ba.StudentId == studentId)
                .Join(_dbContext.Students,
                    ba => ba.StudentId,
                    s => s.StudentId,
                    (ba, s) => new { BookAllocation = ba, Student = s })
                .Join(_dbContext.Books,
                    bs => bs.BookAllocation.BookId,
                    b => b.BookId,
                    (bs, b) => new { bs.Student, Book = b })
                .Select(bsb => new
                {
                    bsb.Student.StudentId,
                    bsb.Book.BookId,
                    bsb.Book.BookName,
                    bsb.Book.BookAuthor
                })
                .ToListAsync();
            return books.Cast<object>().ToList();
        }

        public async Task<List<object>> GetAllocatedBookByUniversity(string universityIdOrUniversityName, int term)
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

        public async Task<BookAllocation> BookAllocateToStudent(int studentId, int bookId, string universityIdOrUniversityName)
        {
            var university = await _dbContext.Universities
                .Include(u => u.Students)
                .FirstOrDefaultAsync(u => u.UniversityId.ToString() == universityIdOrUniversityName || u.Name == universityIdOrUniversityName);

            if (university == null)
            {
                throw new ArgumentException($"University with id or name {universityIdOrUniversityName} does not exist.");
            }

            var book = await _dbContext.Books.FindAsync(bookId);

            if (book == null)
            {
                throw new ArgumentException($"Book with id {bookId} does not exist.");
            }
            else if (book.IsDeleted)
            {
                throw new Exception($"Book with id {bookId} is not available. Please contact to admin.");
            }

            var student = university.Students.FirstOrDefault(s => s.StudentId == studentId);

            if (student == null)
            {
                throw new ArgumentException($"Student with id {studentId} does not exist in university {university.Name}.");
            }

            var allocation = await _dbContext.BookAllocations
                .FirstOrDefaultAsync(a => a.BookId == bookId);

            if (allocation == null)
            {
                allocation = new BookAllocation
                {
                    BookId = bookId,
                    StudentId = studentId,
                    UniversityId = university.UniversityId
                };

                _dbContext.BookAllocations.Add(allocation);
                await _dbContext.SaveChangesAsync();
            }
            else if (allocation.StudentId == studentId)
            {
                throw new Exception($"This book is already allocated to student {studentId}.");
            }
            else if (allocation.UniversityId == university.UniversityId)
            {
                throw new Exception($"This book is already allocated to a student in university {university.Name}.");
            }
            else
            {
                allocation.StudentId = studentId;
                allocation.UniversityId = university.UniversityId;
                await _dbContext.SaveChangesAsync();
            }

            return allocation;
        }



        public async Task<BookAllocation> UpdateAllocatedBookToStudent(BookAllocation allocatedBook)
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


        public async Task<BookAllocation> DeleteAllocatedBook(int studentId, int bookId)
        {
            try
            {
                var studentBook = await _dbContext.BookAllocations
                    .FirstOrDefaultAsync(ba => ba.StudentId == studentId && ba.BookId == bookId);

                if (studentBook != null)
                {
                    _dbContext.BookAllocations.Remove(studentBook);
                    await _dbContext.SaveChangesAsync();
                    return studentBook;
                }
                else
                {
                    throw new Exception("BookAllocation not found for the specified student and book");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}

