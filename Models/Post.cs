using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASP.NET_Blog_MVC_Identity.Models
{
    public class Post
    {
        public int? Id { get; set; }
        public string Title { get; set; }
        [DataType(DataType.Date)]
        public DateTime Published { get; set; }
        [ValidateNever]
        public string ImagePath { get; set; }
        public string Content { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public List<Comment> Comments { get; set; } = new List<Comment>();
        public List<Order> Orders { get; set; } = new List<Order>();
        public decimal Price { get; set; }
    }
}
