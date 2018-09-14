using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Retrospective.Models;

namespace Retrospective.Controllers
{
    public class SubjectController: Controller
    {
        private readonly AppDbContext _dbContext;

        public SubjectController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult Welcome()
        {
            var result = new SubjectViewModel();
            return View("Welcome", result);
        }

        public IActionResult Entry(SubjectViewModel viewModel)
        {
            if(ModelState.IsValid)
            {
                string hash = Password.GetHash(viewModel.Password);
                var subject = _dbContext.Subjects.SingleOrDefaultAsync(
                    s => s.Name == viewModel.Name && s.Password == hash);
                if (subject.Result == null)
                    return NotFound();
                return RedirectToAction("Index", "Record", new 
                    { subjectId = subject.Result.Id, subjectName = subject.Result.Name });
            }
            return View();
        }

        public async Task<ActionResult> Create(SubjectViewModel viewModel)
        {
            if(ModelState.IsValid)
            {
                Subject subject = new Subject();
                subject.Name = viewModel.Name;
                subject.Password = Password.GetHash(viewModel.Password);
                _dbContext.Subjects.Add(subject);
                await _dbContext.SaveChangesAsync();
                return RedirectToAction("Index", "Record", new { subjectId = subject.Id, subjectName = subject.Name } );
            }
            return View();
        }
    }
}