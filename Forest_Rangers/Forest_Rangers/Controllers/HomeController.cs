using Forest_Rangers.Areas.Identity.Data;
using Forest_Rangers.Data;
using Forest_Rangers.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Forest_Rangers.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly UserManager<Forest_RangersUser> _userManager;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, UserManager<Forest_RangersUser> userManager)
        {
            _context = context;

            _userManager = userManager;
        }

        public IActionResult Forbidden()
        {
            return View();
        }

        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Post.Include(p => p.Forest_RangersUser);

            ViewData["userId"] = User.FindFirstValue(ClaimTypes.NameIdentifier);

            return View(await appDbContext.ToListAsync());
        }

        public IActionResult Posts()
        {
            return Redirect("/Posts");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        // GET: Posts/Details/5
        public async Task<IActionResult> Details(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Post
                .Include(p => p.Forest_RangersUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (post == null)
            {
                return NotFound();
            }

            return Redirect("/Posts/Details/" + id);
        }

        // GET: Posts/Edit/5
        public async Task<IActionResult> Edit(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Post.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            if (post.Forest_RangersUserId != User.FindFirstValue(ClaimTypes.NameIdentifier))
            {
                return RedirectToAction("Forbidden", "Home");
            }

            ViewData["Forest_RangersUserId"] = new SelectList(_context.Set<Forest_RangersUser>(), "Id", "Id", post.Forest_RangersUserId);
            return Redirect("/Posts/Edit/" + id);
        }

        // GET: Posts/Delete/5
        public async Task<IActionResult> Delete(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Post
                .Include(p => p.Forest_RangersUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (post == null)
            {
                return NotFound();
            }

            if (post.Forest_RangersUserId != User.FindFirstValue(ClaimTypes.NameIdentifier))
            {
                return RedirectToAction("Forbidden", "Home");
            }

            return Redirect("/Posts/Delete/" + id);
        }
    }
}
