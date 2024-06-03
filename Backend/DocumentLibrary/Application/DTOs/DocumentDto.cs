using Application.Enums;

namespace Application.DTOs
{
    public class DocumentDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public FileType FileType { get; set; }
        public DateTime UploadDate { get; set; }
        public int DownloadCount { get; set; }
        public string PreviewImage { get; set; }
        public string Icon { get; set; }
    }
}