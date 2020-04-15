using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ResourceCraft.Model
{
    public class ProfilePicture : FirebaseImage, ICroppableImage
    {
        public bool IsPrimary { get; set; }

        public ProfilePicture()
        {
        }

        public ProfilePicture(int width, int height, string extension)
            : base(width, height, 1 / 1, extension)
        {
        }
    }
}