using System;
using System.ComponentModel.DataAnnotations;

namespace Retrospective.Models
{
    public class Record
    {
        public int Id { get; set; }
        
        [Required]
        public int SubjectId { get; set; }

        [Required, StringLength(100)]
        public string Author { get; set; }

        [Required]
        public DateTime CreatedOn { get; set; }

        [Required]
        public byte RecordType { get; set; }

        [Required, StringLength(500)]
        public string Text { get; set; }
    }
}