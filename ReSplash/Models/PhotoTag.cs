using Microsoft.EntityFrameworkCore;

namespace ReSplash.Models
{
    [PrimaryKey(nameof(PhotoId), nameof(TagId))]
    public class PhotoTag
    {
        public int PhotoId { get; set; }

        public int TagId { get; set; }

        public Photo Photo { get; set; } = new();

        public Tag Tag { get; set; } = new();

    }
}
