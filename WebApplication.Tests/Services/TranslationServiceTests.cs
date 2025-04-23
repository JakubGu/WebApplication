using Moq;
using WebApplication.Models.DTOs;
using WebApplication.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using System.Net.Http;
using System.Text;
using System.Linq;
using WebApplication.Models;
using System.Threading;
using Moq.Protected;
using System.Data.Entity.Infrastructure;

namespace WebApplication.Tests.Services
{
    public class TranslationServiceTests
    {
        private readonly Mock<AppDbContext> _contextMock;
        private readonly Mock<HttpMessageHandler> _handlerMock;
        private readonly HttpClient _httpClient;

        public TranslationServiceTests()
        {
            // Mockowanie HttpClient
            _handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            _handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Content = new StringContent("{\"contents\": {\"translated\": \"Hola\"}}", Encoding.UTF8, "application/json")
                });

            _httpClient = new HttpClient(_handlerMock.Object);

            // Mockowanie DbContext
            _contextMock = new Mock<AppDbContext>();
        }

        [Fact]
        public async Task GetAllTranslationsAsync_ReturnsTranslations()
        {
            // Arrange - symulujemy dane
            var translations = new List<TranslationDto>
            {
                new TranslationDto { SourceText = "Hello", TranslatedText = "Hola" },
                new TranslationDto { SourceText = "Goodbye", TranslatedText = "Adiós" }
            };

            // Mockowanie DbRawSqlQuery
            var mockDbRawSqlQuery = new Mock<DbRawSqlQuery<TranslationDto>>();
            mockDbRawSqlQuery.Setup(q => q.ToListAsync()).ReturnsAsync(translations);

            // Mockowanie bazy danych
            _contextMock.Setup(c => c.Database.SqlQuery<TranslationDto>(It.IsAny<string>()))
                .Returns(mockDbRawSqlQuery.Object);

            // Utworzenie serwisu z mockowanym kontekstem i HttpClient
            var service = new TranslationService(_contextMock.Object, _httpClient);

            // Act
            var result = await service.GetAllTranslationsAsync();

            // Assert
            Assert.Equal(2, result.Count());
            Assert.Equal("Hola", result.First().TranslatedText);
        }

        [Fact]
        public async Task AddTranslationFromExternalApiAsync_CallsApiAndInserts()
        {
            // Arrange
            var translationInputDto = new TranslationInputDto
            {
                SourceText = "Hello",
                TranslationLanguage = "yoda"
            };
            var userId = "user-123";

            // Mockowanie odpowiedzi z API
            _handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Content = new StringContent("{\"contents\": {\"translated\": \"Hola\"}}", Encoding.UTF8, "application/json")
                });

            // Mockowanie DbContext ExecuteSqlCommandAsync
            _contextMock.Setup(c => c.Database.ExecuteSqlCommandAsync(It.IsAny<string>(), It.IsAny<object[]>()))
                .ReturnsAsync(1);

            // Utworzenie serwisu z mockowanym kontekstem i HttpClient
            var service = new TranslationService(_contextMock.Object, _httpClient);

            // Act
            await service.AddTranslationFromExternalApiAsync(translationInputDto, userId);

            // Assert
            _contextMock.Verify(c => c.Database.ExecuteSqlCommandAsync(It.Is<string>(s => s.Contains("InsertTranslation"))), Times.Once);
        }
    }
}
