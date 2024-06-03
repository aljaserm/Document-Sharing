using Application.Commands.Documents.UploadDocumentCommands;
using FluentAssertions;
using Infrastructure.Data;
using Infrastructure.Models;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Test.Commands.Documents
{
    public class UploadDocumentCommandHandlerTests
    {
        private readonly Mock<ApplicationDbContext> _contextMock;
        private readonly Mock<ILogger<UploadDocumentCommandHandler>> _loggerMock;
        private readonly UploadDocumentCommandHandler _handler;

        public UploadDocumentCommandHandlerTests()
        {
            _contextMock = new Mock<ApplicationDbContext>();
            _loggerMock = new Mock<ILogger<UploadDocumentCommandHandler>>();
            _handler = new UploadDocumentCommandHandler(_contextMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldUploadDocumentSuccessfully()
        {
            // Arrange
            var command = new UploadDocumentCommand
            {
                Name = "TestDocument",
                FileType = "pdf",
                Content = new byte[] { 0x01, 0x02, 0x03 }
            };

            var filePath = Path.Combine("Documents", $"{Guid.NewGuid()}{Path.GetExtension(command.Name)}");

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeGreaterThan(0);
            _contextMock.Verify(x => x.Documents.Add(It.IsAny<Document>()), Times.Once);
            _contextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
            File.Exists(filePath).Should().BeTrue();
            File.Delete(filePath); // Clean up the test file
        }

        [Fact]
        public async Task Handle_ShouldLogErrorWhenExceptionThrown()
        {
            // Arrange
            var command = new UploadDocumentCommand
            {
                Name = "TestDocument",
                FileType = "pdf",
                Content = new byte[] { 0x01, 0x02, 0x03 }
            };

            _contextMock.Setup(x => x.Documents.Add(It.IsAny<Document>()))
                .Throws(new Exception("Test exception"));

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<Exception>().WithMessage("An error occurred while uploading the document: Test exception");
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
