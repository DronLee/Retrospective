using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Retrospective.Models;

namespace Retrospective.Controllers
{
    public class RecordController: Controller
    {
        private readonly AppDbContext _dbContext;

        public RecordController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
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
            string[] days = GetDays(subjectId).Append("12.08.2018").ToArray();
            DateTime currentDay = DateTime.Parse(days.Last()).Date;
            return View("Index", new RecordsViewModel {
                SubjectName = subjectName,
                Records = GetRecordsPerDay(subjectId, currentDay),
                Days = days.Select(d => new SelectListItem(d, d)).ToList(),
                CurrentDay = days.First()
            });
        }

        public IActionResult GetRecordsData(int subjectId, string currentDay)
        {
            return PartialView("_Data", GetRecordsPerDay(subjectId, DateTime.Parse(currentDay)));
        }
    }
}