using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using MyProject.Server.Controllers;
using MyProject.Server.Models;
using Xunit;

namespace MyProject.Tests
{
    public class CitizenControllerTests
    {
        private AppDbContext GetDatabaseContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            var databaseContext = new AppDbContext(options);
            databaseContext.Database.EnsureCreated();
            return databaseContext;
        }

        [Fact]
        public async Task PostCitizen_ReturnsCreatedAtActionResult_WithCitizen()
        {
            // Arrange (Подготовка)
            var context = GetDatabaseContext();
            var mockConfig = new Mock<IConfiguration>();
            // Поскольку в вашем коде контекст создается через new внутри контроллера, 
            // для полноценного Unit-теста в будущем стоит использовать Dependency Injection.
            // Сейчас мы протестируем логику создания объекта.
            var controller = new CitizenController(mockConfig.Object); 
            var newCitizen = new Citizen { Id = 1, FirstName = "Иван", LastName = "Иванов" };

            // Act (Действие)
            var result = await controller.PostCitizen(newCitizen);

            // Assert (Проверка результата)
            var actionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var returnValue = Assert.IsType<Citizen>(actionResult.Value);
            Assert.Equal("Иван", returnValue.FirstName);
        }
    }
}