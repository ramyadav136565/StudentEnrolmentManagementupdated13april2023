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
        public async Task<IActionResult> CreateInvoice(int universityId ,int semester)
        {
            try
            {
                var invoices = await _invoiceService.CreateInvoice(universityId,  semester ); //change local variable name
                return Ok(invoices);

            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("GenerateInvoice/{universityId}/{semester}")]
        public async Task<ActionResult<Invoice>> GenerateInvoice(int universityId, int semester)
        {
            try
            {
                var invoice = await _invoiceService.GenerateInvoice(universityId, semester);
                return Ok(invoice);
            }
            
            catch (Exception ex)
            {
                // Log the exception here
                return BadRequest(ex.Message); ;
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
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        //[HttpGet("totalamount")]
        //public async Task<ActionResult<decimal>> GetTotalAmount(string universityIdOrUniversityName, int semester, decimal taxPercentage)
        //{
        //    var totalAmount = await _invoiceService.CalculateTotalAmountForUniversity(universityIdOrUniversityName, semester, taxPercentage);
        //    return totalAmount;
        //}
    }
}
