using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Retrospective.Models
{
    public class RecordsViewModel
    {
        public int SubjectId { get; set; }
        
        public string SubjectName { get; set; }

        public Record[] Records { get; set; }

        public string CurrentDay { get; set; }

        public List<SelectListItem> Days { get; set; }
    }
}