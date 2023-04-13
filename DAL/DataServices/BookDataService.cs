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
        //Only show not deleted book
        //public async Task<List<Book>> ShowAllBooks()
        //{
        //    List<Book> bookList;
        //    try
        //    {
        //        bookList = await _dbContext.Books.Where(b => !b.IsDeleted).ToListAsync();
        //        return bookList;
        //    }
        //    catch (Exception e)
        //    {
        //        throw new Exception(e.Message);
        //    }
        //}

        public async Task<Book> GetBookById(int bookId)
        {
            try
            {
                var book = await _dbContext.Books.FindAsync(bookId);
                if (book != null && book.IsDeleted == false)
                {
                    return book;
                }
                else
                {
                    throw new Exception("Book not found");
                }
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
                var existingBook = await _dbContext.Books.FirstOrDefaultAsync(b => b.BookName == book.BookName && b.BookAuthor == book.BookAuthor);

                if (existingBook != null)
                {
                    throw new Exception("Book already exists");
                }

                await _dbContext.Books.AddAsync(book);
                _dbContext.SaveChanges();
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
                throw new Exception("Error...", ex);
            }
        }



        public async Task<Book> DeleteBook(int bookId)
        {
            try
            {
                var book = await _dbContext.Books.FindAsync(bookId);
                if (book != null && book.IsDeleted == false)
                {
                    book.IsDeleted = true;
                    _dbContext.SaveChanges();
                    return book;
                }
                else if (book != null && book.IsDeleted == true)
                {
                    throw new Exception("Book has already been deleted");
                }
                else
                {
                    throw new Exception("Record not Found");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

    }
}
