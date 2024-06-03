namespace Application.DTOs
{
    public class ShareLinkDto
    {
        public int Id { get; set; }
        public string Link { get; set; }
        public DateTime Expiration { get; set; }
    }
}