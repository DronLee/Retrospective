using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
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
            string[] days = await GetDays(subjectId);
            return View("Index", new RecordsViewModel {
                SubjectName = subjectName,
                Records = await GetRecordsPerDay(subjectId, ConvertStringToDay(days.First()).Date),
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

        public async Task<IActionResult> GetRecordsData(int subjectId, string currentDay)
        {
            return PartialView("_Data", 
                new RecordsViewModel {
                    Records = await GetRecordsPerDay(subjectId, ConvertStringToDay(currentDay)),
                    RecordTypes = GetRecordTypes(),
                    CurrentDay = currentDay
                });
        }

        public async Task<IActionResult> AddRecord(int subjectId, string currentDay, string nickname, byte recordType, string text)
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
            return PartialView("_Data", 
                new RecordsViewModel {
                    Records = await GetRecordsPerDay(subjectId, ConvertStringToDay(currentDay)),
                    RecordTypes = GetRecordTypes(),
                    CurrentDay = currentDay
                });
        }
    }
}