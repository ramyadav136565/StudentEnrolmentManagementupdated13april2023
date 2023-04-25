namespace StudentEnrolmentManagement.Controllers
{
    using DAL;
    using DAL.Models;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;
    [Authorize(Roles = "Admin")]
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
        [Route("GetInvoiceDetailsById/{InvoiceId}")]
        public async Task<IActionResult> GetInvoiceDetailsById(int invoiceId)
        {
            try
            {
                var invoice = await _invoiceService.GetInvoiceDetailsById(invoiceId);
                return Ok(invoice);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        [Route("CreateInvoice/{universityId}/{semester}")]
        public async Task<IActionResult> CreateInvoice(int universityId, int semester)
        {
            try
            {
                var invoices = await _invoiceService.CreateInvoice(universityId, semester); //change local variable name
                return Ok(invoices);

            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("GenerateInvoice/{universityId}/{term}")]
     
        public async Task<ActionResult<Invoice>> GenerateInvoice(int universityId, int term)
        {
            try
            {
                var invoice = await _invoiceService.GenerateInvoice(universityId, term);
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
        [HttpGet]
        [Route("invoices/{invoiceId}")]
        public async Task<IActionResult> DownloadCSV(int invoiceId)
        {
            try
            {
                var invoiceDetails = await _invoiceService.GetInvoiceDetails(invoiceId);
                var csvString = await _invoiceService.ConvertInvoiceDetailsToCSV(invoiceDetails);
                var bytes = Encoding.UTF8.GetBytes(csvString);
                var stream = new MemoryStream(bytes);

                return File(stream, "text/csv", "InvoiceDetails.csv");
            }
            catch (Exception ex)
            {
                // Handle the exception here, for example log it or return an error message
                Console.WriteLine($"An error occurred: {ex.Message}");
                return BadRequest("An error occurred while generating the CSV file.");
            }
        }
    }
}
