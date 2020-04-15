using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace ResourceCraft.Minecraft
{
    public static class TerrainConverter
    {
        public static Image[] FromTerrainToImages(Image terrain)
        {
            if (terrain == null)
            {
                throw new ArgumentNullException(nameof(terrain));
            }

            var images = new Image[256];
            for (int i = 0; i < 16; i++)
            {
                for (int j = 0; j < 16; j++)
                {
                    var index = i * 16 + j;
                    images[index] = new Bitmap(16, 16);
                    var graphics = Graphics.FromImage(images[index]);
                    graphics.DrawImage(terrain, new Rectangle(0, 0, 16, 16), new Rectangle(i * 16, j * 16, 16, 16), GraphicsUnit.Pixel);
                    graphics.Dispose();
                }
            }

            return images;
        }
    }
}
