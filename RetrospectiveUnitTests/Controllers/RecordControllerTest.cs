using System;
using Xunit;
using Retrospective.Controllers;
using Retrospective.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace RetrospectiveUnitTests.Controllers
{
    public class RecordControllerTest
    {
        /// <summary>
        /// Сценарий тестирования контроллера записей, при котором изначально в БД нет ни одной записи.
        /// </summary>
        public class NoRecords
        {
            private const int _subjectId = 1;
            private const string _subjectName = "Test";

            private readonly AppDbContext _dbContext;
            private readonly RecordController _controller;

            public NoRecords()
            {
                _dbContext = new AppDbContext(InMemoryDbContextOptionsFactory.Create<AppDbContext>());
                _dbContext.Add(new Subject { Id = _subjectId, Name = _subjectName });
                _dbContext.SaveChanges();

                _controller = new RecordController(_dbContext, new MyStringLocalizer(), null, null);
            }

            [Fact]
            public async Task Index()
            {
                var result = await _controller.Index(_subjectId, _subjectName);

                Assert.IsType<ViewResult>(result);
                var viewResult = result as ViewResult;
                Assert.Equal("Index", viewResult.ViewName);

                Assert.IsType<RecordsViewModel>(viewResult.Model);
                var model = viewResult.Model as RecordsViewModel;
                Assert.Equal(3, model.RecordTypes.Count);
                Assert.Equal(_subjectName, model.SubjectName);
                Assert.Single(model.Days); // Даже если записей нет, один день (текущий) всегда есть.
                Assert.Empty(model.Records);

                Assert.True(_controller.ModelState.IsValid);
            }

            [Theory]
            [InlineData("01.02.2019")]
            [InlineData("05.12.2018")]
            public async Task GetRecordsData(string currentday)
            {
                var result = await _controller.GetRecordsData(_subjectId, currentday);

                Assert.IsType<PartialViewResult>(result);
                var viewResult = result as PartialViewResult;
                Assert.Equal("_Data", viewResult.ViewName);

                Assert.IsType<RecordsViewModel>(viewResult.Model);
                var model = viewResult.Model as RecordsViewModel;
                Assert.Empty(model.Records);
                Assert.Equal(3, model.RecordTypes.Count);
                Assert.Equal(currentday, model.CurrentDay);

                Assert.True(_controller.ModelState.IsValid);
            }

            [Theory]
            [InlineData("18.05.2017", "User1", 0, "Text1")]
            [InlineData("10.12.2019", "User2", 5, "Test")]
            public async Task AddRecord(string currentDay, string nickname, byte recordType, string text)
            {
                var result = await _controller.AddRecord(_subjectId, currentDay, nickname, recordType, text);
                
                Assert.IsType<PartialViewResult>(result);
                var viewResult = result as PartialViewResult;
                Assert.Equal("_Data", viewResult.ViewName);

                Assert.IsType<RecordsViewModel>(viewResult.Model);
                var model = viewResult.Model as RecordsViewModel;
                Assert.Equal(currentDay, model.CurrentDay);
                Assert.Equal(3, model.RecordTypes.Count);
                Assert.Equal(text, model.NewMessage);
                Assert.Equal(nickname, model.Nickname);
                Assert.Single(model.Records);

                var record = model.Records[0];
                Assert.Equal(nickname, record.Author);
                Assert.Equal(recordType, record.RecordType);
                Assert.Equal(_subjectId, record.SubjectId);
                Assert.Equal(text, record.Text);

                Assert.True(_controller.ModelState.IsValid);
            }

            /// <summary>
            /// Проверка обработки отсутствия псевдонима при создании записи.
            /// </summary>
            /// <param name="currentDay">Дата, за которую создаётся запись.</param>
            /// <param name="recordType">Тип создаваемой записи.</param>
            /// <param name="text">Текст записи.</param>
            [Theory]
            [InlineData("10.09.2016", 1, "Test1")]
            [InlineData("15.12.2015", 7, "Test2")]
            public async Task AddRecord_WithoutNickname(string currentDay, byte recordType, string text)
            {
                var result = await _controller.AddRecord(_subjectId, currentDay, null, recordType, text);

                Assert.IsType<PartialViewResult>(result);
                var viewResult = result as PartialViewResult;
                Assert.Equal("_Data", viewResult.ViewName);

                Assert.IsType<RecordsViewModel>(viewResult.Model);
                var model = viewResult.Model as RecordsViewModel;
                Assert.Equal(currentDay, model.CurrentDay);
                Assert.Equal(3, model.RecordTypes.Count);
                Assert.Equal(text, model.NewMessage);
                Assert.Null(model.Nickname);
                Assert.Empty(model.Records);

                Assert.False(_controller.ModelState.IsValid);
                Assert.Single(_controller.ModelState.Keys);
                Assert.Equal("Nickname", _controller.ModelState.Single().Key);
                var modelState = _controller.ModelState.Values.Single();
                Assert.Single(modelState.Errors);
                Assert.Equal("Please enter your nickname.", modelState.Errors.Single().ErrorMessage);
            }

            /// <summary>
            /// Проверка обработки отсутствия текста при создании записи.
            /// </summary>
            /// <param name="currentDay">Дата, за которую создаётся запись.</param>
            /// <param name="nickname">Псевдоним.</param>
            /// <param name="recordType">Тип создаваемой записи.</param>
            [Theory]
            [InlineData("08.11.2011", "User7", 2)]
            [InlineData("01.01.2019", "User8", 3)]
            public async Task AddRecord_WithoutText(string currentDay, string nickname, byte recordType)
            {
                var result = await _controller.AddRecord(_subjectId, currentDay, nickname, recordType, null);

                Assert.IsType<PartialViewResult>(result);
                var viewResult = result as PartialViewResult;
                Assert.Equal("_Data", viewResult.ViewName);

                Assert.IsType<RecordsViewModel>(viewResult.Model);
                var model = viewResult.Model as RecordsViewModel;
                Assert.Equal(currentDay, model.CurrentDay);
                Assert.Equal(3, model.RecordTypes.Count);
                Assert.Null(model.NewMessage);
                Assert.Equal(nickname, model.Nickname);
                Assert.Empty(model.Records);

                Assert.False(_controller.ModelState.IsValid);
                Assert.Single(_controller.ModelState.Keys);
                Assert.Equal("NewMessage", _controller.ModelState.Single().Key);
                var modelState = _controller.ModelState.Values.Single();
                Assert.Single(modelState.Errors);
                Assert.Equal("Write your wish.", modelState.Errors.Single().ErrorMessage);
            }
        }

        public class RecordsExist
        {
            [Fact]
            public void Index()
            {
                throw new NotImplementedException();
            }

            [Fact]
            public void GetRecordsData()
            {
                throw new NotImplementedException();
            }

            [Fact]
            public void AddRecord()
            {
                throw new NotImplementedException();
            }
        }
    }
}