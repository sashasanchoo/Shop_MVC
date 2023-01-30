using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASP.NET_Blog_MVC_Identity.Models
{
    public class Comment
    {
        //[BindNever]
        public int Id { get; set; }
        [Required(ErrorMessage = "Commentary can not be empty")]
        [MinLength(3, ErrorMessage = "Commentary must be from 3 to 20 symbols")]
        [MaxLength(20)]
        public string Content { get; set; }
        [DataType(DataType.Date)]
        public DateTime Published { get; set; }
        public int PostId { get; set; }
        [BindNever]
        public Post Post { get; set; }
        [NotMapped]
        public int UserId { get; set; }
        [BindNever]
        public User User { get; set; }
    }
}
