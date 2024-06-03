using System;
using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Models
{
    public class Document
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        [Required]
        [MaxLength(50)]
        public string FileType { get; set; }
        public DateTime UploadDate { get; set; }
        public int DownloadCount { get; set; }
        public string FilePath { get; set; }
        public byte[] Content { get; set; }
        public string PreviewImage { get; set; }
        public string Icon { get; set; }
    }
}