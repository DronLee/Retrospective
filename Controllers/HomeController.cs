using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Retrospective.Models;

namespace Retrospective.Controllers
{
    public class HomeController: Controller
    {
        private readonly AppDbContext _dbContext;
        private readonly IStringLocalizer<SharedResources> _stringLocalizer;

        public HomeController(AppDbContext dbContext, IStringLocalizer<SharedResources> stringLocalizer)
        {
            _dbContext = dbContext;
            _stringLocalizer = stringLocalizer;
        }

        public IActionResult Welcome()
        {
            var result = new SubjectViewModel();
            return View("Welcome", result);
        }

        public async Task<IActionResult> Entry(SubjectViewModel viewModel)
        {
            if(ModelState.IsValid)
            {
                var subject = await _dbContext.Subjects.SingleOrDefaultAsync(s => s.Name == viewModel.Name);
                if (subject == null)
                    return NotFound();
                string hash = Password.GetHash(viewModel.Password);
                if (subject.Password == hash)
                    return RedirectToAction("Index", "Record", new 
                        { subjectId = subject.Id, subjectName = subject.Name });
            }
            return View("Welcome", viewModel);
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

        public ActionResult Error(ErrorViewModel viewModel)
        {
            return View("Error", viewModel);
        }
    }
}