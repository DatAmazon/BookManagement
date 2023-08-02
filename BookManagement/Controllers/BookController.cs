using BookManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BookManagement.Controllers
{
    public class BookController : Controller
    {
        private readonly Service _service;
        public BookController(Service service)
        {
            _service = service;
        }
        public IActionResult Index(int page = 1)
        {
            //return View(_service.Get());

            var model = _service.Paging(page);
            ViewData["Pages"] = model.pages;
            ViewData["Page"] = model.page;
            ViewData["CountBook"] = _service.CountBook();
            return View(model.books);
        }

        public IActionResult Details(int id)
        {
            var b = _service.Get(id);
            if (b == null) return NotFound();
            else return View(b);
        }

        public IActionResult Delete(int id)
        {
            var b = _service.Get(id);
            if (b == null) return NotFound();
            else return View(b);
        }
        [HttpPost]
        public IActionResult Delete(Book book)
        {
            _service.Delete(book.Id);
            _service.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            var b = _service.Get(id);
            if (b == null) return NotFound();
            else return View(b);
        }
        [HttpPost]
        public IActionResult Edit(Book book, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                if (file != null)
                    _service.Upload(book, file);
                _service.Update(book);
                _service.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(book);
        }

        public IActionResult Create() => base.View(_service.Create());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Name,Authors,Publisher,Year,DataFile")] Book book, IFormFile? file)
        {

            if (ModelState.IsValid)
            {
                if (file != null)
                    _service.Upload(book, file);
                _service.Add(book);
                _service.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(book);
        }

        // Read
        public IActionResult Read(int id)
        {
            var b = _service.Get(id);
            if (b == null) return NotFound();
            if (!System.IO.File.Exists(_service.GetDataPath(b.DataFile))) return NotFound();
            var (stream, type) = _service.Download(b);
            return File(stream, type, b.DataFile);
        }




    }
}


