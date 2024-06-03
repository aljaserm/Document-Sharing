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

namespace Application.Queries.Documents.GetDocumentsQuery
{
    public class GetDocumentsQuery : IRequest<List<DocumentDto>>
    {
    }

    public class GetDocumentsQueryHandler : IRequestHandler<GetDocumentsQuery, List<DocumentDto>>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<GetDocumentsQueryHandler> _logger;

        public GetDocumentsQueryHandler(ApplicationDbContext context, ILogger<GetDocumentsQueryHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<DocumentDto>> Handle(GetDocumentsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var documents = await _context.Documents
                    .Select(d => new DocumentDto
                    {
                        Id = d.Id,
                        Name = d.Name,
                        FileType = Enum.Parse<FileType>(d.FileType, true),
                        UploadDate = d.UploadDate,
                        DownloadCount = d.DownloadCount,
                        PreviewImage = d.PreviewImage,
                        Icon = FileTypeHelper.DetermineIcon(d.FileType),
                    })
                    .ToListAsync(cancellationToken);

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