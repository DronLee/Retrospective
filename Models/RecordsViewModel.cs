using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Retrospective.Models
{
    /// <summary>
    /// Модель представления записей со всей информацией, необходимой для отображения.
    ///</summary>
    public class RecordsViewModel
    {
        /// <summary>
        /// Идентификатор темы, к который относятся представленные здесь записи.
        ///</summary>
        public int SubjectId { get; set; }
        
        /// <summary>
        /// Наименование темы, к который относятся представленные здесь записи.
        ///</summary>
        public string SubjectName { get; set; }

        /// <summary>
        /// Сами записи.
        ///</summary>
        public Record[] Records { get; set; }

        /// <summary>
        /// Выбранный день.
        ///</summary>
        public string CurrentDay { get; set; }

        /// <summary>
        /// Список дней, доступных для выбора.
        ///</summary>
        public List<SelectListItem> Days { get; set; }

        /// <summary>
        /// Псевдоним создателя новой записи.
        /// </summary>
        public string Nickname { get; set; }

        /// <summary>
        /// Возможные типы записей.
        /// </summary>
        public List<SelectListItem> RecordTypes { get; set; }

        /// <summary>
        /// Выбранный тип для новой записи.  
        /// </summary>
        public byte SelectedRecordType { get; set; }

        /// <summary>
        /// Текст новой записи.
        /// </summary>
        public string NewMessage { get; set; }
    }
}