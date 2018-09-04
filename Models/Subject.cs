using System.ComponentModel.DataAnnotations;

namespace Retrospective.Models
{
    public class Subject
    {
        public int Id { get; set; }
        
        [Required, StringLength(100)]
        public string Name { get; set; }
        
        [Required, StringLength(20)]
        public string Password { get; set; }
    }
}