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

       
        public async Task<BookAllocation> BookAllocateToStudent(int studentId, int bookId)
        {
            var book = await _dbContext.Books.SingleOrDefaultAsync(b => b.BookId == bookId && !b.IsDeleted);
            if (book == null)
            {
                throw new Exception("Book not found or deleted");
            }

            var student = await _dbContext.Students.SingleOrDefaultAsync(s => s.StudentId == studentId && !s.IsDeleted);
            if (student == null)
            {
                throw new Exception("Student not found or deleted");
            }

            var existingAllocation = await _dbContext.BookAllocations
                .SingleOrDefaultAsync(a => a.StudentId == studentId && a.BookId == bookId);
            if (existingAllocation != null)
            {
                throw new Exception("Book already allocated to the student");
            }

            var bookAllocation = new BookAllocation
            {
                StudentId = studentId,
                BookId = bookId,
            };

            await _dbContext.BookAllocations.AddAsync(bookAllocation);
            await _dbContext.SaveChangesAsync();
            return bookAllocation;
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
                    throw new Exception("Book is marked as deleted.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Same book can not be allocated to same student", ex);
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


/*
 
public async Task<BookAllocation> AllocateBookToStudent(int studentId, int bookId)
{
    try
    {
        // Check if the book exists and is available for allocation
        var book = await _dbContext.Books.FirstOrDefaultAsync(b => b.BookId == bookId && !b.IsDeleted);
        if (book == null)
        {
            throw new Exception("Book not found or has been deleted");
        }

        // Check if the student exists
        var student = await _dbContext.Students.FirstOrDefaultAsync(s => s.StudentId == studentId && !s.IsDeleted);
        if (student == null)
        {
            throw new Exception("Student not found or has been deleted");
        }

        // Check if the student already has the book allocated
        var existingAllocation = await _dbContext.BookAllocations
            .FirstOrDefaultAsync(a => a.StudentId == studentId && a.BookId == bookId && !a.IsReturned);
        if (existingAllocation != null)
        {
            throw new Exception("Book already allocated to the student");
        }

        // Allocate the book to the student
        var allocation = new BookAllocation
        {
            StudentId = studentId,
            BookId = bookId,
            AllocationDate = DateTime.UtcNow,
        };
        await _dbContext.BookAllocations.AddAsync(allocation);
        await _dbContext.SaveChangesAsync();
        return allocation;
    }
    catch (Exception ex)
    {
        throw new Exception(ex.Message);
    }
}

 
 */