using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ResourceCraft.Model
{
    public class Video : FirebaseEntity
    {
        public string ContentKey { get; set; }

        private string _youTubeId;
        public string YouTubeId
        {
            get => _youTubeId;
            set => SetField(ref _youTubeId, value);
        }

        private string _title;
        public string Title
        {
            get => _title;
            set => SetField(ref _title, value);
        }

        public Video() { }

        public Video(string youTubeID)
        {
            YouTubeId = youTubeID;
        }

        [NotMapped]
        public Uri YouTubeUri => new Uri(@"https://www.youtube.com/embed/" + YouTubeId);

        [NotMapped]
        public Uri YouTubeThumbnail => new Uri(@"https://img.youtube.com/vi/" + YouTubeId + @"/0.jpg");

        [NotMapped]
        public bool Empty => string.IsNullOrWhiteSpace(YouTubeId);
    }
}
