using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ASP.NET_Blog_MVC_Identity.Data;
using ASP.NET_Blog_MVC_Identity.Models;
using Microsoft.AspNetCore.Authorization;

namespace ASP.NET_Blog_MVC_Identity.Areas.Admin
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class PostsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private const string _imageDir = "images";
        private readonly string _imagesPath;
        public PostsController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
            _imagesPath = Path.Combine(_environment.WebRootPath, _imageDir);
        }

        // GET: Admin/Posts
        public async Task<IActionResult> Index()
        {
              return View(await _context.Posts.Include("Category").ToListAsync());
        }

        // GET: Admin/Posts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Posts == null)
            {
                return NotFound();
            }

            var post = await _context.Posts.Include("Category").Include("Comments")
                .FirstOrDefaultAsync(m => m.Id == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // GET: Admin/Posts/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Posts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title, Price, Published, ImagePath, Content, Category")] Post post, IFormFile image)
        {
            if(!string.IsNullOrEmpty(image.FileName) && ModelState.IsValid)
            {
                var category = await _context.Categories.Include("Posts").Where(cat => cat.Name == post.Category.Name).FirstOrDefaultAsync();
                if(category == null)
                {
                    return BadRequest($"Category \"{post.Category.Name}\" does not exist");
                }
                else
                {
                    post.Category = category;
                    post.ImagePath = image.FileName;
                    if (!System.IO.File.Exists(image.FileName))
                    {
                        await using (var file = new FileStream(Path.Join(_imagesPath, image.FileName), FileMode.Create, FileAccess.Write))
                        {
                            await image.CopyToAsync(file);
                        }
                    }
                    _context.Posts.Add(post);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }        
            return View(post);
        }

        // GET: Admin/Posts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Posts == null)
            {
                return NotFound();
            }

            var post = await _context.Posts.Include("Category").FirstOrDefaultAsync(p => p.Id == id);
            if (post == null)
            {
                return NotFound();
            }
            return View(post);
        }

        // POST: Admin/Posts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, [Bind("Id,Title,Price,Published,ImagePath,Content,Category")] Post post, IFormFile image)
        {
            if (id != post.Id)
            {
                return NotFound();
            }
            if (!string.IsNullOrEmpty(image.FileName) && ModelState.IsValid)
            {
                try
                {
                    var category = await _context.Categories.Include("Posts").Where(cat => cat.Name == post.Category.Name).FirstOrDefaultAsync();
                    if (category == null)
                    {
                        return BadRequest($"Category \"{post.Category.Name}\" does not exist");
                    }
                    else
                    {
                        var postToUpdate = await _context.Posts.FindAsync(post.Id);
                        postToUpdate.Category = category;
                        postToUpdate.Title = post.Title;
                        postToUpdate.Price = post.Price;
                        postToUpdate.Published = post.Published;
                        postToUpdate.Content = post.Content;
                        postToUpdate.ImagePath = image.FileName;
                        CopyImage(image);
                        _context.Update(postToUpdate);
                        await _context.SaveChangesAsync();
                    }

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(post.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(post);
        }

        // GET: Admin/Posts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Posts == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // POST: Admin/Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            if (_context.Posts == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Post'  is null.");
            }
            var post = await _context.Posts.FindAsync(id);
            if (post != null)
            {
                _context.Posts.Remove(post);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PostExists(int? id)
        {
          return _context.Posts.Any(e => e.Id == id);
        }
        private async void CopyImage(IFormFile image)
        {
            if (!System.IO.File.Exists(image.FileName))
            {
                await using (var file = new FileStream(Path.Join(_imagesPath, image.FileName), FileMode.Create, FileAccess.Write))
                {
                    await image.CopyToAsync(file);
                }
            }
        }
    }
}
