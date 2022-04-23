using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;

namespace Licenta.Models
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        
        [RegularExpression(@"^(\+4|)?(07[0-8]{1}[0-9]{1}|02[0-9]{2}|03[0-9]{2}){1}?(\s|\.|\-)?([0-9]{3}(\s|\.|\-|)){2}$", ErrorMessage ="Formatul numarului de telefon este invalid")]
        public string PhoneNumber { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public string City { get; set; }

        public float Total { get; set; }

        public DateTime OrderDate { get; set; }

        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}