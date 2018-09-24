using System.ComponentModel.DataAnnotations;

namespace Retrospective.Models
{
    /// <summary>
    /// Тема.
    /// </summary>
    public class Subject
    {
        /// <summary>
        /// Идентификатор.
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// Наименование.
        /// </summary>
        [Required, StringLength(100)]
        public string Name { get; set; }
        
        /// <summary>
        /// Хэш пароля.
        /// </summary>
        [Required, StringLength(344)]
        public string Password { get; set; }
    }
}