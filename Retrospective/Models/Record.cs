using System;
using System.ComponentModel.DataAnnotations;

namespace Retrospective.Models
{
    /// <summary>
    /// Описывает запись ретроспективы.
    /// </summary>
    public class Record
    {
        /// <summary>
        /// Идентификатор записи.
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// Идентификатор темы, к которой относится запись.
        /// </summary>
        [Required]
        public int SubjectId { get; set; }

        /// <summary>
        /// Автор записи.
        /// </summary>
        [Required, StringLength(100)]
        public string Author { get; set; }

        /// <summary>
        /// Дата и время добавления записи.
        /// </summary>
        [Required]
        public DateTime CreatedOn { get; set; }

        /// <summary>
        /// Тип записи.
        /// </summary>
        [Required]
        public byte RecordType { get; set; }

        /// <summary>
        /// Сам текст записи.
        /// </summary>
        [Required, StringLength(500)]
        public string Text { get; set; }
    }
}