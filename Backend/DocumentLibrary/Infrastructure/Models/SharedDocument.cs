using System;
using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Models
{
    public class SharedDocument
    {
        [Key]
        public int Id { get; set; }
        public int DocumentId { get; set; }
        public int ShareLinkId { get; set; }
        public DateTime Expiration { get; set; }

        public Document Document { get; set; }
        public ShareLink ShareLink { get; set; }
    }
}