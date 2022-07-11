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
    public class ReviewersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReviewersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Reviewers
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> Index()
        {
            return _context.Reviewers != null ?
                        View(await _context.Reviewers.ToListAsync()) :
                        Problem("Entity set 'ApplicationDbContext.Reviewers'  is null.");
        }

        // GET: Reviewers/Details
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Reviewers == null)
            {
                return NotFound();
            }

            var reviewer = await _context.Reviewers
                .FirstOrDefaultAsync(m => m.ReviewerId == id);
            if (reviewer == null)
            {
                return NotFound();
            }

            return View(reviewer);
        }

        // GET: Reviewers/Create
        [AllowAnonymous]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Reviewers/Create   
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Create([Bind("ReviewerId,Active,FirstName,MiddleInitial,LastName,Affiliation,Department,Address,City,State,ZipCode,PhoneNumber,EmailAddress,Password,AnalysisOfAlgorithms,Applications,Architecture,ArtificialIntelligence,ComputerEngineering,Curriculum,DataStructures,Databases,DistancedLearning,DistributedSystems,EthicalSocietalIssues,FirstYearComputing,GenderIssues,GrantWriting,GraphicsImageProcessing,HumanComputerInteraction,LaboratoryEnvironments,Literacy,MathematicsInComputing,Multimedia,NetworkingDataCommunications,NonMajorCourses,ObjectOrientedIssues,OperatingSystems,ParallelProcessing,Pedagogy,ProgrammingLanguages,Research,Security,SoftwareEngineering,SystemsAnalysisAndDesign,UsingTechnologyInTheClassroom,WebAndInternetProgramming,Other,OtherDescription,ReviewsAcknowledged")] Reviewer reviewer)
        {
            if (ModelState.IsValid)
            {
                _context.Add(reviewer);
                await _context.SaveChangesAsync();
                TempData["success"] = "You have been registered successfully! Please Login!";
                return RedirectToAction("Index", "Home");
            }
            return View(reviewer);
        }

        // GET: Reviewers/Edit/5
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Reviewers == null)
            {
                return NotFound();
            }

            var reviewer = await _context.Reviewers.FindAsync(id);
            if (reviewer == null)
            {
                return NotFound();
            }
            return View(reviewer);
        }

        // POST: Reviewers/Edit    
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("ReviewerId,Active,FirstName,MiddleInitial,LastName,Affiliation,Department,Address,City,State,ZipCode,PhoneNumber,EmailAddress,Password,AnalysisOfAlgorithms,Applications,Architecture,ArtificialIntelligence,ComputerEngineering,Curriculum,DataStructures,Databases,DistancedLearning,DistributedSystems,EthicalSocietalIssues,FirstYearComputing,GenderIssues,GrantWriting,GraphicsImageProcessing,HumanComputerInteraction,LaboratoryEnvironments,Literacy,MathematicsInComputing,Multimedia,NetworkingDataCommunications,NonMajorCourses,ObjectOrientedIssues,OperatingSystems,ParallelProcessing,Pedagogy,ProgrammingLanguages,Research,Security,SoftwareEngineering,SystemsAnalysisAndDesign,UsingTechnologyInTheClassroom,WebAndInternetProgramming,Other,OtherDescription,ReviewsAcknowledged")] Reviewer reviewer)
        {
            if (id != reviewer.ReviewerId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(reviewer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReviewerExists(reviewer.ReviewerId))
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
            return View(reviewer);
        }

        // GET: Reviewers/Delete
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Reviewers == null)
            {
                return NotFound();
            }

            var reviewer = await _context.Reviewers
                .FirstOrDefaultAsync(m => m.ReviewerId == id);
            if (reviewer == null)
            {
                return NotFound();
            }

            return View(reviewer);
        }

        // POST: Reviewers/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Reviewers == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Reviewers'  is null.");
            }
            var reviewer = await _context.Reviewers.FindAsync(id);
            if (reviewer != null)
            {
                _context.Reviewers.Remove(reviewer);
            }

            await _context.SaveChangesAsync();
            TempData["success"] = "Table Updated Successfully!";
            return RedirectToAction(nameof(Index));
        }

        private bool ReviewerExists(int id)
        {
            return (_context.Reviewers?.Any(e => e.ReviewerId == id)).GetValueOrDefault();
        }

        [AllowAnonymous]
        public IActionResult ReviewerLogin()
        {
            return View();
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> ReviewerLogin(string EmailAddress, string Password)
        {
            if (EmailAddress == "Admin@gmail.com" && Password == "Admin")
            {
                await SignInAdmin(EmailAddress);
                TempData["success"] = "You have logged in as Admin succesfully!";
                return RedirectToAction("Index", "Home");
            }
            bool ValidCredentials = ReviewerCredentials(EmailAddress, Password);
            if (ValidCredentials == false)
            {
                return View();
            }
            await SignInReviewer(EmailAddress);
            TempData["success"] = "You have logged in succesfully!";
            return RedirectToAction("Index", "Home");
        }

        //Sign In Reviewer
        private async Task SignInReviewer(string EmailAddress)
        {
            var Claims = new List<Claim>
            {
                new Claim("Reviewer", "EmailAddress")
            };

            var claimsIdentity = new ClaimsIdentity(
                Claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity));
        }

        private async Task SignInAdmin(string EmailAddress)
        {
            var Claims = new List<Claim>
            {
                new Claim("Admin", "EmailAddress")
            };

            var claimsIdentity = new ClaimsIdentity(
                Claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity));
        }

        //ReviewerExistChecker
        //Uses SQL procedure from database "Reviewer Login"
        private bool ReviewerCredentials(string Email, string Password)
        {
            using (SqlConnection SqlConn = new SqlConnection("Server=GLENN-LAPTOP;Database=CPMS;Trusted_Connection=True;"))
            {
                using (SqlCommand SqlCmd = new SqlCommand("spReviewerLogin", SqlConn))
                {
                    int RowCount = 0;

                    //Inputs
                    SqlCmd.CommandType = System.Data.CommandType.StoredProcedure;
                    SqlCmd.Parameters.AddWithValue("@EmailAddress", Email);
                    SqlCmd.Parameters.AddWithValue("@Password", Password);

                    //Outputs
                    SqlCmd.Parameters.Add("@ReviewerID", SqlDbType.Int);
                    SqlCmd.Parameters["@ReviewerID"].Direction = ParameterDirection.Output;
                    SqlCmd.Parameters.Add("@FirstName", SqlDbType.VarChar, 50);
                    SqlCmd.Parameters["@FirstName"].Direction = ParameterDirection.Output;
                    SqlCmd.Parameters.Add("@User", SqlDbType.VarChar, 100);
                    SqlCmd.Parameters["@User"].Direction = ParameterDirection.Output;
                    SqlCmd.Parameters.Add("@RowCount", SqlDbType.Int);
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

        //Logout for Authors and Reviewers
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            TempData["success"] = "You logged out succesfully!";
            return RedirectToAction("Index", "Home");
        }
    }
}
