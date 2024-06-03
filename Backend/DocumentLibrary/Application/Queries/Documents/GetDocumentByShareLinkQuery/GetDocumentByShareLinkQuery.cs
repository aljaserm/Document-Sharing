using Application.DTOs;
using Application.Enums;
using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Queries.Documents.GetDocumentByShareLinkQuery
{
    /// <summary>
    /// Query to get a document by its share link.
    /// </summary>
    public class GetDocumentByShareLinkQuery : IRequest<DocumentDto>
    {
        /// <summary>
        /// Gets or sets the share link.
        /// </summary>
        public string ShareLink { get; set; }
    }

    public class GetDocumentByShareLinkQueryHandler : IRequestHandler<GetDocumentByShareLinkQuery, DocumentDto>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<GetDocumentByShareLinkQueryHandler> _logger;

        public GetDocumentByShareLinkQueryHandler(ApplicationDbContext context, ILogger<GetDocumentByShareLinkQueryHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetDocumentByShareLinkQuery request.
        /// </summary>
        /// <param name="request">The request containing the share link.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The document DTO.</returns>
        public async Task<DocumentDto> Handle(GetDocumentByShareLinkQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling GetDocumentByShareLinkQuery for link {Link}", request.ShareLink);

            var shareLink = await _context.ShareLinks
                .Include(sl => sl.Document)
                .FirstOrDefaultAsync(sl => sl.Link == request.ShareLink, cancellationToken);

            if (shareLink == null || shareLink.Expiration < DateTime.UtcNow)
            {
                _logger.LogWarning("Share link is invalid or has expired");
                throw new ArgumentException("Share link is invalid or has expired");
            }

            var document = shareLink.Document;

            // Map string to FileType enum
            FileType fileTypeEnum;
            if (!Enum.TryParse(document.FileType, true, out fileTypeEnum))
            {
                _logger.LogWarning("Invalid file type");
                throw new ArgumentException("Invalid file type");
            }

            return new DocumentDto
            {
                Id = document.Id,
                Name = document.Name,
                FileType = fileTypeEnum,
                UploadDate = document.UploadDate,
                DownloadCount = document.DownloadCount,
                PreviewImage = document.PreviewImage,
                Icon = document.Icon,
            };
        }
    }
}
