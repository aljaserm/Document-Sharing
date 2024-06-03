using Infrastructure.Data;
using MediatR;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Application.Commands.Documents.DownloadDocumentCommand
{
    /// <summary>
    /// Command to download a document.
    /// </summary>
    public class DownloadDocumentCommand : IRequest<DownloadDocumentResult>
    {
        /// <summary>
        /// Gets or sets the document ID.
        /// </summary>
        public int Id { get; set; }
    }

    /// <summary>
    /// Result of downloading a document.
    /// </summary>
    public class DownloadDocumentResult
    {
        /// <summary>
        /// Gets or sets the file content of the document.
        /// </summary>
        public byte[] FileContent { get; set; }

        /// <summary>
        /// Gets or sets the file name of the document.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Gets or sets the file type of the document.
        /// </summary>
        public string FileType { get; set; }
    }

    public class DownloadDocumentCommandHandler : IRequestHandler<DownloadDocumentCommand, DownloadDocumentResult>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DownloadDocumentCommandHandler> _logger;

        public DownloadDocumentCommandHandler(ApplicationDbContext context, ILogger<DownloadDocumentCommandHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<DownloadDocumentResult> Handle(DownloadDocumentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var document = await _context.Documents.FindAsync(request.Id);
                if (document == null)
                {
                    throw new ArgumentException("Document not found");
                }

                document.DownloadCount++;
                await _context.SaveChangesAsync(cancellationToken);

                var fileContent = await File.ReadAllBytesAsync(document.FilePath);

                return new DownloadDocumentResult
                {
                    FileContent = fileContent,
                    FileName = $"{document.Name}.{document.FileType}", 
                    FileType = document.FileType
                };
            }
            catch (Exception ex)
            {
                // Log the exception
                _logger.LogError(ex, "An error occurred while downloading the document");
                throw new Exception("An error occurred while downloading the document: " + ex.Message);
            }
        }
    }
}
