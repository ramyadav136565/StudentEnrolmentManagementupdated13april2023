using System;
using System.Collections.Generic;

#nullable disable
using System.Text.Json.Serialization;

namespace DAL.Models
{
    public partial class Student
    {
        public Student()
        {
            BookAllocations = new HashSet<BookAllocation>();
        }

        public int StudentId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public int UniversityId { get; set; }
        public int Term { get; set; }
        public string Course { get; set; }
        public bool IsDeleted { get; set; }

        [JsonIgnore]
        public virtual University University { get; set; }
        [JsonIgnore]
        public virtual ICollection<BookAllocation> BookAllocations { get; set; }
    }
}
