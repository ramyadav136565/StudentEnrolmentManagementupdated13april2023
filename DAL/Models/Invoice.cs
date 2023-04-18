using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

#nullable disable

namespace DAL.Models
{
    public partial class Invoice
    {
        public int InvoiceId { get; set; }
        public int UniversityId { get; set; }
        public int Term { get; set; }
        public int BookQuantity { get; set; }
        public decimal Tax { get; set; }
        public decimal TotalAmount { get; set; }

        [JsonIgnore]
        public virtual University University { get; set; }
    }
}
