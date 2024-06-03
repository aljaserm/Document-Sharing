using Application.Commands.Documents.GenerateShareLinkCommand;
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
    public class GenerateShareLinkCommandHandlerTests
    {
        private readonly Mock<ApplicationDbContext> _contextMock;
        private readonly Mock<ILogger<GenerateShareLinkCommandHandler>> _loggerMock;
        private readonly GenerateShareLinkCommandHandler _handler;

        public GenerateShareLinkCommandHandlerTests()
        {
            _contextMock = new Mock<ApplicationDbContext>();
            _loggerMock = new Mock<ILogger<GenerateShareLinkCommandHandler>>();
            _handler = new GenerateShareLinkCommandHandler(_contextMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldGenerateShareLinkSuccessfully()
        {
            // Arrange
            var document = new Document { Id = 1, Name = "TestDocument" };

            _contextMock.Setup(x => x.Documents.FindAsync(It.IsAny<int>()))
                .ReturnsAsync(document);

            var command = new GenerateShareLinkCommand { DocumentId = 1, Expiration = TimeSpan.FromDays(1) };

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

            var command = new GenerateShareLinkCommand { DocumentId = 1, Expiration = TimeSpan.FromDays(1) };

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
