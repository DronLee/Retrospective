using System.ComponentModel.DataAnnotations;

namespace Retrospective.Models
{
    public class SubjectViewModel
    {
        public string Name { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}