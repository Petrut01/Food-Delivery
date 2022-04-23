using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;

namespace Licenta.Models
{
    public class Delivery
    {
        [Key]
        public int DeliveryId { get; set; }

        public int OrderId { get; set; }

        public string DriverId { get; set; }

        public bool IsFinished { get; set; }

        public bool IsTakenByDriver { get; set; }

        public virtual Order Order { get; set; }
        public virtual ApplicationUser User { get; set; }


    }
}