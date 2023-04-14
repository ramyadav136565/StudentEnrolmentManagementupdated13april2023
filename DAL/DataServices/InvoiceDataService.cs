namespace DAL
{
    using DAL.Models;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    public class InvoiceDataService
    {
        private BookStoreContext _dbContext;
        public InvoiceDataService(BookStoreContext dbContext)
        {
            _dbContext = dbContext;
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
        public async Task<Invoice> GetInvoiceDetails(int invoiceId)
        {
            Invoice invoice;
            try
            {
                invoice = await _dbContext.Invoices.FindAsync(invoiceId);
                if (invoice != null)
                {
                    return invoice;
                }
                else
                {
                    throw new Exception("Record not found");
                }
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }
        public async Task<Invoice> CreateInvoice(Invoice invoice)
        {
            try
            {
                _dbContext.Invoices.Add(invoice);
               await _dbContext.SaveChangesAsync();
                return invoice;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
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
        public async Task<decimal> CalculateTotalAmountForUniversity(string universityIdOrUniversityName, int semester, decimal taxPercentage)
        {
            try
            {
                var university = await _dbContext.Universities
                    .FirstOrDefaultAsync(u => u.UniversityId.ToString() == universityIdOrUniversityName || u.Name == universityIdOrUniversityName);

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

                var totalAmount = bookAllocations.Sum(ba_s => ba_s.BookAllocation.Book.BookPrice);
                var taxAmount = totalAmount * taxPercentage / 100;
                var totalAmountWithTax = totalAmount + taxAmount;

                return totalAmountWithTax;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
               
            }
        }





    }
}
