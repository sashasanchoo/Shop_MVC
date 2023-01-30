using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASP.NET_Blog_MVC_Identity.Models
{
    public class Order
    {
        public int Id { get; set; }
        [NotMapped]
        public int UserId { get; set; }
        [ValidateNever]
        public User User { get; set; }

        public int PostId { get; set; }
        [ValidateNever]
        public Post Post { get; set; }

        [Required]
        [StringLength(20, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
        [RegularExpression(@"[a-zA-Z]+")]
        public string FirstName { get; set; }
        [Required]
        [StringLength(20, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 4)]
        [RegularExpression(@"[a-zA-Z]+")]
        public string LastName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [StringLength(20, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 4)]
        [RegularExpression(@"[a-zA-Z]+")]
        public string Address { get; set; }
        [Phone]
        public string ContactPhone { get; set; }
    }
}
