using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
#nullable disable

namespace DAL.Models
{
    public partial class Role
    {
        public Role()
        {
            UserRoles = new HashSet<UserRole>();
            Users = new HashSet<User>();
        }

        public int RoleId { get; set; }
        public string Role1 { get; set; }
        [JsonIgnore]
        public virtual ICollection<UserRole> UserRoles { get; set; }
        [JsonIgnore]
        public virtual ICollection<User> Users { get; set; }
    }
}
