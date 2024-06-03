using System;
using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Models
{
    public class ShareLink
    {
        [Key]
        public int Id { get; set; }
        public int DocumentId { get; set; }
        public string Link { get; set; }
        public DateTime Expiration { get; set; }

        public Document Document { get; set; }
    }
}