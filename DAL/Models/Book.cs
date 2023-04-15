using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

#nullable disable

namespace DAL.Models
{
    public partial class Book
    {
        public Book()
        {
            BookAllocations = new HashSet<BookAllocation>();
        }

        public int BookId { get; set; }
        public string BookName { get; set; }
        public string BookAuthor { get; set; }
        public int BookPrice { get; set; }
        public string Course { get; set; }
        public bool IsDeleted { get; set; }

        [JsonIgnore]
        public virtual ICollection<BookAllocation> BookAllocations { get; set; }
    }
}
