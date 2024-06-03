using Application.DTOs;
using Application.Enums;
using Application.Utils;
using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Queries.Documents.GetDocumentsByIdsQuery
{
    /// <summary>
    /// Query to get documents by their IDs.
    /// </summary>
    public class GetDocumentsByIdsQuery : IRequest<List<DocumentDto>>
    {
        /// <summary>
        /// Gets or sets the list of document IDs.
        /// </summary>
        public List<int> Ids { get; set; }

        public class GetDocumentsByIdsQueryHandler : IRequestHandler<GetDocumentsByIdsQuery, List<DocumentDto>>
        {
            private readonly ApplicationDbContext _context;
            private readonly ILogger<GetDocumentsByIdsQueryHandler> _logger;

            public GetDocumentsByIdsQueryHandler(ApplicationDbContext context, ILogger<GetDocumentsByIdsQueryHandler> logger)
            {
                _context = context;
                _logger = logger;
            }

            public async Task<List<DocumentDto>> Handle(GetDocumentsByIdsQuery request, CancellationToken cancellationToken)
            {
                try
                {
                    var documentEntities = await _context.Documents
                        .Where(d => request.Ids.Contains(d.Id))
                        .ToListAsync(cancellationToken);

                    var documents = documentEntities.Select(d => new DocumentDto
                    {
                        Id = d.Id,
                        Name = d.Name,
                        FileType = Enum.TryParse<FileType>(d.FileType, true, out var fileType) ? fileType : FileType.default_icon,
                        UploadDate = d.UploadDate,
                        DownloadCount = d.DownloadCount,
                        PreviewImage = d.PreviewImage,
                        Icon = FileTypeHelper.DetermineIcon(d.FileType)
                    }).ToList();

                    return documents;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while retrieving the documents");
                    throw new Exception("An error occurred while retrieving the documents: " + ex.Message);
                }
            }
        }
    }
}
