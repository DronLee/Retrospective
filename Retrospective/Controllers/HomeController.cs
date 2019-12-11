using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Retrospective.Models;

namespace Retrospective.Controllers
{
    /// <summary>
    /// Контроллер для вхождения в тему и её создания.
    /// </summary>
    public class HomeController: MyController
    {        
        public HomeController(AppDbContext dbContext, IStringLocalizer<SharedResources> stringLocalizer, 
            ILogger<HomeController> logger, IHttpContextAccessor httpContextAccessor):
                base(dbContext, stringLocalizer, logger, httpContextAccessor) {}

        /// <summary>
        /// Вывод начальной страницы.
        /// </summary>
        public IActionResult Welcome()
        {
            var result = new SubjectViewModel();
            return View("Welcome", result);
        }

        /// <summary>
        /// Вход в тему.
        /// </summary>
        /// <param name="viewModel">Модель представления темы, в которую осуществляется вход.</param>
        public async Task<IActionResult> Entry(SubjectViewModel viewModel)
        {
            LogInformation(string.Format("Вход в тему \"{0}\".", viewModel.Name));
            if(VerifyViewModel(viewModel))
            {
                var subject = await dbContext.Subjects.SingleOrDefaultAsync(s => s.Name == viewModel.Name);
                if (subject == null)
                    ModelState.AddModelError("Name", stringLocalizer["Topic not found."]);
                else
                {
                    if (subject.Password == Password.GetHash(viewModel.Password))
                    {
                        LogInformation(string.Format("Вход в тему \"{0}\" выполнен.", viewModel.Name));
                        return RedirectToAction("Index", "Record", new 
                            { subjectId = subject.Id, subjectName = subject.Name });   
                    }
                    else
                        ModelState.AddModelError("Password", stringLocalizer["Incorrect password."]);
                }
            }
            return View("Welcome", viewModel);
        }

        /// <summary>
        /// Создание новой темы.
        /// </summary>
        /// <param name="viewModel">Модель представления новой темы.</param>
        public async Task<ActionResult> Create(SubjectViewModel viewModel)
        {
            LogInformation(string.Format("Создание темы \"{0}\".", viewModel.Name));
            if(VerifyViewModel(viewModel))
            {
                var subject = await dbContext.Subjects.SingleOrDefaultAsync(s => s.Name == viewModel.Name);
                if (subject == null)
                { // Значит такой темы ещё нет, и можно создавать.
                    subject = new Subject()
                    {
                        Name = viewModel.Name,
                        Password = Password.GetHash(viewModel.Password)
                    };

                    dbContext.Subjects.Add(subject);
                    await dbContext.SaveChangesAsync();
                }
                else
                    ModelState.AddModelError("Name", stringLocalizer["This topic already exists."]);

                if (ModelState.IsValid)
                {
                    LogInformation(string.Format("Создание темы \"{0}\" выполнено.", viewModel.Name));
                    return RedirectToAction("Index", "Record", new { subjectId = subject.Id, subjectName = subject.Name } );
                }
            }
            return View("Welcome", viewModel);
        }

        public ActionResult Error()
        {
            return View("Error");
        }

        /// <summary>
        /// Проверка данных модели представления.
        /// Если в данных есть ошибки, они указываются в ModelState с соответсвующим ключём.
        /// </summary>
        /// <param name="viewModel">Модель представления для проверки.</param>
        /// <returns>True - все данные корректные.</returns>
        private bool VerifyViewModel(SubjectViewModel viewModel)
        {
            if (string.IsNullOrEmpty(viewModel.Name))
                ModelState.AddModelError("Name", stringLocalizer["No topic name."]);
            if (string.IsNullOrEmpty(viewModel.Password))
                ModelState.AddModelError("Password", stringLocalizer["Enter password."]);
            return ModelState.IsValid;
        }
    }
}