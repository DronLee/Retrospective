using System;
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

        private bool Verify(SubjectViewModel viewModel)
        {
            if(string.IsNullOrEmpty(viewModel.Name))
                ModelState.AddModelError("Name", _stringLocalizer["No topic name."]);
            if(string.IsNullOrEmpty(viewModel.Password))
                ModelState.AddModelError("Password", _stringLocalizer["Enter password."]);    
            return ModelState.IsValid;
        }

        public async Task<IActionResult> Entry(SubjectViewModel viewModel)
        {
            if(Verify(viewModel))
            {
                var subject = await _dbContext.Subjects.SingleOrDefaultAsync(s => s.Name == viewModel.Name);
                if (subject == null)
                    ModelState.AddModelError("Name", _stringLocalizer["Topic not found."]);
                else
                {
                    if (subject.Password == Password.GetHash(viewModel.Password))
                        return RedirectToAction("Index", "Record", new 
                            { subjectId = subject.Id, subjectName = subject.Name });
                    else
                        ModelState.AddModelError("Password", _stringLocalizer["Incorrect password."]);
                }
            }
            return View("Welcome", viewModel);
        }

        public async Task<ActionResult> Create(SubjectViewModel viewModel)
        {
            if(Verify(viewModel))
            {
                Subject subject = new Subject();
                subject.Name = viewModel.Name;
                subject.Password = Password.GetHash(viewModel.Password);
                _dbContext.Subjects.Add(subject);
                try
                {
                    await _dbContext.SaveChangesAsync();
                }
                catch(Exception exc)
                {
                    if(exc.InnerException.Message.Contains("UK_Subject_Name"))
                        ModelState.AddModelError("Name", _stringLocalizer["This topic already exists."]);
                    else
                        throw;
                }
                if (ModelState.IsValid)
                    return RedirectToAction("Index", "Record", new { subjectId = subject.Id, subjectName = subject.Name } );
            }
            return View("Welcome", viewModel);
        }

        public ActionResult Error(ErrorViewModel viewModel)
        {
            return View("Error", viewModel);
        }
    }
}