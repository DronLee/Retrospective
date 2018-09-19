// using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc;
using Retrospective.Models;

namespace Retrospective.ViewComponents
{
    public class RecordViewComponent: ViewComponent
    {
        public IViewComponentResult Invoke(Record record)
        {
            return View(record);
        }
    }
}