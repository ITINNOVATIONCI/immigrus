using System.Linq;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.Data.Entity;
using immigrus.Models;
using System;

namespace immigrus.Controllers
{
    public class FaqsController : Controller
    {
        private ApplicationDbContext _context;

        public FaqsController(ApplicationDbContext context)
        {
            _context = context;    
        }

        // GET: Faqs
        public IActionResult Index()
        {
            return View(_context.Faq.OrderBy(c=>c.Ordre).ToList());
        }

        // GET: Faqs/Details/5
        public IActionResult Details(string id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            Faq faq = _context.Faq.Single(m => m.Id == id);
            if (faq == null)
            {
                return HttpNotFound();
            }

            return View(faq);
        }

        // GET: Faqs/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Faqs/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Faq faq)
        {
            if (ModelState.IsValid)
            {
                faq.Id = Guid.NewGuid().ToString();
                faq.Etat = "ACTIF";
                _context.Faq.Add(faq);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(faq);
        }

        // GET: Faqs/Edit/5
        public IActionResult Edit(string id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            Faq faq = _context.Faq.Single(m => m.Id == id);
            if (faq == null)
            {
                return HttpNotFound();
            }
            return View(faq);
        }

        // POST: Faqs/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Faq faq)
        {
            if (ModelState.IsValid)
            {
                _context.Update(faq);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(faq);
        }

        // GET: Faqs/Delete/5
        [ActionName("Delete")]
        public IActionResult Delete(string id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            Faq faq = _context.Faq.Single(m => m.Id == id);
            if (faq == null)
            {
                return HttpNotFound();
            }

            return View(faq);
        }

        // POST: Faqs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(string id)
        {
            Faq faq = _context.Faq.Single(m => m.Id == id);
            _context.Faq.Remove(faq);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
