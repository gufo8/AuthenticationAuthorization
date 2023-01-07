﻿using System.Collections.Generic;

namespace TaskAuthenticationAuthorization.Models
{
    public class Role
    {
        public int Id { get; set; }
        public RoleNames Name { get; set; }
        public List<User> Users { get; set; }
        public Role()
        {
            Users = new List<User>();
        }

    }

    public enum RoleNames { Admin, User, Buyer }
}
