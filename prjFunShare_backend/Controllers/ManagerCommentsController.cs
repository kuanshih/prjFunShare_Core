using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using prjFunShare_backend.Models;

namespace prjFunShare_backend.Controllers
{
    public class ManagerCommentsController : Controller
    {
        private readonly FUNShareContext _context;

        public ManagerCommentsController(FUNShareContext context)
        {
            _context = context;
        }

        // GET: ManagerComments
        public async Task<IActionResult> Index()
        {
            var fUNShareContext = _context.Comment.Include(c => c.Member).Include(c => c.Order).ThenInclude(c=>c.OrderDetail).ThenInclude(c=>c.ProductDetail).ThenInclude(c=>c.Product);
            return View(await fUNShareContext.ToListAsync());
        }

        // GET: ManagerComments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Comment == null)
            {
                return NotFound();
            }

            var comment = await _context.Comment
                .Include(c => c.Member)
                .Include(c => c.Order)
                .FirstOrDefaultAsync(m => m.CommentId == id);
            if (comment == null)
            {
                return NotFound();
            }

            return View(comment);
        }

        // GET: ManagerComments/Create
        public IActionResult Create()
        {
            ViewData["MemberId"] = new SelectList(_context.CustomerInfomation, "MemberId", "Email");
            ViewData["OrderId"] = new SelectList(_context.Order, "OrderId", "OrderId");
            return View();
        }

        // POST: ManagerComments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CommentId,OrderId,MemberId,Review,Rank,Date,ImagePath")] Comment comment)
        {
            if (ModelState.IsValid)
            {
                _context.Add(comment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MemberId"] = new SelectList(_context.CustomerInfomation, "MemberId", "Email", comment.MemberId);
            ViewData["OrderId"] = new SelectList(_context.Order, "OrderId", "OrderId", comment.OrderId);
            return View(comment);
        }

        // GET: ManagerComments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Comment == null)
            {
                return NotFound();
            }

            var comment = await _context.Comment.FindAsync(id);
            if (comment == null)
            {
                return NotFound();
            }
            ViewData["MemberId"] = new SelectList(_context.CustomerInfomation, "MemberId", "Email", comment.MemberId);
            ViewData["OrderId"] = new SelectList(_context.Order, "OrderId", "OrderId", comment.OrderId);
            return View(comment);
        }

        // POST: ManagerComments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CommentId,OrderId,MemberId,Review,Rank,Date,ImagePath")] Comment comment)
        {
            if (id != comment.CommentId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(comment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CommentExists(comment.CommentId))
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
            ViewData["MemberId"] = new SelectList(_context.CustomerInfomation, "MemberId", "Email", comment.MemberId);
            ViewData["OrderId"] = new SelectList(_context.Order, "OrderId", "OrderId", comment.OrderId);
            return View(comment);
        }

        // GET: ManagerComments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Comment == null)
            {
                return NotFound();
            }

            var comment = await _context.Comment
                .Include(c => c.Member)
                .Include(c => c.Order)
                .FirstOrDefaultAsync(m => m.CommentId == id);
            if (comment == null)
            {
                return NotFound();
            }

            return View(comment);
        }

        // POST: ManagerComments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Comment == null)
            {
                return Problem("Entity set 'FUNShareContext.Comment'  is null.");
            }
            var comment = await _context.Comment.FindAsync(id);
            if (comment != null)
            {
                _context.Comment.Remove(comment);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CommentExists(int id)
        {
          return (_context.Comment?.Any(e => e.CommentId == id)).GetValueOrDefault();
        }
    }
}
