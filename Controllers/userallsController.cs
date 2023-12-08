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
using System.Net.Mail;
using Microsoft.AspNetCore.Identity;
using System.Data;

namespace finalproject.Controllers
{
    public class userallsController : Controller
    {
        private readonly finalprojectContext _context;

        public userallsController(finalprojectContext context)
        {
            _context = context;
        }

        // GET: useralls
        public async Task<IActionResult> Index()
        {
            return _context != null ?
                      View(await _context.userall.ToListAsync()) :
                      Problem("Entity set 'finalprojectContext.userall'  is null.");
        }
        public IActionResult Create()
        {
            return View();
        }

        // POST: useralls/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,name,password,role,registdate")] userall userall)
        {
            if (ModelState.IsValid)
            {
                _context.Add(userall);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(userall);
        }

        public IActionResult login()
        {
            if (!HttpContext.Request.Cookies.ContainsKey("Name"))
                return View();
            else
            {
                string na = HttpContext.Request.Cookies["Name"].ToString();
                string ro = HttpContext.Request.Cookies["Role"].ToString();
                HttpContext.Session.SetString("Name", na);
                HttpContext.Session.SetString("Role", ro);

                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost, ActionName("login")]
        public async Task<IActionResult> login(string na, string pa, string auto)
        {
            SqlConnection conn1 = new SqlConnection("Data Source=.\\sqlexpress;Initial Catalog=project;Integrated Security=True;Pooling=False");
            string sql;
            sql = "SELECT * FROM userall where name ='" + na + "' and  password ='" + pa + "' ";
            SqlCommand comm = new SqlCommand(sql, conn1);
            conn1.Open();
            SqlDataReader reader = comm.ExecuteReader();

            if (reader.Read())
            {
                string id = Convert.ToString((int)reader["Id"]);
                string na1 = (string)reader["name"];
                string ro = (string)reader["role"];
                HttpContext.Session.SetString("userid", id);
                HttpContext.Session.SetString("Name", na1);
                HttpContext.Session.SetString("Role", ro);
                reader.Close();
                conn1.Close();
                if (auto == "on")
                {
                    HttpContext.Response.Cookies.Append("Name", na1);
                    HttpContext.Response.Cookies.Append("Role", ro);
                }

                string ss = HttpContext.Session.GetString("Role");
                if (ss == "admin")
                    return RedirectToAction("adminhome", "useralls");
                else
                    return RedirectToAction("customerhome", "useralls");
            }
            else
            {
                ViewData["Message"] = "wrong username or password";
                return View();
            }
        }

    public async Task<IActionResult> customerhome()

        {    ViewData["name"] = HttpContext.Session.GetString("Name");

            string ss = HttpContext.Session.GetString("Role");
            if (ss == "customer")
                return _context.items != null ?
                         View(await _context.items.ToListAsync()) :
                         Problem("Entity set 'finalprojectContext.userall'  is null.");
            else
                return RedirectToAction("login", "useralls");        

        }

        public async Task<IActionResult> adminhome()
        {
            ViewData["name"] = HttpContext.Session.GetString("Name");

            string ss = HttpContext.Session.GetString("Role");
            if (ss == "admin")
                return View();
            else
                return RedirectToAction("login", "useralls");
        }

        public IActionResult addadmin()
        {
            string ss = HttpContext.Session.GetString("Role");
            if (ss == "admin")
                return View();
            else
                return RedirectToAction("login", "useralls");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult addadmin([Bind("name,password,role,registdate")] userall cust)
        {
            SqlConnection conn = new SqlConnection("Data Source=.\\sqlexpress;Initial Catalog=project;Integrated Security=True;Pooling=False");
            conn.Open();
            string sql;
            Boolean flage = false;
            sql = "select * from userall where name = '" + cust.name + "'";
            SqlCommand comm = new SqlCommand(sql, conn);
            SqlDataReader reader = comm.ExecuteReader();
            if (reader.Read())
            {
                flage = true;
                cust.role = HttpContext.Session.GetString("Role");
                string id = Convert.ToString((int)reader["Id"]);
                HttpContext.Session.SetString("userid", id);
                HttpContext.Session.SetString("Name", cust.name);
                HttpContext.Session.SetString("Role", cust.role);

            }
            reader.Close();
            if (flage == true)
            {
                ViewData["message"] = "name already exists";

            }
            else
            {
                cust.role = HttpContext.Session.GetString("Role");
                cust.registdate = DateTime.Today;
                string mm = cust.registdate.ToString("yyyy-MM-dd");

                sql = "insert into userall (name,password,role,registdate)  values  ('" + cust.name + "','" + (string)cust.password + "','" + cust.role + "','" + mm + "')";
                comm = new SqlCommand(sql, conn);
                comm.ExecuteNonQuery();
                ViewData["message"] = "Sucessfully added";
                return RedirectToAction("adminhome", "useralls");
            }
            conn.Close();
            return View();
        }



        // GET: useralls/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.userall == null)
            {
                return NotFound();
            }

            var userall = await _context.userall
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userall == null)
            {
                return NotFound();
            }

            return View(userall);
        }

        // GET: useralls/Create
        public IActionResult registration()
        {
            string ss = HttpContext.Session.GetString("Role");
            if (ss == "customer")
                return View();
            else
                return RedirectToAction("login", "useralls");

            return View();

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult registration([Bind("name,password,role,registdate")] userall cust)
        {
            SqlConnection conn = new SqlConnection("Data Source=.\\sqlexpress;Initial Catalog=project;Integrated Security=True;Pooling=False");
            conn.Open();
            string sql;
            Boolean flage = false;
            sql = "select * from userall where name = '" + cust.name + "'";
            SqlCommand comm = new SqlCommand(sql, conn);
            SqlDataReader reader = comm.ExecuteReader();
            if (reader.Read())
            {
                flage = true;

                cust.role = HttpContext.Session.GetString("Role");
                string id = Convert.ToString((int)reader["Id"]);
                HttpContext.Session.SetString("userid", id);
                HttpContext.Session.SetString("Name", cust.name);
                HttpContext.Session.SetString("Role", cust.role);

            }
            reader.Close();
            if (flage == true)
            {
                ViewData["message"] = "name already exists";

            }
            else
            {   
                cust.role = HttpContext.Session.GetString("Role");
                cust.registdate = DateTime.Today;
                string mm = cust.registdate.ToString("yyyy-MM-dd");

                sql = "insert into userall (name,password,role,registdate)  values  ('" + cust.name + "','" + (string)cust.password + "','" + cust.role + "','" + mm +"')";
                comm = new SqlCommand(sql, conn);
                comm.ExecuteNonQuery();
                ViewData["message"] = "Sucessfully added";
                return RedirectToAction("adminhome", "useralls");
            }
            conn.Close();


            return View();
        }


        // GET: useralls/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.userall == null)
            {
                return NotFound();
            }

            var userall = await _context.userall.FindAsync(id);
            if (userall == null)
            {
                return NotFound();
            }
            return View(userall);
        }

        // POST: useralls/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,name,password,role,registdate")] userall userall)
        {
            if (id != userall.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(userall);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!userallExists(userall.Id))
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
            return View(userall);
        }

        // GET: useralls/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.userall == null)
            {
                return NotFound();
            }

            var userall = await _context.userall
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userall == null)
            {
                return NotFound();
            }

