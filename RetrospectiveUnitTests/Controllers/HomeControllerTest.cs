using System.Linq;
using Xunit;
using Retrospective.Controllers;
using Retrospective.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc;
using System;

namespace RetrospectiveUnitTests.Controllers
{
    /// <summary>
    /// Тесты контроллера Home.
    /// </summary>
    public class HomeControllerTest
    {
        /// <summary>
        /// Сценарии с не существующей темой.
        /// </summary>
        public class TopicNotExist : IDisposable
        {
            private readonly AppDbContext _dbContext;
            private readonly HomeController _controller;

            public TopicNotExist()
            {
                _dbContext = new AppDbContext(InMemoryDbContextOptionsFactory.Create<AppDbContext>());
                _controller = new HomeController(_dbContext, new MyStringLocalizer(), null, null);
            }

            public void Dispose()
            {
                _dbContext.Dispose();
            }

            /// <summary>
            /// Проверка, что при попытке войти в не существующую тему, будет получена соответсвующая ошибка.
            /// </summary>
            [Fact]
            public async Task Entry()
            {
                await _controller.Entry(new SubjectViewModel() { Name = "Test", Password = "Test" });

                Assert.Equal(ModelValidationState.Invalid, _controller.ModelState.ValidationState);
                Assert.Single(_controller.ModelState.Keys);
                Assert.Equal("Name", _controller.ModelState.Single().Key);

                var modelStateValue = _controller.ModelState.Values.Single();
                Assert.Single(modelStateValue.Errors);
                Assert.Equal("Topic not found.", modelStateValue.Errors.Single().ErrorMessage);
            }

            /// <summary>
            /// Проверка успешного создания темы.
            /// </summary>
            [Fact]
            public async Task Create()
            {
                const string subjectName = "Test";

                var result = await _controller.Create(new SubjectViewModel() { Name = subjectName, Password = "Test" });

                Assert.Equal(ModelValidationState.Valid, _controller.ModelState.ValidationState);

                // Должно произойти перенаправление на Record.
                Assert.IsType<RedirectToActionResult>(result);
                Assert.Equal("Record", ((RedirectToActionResult)result).ControllerName);
                Assert.Equal(2, ((RedirectToActionResult)result).RouteValues.Count);
                Assert.Equal(subjectName, ((RedirectToActionResult)result).RouteValues.Values.ElementAt(1));
            }

            /// <summary>
            /// Проверка, что не удаётся создать тему без имени.
            /// </summary>
            [Fact]
            public async Task Create_NotName()
            {
                await _controller.Create(new SubjectViewModel() { Password = "Test" });

                Assert.Equal(ModelValidationState.Invalid, _controller.ModelState.ValidationState);
                Assert.Single(_controller.ModelState.Keys);
                Assert.Equal("Name", _controller.ModelState.Single().Key);

                var modelStateValue = _controller.ModelState.Values.Single();
                Assert.Single(modelStateValue.Errors);
                Assert.Equal("No topic name.", modelStateValue.Errors.Single().ErrorMessage);
            }
        }

        /// <summary>
        /// Сценарии с уже существующей темой.
        /// </summary>
        public class TopicExist : IDisposable
        {
            private const string _passwordHash = "k2ElK3Ig76OaEjXBph37CuR/uAXZD1BVDtvGfpMihbOZkE/LHCsmywcOo/SlKN0yOUdCyWU7QgmKAzXztXINxQCovZ0ns4i3gnvGDSQokZcYaxThGp96mrJV3iib1H79uskhhdAg0ndEbcaoyU+GOryPUny0IRiNhn4ucGZLVm6SZtTOIBz7uhNvdYrIQdRC/Ud8GgzqMp4CXbSZGq8SRgWf6XurqsQAMued2oTp+TA46ljVVvN/+D9ilOFH3ABwg60WKAUuym+k6EPcowhemlu6mXQE1L0UAE4xQv3eR8aOOsZXCQLa8TvUft/suwBF2u3qh4++YevlVqlEAOFBig==";
            private const string _subjectName = "Test";

            private readonly AppDbContext _dbContext;
            private readonly HomeController _controller;

            public TopicExist()
            {
                _dbContext = new AppDbContext(InMemoryDbContextOptionsFactory.Create<AppDbContext>());
                _dbContext.Add(new Subject() { Name = _subjectName, Password = _passwordHash });
                _dbContext.SaveChanges();
                _controller = new HomeController(_dbContext, new MyStringLocalizer(), null, null);
            }

            public void Dispose()
            {
                _dbContext.Dispose();
            }

            /// <summary>
            /// Проверка успешного входа в тему.
            /// </summary>
            [Fact]
            public async Task Entry()
            {
                var result = await _controller.Entry(new SubjectViewModel() { Name = _subjectName, Password = "test" });

                Assert.Equal(ModelValidationState.Valid, _controller.ModelState.ValidationState);

                // Должно произойти перенаправление на Record.
                Assert.IsType<RedirectToActionResult>(result);
                Assert.Equal("Record", ((RedirectToActionResult)result).ControllerName);
                Assert.Equal(2, ((RedirectToActionResult)result).RouteValues.Count);
                Assert.Equal(_subjectName, ((RedirectToActionResult)result).RouteValues.Values.ElementAt(1));
            }

            /// <summary>
            /// Проверка обработки попытки войти в тему с неверным паролем.
            /// </summary>
            [Fact]
            public async Task Entry_WrongPassword()
            {
                await _controller.Entry(new SubjectViewModel() { Name = "Test", Password = "Test" });

                Assert.Equal(ModelValidationState.Invalid, _controller.ModelState.ValidationState);
                Assert.Single(_controller.ModelState.Keys);
                Assert.Equal("Password", _controller.ModelState.Single().Key);

                var modelStateValue = _controller.ModelState.Values.Single();
                Assert.Single(modelStateValue.Errors);
                Assert.Equal("Incorrect password.", modelStateValue.Errors.Single().ErrorMessage);
            }

            /// <summary>
            /// Проверка обработки попытки повторно создать тему.
            /// </summary>
            [Fact]
            public async Task Create()
            {
                await _controller.Create(new SubjectViewModel() { Name = _subjectName, Password = "test" });

                Assert.Equal(ModelValidationState.Invalid, _controller.ModelState.ValidationState);
                Assert.Single(_controller.ModelState.Keys);
                Assert.Equal("Name", _controller.ModelState.Single().Key);

                var modelStateValue = _controller.ModelState.Values.Single();
                Assert.Single(modelStateValue.Errors);
                Assert.Equal("This topic already exists.", modelStateValue.Errors.Single().ErrorMessage);
            }
        }
    }
}