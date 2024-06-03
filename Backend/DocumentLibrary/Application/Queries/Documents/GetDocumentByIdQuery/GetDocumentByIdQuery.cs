using Application.DTOs;
using Application.Enums;
using Application.Utils;
using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Queries.Documents.GetDocumentByIdQuery
{
    /// <summary>
    /// Query to get a document by its ID.
    /// </summary>
    public class GetDocumentByIdQuery : IRequest<DocumentDto>
    {
        /// <summary>
        /// Gets or sets the document ID.
        /// </summary>
        public int Id { get; set; }

        public class GetDocumentByIdQueryHandler : IRequestHandler<GetDocumentByIdQuery, DocumentDto>
        {
            private readonly ApplicationDbContext _context;
            private readonly ILogger<GetDocumentByIdQueryHandler> _logger;

            public GetDocumentByIdQueryHandler(ApplicationDbContext context, ILogger<GetDocumentByIdQueryHandler> logger)
            {
                _context = context;
                _logger = logger;
            }

            public async Task<DocumentDto> Handle(GetDocumentByIdQuery request, CancellationToken cancellationToken)
            {
                try
                {
                    var documentEntity = await _context.Documents
                        .Where(d => d.Id == request.Id)
                        .FirstOrDefaultAsync(cancellationToken);

                    if (documentEntity == null)
                    {
                        throw new ArgumentException("Document not found");
                    }

                    var documentDto = new DocumentDto
                    {
                        Id = documentEntity.Id,
                        Name = documentEntity.Name,
                        FileType = Enum.TryParse<FileType>(documentEntity.FileType, true, out var fileType) ? fileType : FileType.default_icon,
                        UploadDate = documentEntity.UploadDate,
                        DownloadCount = documentEntity.DownloadCount,
                        PreviewImage = documentEntity.PreviewImage,
                        Icon = FileTypeHelper.DetermineIcon(documentEntity.FileType)
                    };

                    return documentDto;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while retrieving the document");
                    throw new Exception("An error occurred while retrieving the document: " + ex.Message);
                }
            }
        }
    }
}
