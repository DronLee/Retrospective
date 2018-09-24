using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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
    public class RecordController: MyController
    {
        private readonly AppDbContext _dbContext;
        private readonly IStringLocalizer<SharedResources> _stringLocalizer;

        public RecordController(AppDbContext dbContext, IStringLocalizer<SharedResources> stringLocalizer, 
            ILogger<HomeController> logger, IHttpContextAccessor httpContextAccessor): base(logger, httpContextAccessor)
        {
            _dbContext = dbContext;
            _stringLocalizer = stringLocalizer;
        }

        private async Task<Record[]> GetRecordsPerDay(int subjectId, DateTime currentDay)
        {
            return await _dbContext.Records.Where(
                r => r.SubjectId == subjectId && r.CreatedOn.Date == currentDay).ToArrayAsync();
        }

        private async Task<string[]> GetDays(int subjectId)
        {
            List<DateTime> days = await _dbContext.Records.Where(
                r => r.SubjectId == subjectId).Select(r => r.CreatedOn.Date).ToListAsync();
            days.Add(DateTime.Now.Date);
            return days.Distinct().OrderByDescending(d => d).Take(20).Select(
                d => d.ToString("dd.MM.yyyy")).ToArray();
        }

        public async Task<IActionResult> Index(int subjectId, string subjectName)
        {
            LogInformation("Загрузка страницы записей.");
            string[] days = await GetDays(subjectId);
            var viewModel = new RecordsViewModel {
                SubjectName = subjectName,
                Records = await GetRecordsPerDay(subjectId, ConvertStringToDay(days.First()).Date),
                Days = days.Select(d => new SelectListItem(d, d)).ToList(),
                CurrentDay = days.First(),
                RecordTypes = GetRecordTypes()
            };
            LogInformation("Все данные для страницы записей успешно получены.");
            return View("Index", viewModel);
        }

        private List<SelectListItem> GetRecordTypes()
        {
            return new List<SelectListItem>
                {
                    new SelectListItem(_stringLocalizer["To begin"], "0"),
                    new SelectListItem(_stringLocalizer["Stop"], "1"),
                    new SelectListItem(_stringLocalizer["Continue"], "2"),
                };
        }

        private DateTime ConvertStringToDay(string value)
        {
            GroupCollection groups = Regex.Match(value, @"^(?<day>\d\d)\.(?<month>\d\d)\.(?<year>\d{4})$").Groups;
            return new DateTime(int.Parse(groups["year"].Value), 
                int.Parse(groups["month"].Value), int.Parse(groups["day"].Value));
        }

        public async Task<IActionResult> GetRecordsData(int subjectId, string currentDay)
        {
            LogInformation(string.Format("Получение записей за день \"{0}\".", currentDay));
            var records = await GetRecordsPerDay(subjectId, ConvertStringToDay(currentDay));
            LogInformation(string.Format("Записи за день \"{0}\" успешно получены.", currentDay));
            return PartialView("_Data", 
                new RecordsViewModel {
                    Records = records,
                    RecordTypes = GetRecordTypes(),
                    CurrentDay = currentDay
                });
        }

        private bool Verify(string nickname, string text)
        {
            if(string.IsNullOrEmpty(nickname))
                ModelState.AddModelError("Nickname", _stringLocalizer["Please enter your nickname."]);
            if (string.IsNullOrEmpty(text))
                ModelState.AddModelError("NewMessage", _stringLocalizer["Write your wish."]);
            return ModelState.IsValid;
        }

        public async Task<IActionResult> AddRecord(int subjectId, string currentDay, string nickname, byte recordType, string text)
        {
            LogInformation("Добавление записи.");
            if(Verify(nickname, text))
            {
                _dbContext.Records.Add(new Record
                {
                    SubjectId = subjectId,
                    CreatedOn = DateTime.Now,
                    Author = nickname,
                    RecordType = recordType,
                    Text = text
                });
                await _dbContext.SaveChangesAsync();
                LogInformation("Запись успешно добавлена.");
            }
            return PartialView("_Data", 
                    new RecordsViewModel {
                        Records = await GetRecordsPerDay(subjectId, ConvertStringToDay(currentDay)),
                        RecordTypes = GetRecordTypes(),
                        CurrentDay = currentDay,
                        NewMessage = text,
                        Nickname = nickname
                    });
        }
    }
}