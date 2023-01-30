using Microsoft.AspNetCore.Identity;

namespace ASP.NET_Blog_MVC_Identity.Models
{
    public class User:IdentityUser
    {
        public List<Comment> Comments { get; set; } = new List<Comment>();
        public List<Order> Orders { get; set; } = new List<Order>();
    }
}
