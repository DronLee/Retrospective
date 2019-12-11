using System.ComponentModel.DataAnnotations;

namespace Retrospective.Models
{
    /// <summary>
    /// Модель представления темы.
    /// </summary>
    public class SubjectViewModel
    {
        /// <summary>
        /// Наименование темы.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Пароль темы.
        /// </summary>
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}