using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using finalproject.Data;
using finalproject.Models;
using Microsoft.Data.SqlClient;

namespace finalproject.Controllers
{
    public class itemsController : Controller
    {
        private readonly finalprojectContext _context;

        public itemsController(finalprojectContext context)
        {
            _context = context;
        }





        public async Task<IActionResult> dashbourd()
        {
            string sql = "";

            SqlConnection conn = new SqlConnection("Data Source=.\\sqlexpress;Initial Catalog=project;Integrated Security=True;Pooling=False");

            SqlCommand comm;
            conn.Open();
            sql = "SELECT COUNT(Id)  FROM items where category = 'Necklaces' ";
            comm = new SqlCommand(sql, conn);
            ViewData["d1"] = (int)comm.ExecuteScalar();

            sql = "SELECT COUNT(Id)  FROM items where category = 'Bracelets'";
            comm = new SqlCommand(sql, conn);
            ViewData["d2"] = (int)comm.ExecuteScalar();

            sql = "SELECT COUNT(Id)  FROM items where category = 'Rings'";
            comm = new SqlCommand(sql, conn);
            ViewData["d3"] = (int)comm.ExecuteScalar();
            return View();


        }



        public async Task<IActionResult> imageslider()
        {
            return _context.items != null ?
                        View(await _context.items.ToListAsync()) :
                        Problem("Entity set 'finalprojectContext.items'  is null.");


        }



        public async Task<IActionResult> list()////////////////// يبيله تعديل
        {

            return _context.items != null ?
                        View(await _context.items.OrderBy(m => m.category).ToListAsync()
) :
                        Problem("Entity set 'finalprojectContext.items'  is null.");




        }//end list



        // GET: items
        public async Task<IActionResult> Index()////////////يبيه تعديل واقدر استخدم طريقتين كلها كتبتها
        {
            string ss = HttpContext.Session.GetString("Role");
            string nn = HttpContext.Session.GetString("Name");


            ViewData["name"] = nn;
            ViewData["role"] = ss;


            return _context.items != null ?
                         View(await _context.items.ToListAsync()) :
                         Problem("Entity set 'finalprojectContext.items'  is null.");


        }


        // GET: items/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.items == null)
            {
                return NotFound();
            }

            var items = await _context.items
                .FirstOrDefaultAsync(m => m.Id == id);
            if (items == null)
            {
                return NotFound();
            }

            return View(items);
        }

        // GET: items/Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(IFormFile file, [Bind("Id,name,descr,price,discount,quantity,category")] items items)
        {
            {
                if (file != null)
                {
                    string filename = file.FileName;
                    //  string  ext = Path.GetExtension(file.FileName);
                    string path = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\imagesP"));
                    using (var filestream = new FileStream(Path.Combine(path, filename), FileMode.Create))
                    { await file.CopyToAsync(filestream); }

                    items.imagefilename = filename;
                }

                _context.Add(items);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
        }


        // GET: items/Edit/5



        public async Task<IActionResult> edit_items(int? id)
        {
            if (id == null || _context.items == null)
            {
                return NotFound();
            }

            var book = await _context.items.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            return View(book);
        }


        // POST: books/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> edit_items(IFormFile file, int id, [Bind("Id,name,descr,price,quantity,discount,category,imagefilename")] items items)
        {
            {
                if (id != items.Id)
                { return NotFound(); }

                if (file != null)
                {
                    string filename = file.FileName;
                    string ext = Path.GetExtension(file.FileName);
                    string path = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\imagesP"));
                    using (var filestream = new FileStream(Path.Combine(path, filename), FileMode.Create))
                    { await file.CopyToAsync(filestream); }

                    items.imagefilename = filename;
                }
                _context.Update(items);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
        }
        /*
        // POST: books/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> edit_items(IFormFile file, int id, [Bind("Id,name,descr,price,quantity,discount,category,imagefilename")] items book)
        {
            if (id != book.Id)
            { return NotFound(); }

            if (file != null)
            {
                string filename = file.FileName;
                 string  ext = Path.GetExtension(file.FileName);
                string path = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images"));
                using (var filestream = new FileStream(Path.Combine(path, filename), FileMode.Create))
                { await file.CopyToAsync(filestream); }

                book.imagefilename = filename;
            }
            _context.Update(book);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        */



        // GET: items/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.items == null)
            {
                return NotFound();
            }

            var items = await _context.items
                .FirstOrDefaultAsync(m => m.Id == id);
            if (items == null)
            {
                return NotFound();
            }

            return View(items);
        }

        // POST: items/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.items == null)
            {
                return Problem("Entity set 'finalprojectContext.items'  is null.");
            }
            var items = await _context.items.FindAsync(id);
            if (items != null)
            {
                _context.items.Remove(items);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool itemsExists(int id)
        {
            return (_context.items?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}