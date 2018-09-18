using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Retrospective.Models;

namespace Retrospective.Controllers
{
    public class RecordController: Controller
    {
        private readonly AppDbContext _dbContext;
        private readonly IStringLocalizer<SharedResources> _stringLocalizer;

        public RecordController(AppDbContext dbContext, IStringLocalizer<SharedResources> stringLocalizer)
        {
            _dbContext = dbContext;
            _stringLocalizer = stringLocalizer;
        }

        private Record[] GetRecordsPerDay(int subjectId, DateTime currentDay)
        {
            return _dbContext.Records.Where(r => r.SubjectId == subjectId && r.CreatedOn.Date == currentDay).ToArray();
        }

        private string[] GetDays(int subjectId)
        {
            List<DateTime> days = _dbContext.Records.Where(
                r => r.SubjectId == subjectId).Select(r => r.CreatedOn.Date).ToList();
            days.Add(DateTime.Now.Date);
            return days.Distinct().OrderByDescending(d => d).Take(20).Select(d => d.ToString("dd.MM.yyyy")).ToArray();
        }

        public IActionResult Index(int subjectId, string subjectName)
        {
            string[] days = GetDays(subjectId);
            return View("Index", new RecordsViewModel {
                SubjectName = subjectName,
                Records = GetRecordsPerDay(subjectId, ConvertStringToDay(days.First()).Date),
                Days = days.Select(d => new SelectListItem(d, d)).ToList(),
                CurrentDay = days.First(),
                RecordTypes = GetRecordTypes()
            });
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

        public IActionResult GetRecordsData(int subjectId, string currentDay)
        {
            return PartialView("_Data", 
                new RecordsViewModel {
                    Records = GetRecordsPerDay(subjectId, ConvertStringToDay(currentDay)),
                    RecordTypes = GetRecordTypes()
                });
        }
    }
}