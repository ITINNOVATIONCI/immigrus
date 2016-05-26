using System.Linq;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.Data.Entity;
using immigrus.Models;
using System;

namespace immigrus.Controllers
{
    public class ActualitesController : Controller
    {
        private ApplicationDbContext _context;

        public ActualitesController(ApplicationDbContext context)
        {
            _context = context;    
        }

        // GET: Actualites
        public IActionResult Index()
        {
            return View(_context.Actualite.OrderBy(c => c.Ordre).ToList());
        }

        // GET: Actualites/Details/5
        public IActionResult Details(string id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            Actualite actualite = _context.Actualite.Single(m => m.Id == id);
            if (actualite == null)
            {
                return HttpNotFound();
            }

            return View(actualite);
        }

        // GET: Actualites/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Actualites/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Actualite actualite)
        {
            if (ModelState.IsValid)
            {
                actualite.Id = Guid.NewGuid().ToString();
                actualite.Etat = "ACTIF";
                _context.Actualite.Add(actualite);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(actualite);
        }

        // GET: Actualites/Edit/5
        public IActionResult Edit(string id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            Actualite actualite = _context.Actualite.Single(m => m.Id == id);
            if (actualite == null)
            {
                return HttpNotFound();
            }
            return View(actualite);
        }

        // POST: Actualites/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Actualite actualite)
        {
            if (ModelState.IsValid)
            {
                _context.Update(actualite);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(actualite);
        }

        // GET: Actualites/Delete/5
        [ActionName("Delete")]
        public IActionResult Delete(string id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            Actualite actualite = _context.Actualite.Single(m => m.Id == id);
            if (actualite == null)
            {
                return HttpNotFound();
            }

            return View(actualite);
        }

        // POST: Actualites/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(string id)
        {
            Actualite actualite = _context.Actualite.Single(m => m.Id == id);
            _context.Actualite.Remove(actualite);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
