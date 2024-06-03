namespace Application.DTOs
{
    public class SharedDocumentDto
    {
        public int Id { get; set; }
        public int DocumentId { get; set; }
        public int UserId { get; set; }
        public int ShareLinkId { get; set; }
        public DateTime Expiration { get; set; }
    }
}