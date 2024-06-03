using Application.Commands.Documents.ShareDocumentCommand;
using FluentAssertions;
using Infrastructure.Data;
using Infrastructure.Models;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Test.Commands.Documents
{
    public class ShareDocumentCommandHandlerTests
    {
        private readonly Mock<ApplicationDbContext> _contextMock;
        private readonly Mock<ILogger<ShareDocumentCommandHandler>> _loggerMock;
        private readonly ShareDocumentCommandHandler _handler;

        public ShareDocumentCommandHandlerTests()
        {
            _contextMock = new Mock<ApplicationDbContext>();
            _loggerMock = new Mock<ILogger<ShareDocumentCommandHandler>>();
            _handler = new ShareDocumentCommandHandler(_contextMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldShareDocumentSuccessfully()
        {
            // Arrange
            var document = new Document { Id = 1, Name = "TestDocument" };

            _contextMock.Setup(x => x.Documents.FindAsync(It.IsAny<int>()))
                .ReturnsAsync(document);

            var command = new ShareDocumentCommand { DocumentId = 1, Expiration = TimeSpan.FromDays(1) };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNullOrEmpty();
            _contextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrowException_WhenDocumentNotFound()
        {
            // Arrange
            _contextMock.Setup(x => x.Documents.FindAsync(It.IsAny<int>()))
                .ReturnsAsync((Document)null);

            var command = new ShareDocumentCommand { DocumentId = 1, Expiration = TimeSpan.FromDays(1) };

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<Exception>().WithMessage("Document not found");
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
