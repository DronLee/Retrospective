using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Retrospective.Models;

namespace Retrospective.Controllers
{
    /// <summary>
    /// Контроллер, реализующий работу с записями.
    /// </summary>
    public class RecordController: MyController
    {
        private const string _dayFormat = "dd.MM.yyyy";

        public RecordController(AppDbContext dbContext, IStringLocalizer<SharedResources> stringLocalizer, 
            ILogger<HomeController> logger, IHttpContextAccessor httpContextAccessor):
                base(dbContext, stringLocalizer, logger, httpContextAccessor) {}

        /// <summary>
        /// Получение записей за определённый день, относящихся к указанной теме.
        /// </summary>
        /// <param name="subjectId">Идентификатор темы.</param>
        /// <param name="currentDay">День, за который будут отобраны записи.</param>
        /// <returns>Отобранные записи.</returns>
        private async Task<Record[]> GetRecordsPerDay(int subjectId, DateTime currentDay)
        {
            return await dbContext.Records.Where(
                r => r.SubjectId == subjectId && r.CreatedOn.Date == currentDay).ToArrayAsync();
        }

        /// <summary>
        /// Получение дней, по которым есть записи для указанной темы.
        /// </summary>
        /// <param name="subjectId">Идентификатор темы, для которой будут отобраны дни.</param>
        /// <returns>Дни в формате для отображения на странице.</returns>
        private async Task<string[]> GetDays(int subjectId)
        {
            List<DateTime> days = await dbContext.Records.Where(
                r => r.SubjectId == subjectId).Select(r => r.CreatedOn.Date).ToListAsync();
            days.Add(DateTime.Now.Date);
            return days.Distinct().OrderByDescending(d => d).Take(20).Select(
                d => d.ToString("dd.MM.yyyy")).ToArray();
        }

        /// <summary>
        /// Загрузка страницы с записями. Отображается сразу после входа в тему.
        /// </summary>
        /// <param name="subjectId">Идентификатор темы.</param>
        /// <param name="subjectName">Тема.</param>
        /// <returns>Страница записей.</returns>
        public async Task<IActionResult> Index(int subjectId, string subjectName)
        {
            LogInformation("Загрузка страницы записей.");
            string[] days = await GetDays(subjectId);
            var viewModel = new RecordsViewModel {
                SubjectName = subjectName,
                Records = await GetRecordsPerDay(subjectId, DateTime.ParseExact(days.First(), _dayFormat, null).Date),
                Days = days.Select(d => new SelectListItem(d, d)).ToList(),
                CurrentDay = days.First(),
                RecordTypes = GetRecordTypes()
            };
            LogInformation("Все данные для страницы записей успешно получены.");
            return View("Index", viewModel);
        }

        /// <summary>
        /// Получение списка типов записей для предоставления выбора на странице.
        /// </summary>
        /// <returns>Список типов записей.</returns>
        private List<SelectListItem> GetRecordTypes()
        {
            return new List<SelectListItem>
                {
                    new SelectListItem(stringLocalizer["To begin"], "0"),
                    new SelectListItem(stringLocalizer["Stop"], "1"),
                    new SelectListItem(stringLocalizer["Continue"], "2"),
                };
        }

        /// <summary>
        /// Получение частичного представления с записями.
        /// </summary>
        /// <param name="subjectId">Идентификатор темы.</param>
        /// <param name="currentDay">День, за который требуется отобрать в представление записи.</param>
        /// <returns>Частичное представление с записями.</returns>
        public async Task<IActionResult> GetRecordsData(int subjectId, string currentDay)
        {
            LogInformation(string.Format("Получение записей за день \"{0}\".", currentDay));
            var records = await GetRecordsPerDay(subjectId, DateTime.ParseExact(currentDay, _dayFormat, null));
            LogInformation(string.Format("Записи за день \"{0}\" успешно получены.", currentDay));
            return PartialView("_Data", 
                new RecordsViewModel {
                    Records = records,
                    RecordTypes = GetRecordTypes(),
                    CurrentDay = currentDay
                });
        }

        /// <summary>
        /// Проверка данных. Если в данных есть ошибки, они указываются в ModelState с соответсвующим ключём.
        /// </summary>
        /// <param name="nickname">Псевдоним автора.</param>
        /// <param name="text">Текст записи.</param>
        /// <returns>True - все данные корректные.</returns>
        private bool Verify(string nickname, string text)
        {
            if(string.IsNullOrEmpty(nickname))
                ModelState.AddModelError("Nickname", stringLocalizer["Please enter your nickname."]);
            if (string.IsNullOrEmpty(text))
                ModelState.AddModelError("NewMessage", stringLocalizer["Write your wish."]);
            return ModelState.IsValid;
        }

        /// <summary>
        /// Добавление новой записи.
        /// </summary>
        /// <param name="subjectId">Идентификатор темы, в которой будет создана запись.</param>
        /// <param name="currentDay">День, за который будет создана запись.</param>
        /// <param name="nickname">Псевдоним автора записи.</param>
        /// <param name="recordType">Тип создаваемой записи.</param>
        /// <param name="text">Текст создаваемой записи.</param>
        /// <returns>Частичное представление с записями.</returns>
        public async Task<IActionResult> AddRecord(int subjectId, string currentDay, string nickname, byte recordType, string text)
        {
            LogInformation("Добавление записи.");

            var createdOn = DateTime.ParseExact(currentDay, _dayFormat, null);

            if (Verify(nickname, text))
            {
                dbContext.Records.Add(new Record
                {
                    SubjectId = subjectId,
                    CreatedOn = createdOn,
                    Author = nickname,
                    RecordType = recordType,
                    Text = text
                });
                await dbContext.SaveChangesAsync();
                LogInformation("Запись успешно добавлена.");
            }
            return PartialView("_Data", 
                    new RecordsViewModel {
                        Records = await GetRecordsPerDay(subjectId, createdOn),
                        RecordTypes = GetRecordTypes(),
                        CurrentDay = currentDay,
                        NewMessage = text,
                        Nickname = nickname
                    });
        }
    }
}