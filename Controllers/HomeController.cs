using ASP.NET_Blog_MVC_Identity.Data;
using ASP.NET_Blog_MVC_Identity.Models;
using ASP.NET_Blog_MVC_Identity.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;
using X.PagedList;

namespace ASP.NET_Blog_MVC_Identity.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly BotSender _bot;
        private readonly IEmailSender _emailSender;
        public PostCommentViewModel PostCommentViewModel { get; set; }

        public IEnumerable<Post> Posts { get; set; }

        public const int pageSize = 2;
        public HomeController(
            ILogger<HomeController> logger, 
            ApplicationDbContext context, 
            UserManager<User> userManager, 
            BotSender bot,
            IEmailSender emailSender)
        {
            _logger = logger;
            _context = context;
            Posts = new List<Post>();
            PostCommentViewModel = new PostCommentViewModel();
            _userManager = userManager;
            _bot = bot;
            _emailSender = emailSender;
        }
        public IActionResult Index(string categoryName, int? page)
        {
            Posts = string.IsNullOrEmpty(categoryName) ? _context.Posts.Include("Category").OrderByDescending(p => p.Published).ToList(): _context.Posts.Include("Category").Where(p => p.Category.Name.Equals(categoryName)).OrderByDescending(p => p.Published).ToList();
            int pageNumber = page ?? 1;
            ViewBag.Categories = _context.Categories.Include("Posts");
            ViewBag.CategoryName = categoryName ?? null;
            return View(Posts.ToPagedList(pageNumber, pageSize));
        }
        public async Task<IActionResult> Post(int? postId)
        {
            if(postId == null)
            {
                return NotFound();
            }
            PostCommentViewModel.Post = await _context.Posts.Where(p => p.Id == postId).Include("Category").Include("Comments").FirstOrDefaultAsync();
            if (PostCommentViewModel.Post == null)
            {
                return NotFound();
            }
            PostCommentViewModel.Post.Comments = await _context.Comments.Where(c => c.PostId == PostCommentViewModel.Post.Id).Include("User").ToListAsync();
            return View(PostCommentViewModel);
        }
        public async Task<IActionResult> Search(string postTitle)
        {
            var postId = (await _context.Posts.Where(p => p.Title == postTitle).FirstOrDefaultAsync())?.Id;
            if(postId == null)
            {
                return NotFound();
            }
            return RedirectToAction($"Post", new { postId });
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> PublishComment([Bind("Comment", "Post")]PostCommentViewModel viewModel)
        {
            if (!PostExists(viewModel.Post.Id))
            {
                return NotFound();
            }
            if (!ModelState.Values.FirstOrDefault(x => x.AttemptedValue == viewModel.Comment.Content).Errors.Any())
            {
                Post post = await _context.Posts.Where(p => p.Id == viewModel.Post.Id).FirstOrDefaultAsync();
                ClaimsPrincipal currentUser = User;
                _context.Comments.Add(new Comment { 
                    Content = viewModel.Comment.Content, 
                    Post = post, User = _userManager.Users.Where(u => u.UserName == currentUser.Identity.Name).FirstOrDefault(),
                    Published = DateTime.Now
                });
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Post", new { postId = viewModel.Post.Id });
        }
        [HttpGet]
        public async Task<IActionResult> Buy(int? id)
        {
            if(id == null || _context.Posts == null)
            {
                return NotFound();
            }
            var post = await _context.Posts.FindAsync(id);
            if(post == null)
            {
                return NotFound();
            }
            return View(new Order { PostId = (int)id });
        }
        [HttpPost]
        public async Task<IActionResult> Buy([Bind("PostId, FirstName, LastName, Address, ContactPhone, Email")]Order order)
        {
            if (ModelState.IsValid)
            {
                ClaimsPrincipal currentUser = User;
                if (currentUser.Identity.IsAuthenticated)
                {
                    _context.Orders.Add(new Order
                    {
                        FirstName = order.FirstName,
                        LastName = order.LastName,
                        Email = order.Email,
                        Address = order.Address,
                        ContactPhone = order.ContactPhone,
                        PostId = order.PostId,
                        User = await _userManager.Users.Where(u => u.UserName == currentUser.Identity.Name).FirstOrDefaultAsync()
                    });
                    await _context.SaveChangesAsync();
                }
                var post = await _context.Posts.Where(p => p.Id == order.PostId).FirstOrDefaultAsync();
                _bot.SendMessage($"{nameof(post.Title)}: {post?.Title}\n" +
                    $"{nameof(order.FirstName)}: {order.FirstName}\n" +
                    $"{nameof(order.LastName)}: {order.LastName}\n" +
                    $"{nameof(order.Email)}: {order.Email}\n" +
                    $"{nameof(order.Address)}: {order.Address}\n" +
                    $"{nameof(order.ContactPhone)}: {order.ContactPhone}\n");
                await _emailSender.SendEmailAsync(order.Email, "Order details",
                       $"<h2>Greetings {order.FirstName} {order.LastName}</h2>" +
                       $"<p>We have received your order for {post?.Title}</p>" +
                       $"<p>If you have any additional info, please contact us by email message on <a href='mailto:archivesexplorermail@gmail.com'>archivesexplorermail@gmail.com</a></p>");
            }
            return RedirectToAction("Index");
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        private bool PostExists(int? id)
        {
            return _context.Posts.Any(p => p.Id == id);
        }
    }
}
