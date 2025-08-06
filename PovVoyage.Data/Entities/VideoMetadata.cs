using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PovVoyage.Data.Entities
{
    public class VideoMetadata

    {
        public int Id { get; set; }
        public string VideoUrl { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime UploadDate { get; set; }
        public TimeSpan Duration { get; set; }
        public string ThumbnailUrl { get; set; }
        public int Likes { get; set; } = 0;
        public int Views { get; set; } = 0;
        public int Dislikes { get; set; } = 0;
    }
}
