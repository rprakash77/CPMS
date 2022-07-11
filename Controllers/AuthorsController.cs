using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Collections.Generic;
using Microsoft.Data;
using Microsoft.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using CPMSDbFirst.Data;
using CPMSDbFirst.Models;


namespace CPMSDbFirst.Controllers
{
    public class AuthorsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AuthorsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Authors
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> Index()
        {
              return _context.Authors != null ? 
                          View(await _context.Authors.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.Authors'  is null.");
        }

        // GET: Authors/Details
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Authors == null)
            {
                return NotFound();
            }

            var author = await _context.Authors
                .FirstOrDefaultAsync(m => m.AuthorId == id);
            if (author == null)
            {
                return NotFound();
            }

            return View(author);
        }

        // GET: Authors/Create
        [AllowAnonymous]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Authors/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Create([Bind("AuthorId,FirstName,MiddleInitial,LastName,Affiliation,Department,Address,City,State,ZipCode,PhoneNumber,EmailAddress,Password")] Author author)
        {
            if (ModelState.IsValid)
            {
                _context.Add(author);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }
            return View(author);
        }

        // GET: Authors/Edit
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Authors == null)
            {
                return NotFound();
            }

            var author = await _context.Authors.FindAsync(id);
            if (author == null)
            {
                return NotFound();
            }
            return View(author);
        }

        // POST: Authors/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("AuthorId,FirstName,MiddleInitial,LastName,Affiliation,Department,Address,City,State,ZipCode,PhoneNumber,EmailAddress,Password")] Author author)
        {
            if (id != author.AuthorId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(author);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AuthorExists(author.AuthorId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index"); ;
            }
            return View(author);
        }

        // GET: Authors/Delete
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Authors == null)
            {
                return NotFound();
            }

            var author = await _context.Authors
                .FirstOrDefaultAsync(m => m.AuthorId == id);
            if (author == null)
            {
                return NotFound();
            }

            return View(author);
        }

        // POST: Authors/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Authors == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Authors'  is null.");
            }
            var author = await _context.Authors.FindAsync(id);
            if (author != null)
            {
                _context.Authors.Remove(author);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction("Index"); ;
        }

        private bool AuthorExists(int id)
        {
          return (_context.Authors?.Any(e => e.AuthorId == id)).GetValueOrDefault();
        }

        //GET
        
        public IActionResult AuthorLogin()
        {
            return View();
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AuthorLogin(string EmailAddress,string Password)
        {
            bool ValidCredentials = AuthorCredentials(EmailAddress, Password);
            if (ValidCredentials == false)
            {
                return View();
            }
            await SignInAuthor(EmailAddress);
            return RedirectToAction("Index", "Home");
        }

        //Sign In Author
        private async Task SignInAuthor(string EmailAddress)
        {
            var Claims = new List<Claim>
            {
                new Claim("Author", "EmailAddress")
            };

            var claimsIdentity = new ClaimsIdentity(
                Claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity));
        }

        //Checks if Author has register
        private bool AuthorCredentials(string Email, string Password)
        {
            using (SqlConnection SqlConn = new SqlConnection("Server=GLENN-LAPTOP;Database=CPMS;Trusted_Connection=True;"))
            {
                using (SqlCommand SqlCmd = new SqlCommand("spAuthorLogin", SqlConn))
                {
                    int RowCount = 0;

                    //Inputs
                    SqlCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    SqlCmd.Parameters.AddWithValue("@EmailAddress", Email);
                    SqlCmd.Parameters.AddWithValue("@Password", Password);

                    //Outputs
                    SqlCmd.Parameters.Add("@AuthorID", SqlDbType.Int);
                    SqlCmd.Parameters["@AuthorID"].Direction = ParameterDirection.Output;
                    SqlCmd.Parameters.Add("@FirstName", SqlDbType.VarChar, 50);
                    SqlCmd.Parameters["@FirstName"].Direction = ParameterDirection.Output;
                    SqlCmd.Parameters.Add("@User", SqlDbType.VarChar, 100);
                    SqlCmd.Parameters["@User"].Direction = ParameterDirection.Output;
                    SqlCmd.Parameters.Add("@RowCount",SqlDbType.Int);
                    SqlCmd.Parameters["@RowCount"].Direction = ParameterDirection.Output;
                    SqlConn.Open();
                    SqlCmd.ExecuteNonQuery();
                    RowCount = Convert.ToInt32(SqlCmd.Parameters["@RowCount"].Value);
                    SqlConn.Close();   
                    

                    if (RowCount != 1)
                    {
                        return false;
                    }
                    return true;
                }
            }
        }
    }
}
