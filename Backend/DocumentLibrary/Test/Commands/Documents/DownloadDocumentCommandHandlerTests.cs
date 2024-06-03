using Application.Commands.Documents.DownloadDocumentCommand;
using FluentAssertions;
using Infrastructure.Data;
using Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Test.Commands.Documents
{
    public class DownloadDocumentCommandHandlerTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly Mock<ILogger<DownloadDocumentCommandHandler>> _loggerMock;
        private readonly DownloadDocumentCommandHandler _handler;
        private readonly string _testFilePath = "test.pdf";

        public DownloadDocumentCommandHandlerTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "DocumentLibraryDB")
                .Options;

            _context = new ApplicationDbContext(options);
            _loggerMock = new Mock<ILogger<DownloadDocumentCommandHandler>>();
            _handler = new DownloadDocumentCommandHandler(_context, _loggerMock.Object);

            // Create a test file
            File.WriteAllText(_testFilePath, "This is a test file content");
        }

        [Fact]
        public async Task Handle_ShouldDownloadDocumentSuccessfully()
        {
            // Arrange
            var document = new Document { Id = 1, Name = "TestDocument", FileType = "pdf", FilePath = _testFilePath, DownloadCount = 0 };
            _context.Documents.Add(document);
            await _context.SaveChangesAsync();

            var command = new DownloadDocumentCommand { Id = 1 };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.FileName.Should().Be(document.Name);
            _context.Documents.Find(1).DownloadCount.Should().Be(1);
        }

        [Fact]
        public async Task Handle_ShouldThrowException_WhenDocumentNotFound()
        {
            // Arrange
            var command = new DownloadDocumentCommand { Id = 999 };

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

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

        public void Dispose()
        {
            // Clean up the test file
            if (File.Exists(_testFilePath))
            {
                File.Delete(_testFilePath);
            }
        }
    }
}
