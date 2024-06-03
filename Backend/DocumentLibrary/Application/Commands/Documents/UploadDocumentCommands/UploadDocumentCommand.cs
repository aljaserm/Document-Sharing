using Application.DTOs;
using Application.Enums;
using Application.Utils;
using Infrastructure.Data;
using Infrastructure.Models;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Commands.Documents.UploadDocumentCommands
{
    /// <summary>
    /// Command to upload a document.
    /// </summary>
    public class UploadDocumentCommand : IRequest<int>
    {
        /// <summary>
        /// Gets or sets the name of the document.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the file type of the document.
        /// </summary>
        public FileType FileType { get; set; }

        /// <summary>
        /// Gets or sets the content of the document.
        /// </summary>
        public byte[] Content { get; set; }
    }

    public class UploadDocumentCommandHandler : IRequestHandler<UploadDocumentCommand, int>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UploadDocumentCommandHandler> _logger;

        public UploadDocumentCommandHandler(ApplicationDbContext context, ILogger<UploadDocumentCommandHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<int> Handle(UploadDocumentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var document = new Document
                {
                    Name = request.Name,
                    FileType = request.FileType.ToString(),
                    UploadDate = DateTime.UtcNow,
                    Content = request.Content,
                    DownloadCount = 0,
                    PreviewImage = GeneratePreviewImage(request.Content),
                    Icon = FileTypeHelper.DetermineIcon(request.FileType.ToString()) // Convert FileType to string
                };

                // Ensure the directory exists
                string directoryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Documents");
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                // Generate a unique file path
                var filePath = Path.Combine(directoryPath, $"{Guid.NewGuid()}{Path.GetExtension(request.Name)}");

                // Save the file to the filesystem
                await File.WriteAllBytesAsync(filePath, request.Content, cancellationToken);

                document.FilePath = filePath;

                _context.Documents.Add(document);
                await _context.SaveChangesAsync(cancellationToken);

                return document.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while uploading the document");
                throw new Exception("An error occurred while uploading the document: " + ex.Message);
            }
        }

        private string GeneratePreviewImage(byte[] content)
        {
            // Implement logic to generate a preview image for the document
            return "previewImage.png";
        }
    }
}
