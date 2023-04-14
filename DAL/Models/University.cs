using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

#nullable disable

namespace DAL.Models
{
    public partial class University
    {
        public University()
        {
            BookAllocations = new HashSet<BookAllocation>();
            Invoices = new HashSet<Invoice>();
            Students = new HashSet<Student>();
        }

        public int UniversityId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public bool IsDeleted { get; set; }
        [JsonIgnore]
        public virtual ICollection<BookAllocation> BookAllocations { get; set; }
        [JsonIgnore]
        public virtual ICollection<Invoice> Invoices { get; set; }
        [JsonIgnore]
        public virtual ICollection<Student> Students { get; set; }
    }
}
