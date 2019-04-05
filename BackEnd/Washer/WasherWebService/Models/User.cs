using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WasherWebService.Models
{
    public class User
    {
        
        public string Userid { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public string Useremail { get; set; }
        [Required]
        public string Usermobile { get; set; }
        [Required]
        public string Latitude { get; set; }
        [Required]
        public string Longitude { get; set; }
        [Required]
        public string Userpassword { get; set; }
        public bool Washing { get; set; }
    }
}
