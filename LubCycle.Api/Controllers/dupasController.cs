using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LubCycle.Api.Data;
using LubCycle.Api.Models;

namespace LubCycle.Api.Controllers
{
    public class dupasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public dupasController(ApplicationDbContext context)
        {
            _context = context;    
        }

        // GET: dupas
        public async Task<IActionResult> Index()
        {
            return View(await _context.Dupas.ToListAsync());
        }

        // GET: dupas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dupa = await _context.Dupas.SingleOrDefaultAsync(m => m.id == id);
            if (dupa == null)
            {
                return NotFound();
            }

            return View(dupa);
        }

        // GET: dupas/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: dupas/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,name")] dupa dupa)
        {
            if (ModelState.IsValid)
            {
                _context.Add(dupa);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(dupa);
        }

        // GET: dupas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dupa = await _context.Dupas.SingleOrDefaultAsync(m => m.id == id);
            if (dupa == null)
            {
                return NotFound();
            }
            return View(dupa);
        }

        // POST: dupas/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,name")] dupa dupa)
        {
            if (id != dupa.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(dupa);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!dupaExists(dupa.id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index");
            }
            return View(dupa);
        }

        // GET: dupas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dupa = await _context.Dupas.SingleOrDefaultAsync(m => m.id == id);
            if (dupa == null)
            {
                return NotFound();
            }

            return View(dupa);
        }

        // POST: dupas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var dupa = await _context.Dupas.SingleOrDefaultAsync(m => m.id == id);
            _context.Dupas.Remove(dupa);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool dupaExists(int id)
        {
            return _context.Dupas.Any(e => e.id == id);
        }
    }
}
