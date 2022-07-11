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
    public class PapersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PapersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Papers
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Papers.Include(p => p.Author);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Papers/Details
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Papers == null)
            {
                return NotFound();
            }

            var paper = await _context.Papers
                .Include(p => p.Author)
                .FirstOrDefaultAsync(m => m.PaperId == id);
            if (paper == null)
            {
                return NotFound();
            }

            return View(paper);
        }

        // GET: Papers/Create
        [Authorize(Policy = "AuthorUser")]
        public IActionResult Create()
        {
            ViewData["AuthorId"] = new SelectList(_context.Authors, "AuthorId", "AuthorId");
            return View();
        }

        public IFormFile File { get; set;}

        // POST: Papers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "AuthorUser")]
        public async Task<IActionResult> Create(IFormFile File, [Bind("PaperId,AuthorId,Active,FilenameOriginal,Filename,Content,Title,Certification,NotesToReviewers,AnalysisOfAlgorithms,Applications,Architecture,ArtificialIntelligence,ComputerEngineering,Curriculum,DataStructures,Databases,DistanceLearning,DistributedSystems,EthicalSocietalIssues,FirstYearComputing,GenderIssues,GrantWriting,GraphicsImageProcessing,HumanComputerInteraction,LaboratoryEnvironments,Literacy,MathematicsInComputing,Multimedia,NetworkingDataCommunications,NonMajorCourses,ObjectOrientedIssues,OperatingSystems,ParallelsProcessing,Pedagogy,ProgrammingLanguages,Research,Security,SoftwareEngineering,SystemsAnalysisAndDesign,UsingTechnologyInTheClassroom,WebAndInternetProgramming,Other,OtherDescription,file")] Paper paper)
        {       
            _context.Add(paper);
            await _context.SaveChangesAsync();
            await UploadFile(File, paper.PaperId);
            return RedirectToAction("Index","Home");       
        }

        // GET: Papers/Edit
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Papers == null)
            {
                return NotFound();
            }

            var paper = await _context.Papers.FindAsync(id);
            if (paper == null)
            {
                return NotFound();
            }
            ViewData["AuthorId"] = new SelectList(_context.Authors, "AuthorId", "AuthorId", paper.AuthorId);
            return View(paper);
        }

        // POST: Papers/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("PaperId,AuthorId,Active,FilenameOriginal,Filename,Title,Certification,NotesToReviewers,AnalysisOfAlgorithms,Applications,Architecture,ArtificialIntelligence,ComputerEngineering,Curriculum,DataStructures,Databases,DistanceLearning,DistributedSystems,EthicalSocietalIssues,FirstYearComputing,GenderIssues,GrantWriting,GraphicsImageProcessing,HumanComputerInteraction,LaboratoryEnvironments,Literacy,MathematicsInComputing,Multimedia,NetworkingDataCommunications,NonMajorCourses,ObjectOrientedIssues,OperatingSystems,ParallelsProcessing,Pedagogy,ProgrammingLanguages,Research,Security,SoftwareEngineering,SystemsAnalysisAndDesign,UsingTechnologyInTheClassroom,WebAndInternetProgramming,Other,OtherDescription")] Paper paper)
        {
            if (id != paper.PaperId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(paper);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PaperExists(paper.PaperId))
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
            ViewData["AuthorId"] = new SelectList(_context.Authors, "AuthorId", "AuthorId", paper.AuthorId);
            return View(paper);
        }

        // GET: Papers/Delete
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Papers == null)
            {
                return NotFound();
            }

            var paper = await _context.Papers
                .Include(p => p.Author)
                .FirstOrDefaultAsync(m => m.PaperId == id);
            if (paper == null)
            {
                return NotFound();
            }

            return View(paper);
        }

        // POST: Papers/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Papers == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Papers'  is null.");
            }
            var paper = await _context.Papers.FindAsync(id);
            if (paper != null)
            {
                _context.Papers.Remove(paper);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PaperExists(int id)
        {
            return (_context.Papers?.Any(e => e.PaperId == id)).GetValueOrDefault();
        }

        public async Task<IActionResult> UploadFile(IFormFile file, int id)
        {
            if (file != null)
            {
                if(file.Length > 0 && file.Length < 300000)
                {
                    var myPaper = _context.Papers.Find(id);

                    using(var target = new MemoryStream())
                    {
                        file.CopyTo(target);
                        myPaper.Content = target.ToArray();
                    }

                    _context.Papers.Update(myPaper);
                    await _context.SaveChangesAsync();
                }
            }
            return RedirectToAction("Index", "Home");
        }
    }
}
