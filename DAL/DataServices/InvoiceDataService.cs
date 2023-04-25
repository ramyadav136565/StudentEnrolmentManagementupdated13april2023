namespace DAL
{
    using DAL.Models;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using OfficeOpenXml;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
  
    public class InvoiceDataService
    {

        private IConfiguration _config;

        private BookStoreContext _dbContext;
        public InvoiceDataService(BookStoreContext dbContext, IConfiguration config)
        {
            _dbContext = dbContext;
            _config = config;
        }


        public async Task<List<Invoice>> ShowAllInvoices()
        {
            List<Invoice> invoices;
            try
            {
                invoices = await _dbContext.Invoices.ToListAsync();
                return invoices;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        public async Task<Invoice> GetInvoiceDetailsById(int invoiceId)
        {

            try
            {
                var invoice = await _dbContext.Invoices.FindAsync(invoiceId);
                if (invoice == null)
                {
                    throw new Exception("Record not found");
                }
                return invoice;
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }





        public async Task<List<object>> GetInvoiceDetails(int invoiceId)
        {
            try
            {
                var invoiceDetails = await (from inv in _dbContext.Invoices
                                            join un in _dbContext.Universities on inv.UniversityId equals un.UniversityId
                                            join ba in _dbContext.BookAllocations on un.UniversityId equals ba.UniversityId
                                            join s in _dbContext.Students on ba.StudentId equals s.StudentId
                                            join b in _dbContext.Books on ba.BookId equals b.BookId
                                            where inv.InvoiceId == invoiceId
                                            select new
                                            {
                                                inv.InvoiceId,
                                                UniversityName = un.Name,
                                                UniversityAddress = un.Address,
                                                inv.Term,
                                                inv.BookQuantity,
                                                inv.Tax,
                                                inv.TotalAmount,
                                                s.StudentId,
                                                s.FullName,
                                                s.Email,
                                                StudentAddress = s.Address,
                                                b.Course,
                                                b.BookName,
                                                b.BookAuthor,
                                                b.BookPrice
                                            }).ToListAsync<object>();

                return invoiceDetails.Cast<object>().ToList();
            }
            catch (Exception ex)
            {
                // Handle the exception here, for example log it or return a default value
                Console.WriteLine($"An error occurred: {ex.Message}");
                return new List<object>();
            }
        }

        public async Task<string> ConvertInvoiceDetailsToCSV(List<object> invoiceDetails)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Invoice Id, University Name, Term, Book Quantity, Tax, Total Amount");


                // Check if there are any invoiceDetails
                if (invoiceDetails.Count > 0)
                {
                    // Append the first row with invoice details
                    dynamic firstDetail = invoiceDetails[0];
                    sb.AppendLine($"{firstDetail.InvoiceId}, {firstDetail.UniversityName}, {firstDetail.Term}, {firstDetail.BookQuantity}, {firstDetail.Tax}, {firstDetail.TotalAmount}");
                }
                sb.AppendLine();

                sb.AppendLine("Student Name, Email, Book Name, Price, Course");

                // Append the rows with student details
                foreach (dynamic detail in invoiceDetails)
                {
                    sb.AppendLine($"{detail.FullName}, {detail.Email}, {detail.BookName}, {detail.BookPrice}, {detail.Course}");
                }

                return await Task.FromResult(sb.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in ConvertInvoiceDetailsToCSV: {ex.Message}");
                return string.Empty;
            }
        }








        public async Task<Invoice> CreateInvoice(int universityId, int semester)
            {
                try
                {
                    var university = await _dbContext.Universities
                        .FirstOrDefaultAsync(u => u.UniversityId == universityId);

                    if (university == null)
                    {
                        throw new ArgumentException("University not found");
                    }

                    var bookAllocations = await _dbContext.BookAllocations
                        .Include(ba => ba.Book)
                        .Join(_dbContext.Students, ba => ba.StudentId, s => s.StudentId, (ba, s) => new { BookAllocation = ba, Student = s })
                        .Where(ba_s => ba_s.Student.UniversityId == university.UniversityId && ba_s.BookAllocation.Student.Term == semester)
                        .ToListAsync();

                    if (bookAllocations.Count == 0)
                    {
                        throw new ArgumentException("No book allocations found for the specified university and semester combination");
                    }
                var numberOfBooks = bookAllocations.Count;
                    var Amount = bookAllocations.Sum(ba_s => ba_s.BookAllocation.Book.BookPrice);
                    var taxAmount = Amount * 5 / 100;
                    var totalAmount = Amount + taxAmount;

                var invoice = new Invoice
                {

                    UniversityId = university.UniversityId,
                    Term = semester,
                    BookQuantity = numberOfBooks,
                    Tax = totalAmount- Amount,
                    TotalAmount = totalAmount

                    };
                var existInvoice = await _dbContext.Invoices
     .FirstOrDefaultAsync(i => i.UniversityId == universityId && i.Term == semester && i.BookQuantity == numberOfBooks);

                if (existInvoice != null)
                {
                    throw new Exception("Invoice already generated for the specified university, semester, and book quantity");
                }

                // Save the invoice to the database
                _dbContext.Invoices.Add(invoice);
                    await _dbContext.SaveChangesAsync();

                    return invoice;
                }
                catch (Exception ex)
                {
                 throw new Exception(ex.Message);
                }
            }
        public async Task<Invoice> GenerateInvoice(int universityId, int term)
        {
            var tax = int.Parse(_config["TexSettings:Tax"]);
            var university = await _dbContext.Universities
                .FirstOrDefaultAsync(u => u.UniversityId == universityId);

            if (university == null)
            {
                throw new ArgumentException("University not found");
            }

            var bookAllocations = await _dbContext.BookAllocations
                .Include(ba => ba.Book)
                .Join(_dbContext.Students, ba => ba.StudentId, s => s.StudentId, (ba, s) => new { BookAllocation = ba, Student = s })
                .Where(ba_s => ba_s.Student.UniversityId == university.UniversityId && ba_s.BookAllocation.Student.Term == term)
                .ToListAsync();

            if (bookAllocations.Count == 0)
            {
                throw new ArgumentException("No book allocations found for the specified university and semester combination");
            }

            var numberOfBooksAllocated = bookAllocations.Count;
            var Amount = bookAllocations.Sum(ba_s => ba_s.BookAllocation.Book.BookPrice);
            var taxAmount = Amount * tax/ 100;
            var totalAmount = Amount + taxAmount;

            var invoice = new Invoice
            {
                UniversityId = university.UniversityId,
                Term = term,
                 BookQuantity = numberOfBooksAllocated,
                Tax = totalAmount- Amount,
                TotalAmount = totalAmount
            };

            return invoice;
        }





        public async Task<Invoice> UpdateInvoice(Invoice invoice)
        {
            try
            {
                if (invoice != null)
                {
                    _dbContext.Entry(invoice).State = EntityState.Modified;
                     await _dbContext.SaveChangesAsync();
                    return invoice;
                }
                else
                {
                    throw new Exception("Record Not Found");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
