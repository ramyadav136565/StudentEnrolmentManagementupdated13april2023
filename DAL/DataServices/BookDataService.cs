namespace DAL
{
    using DAL.Models;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    public class BookDataService
    {
        private BookStoreContext _dbContext;
        public BookDataService(BookStoreContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<List<Book>> ShowAllBooks()
        {
            List<Book> books;
            try
            {
                books = await _dbContext.Books.ToListAsync();
                return books;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    
        public async Task<Book> GetBookById(int bookId)
        {
            try
            {
                var book = await _dbContext.Books.FindAsync(bookId);
                if (book == null)
                {
                    throw new Exception($"Book with ID {bookId} not found.");
                }
                return book;
               
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Book> AddBook(Book book)
        {
            try
            {

                if (await _dbContext.Books.AnyAsync(b => b.BookName == book.BookName && b.BookAuthor == book.BookAuthor))
                {
                    throw new Exception("A book with the same name and author already exists.");
                }


                await _dbContext.Books.AddAsync(book);
                await _dbContext.SaveChangesAsync();
                return book;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }



        public async Task<Book> UpdateBook(Book book)
        {
            try
            {
                var existingBook = await _dbContext.Books.FindAsync(book.BookId);
                if (existingBook != null)
                {
                    _dbContext.Entry(existingBook).CurrentValues.SetValues(book);
                    await _dbContext.SaveChangesAsync();
                    return existingBook;
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





        
        public async Task<string> DeleteBook(int bookId)
        {
            try
            {
                var book = await _dbContext.Books.FindAsync(bookId);

                if (book != null)
                {
                    if (!book.IsDeleted)
                    {
                        book.IsDeleted = true;
                        await _dbContext.SaveChangesAsync();
                        return "The book is deleted successfully.";
                    }
                    else
                    {
                        return "The book is deleted already.";
                    }
                }
                else
                {
                    return "The book is not present.";
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating the status of book.", ex);
            }
        }

    }
}
