using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
#nullable disable

namespace DAL.Models
{
    public partial class BookAllocation
    {
        [Key]
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int BookId { get; set; }
        public int UniversityId { get; set; }

        [JsonIgnore]
        public virtual Book Book { get; set; }
        [JsonIgnore]
        public virtual Student Student { get; set; }
        [JsonIgnore]
        public virtual University University { get; set; }
    }
}
