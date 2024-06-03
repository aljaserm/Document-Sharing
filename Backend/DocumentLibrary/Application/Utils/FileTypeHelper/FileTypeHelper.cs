namespace Application.Utils
{
    public static class FileTypeHelper
    {
        public static string DetermineIcon(string fileType)
        {
            return fileType switch
            {
                "pdf" => "/icons/pdf-icon.png",
                "doc" => "/icons/word-icon.png",
                "docx" => "/icons/word-icon.png",
                "xls" => "/icons/excel-icon.png",
                "xlsx" => "/icons/excel-icon.png",
                "txt" => "/icons/text-icon.png",
                "jpg" => "/icons/image-icon.png",
                "png" => "/icons/image-icon.png",
                _ => "/icons/default-icon.png"
            };
        }
    }
}