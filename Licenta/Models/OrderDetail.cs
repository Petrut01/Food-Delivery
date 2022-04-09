using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;

namespace Licenta.Models
{
    public class OrderDetail
    {
        [Key]
        public int OrderDetailId { get; set; }

        public int ProductId { get; set; }

        public int ShoppingCartId { get; set; }

        public int Quantity { get; set; }

        public float UnitPrice { get; set; }

        public bool IsInCurrentCart { get; set; }

        public virtual Product Product { get; set; }
        public virtual ShoppingCart ShoppingCart { get; set; }
    }
}