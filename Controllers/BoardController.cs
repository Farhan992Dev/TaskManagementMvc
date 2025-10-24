using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using TaskManagementMvc.Data;
using TaskManagementMvc.Models;

namespace TaskManagementMvc.Controllers
{
    [Authorize]
    public class BoardController : Controller
    {
        private readonly TaskManagementContext _context;

        public BoardController(TaskManagementContext context)
        {
            _context = context;
        }

        // GET: Board
        public async Task<IActionResult> Index()
        {
            var boardColumns = await _context.BoardColumns.Include(b => b.Project).ToListAsync();
            return View(boardColumns);
        }

        // GET: Board/Create
        public IActionResult Create()
        {
            ViewData["Projects"] = new SelectList(_context.Projects, "Id", "Name");
            return View();
        }

        // POST: Board/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Order,IsDoneColumn,ProjectId")] BoardColumn boardColumn)
        {
            if (ModelState.IsValid)
            {
                _context.Add(boardColumn);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Projects"] = new SelectList(_context.Projects, "Id", "Name", boardColumn.ProjectId);
            return View(boardColumn);
        }

        // GET: Board/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var boardColumn = await _context.BoardColumns.FindAsync(id);
            if (boardColumn == null)
            {
                return NotFound();
            }
            ViewData["Projects"] = new SelectList(_context.Projects, "Id", "Name", boardColumn.ProjectId);
            return View(boardColumn);
        }

        // POST: Board/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Order,IsDoneColumn,ProjectId")] BoardColumn boardColumn)
        {
            if (id != boardColumn.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(boardColumn);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BoardColumnExists(boardColumn.Id))
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
            ViewData["Projects"] = new SelectList(_context.Projects, "Id", "Name", boardColumn.ProjectId);
            return View(boardColumn);
        }

        // GET: Board/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var boardColumn = await _context.BoardColumns
                .Include(b => b.Project)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (boardColumn == null)
            {
                return NotFound();
            }

            return View(boardColumn);
        }

        // POST: Board/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var boardColumn = await _context.BoardColumns.FindAsync(id);
            _context.BoardColumns.Remove(boardColumn);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BoardColumnExists(int id)
        {
            return _context.BoardColumns.Any(e => e.Id == id);
        }
    }
}
