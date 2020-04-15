using System.ComponentModel.DataAnnotations.Schema;

namespace ResourceCraft.Model
{
    [ComplexType]
    public class Color
    {
        public byte R { get; set; } = 255;
        public byte G { get; set; } = 255;
        public byte B { get; set; } = 255;
        public byte A { get; set; } = 255;

        public Color() { }

        public Color(byte r, byte g, byte b, byte a)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }

        public override string ToString()
        {
            return $"R:{R} G:{G} B:{B} A:{A}";
        }
    }
}
