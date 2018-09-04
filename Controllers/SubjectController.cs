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
                var subject = _dbContext.Subjects.SingleOrDefaultAsync(
                    s => s.Name == viewModel.Name && s.Password == viewModel.Password);
                if (subject.Result == null)
                    return NotFound();
                return View();
            }
            return View();
        }

        public async Task<ActionResult> Create(SubjectViewModel viewModel)
        {
            if(ModelState.IsValid)
            {
                Subject subject = new Subject();
                subject.Name = viewModel.Name;
                subject.Password = viewModel.Password;
                _dbContext.Subjects.Add(subject);
                await _dbContext.SaveChangesAsync();
            }
            return View();
        }
    }
}