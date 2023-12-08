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
using System.Net;

namespace finalproject.Controllers
{
    public class ordersController : Controller
    {
        private readonly finalprojectContext _context;

        public ordersController(finalprojectContext context)
        {
            _context = context;
        }






        // buy action

        public async Task<IActionResult> buy(int? id)
        {
            var book = await _context.items.FindAsync(id);
            return View(book);
        }




        [HttpPost]
        public async Task<IActionResult> buy(int bookId, int quantity)
        {
            orders order = new orders();
            order.itemid = bookId;
            order.quantity = quantity;
            order.userid = Convert.ToInt32(HttpContext.Session.GetString("userid"));
            order.buydate = DateTime.Today;
            SqlConnection conn = new SqlConnection("Data Source=.\\sqlexpress;Initial Catalog=project;Integrated Security=True;Pooling=False");
            string sql;
            int qt = 0;
            sql = "select * from items where (id ='" + order.itemid + "' )";
            SqlCommand comm = new SqlCommand(sql, conn);
            conn.Open();
            SqlDataReader reader = comm.ExecuteReader();
            if (reader.Read())
            {
                qt = (int)reader["quantity"]; // store quantity
            }
            reader.Close();
            conn.Close();
            if (order.quantity > qt)
            {
                ViewData["message"] = "maxiumam order quantity should be " + qt;
                var book = await _context.items.FindAsync(bookId);
                return View(book);
            }
            else
            {
                _context.Add(order);
                await _context.SaveChangesAsync();
                sql = "UPDATE items  SET quantity  = quantity   - '" + order.quantity + "'  where (id ='" + order.itemid + "' )";
                comm = new SqlCommand(sql, conn);
                conn.Open();
                comm.ExecuteNonQuery();
                conn.Close();
                return RedirectToAction("customerhome","useralls");
            }
        }
        // need to modify
        public async Task<IActionResult> my_purchase()
        {
            int userid = Convert.ToInt32(HttpContext.Session.GetString("userid"));

            var orItems = await _context.my_purchase.FromSqlRaw("select userall.id as Id, items.name as itemname,orders.buydate as buydate, price* orders.quantity as totalprice, orders.quantity as quantity from items, orders , userall  where userid = '" + userid + "' and  itemid= items.Id and userid= userall.Id  ").ToListAsync();
            return View(orItems);

        }


        // 
        public async Task<IActionResult> report()
        {
            var orItems = await _context.report.FromSqlRaw("select userall.id as Id, userall.name as customername, sum (orders.quantity * items.price)  as total from items, orders , userall where  itemid= items.Id and userid= userall.Id  group by userall.id, userall.name,orders.buydate order By orders.buydate  ").ToListAsync();
            return View(orItems);
        }

        public async Task<IActionResult> orderdetails(int? id)// need to modify
        {

            var orItems = await _context.orderdetails.FromSqlRaw("select userall.Id as Id, userall.name as username," +
                " orders.buydate as buydate, items.price*orders.quantity as totalprice ,orders.quantity as quantity " +
                "from orders, userall,items  where  userall.Id= '" + id + "'  and userall.Id = orders.userid and orders.itemid = items.id   ").ToListAsync();
            return View(orItems);

        }



        // GET: orders
        public async Task<IActionResult> Index()
        {
            return _context.orders != null ?
                        View(await _context.orders.ToListAsync()) :
                        Problem("Entity set 'finalprojectContext.orders'  is null.");
        }

        // GET: orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.orders == null)
            {
                return NotFound();
            }

            var orders = await _context.orders
                .FirstOrDefaultAsync(m => m.Id == id);
            if (orders == null)
            {
                return NotFound();
            }

            return View(orders);
        }

        // GET: orders/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,itemid,userid,buydate,quantity")] orders orders)
        {
            if (ModelState.IsValid)
            {
                _context.Add(orders);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(orders);
        }

        // GET: orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.orders == null)
            {
                return NotFound();
            }

            var orders = await _context.orders.FindAsync(id);
            if (orders == null)
            {
                return NotFound();
            }
            return View(orders);
        }

        // POST: orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,itemid,userid,buydate,quantity")] orders orders)
        {
            if (id != orders.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(orders);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ordersExists(orders.Id))
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
            return View(orders);
        }

        // GET: orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.orders == null)
            {
                return NotFound();
            }

            var orders = await _context.orders
                .FirstOrDefaultAsync(m => m.Id == id);
            if (orders == null)
            {
                return NotFound();
            }

            return View(orders);
        }

        // POST: orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.orders == null)
            {
                return Problem("Entity set 'finalprojectContext.orders'  is null.");
            }
            var orders = await _context.orders.FindAsync(id);
            if (orders != null)
            {
                _context.orders.Remove(orders);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ordersExists(int id)
        {
            return (_context.orders?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}