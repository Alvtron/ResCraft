using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ResourceCraft.Model
{
    [ComplexType]
    public class Crop : ObservableObject
    {
        private int _x;
        public int X
        {
            get => _x;
            set => SetField(ref _x, value);
        }
        private int _y;
        public int Y
        {
            get => _y;
            set => SetField(ref _y, value);
        }
        private int _width;
        public int Width
        {
            get => _width;
            set => SetField(ref _width, value);
        }
        private int _height;
        public int Height
        {
            get => _height;
            set => SetField(ref _height, value);
        }

        public double AspectRatio { get; set; }

        public Crop() { }

        public Crop(int x, int y, int width, int height, double aspectRatio)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            AspectRatio = aspectRatio;
        }
    }
}
