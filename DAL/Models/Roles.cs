using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

#nullable disable

namespace DAL.Models
{
    
    public partial class Roles
    {
        public Roles()
        {
            UserRoles = new HashSet<UserRole>();
            Users = new HashSet<User>();
        }

        public int RoleId { get; set; }
        public string UserRole { get; set; }

        public virtual ICollection<UserRole> UserRoles { get; set; }
        public virtual ICollection<User> Users { get; set; }
    }
}
