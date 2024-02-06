namespace ReSplash.ViewModels
{
    public class ModalViewModel
    {
        public int PhotoId { get; set; }

        public string FileName { get; set; } = string.Empty;

        public DateTime PublishDate { get; set; }

        public string Description { get; set; } = string.Empty;

        public string CreatedBy { get; set; } = string.Empty;

        public string Category { get; set; } = string.Empty;

        public List<String> Tags { get; set; } = new();

    }
}
