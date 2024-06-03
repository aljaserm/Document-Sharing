using Application.Commands.Documents.UpdateDocumentDownloadCountCommand;
using FluentAssertions;
using Infrastructure.Data;
using Infrastructure.Models;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Test.Commands.Documents
{
    public class UpdateDocumentDownloadCountCommandHandlerTests
    {
        private readonly Mock<ApplicationDbContext> _contextMock;
        private readonly Mock<ILogger<UpdateDocumentDownloadCountCommandHandler>> _loggerMock;
        private readonly UpdateDocumentDownloadCountCommandHandler _handler;

        public UpdateDocumentDownloadCountCommandHandlerTests()
        {
            _contextMock = new Mock<ApplicationDbContext>();
            _loggerMock = new Mock<ILogger<UpdateDocumentDownloadCountCommandHandler>>();
            _handler = new UpdateDocumentDownloadCountCommandHandler(_contextMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldUpdateDownloadCountSuccessfully()
        {
            // Arrange
            var document = new Document { Id = 1, Name = "TestDocument", DownloadCount = 0 };

            _contextMock.Setup(x => x.Documents.FindAsync(It.IsAny<int>()))
                .ReturnsAsync(document);

            var command = new UpdateDocumentDownloadCountCommand { Id = 1 };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            document.DownloadCount.Should().Be(1);
            result.Should().Be(Unit.Value);
            _contextMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_ShouldThrowException_WhenDocumentNotFound()
        {
            // Arrange
            _contextMock.Setup(x => x.Documents.FindAsync(It.IsAny<int>()))
                .ReturnsAsync((Document)null);

            var command = new UpdateDocumentDownloadCountCommand { Id = 1 };

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
