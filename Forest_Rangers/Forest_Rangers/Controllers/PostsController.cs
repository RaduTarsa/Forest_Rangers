using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Forest_Rangers.Data;
using Forest_Rangers.Models;
using Forest_Rangers.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using System.Security.Claims;
using System.IO;
using Microsoft.AspNetCore.Authorization;

namespace Forest_Rangers.Views
{
    [Authorize]
    public class PostsController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly UserManager<Forest_RangersUser> _userManager;

        private readonly IWebHostEnvironment _webHostEnvironment;

        public PostsController(ApplicationDbContext context, UserManager<Forest_RangersUser> userManager, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;

            _userManager = userManager;

            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Posts
        public async Task<IActionResult> Index()
        {
            var context = _context.Post.Include(p => p.Forest_RangersUser).Where(x => x.Forest_RangersUserId == User.FindFirstValue(ClaimTypes.NameIdentifier));
            return View(await context.ToListAsync());
        }

        // GET: Posts
        public async Task<IActionResult> Home()
        {
            return Redirect("/");
        }

        // GET: Posts/Details/5
        public async Task<IActionResult> Details(string id)
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

            ViewData["userId"] = User.FindFirstValue(ClaimTypes.NameIdentifier);

            ViewBag.comments = _context.Comment.Include(p => p.Post).Where(x => x.PostId == post.Id).OrderBy(n => n.Created_at);

            return View(post);
        }

        // GET: Posts/Create
        public IActionResult Create()
        {
            ViewData["Forest_RangersUserId"] = new SelectList(_context.Set<Forest_RangersUser>(), "Id", "Id");
            return View();
        }

        // POST: Posts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,ImagePath,CoordinatesLong,CoordinatesLat,Created_at,Updated_at,Forest_RangersUserId")] Post post)
        {
            if (ModelState.IsValid)
            {
                post.Id = Guid.NewGuid().ToString();
                post.Forest_RangersUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                post.Created_at = DateTime.Now;
                post.Updated_at = DateTime.Now;

                var files = HttpContext.Request.Form.Files;
                foreach (var customFile in files)
                {
                    if (customFile != null && customFile.Length > 0)
                    {
                        using (var fileStream = new FileStream(Path.Combine(Path.Combine(_webHostEnvironment.ContentRootPath, "wwwroot/PostImages/"), post.Id + ".jpg"), FileMode.Create))
                        {
                            await customFile.CopyToAsync(fileStream);
                            post.ImagePath = post.Id.ToString();
                        }
                    }
                }

                _context.Add(post);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            
            return View(post);
        }

        // GET: Posts/Edit/5
        public async Task<IActionResult> Edit(string id)
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
            return View(post);
        }

        // POST: Posts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Title,Description,CoordinatesLong,CoordinatesLat,Created_at,Updated_at,Forest_RangersUserId")] Post post)
        {
            if (post.Forest_RangersUserId != User.FindFirstValue(ClaimTypes.NameIdentifier).ToString())
            {
                return RedirectToAction("Forbidden", "Home");
            }

            post.Updated_at = DateTime.Now;
            post.Forest_RangersUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (id != post.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var files = HttpContext.Request.Form.Files;
                    foreach (var customFile in files)
                    {
                        if (customFile != null && customFile.Length > 0)
                        {
                            System.IO.File.Delete(Path.Combine(_webHostEnvironment.ContentRootPath, "wwwroot/PostImages/" + id + ".jpg"));

                            using (var fileStream = new FileStream(Path.Combine(Path.Combine(_webHostEnvironment.ContentRootPath, "wwwroot/PostImages/"), post.Id + ".jpg"), FileMode.Create))
                            {
                                await customFile.CopyToAsync(fileStream);
                                post.ImagePath = post.Id.ToString();
                            }
                        }
                    }

                    _context.Update(post);
                    await _context.SaveChangesAsync();
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
                return Redirect(string.Format("~/Posts/Details/{0}", post.Id));
            }
           
            return Redirect(string.Format("~/Posts/Details/{0}", post.Id));
        }

        // GET: Posts/Delete/5
        public async Task<IActionResult> Delete(string id)
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

            return View(post);
        }

        // POST: Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var post = await _context.Post.FindAsync(id);

            if (post.Forest_RangersUserId != User.FindFirstValue(ClaimTypes.NameIdentifier))
            {
                return RedirectToAction("Forbidden", "Home");
            }

            _context.Post.Remove(post);
            await _context.SaveChangesAsync();

            System.IO.File.Delete(Path.Combine(_webHostEnvironment.ContentRootPath, "wwwroot/PostImages/" + id + ".jpg"));

            return RedirectToAction(nameof(Index));
        }

        private bool PostExists(string id)
        {
            return _context.Post.Any(e => e.Id == id);
        }

        // POST: Comments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CommentsCreate([Bind("Id,Message,Created_at,Updated_at,Forest_RangersUserId,PostId")] Comment comment)
        {
            if (ModelState.IsValid)
            {
                var postId = comment.PostId;

                comment.Id = Guid.NewGuid().ToString();
                comment.Forest_RangersUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                comment.Created_at = DateTime.Now;
                comment.Updated_at = DateTime.Now;

                var files = HttpContext.Request.Form.Files;
                foreach (var customFile in files)
                {
                    if (customFile != null && customFile.Length > 0)
                    {
                        using (var fileStream = new FileStream(Path.Combine(Path.Combine(_webHostEnvironment.ContentRootPath, "wwwroot/CommentImages/"), comment.Id + ".jpg"), FileMode.Create))
                        {
                            await customFile.CopyToAsync(fileStream);
                            comment.ImagePath = comment.Id.ToString();
                        }
                    }
                }

                if (comment.Message == null)
                {
                    return Redirect(string.Format("~/Posts/Details/{0}", comment.PostId));
                }

                _context.Add(comment);
                await _context.SaveChangesAsync();

                return Redirect(string.Format("~/Posts/Details/{0}", comment.PostId));
            }

            return View(comment);
        }

        // POST: Comments/Delete/5
        [HttpPost, ActionName("CommentDeleteConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CommentDeleteConfirmed(string id)
        {
            var comment = await _context.Comment.FindAsync(id);
            _context.Comment.Remove(comment);
            await _context.SaveChangesAsync();

            if (comment.ImagePath != null)
            {
                System.IO.File.Delete(Path.Combine(_webHostEnvironment.ContentRootPath, "wwwroot/CommentImages/" + id + ".jpg"));
            }

            return Redirect(string.Format("~/Posts/Details/{0}", comment.PostId));
        }
    }
}
