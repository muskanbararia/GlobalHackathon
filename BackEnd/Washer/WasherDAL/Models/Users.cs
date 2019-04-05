using System;
using System.Collections.Generic;

namespace WasherDAL.Models
{
    public partial class Users
    {
        public string Userid { get; set; }
        public string Username { get; set; }
        public string Useremail { get; set; }
        public string Usermobile { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public byte[] Userpassword { get; set; }
        public bool Washing { get; set; }
    }
}
