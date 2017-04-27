using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Northwind.Models
{
    public class CustomerSignIn
    {
        public int CustomerId { get; set; }
        [Required(ErrorMessage = "Have you lost your mind? Put in the password!")]
        public string Password { get; set; }
    }
}