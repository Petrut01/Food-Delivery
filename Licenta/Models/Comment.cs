using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;

namespace Licenta.Models
{
    public class Comment
    {
        [Key]
        public int CommentId { get; set; }

        [Required(ErrorMessage = "Campul nu poate fi necompletat")]
        public string Content { get; set; }
        public DateTime Date { get; set; }
        public int ProductId { get; set; }

        public string UserId { get; set; }


        public virtual ApplicationUser User { get; set; }
        public virtual Product Product { get; set; }
    }
}