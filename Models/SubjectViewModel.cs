using System.ComponentModel.DataAnnotations;

namespace Retrospective.Models
{
    public class SubjectViewModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}