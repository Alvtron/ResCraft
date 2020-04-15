using ResourceCraft.Model;
using ResourceCraft.Utilities;
using System;
using Windows.UI.Input;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace ResourceCraft.Uwp.Utilities
{
    public class ImageCropper<T> where T : FirebaseImage, ICroppableImage, new()
    {
        public T Image { get; }

        public Crop Crop { get; }

        public readonly double HandleSize;

        private readonly double CropHandleTriggerSize;

        private bool PointerIsBusy { get; set; } = false;

        public ImageCropper(T image)
        {
            Image = image;
            Crop = Image.Crop;
            CropHandleTriggerSize = image.Width * 0.10;
            HandleSize = image.Width * 0.03;
        }

        public void Save()
        {
            Image.Crop = Crop;
        }

        private void UpdateCrop(PointerPoint pointer)
        {
            if (PointerIsBusy || !pointer.Properties.IsLeftButtonPressed)
            {
                return;
            }

            PointerIsBusy = true;

            var x = ConvertXToCoordinate(pointer.Position.X);
            var y = ConvertXToCoordinate(pointer.Position.Y);

            var newCropSize = CreateNewCropSize(x, y);
            var newWidth = newCropSize.Item1;
            var newHeight = newCropSize.Item2;

            Crop.Width = newWidth;
            Crop.Height = newHeight;

            PointerIsBusy = false;
        }

        private Tuple<int, int> CreateNewCropSize(int x, int y)
        {
            var newWidth = Math.Abs(x) * 2;
            var newHeight = Math.Abs(y) * 2;
            
            var ratio = Image.Crop.AspectRatio;

            var suggestedHeight = newWidth / ratio;
            var suggestedWidth = newHeight * ratio;

            if (suggestedHeight <= newHeight)
            {
                return Tuple.Create(newWidth, (int)suggestedHeight);
            }
            else
            {
                return Tuple.Create((int)suggestedWidth, newHeight);
            }
        }

        private int ConvertXToCoordinate(double x)
        {
            return -(int)(Image.Width / 2.0 - x);
        }

        private int ConvertYToCoordinate(double y)
        {
            return (int)(Image.Height / 2.0 - y);
        }

        public void OnPointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (!(sender is Grid grid))
            {
                return;
            }
            UpdateCrop(e.GetCurrentPoint(grid));
        }

        private void PrintPointerDebugInfo(PointerPoint pointer)
        {
            var x = ConvertXToCoordinate(pointer.Position.X);
            var y = ConvertXToCoordinate(pointer.Position.Y);

            Logger.WriteLine($"x({ConvertXToCoordinate(x)}), y({ConvertYToCoordinate(y)}");
            Logger.WriteLine($": size: {Math.Abs(x) * 2}w x {Math.Abs(y) * 2}h");
        }
    }
}
