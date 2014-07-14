using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Xml;

namespace Shared
{
    public class Atlas
    {
        private int width;

        public int Width { get { return width; } set { if (value > 0 && value < 4096) width = value; } }

        private int height;

        public int Height { get { return height; } set { if (value > 0 && value <= 4096) height = value; } }

        List<AtlasPart> parts;

        public List<AtlasPart> Parts { get { return parts; } }

        public Atlas()
        {
            Width = 500;
            Height = 500;
            parts = new List<AtlasPart>();
        }

        public Atlas(int width, int height)
        {
            Width = 500;
            Height = 500;
            parts = new List<AtlasPart>();
        }

        public void Reset()
        {
            parts.Clear();
        }

        public void Reset(int width, int height)
        {
            parts.Clear();
            Width = 500;
            Height = 500;
        }

        public void Save(string filename)
        {
            Bitmap result = new Bitmap(Width, Height);
            Graphics g = Graphics.FromImage(result);
            foreach (var item in parts)
            {
                g.DrawImage(item.Image, item.ImageRectangle);
            }
            g.Flush(System.Drawing.Drawing2D.FlushIntention.Flush);
            result.Save(filename + ".png", ImageFormat.Png);
            XmlTextWriter w = new XmlTextWriter(filename + ".xml", Encoding.UTF8);
            w.WriteStartDocument();
            w.WriteStartElement("atlas");
            foreach (var item in parts)
            {
                w.WriteStartElement("part");
                w.WriteAttributeString("name", item.Name);
                w.WriteAttributeString("border", item.Border.ToString());
                w.WriteAttributeString("rect", item.ImageRectangle.X + " " + item.ImageRectangle.Y + " " + item.ImageRectangle.Width + " " + item.ImageRectangle.Height);
                w.WriteEndElement();
            }
            w.WriteEndElement();
            w.WriteEndDocument();
            w.Close();
        }

        public void Load(string filename)
        {
            Image source = Bitmap.FromFile(filename + ".png");
            XmlTextReader r = new XmlTextReader(filename + ".xml");
            Bitmap bmp = new Bitmap(source);
            parts.Clear();

            string[] tmp;
            Rectangle rectangle = Rectangle.Empty;

            while (r.Read())
            {
                if (r.NodeType == XmlNodeType.Element && r.Name == "part")
                {
                    string name = r.GetAttribute("name");
                    string tl = r.GetAttribute("tl");
                    string br = r.GetAttribute("br");
                    string rect = r.GetAttribute("rect");
                    string bor = r.GetAttribute("border");

                    if (tl != null)
                    {
                        tmp = tl.Split(' ');
                        rectangle.X = int.Parse(tmp[0]);
                        rectangle.Y = int.Parse(tmp[1]);
                    }
                    if (br != null)
                    {
                        tmp = br.Split(' ');
                        rectangle.Width = int.Parse(tmp[0]) - rectangle.X;
                        rectangle.Height = int.Parse(tmp[1]) - rectangle.Y;
                    }
                    if (rect != null)
                    {
                        tmp = rect.Split(' ');
                        rectangle.X = int.Parse(tmp[0]);
                        rectangle.Y = int.Parse(tmp[1]);
                        rectangle.Width = int.Parse(tmp[2]);
                        rectangle.Height = int.Parse(tmp[3]);
                    }

                    Bitmap p = bmp.Clone(rectangle, bmp.PixelFormat);
                    AtlasPart item = new AtlasPart(p, name, rectangle);
                    item.Border = int.Parse(bor);
                    parts.Add(item);
                }
            }
            Width = source.Width;
            Height = source.Height;
        }
    }
}