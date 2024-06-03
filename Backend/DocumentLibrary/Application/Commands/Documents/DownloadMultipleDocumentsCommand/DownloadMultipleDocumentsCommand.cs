using Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Commands.Documents.DownloadMultipleDocumentsCommand
{
    /// <summary>
    /// Command to download multiple documents.
    /// </summary>
    public class DownloadMultipleDocumentsCommand : IRequest<DownloadMultipleDocumentsResult>
    {
        /// <summary>
        /// Gets or sets the list of document IDs.
        /// </summary>
        public List<int> DocumentIds { get; set; }
    }

    /// <summary>
    /// Result of downloading multiple documents.
    /// </summary>
    public class DownloadMultipleDocumentsResult
    {
        /// <summary>
        /// Gets or sets the file content of the zipped documents.
        /// </summary>
        public byte[] FileContent { get; set; }

        /// <summary>
        /// Gets or sets the file name of the zipped documents.
        /// </summary>
        public string FileName { get; set; }
    }

    public class DownloadMultipleDocumentsCommandHandler : IRequestHandler<DownloadMultipleDocumentsCommand, DownloadMultipleDocumentsResult>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DownloadMultipleDocumentsCommandHandler> _logger;

        public DownloadMultipleDocumentsCommandHandler(ApplicationDbContext context, ILogger<DownloadMultipleDocumentsCommandHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<DownloadMultipleDocumentsResult> Handle(DownloadMultipleDocumentsCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var documents = await _context.Documents
                    .Where(d => request.DocumentIds.Contains(d.Id))
                    .ToListAsync(cancellationToken);

                if (documents == null || !documents.Any())
                {
                    throw new ArgumentException("Documents not found");
                }

                var tempFileName = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".zip");
                using (var archive = ZipFile.Open(tempFileName, ZipArchiveMode.Create))
                {
                    foreach (var document in documents)
                    {
                        var fileContent = await File.ReadAllBytesAsync(document.FilePath, cancellationToken);
                        var entry = archive.CreateEntry($"{document.Name}.{document.FileType}");
                        using (var entryStream = entry.Open())
                        {
                            await entryStream.WriteAsync(fileContent, 0, fileContent.Length, cancellationToken);
                        }

                        document.DownloadCount++;
                    }

                    await _context.SaveChangesAsync(cancellationToken);
                }

                var zippedFileContent = await File.ReadAllBytesAsync(tempFileName, cancellationToken);
                File.Delete(tempFileName);

                return new DownloadMultipleDocumentsResult
                {
                    FileContent = zippedFileContent,
                    FileName = "documents.zip"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while downloading the documents");
                throw new Exception("An error occurred while downloading the documents: " + ex.Message);
            }
        }
    }
}
