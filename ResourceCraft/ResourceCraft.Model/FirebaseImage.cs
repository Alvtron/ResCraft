using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ResourceCraft.Utilities;

namespace ResourceCraft.Model
{
    public class FirebaseImage : FirebaseFile, ICroppableImage
    {
        public int Width { get; set; }
        public int Height { get; set; }
        private Crop _crop;
        public Crop Crop
        {
            get => _crop;
            set => SetField(ref _crop, value);
        }

        [NotMapped, JsonIgnore]
        public double AspectRatio => Width / Height;

        public FirebaseImage() { }

        public FirebaseImage(int width, int height, string extension)
            : this(width, height, width / height, extension)
        {
        }

        public FirebaseImage(int width, int height, double cropAspectRatio, string extension)
            : base(extension)
        {
            Width = width;
            Height = height;

            CreateCrop(cropAspectRatio);
        }

        public void CreateCrop(double aspectRatio)
        {
            var croppedWidth = Width * 0.9;
            var croppedHeight = Height * 0.9;

            var suggestedHeight = croppedWidth / aspectRatio;
            var suggestedWidth = croppedHeight * aspectRatio;

            if (suggestedHeight <= croppedHeight)
            {
                Crop = new Crop(Width / 2, Height / 2, (int)croppedWidth, (int)suggestedHeight, aspectRatio);
            }
            else
            {
                Crop = new Crop(Width / 2, Height / 2, (int)suggestedWidth, (int)croppedHeight, aspectRatio);
            }
        }
    }
}