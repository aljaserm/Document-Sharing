using Application.Enums;
using Microsoft.AspNetCore.Http;

namespace Application.DTOs
{
    public class UploadDocumentDto
    {
        public string Name { get; set; }
        public FileType FileType { get; set; }
        public IFormFile Content { get; set; }
    }
}