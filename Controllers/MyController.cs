using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Retrospective.Controllers
{
    public class MyController: Controller
    {
        private readonly ILogger<HomeController> _logger;
        private const string _logDateFormat = "dd.MM.yyyy HH:mm:ss";
        private readonly IHttpContextAccessor _accessor;

        protected MyController(ILogger<HomeController> logger, IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _accessor = httpContextAccessor;
        }

        protected void LogInformation(string message)
        {
            _logger.LogInformation(string.Format("{0} (id = {1}): {2}", 
                DateTime.Now.ToString(_logDateFormat), _accessor.HttpContext.Connection.Id, message));
        }
    }
}