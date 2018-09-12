using System.ComponentModel.DataAnnotations;

namespace Retrospective.Models
{
    public class SubjectViewModel
    {
        [Required, StringLength(100)]
        public string Name { get; set; }

        [Required, StringLength(20), DataType(DataType.Password)]
        public string Password { get; set; }
    }
}