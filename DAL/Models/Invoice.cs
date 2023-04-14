using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
#nullable disable

namespace DAL.Models
{
    public partial class Invoice
    {
        public int InvoiceId { get; set; }
        public int StudentId { get; set; }
        public int UniversityId { get; set; }
        public int Semester { get; set; }
        public int BookId { get; set; }
        public int Quantity { get; set; }
        public decimal Tax { get; set; }
        public decimal TotalAmount { get; set; }

        [JsonIgnore]
        public virtual Book Book { get; set; }
        [JsonIgnore]
        public virtual Student Student { get; set; }
        [JsonIgnore]
        public virtual University University { get; set; }
    }
}
