using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Licenta.Models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Numele este obligatoriu")]
        [StringLength(60, ErrorMessage = "Numele nu poate contine mai mult de 60 caractere")]
        public string ProductName { get; set; }

        
        [DataType(DataType.MultilineText)]
        public string Ingredients { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Pretul nu poate fi negativ")]
        public float Price { get; set; }

        [Required(ErrorMessage = "Poza e obligatorie")]
        public string Photo { get; set; }

        [Required(ErrorMessage = "Categoria este obligatorie")]
        public int CategoryId { get; set; }

        public IEnumerable<SelectListItem> Categ { get; set; }

        public virtual Category Category { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }


    }
}