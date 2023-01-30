using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ASP.NET_Blog_MVC_Identity.Data;
using ASP.NET_Blog_MVC_Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace ASP.NET_Blog_MVC_Identity.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class InputRolesController : Controller
    {
        private readonly ApplicationDbContext _context;
        [BindProperty]
        public IEnumerable<IdentityRole>? AllRoles { get; set; }
        private readonly RoleManager<IdentityRole> _roleManager;
        public InputRolesController(ApplicationDbContext context, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _roleManager = roleManager;
        }

        // GET: Admin/InputRoles
        public IActionResult Index()
        {
            //AllRoles = _roleManager.Roles.ToList();
            return View(_roleManager.Roles.ToList());
        }

        // GET: Admin/InputRoles/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _roleManager.Roles == null)
            {
                return NotFound();
            }
            var role = await _roleManager.Roles.FirstOrDefaultAsync(r => r.Id == id);
            if (role == null)
            {
                return NotFound();
            }
            return View(role);
        }

        // GET: Admin/InputRoles/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/InputRoles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,RoleName")] InputRole inputRole)
        {
            if (ModelState.IsValid)
            {
                await _roleManager.CreateAsync(new IdentityRole(inputRole.RoleName));
                return RedirectToAction(nameof(Index));
            }
            return View(inputRole);
        }

        // GET: Admin/InputRoles/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _roleManager.Roles == null)
            {
                return NotFound();
            }
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                return NotFound();
            }
            return View(role);
        }

        // POST: Admin/InputRoles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Name")] IdentityRole identityRole)
        {
            if (id != identityRole.Id.ToString())
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    IdentityRole? role = await _roleManager.FindByIdAsync(identityRole.Id);
                    role.Name = identityRole.Name;
                    await _roleManager.UpdateAsync(role);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InputRoleExists(identityRole.Id))
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
            return View(await _roleManager.FindByIdAsync(identityRole.Id));
        }

        // GET: Admin/InputRoles/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _roleManager.Roles == null)
            {
                return NotFound();
            }

            var identityRole = await _roleManager.FindByIdAsync(id);
            if (identityRole == null)
            {
                return NotFound();
            }

            return View(identityRole);
        }

        // POST: Admin/InputRoles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_roleManager.Roles == null)
            {
                return Problem("Entity set '_roleManager.Roles'  is null.");
            }
            var identityRole = await _roleManager.FindByIdAsync(id);
            if (identityRole != null)
            {
                await _roleManager.DeleteAsync(identityRole);
            }
            return RedirectToAction(nameof(Index));
        }

        private bool InputRoleExists(string id)
        {
            return _roleManager.Roles.Any(e => e.Id == id);
        }
    }
}
