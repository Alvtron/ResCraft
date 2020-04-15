namespace ResourceCraft.Model
{
    public interface ICroppableImage
    {
        Crop Crop { get; set; }
        void CreateCrop(double aspectRatio);
    }
}