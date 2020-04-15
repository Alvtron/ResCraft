using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ResourceCraft.Model
{
    public class Screenshot : FirebaseImage
    {
        public Screenshot()
        {
        }

        public Screenshot(int width, int height, string extension)
            : base(width, height, 16 / 9, extension)
        {
        }
    }
}