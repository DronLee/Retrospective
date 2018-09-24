using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Retrospective.Models;

namespace Retrospective.Controllers
{
    /// <summary>
    /// Контроллер, предназначенный как базовый для других контроллеров,
    /// так как содержит уже необходимый для журналирования функционал.
    /// </summary>
    public class MyController: Controller
    {
        /// <summary>
        /// Объект для работы с БД.
        /// </summary>
        protected readonly AppDbContext dbContext;
        /// <summary>
        /// Объект для реализации локализации.
        /// </summary>
        protected readonly IStringLocalizer<SharedResources> stringLocalizer;
        /// <summary>
        /// Объект, выполняющий журналирование.
        /// </summary>
        private readonly ILogger<HomeController> _logger;
        /// <summary>
        /// Формат даты в создаваемых записях журнала.
        /// </summary>
        private const string _logDateFormat = "dd.MM.yyyy HH:mm:ss";
        /// <summary>
        /// Объект для доступа к свойствам пришедшего запроса. 
        /// </summary>
        private readonly IHttpContextAccessor _accessor;

        protected MyController(AppDbContext dbContext, IStringLocalizer<SharedResources> stringLocalizer, 
            ILogger<HomeController> logger, IHttpContextAccessor httpContextAccessor)
        {
            this.dbContext = dbContext;
            this.stringLocalizer = stringLocalizer;
            _logger = logger;
            _accessor = httpContextAccessor;
        }

        /// <summary>
        /// Добавление записи в журнал уровня Information.
        /// </summary>
        /// <param name="message">Добавляемая запись.</param>
        protected void LogInformation(string message)
        {
            _logger.LogInformation(string.Format("{0} (id = {1}): {2}", 
                DateTime.Now.ToString(_logDateFormat), _accessor.HttpContext.Connection.Id, message));
        }
    }
}