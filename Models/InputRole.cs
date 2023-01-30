using System.ComponentModel.DataAnnotations;

namespace ASP.NET_Blog_MVC_Identity.Models
{
    public class InputRole
    {

        public int Id { get; set; }
        [Required]
        [MinLength(3)]
        [MaxLength(20)]
        public string RoleName { get; set; }
    }
}
