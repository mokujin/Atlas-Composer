using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Shared
{
    public class AtlasPart
    {
        Rectangle rect;
        int border;

        public Rectangle ImageRectangle { get { return Rectangle.Inflate(rect, -2 * border, -2 * border); } }

        public Rectangle Rectangle { get { return rect; } }

        public int Border
        {
            get { return border; }
            set { rect.Inflate((value - border) * 2, (value - border) * 2); border = value; }
        }

        public int X { get { return rect.X; } set { rect.X = value; } }

        public int Y { get { return rect.Y; } set { rect.Y = value; } }

        public Point Position { get { return rect.Location; } set { rect.Location = value; } }

        public string Name { get; set; }

        public Bitmap Image { get; private set; }

        public AtlasPart(Bitmap image, string name, Rectangle rect)
        {
            this.rect = rect;
            Name = name;
            Image = image;
            border = 0;
        }
    }
}