using Application.DTOs;
using Application.Enums;
using Infrastructure.Data;
using Infrastructure.Models;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Commands.Documents.GenerateShareLinkCommand
{
    /// <summary>
    /// Command to generate a share link for a document.
    /// </summary>
    public class GenerateShareLinkCommand : IRequest<ShareLinkDto>
    {
        /// <summary>
        /// Gets or sets the document ID.
        /// </summary>
        public int DocumentId { get; set; }

        /// <summary>
        /// Gets or sets the expiration duration.
        /// </summary>
        public int Duration { get; set; }

        /// <summary>
        /// Gets or sets the expiration unit.
        /// </summary>
        public TimeUnit Unit { get; set; }
    }

    public class GenerateShareLinkCommandHandler : IRequestHandler<GenerateShareLinkCommand, ShareLinkDto>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<GenerateShareLinkCommandHandler> _logger;

        public GenerateShareLinkCommandHandler(ApplicationDbContext context, ILogger<GenerateShareLinkCommandHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ShareLinkDto> Handle(GenerateShareLinkCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var document = await _context.Documents.FindAsync(request.DocumentId);

                if (document == null)
                {
                    throw new ArgumentException("Document not found");
                }

                TimeSpan expirationTime = request.Unit switch
                {
                    TimeUnit.Minutes => TimeSpan.FromMinutes(request.Duration),
                    TimeUnit.Hours => TimeSpan.FromHours(request.Duration),
                    TimeUnit.Days => TimeSpan.FromDays(request.Duration),
                    TimeUnit.Weeks => TimeSpan.FromDays(7 * request.Duration),
                    TimeUnit.Months => TimeSpan.FromDays(30 * request.Duration),
                    TimeUnit.Years => TimeSpan.FromDays(365 * request.Duration),
                    _ => throw new ArgumentOutOfRangeException()
                };

                var shareLink = new ShareLink
                {
                    DocumentId = document.Id,
                    Expiration = DateTime.UtcNow.Add(expirationTime),
                    Link = GenerateLink(document.Id)
                };

                _context.ShareLinks.Add(shareLink);
                await _context.SaveChangesAsync(cancellationToken);

                var sharedDocument = new SharedDocument
                {
                    DocumentId = document.Id,
                    ShareLinkId = shareLink.Id,
                    Expiration = shareLink.Expiration
                };

                _context.SharedDocuments.Add(sharedDocument);
                await _context.SaveChangesAsync(cancellationToken);

                return new ShareLinkDto
                {
                    Id = shareLink.Id,
                    Link = shareLink.Link,
                    Expiration = shareLink.Expiration
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while generating the share link");
                throw new Exception("An error occurred while generating the share link: " + ex.Message);
            }
        }

        private string GenerateLink(int documentId)
        {
            // Implement link generation logic // it is pointing to the frontend localhost
            return $"https://localhost:3000/shared/{documentId}/{Guid.NewGuid()}";
        }
    }
}
