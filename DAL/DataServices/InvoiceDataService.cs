namespace DAL
{
    using DAL.Models;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
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
    }
}
