using Application.Commands.Documents.DownloadDocumentCommand;
using Application.Commands.Documents.GenerateShareLinkCommand;
using Application.Commands.Documents.UploadDocumentCommands;
using Application.DTOs;
using Application.Queries.Documents.GetDocumentByIdQuery;
using Application.Queries.Documents.GetDocumentsByIdsQuery;
using Application.Queries.Documents.GetDocumentsQuery;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Application.Commands.Documents.DownloadMultipleDocumentsCommand;
using Application.Queries;
using Application.Queries.Documents.GetDocumentByShareLinkQuery;

namespace Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public DocumentsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Gets all documents.
        /// </summary>
        /// <returns>A list of documents.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DocumentDto>>> GetDocuments()
        {
            var query = new GetDocumentsQuery();
            var documents = await _mediator.Send(query);
            return Ok(documents);
        }

        /// <summary>
        /// Uploads a document.
        /// </summary>
        /// <param name="uploadDocumentDto">The document details.</param>
        /// <returns>The uploaded document.</returns>
        [HttpPost("upload")]
        public async Task<ActionResult<DocumentDto>> UploadDocument([FromForm] UploadDocumentDto uploadDocumentDto)
        {
            try
            {
                using (var memoryStream = new MemoryStream())
                {
                    await uploadDocumentDto.Content.CopyToAsync(memoryStream);
                    var fileBytes = memoryStream.ToArray();
                    var command = new UploadDocumentCommand
                    {
                        Name = uploadDocumentDto.Name,
                        FileType = uploadDocumentDto.FileType,
                        Content = fileBytes
                    };
                    var documentId = await _mediator.Send(command);

                    var document = await _mediator.Send(new GetDocumentByIdQuery { Id = documentId });

                    return Ok(document);
                }
            }
            catch (FormatException)
            {
                return BadRequest(new { message = "Invalid file format" });
            }
        }

        /// <summary>
        /// Uploads multiple documents.
        /// </summary>
        /// <param name="uploadDocumentDtos">The list of document details.</param>
        /// <returns>A list of uploaded documents.</returns>
        [HttpPost("upload/multiple")]
        public async Task<ActionResult<IEnumerable<DocumentDto>>> UploadMultipleDocuments([FromBody] List<UploadDocumentDto> uploadDocumentDtos)
        {
            var documentIds = new List<int>();
            try
            {
                foreach (var uploadDocumentDto in uploadDocumentDtos)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await uploadDocumentDto.Content.CopyToAsync(memoryStream);
                        var fileBytes = memoryStream.ToArray();
                        var command = new UploadDocumentCommand
                        {
                            Name = uploadDocumentDto.Name,
                            FileType = uploadDocumentDto.FileType,
                            Content = fileBytes
                        };
                        var documentId = await _mediator.Send(command);

                        var document = await _mediator.Send(new GetDocumentByIdQuery { Id = documentId });

                        documentIds.Add(documentId);
                    }
                }

                var documents = await _mediator.Send(new GetDocumentsByIdsQuery { Ids = documentIds });

                return Ok(documents);
            }
            catch (FormatException)
            {
                return BadRequest(new { message = "Invalid file format" });
            }
        }

        /// <summary>
        /// Downloads a document by ID.
        /// </summary>
        /// <param name="id">The document ID.</param>
        /// <returns>The document file.</returns>
        [HttpGet("download/{id}")]
        public async Task<IActionResult> DownloadDocument(int id)
        {
            var command = new DownloadDocumentCommand { Id = id };
            var result = await _mediator.Send(command);

            if (result == null)
                return NotFound();

            var contentDisposition = new System.Net.Mime.ContentDisposition
            {
                FileName = result.FileName,
                Inline = false
            };
            Response.Headers.Add("Content-Disposition", contentDisposition.ToString());

            return File(result.FileContent, "application/octet-stream");
        }

        /// <summary>
        /// Downloads multiple documents by their IDs.
        /// </summary>
        /// <param name="command">The list of document IDs.</param>
        /// <returns>The zipped documents file.</returns>
        [HttpPost("download/multiple")]
        public async Task<IActionResult> DownloadMultipleDocuments([FromBody] DownloadMultipleDocumentsCommand command)
        {
            var result = await _mediator.Send(command);

            if (result == null)
                return NotFound();

            var contentDisposition = new System.Net.Mime.ContentDisposition
            {
                FileName = result.FileName,
                Inline = false
            };
            Response.Headers.Add("Content-Disposition", contentDisposition.ToString());

            return File(result.FileContent, "application/zip");
        }

        /// <summary>
        /// Generates a share link for a document.
        /// </summary>
        /// <param name="command">The share link details.</param>
        /// <returns>The generated share link.</returns>
        [HttpPost("share")]
        public async Task<IActionResult> GenerateShareLink([FromForm] GenerateShareLinkCommand command)
        {
            var link = await _mediator.Send(command);
            return Ok(link);
        }

        /// <summary>
        /// Gets a document by its share link.
        /// </summary>
        /// <param name="link">The share link.</param>
        /// <returns>The document associated with the share link.</returns>
        [HttpGet("shared/{link}")]
        public async Task<IActionResult> GetDocumentByShareLink(string link)
        {
            var query = new GetDocumentByShareLinkQuery { ShareLink = link };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Retrieves a document by its ID.
        /// </summary>
        /// <param name="id">The ID of the document to retrieve.</param>
        /// <returns>The document with the specified ID.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDocumentById(int id)
        {
            var query = new GetDocumentByIdQuery { Id = id };
            var document = await _mediator.Send(query);

            if (document == null)
            {
                return NotFound();
            }

            return Ok(document);
        }

    }
}