            return View(userall);
        }

        // POST: useralls/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.userall == null)
            {
                return Problem("Entity set 'finalprojectContext.userall'  is null.");
            }
            var userall = await _context.userall.FindAsync(id);
            if (userall != null)
            {
                _context.userall.Remove(userall);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

  

        public IActionResult customer_search()
        {
            {
                userall brItem = new userall();
                return View(brItem);
            }

        }
        [HttpPost]
        public async Task<IActionResult> Customer_search(string tit)
        {
            var bkItems = await _context.userall.FromSqlRaw("select * from userall where name = '" + tit + "' ").FirstOrDefaultAsync();
            return View(bkItems);
        }
        public IActionResult email()
        {
            ViewData["message"] = "";
            return View();
        }

        [HttpPost, ActionName("email")]
        [ValidateAntiForgeryToken]
        public IActionResult email(string address, string subject, string body)
        {
            SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
            var mail = new MailMessage();
            mail.From = new MailAddress("jmjeo1122@gmail.com");
            mail.To.Add(address); // receiver email address
            mail.Subject = subject;
            mail.IsBodyHtml = true;
            mail.Body = body;
            SmtpServer.Port = 587;
            SmtpServer.UseDefaultCredentials = false;
            SmtpServer.Credentials = new System.Net.NetworkCredential("jmjeo1122@gmail.com", "jflq soqf mkuk tzzo");
            SmtpServer.EnableSsl = true;
            SmtpServer.Send(mail);
            ViewData["message"] = "Email sent.";
            return View();

        }
        public IActionResult logout()
        {
            HttpContext.Session.Remove("Role");
            HttpContext.Session.Remove("Name");
            HttpContext.Response.Cookies.Delete("Role");
            HttpContext.Response.Cookies.Delete("Name");
            return RedirectToAction("login", "useralls");
        }


        private bool userallExists(int id)
        {
          return (_context.userall?.Any(e => e.Id == id)).GetValueOrDefault();
        }

    }
}
