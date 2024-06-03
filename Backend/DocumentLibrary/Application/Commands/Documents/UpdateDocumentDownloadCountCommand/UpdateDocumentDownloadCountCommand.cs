using Infrastructure.Data;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Commands.Documents.UpdateDocumentDownloadCountCommand
{
    /// <summary>
    /// Command to update the download count of a document.
    /// </summary>
    public class UpdateDocumentDownloadCountCommand : IRequest<Unit>
    {
        /// <summary>
        /// Gets or sets the document ID.
        /// </summary>
        public int Id { get; set; }
    }

    public class UpdateDocumentDownloadCountCommandHandler : IRequestHandler<UpdateDocumentDownloadCountCommand, Unit>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UpdateDocumentDownloadCountCommandHandler> _logger;

        public UpdateDocumentDownloadCountCommandHandler(ApplicationDbContext context, ILogger<UpdateDocumentDownloadCountCommandHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Unit> Handle(UpdateDocumentDownloadCountCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var document = await _context.Documents.FindAsync(request.Id);
                if (document != null)
                {
                    document.DownloadCount++;
                    await _context.SaveChangesAsync(cancellationToken);
                }
                else
                {
                    throw new ArgumentException("Document not found");
                }

                return Unit.Value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the download count");
                throw new Exception("An error occurred while updating the download count: " + ex.Message);
            }
        }
    }
}