using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
#nullable disable

namespace DAL.Models
{
    public partial class BookAllocation
    {
        public int SerialNo { get; set; }
        public int StudentId { get; set; }
        public int BookId { get; set; }
        [JsonIgnore]
        public virtual Book Book { get; set; }
        [JsonIgnore]
        public virtual Student Student { get; set; }
    }
}
