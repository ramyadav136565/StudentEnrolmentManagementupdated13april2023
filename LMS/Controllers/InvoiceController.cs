namespace StudentEnrolmentManagement.Controllers
{
    using DAL;
    using DAL.Models;
    using Microsoft.AspNetCore.Mvc;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    [ApiController]
    public class InvoiceController : ControllerBase
    {
        private readonly InvoiceDataService _invoiceService;
        public InvoiceController(InvoiceDataService invoiceService)
        {
            _invoiceService = invoiceService;
        }

        [HttpGet]
        [Route("ShowAllInvoices")]
        public async Task<IActionResult> ShowAllInvoices()
        {
            List<Invoice> invoices;
            try
            {
                invoices = await _invoiceService.ShowAllInvoices();
                return Ok(invoices);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet]
        [Route("GetInvoiceDetails/{InvoiceId}")]
        public async Task<IActionResult> GetInvoiceDetails(int invoiceId)
        {
            try
            {
                var invoice = await _invoiceService.GetInvoiceDetails(invoiceId);
                return Ok(invoice);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        [Route("CreateInvoice")]
        public async Task<IActionResult> CreateInvoice(Invoice invoice)
        {
            try
            {
                var invoices = await _invoiceService.CreateInvoice(invoice); //change local variable name
                return Ok(invoices);

            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
        [HttpPut("UpdateInvoice")]
        public async Task<IActionResult> UpdateInvoice([FromBody] Invoice invoice)
        {
            Invoice updatedInvoice;
            try
            {
                updatedInvoice = await _invoiceService.UpdateInvoice(invoice);
                return Ok(updatedInvoice);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("totalamount")]
        public async Task<ActionResult<decimal>> GetTotalAmount(string universityIdOrUniversityName, int semester, decimal taxPercentage)
        {
            var totalAmount = await _invoiceService.CalculateTotalAmountForUniversity(universityIdOrUniversityName, semester, taxPercentage);
            return totalAmount;
        }
    }
}
