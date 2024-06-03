using Application.DTOs;
using Application.Queries.Documents.GetDocumentByIdQuery;
using FluentAssertions;
using Infrastructure.Data;
using Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Test.Queries.Documents
{
    public class GetDocumentByIdQueryHandlerTests
    {
        private readonly ApplicationDbContext _context;
        private readonly Mock<ILogger<GetDocumentByIdQuery.GetDocumentByIdQueryHandler>> _loggerMock;
        private readonly GetDocumentByIdQuery.GetDocumentByIdQueryHandler _handler;

        public GetDocumentByIdQueryHandlerTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "DocumentLibraryDB")
                .Options;

            _context = new ApplicationDbContext(options);
            _loggerMock = new Mock<ILogger<GetDocumentByIdQuery.GetDocumentByIdQueryHandler>>();
            _handler = new GetDocumentByIdQuery.GetDocumentByIdQueryHandler(_context, _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnDocumentById()
        {
            // Arrange
            var document = new Document { Id = 1, Name = "TestDocument", FileType = "pdf", UploadDate = DateTime.UtcNow, DownloadCount = 0, PreviewImage = "preview.png" };
            _context.Documents.Add(document);
            await _context.SaveChangesAsync();

            var query = new GetDocumentByIdQuery { Id = 1 };

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(document.Id);
            result.Name.Should().Be(document.Name);
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.IsAny<object>(),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<object, Exception, string>>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_ShouldLogErrorWhenExceptionThrown()
        {
            // Arrange
            var query = new GetDocumentByIdQuery { Id = 1 };

            _context.Documents.Remove(await _context.Documents.FindAsync(1));
            await _context.SaveChangesAsync();

            // Act
            Func<Task> act = async () => await _handler.Handle(query, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>().WithMessage("Document not found");
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.IsAny<object>(),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<object, Exception, string>>()),
                Times.Once);
        }
    }
}
