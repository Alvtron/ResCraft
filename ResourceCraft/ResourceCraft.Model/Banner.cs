using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ResourceCraft.Model
{
    public class Banner : FirebaseImage
    {
        public bool IsPrimary { get; set; }

        public Banner()
        {
        }

        public Banner(int width, int height, string extension)
            : base(width, height, 20 / 8, extension)
        {
            
        }
    }
}