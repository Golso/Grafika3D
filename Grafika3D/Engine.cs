using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Text;

namespace Grafika3D
{
    class Engine : Drawable
    {
        public static Engine Instance { get; } = new Engine();
        static Engine()
        {
        }
        private Engine()
        {
        }
        public RenderWindow Window { get; set; }

        private float[,] ZBuffer { get; set; }
        private Color[,] Bitmap { get; set; }

        public void Draw(RenderTarget target, RenderStates states)
        {
            ZBuffer = new float[Window.Size.X, Window.Size.Y];
            Bitmap = new Color[Window.Size.X, Window.Size.Y];

            Vector3f pik = new Vector3f(1,1,1);

            DrawPixel(pik, Color.White);

            Image img = new Image(Bitmap);
            Texture tex = new Texture(img);
            Sprite s = new Sprite(tex);
            target.Draw(s, states);
            s.Dispose();
            tex.Dispose();
            img.Dispose();
        }
        
        private void DrawPixel(Vector3f pixel, Color color)
        {
            int screenX = (int)pixel.X;
            int screenY = (int)pixel.Y;
            if (screenX < 0 || screenX >= Bitmap.GetLength(0)) return;
            if (screenY < 0 || screenY >= Bitmap.GetLength(1)) return;
            if (pixel.Z <= ZBuffer[screenX, screenY])
            {
                //drawing
                ZBuffer[screenX, screenY] = pixel.Z;
                Bitmap[screenX, screenY] = color;
            }
        }

        private void DrawPixel(Vector2f pixel, float z, Color color)
        {
            DrawPixel(new Vector3f(pixel.X, pixel.Y, z), color);
        }
        
    }
}
