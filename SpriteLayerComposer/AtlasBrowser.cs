using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Shared;

namespace SpriteLayerComposer
{
    public partial class AtlasBrowser : UserControl
    {
        Atlas atlas;

        Font font;
        Pen dashed;

        public AtlasBrowser()
        {
            InitializeComponent();
        }

        public AtlasBrowser(Atlas atlas)
        {
            InitializeComponent();
            this.atlas = atlas;

            font = new Font("courier new", 14);
            dashed = new Pen(Color.Red);
            dashed.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
        }

        private void AtlasBrowser_Paint(object sender, PaintEventArgs e)
        {
            //e.Graphics.TranslateTransform(view_shift.X + mouse_shift.X, view_shift.Y + mouse_shift.Y);
            for (int i = 0; i < atlas.Width; i += 8)
            {
                for (int j = 0; j < atlas.Height; j += 8)
                {
                    e.Graphics.DrawRectangle(Pens.Black, new Rectangle(i, j, 1, 1));
                }
            }

            foreach (var item in atlas.Parts)
            {
                e.Graphics.DrawImage(item.Image, item.ImageRectangle);
                //if (item == grabbed_element)
                //{
                Rectangle place = item.Rectangle;
                //place.Offset(magnet_shift);

                bool collide = false;
                foreach (var i in atlas.Parts)
                    if (item.Rectangle != i.Rectangle && place.IntersectsWith(i.Rectangle))
                    {
                        collide = true;
                        e.Graphics.DrawRectangle(new Pen(Color.Red, 2), i.Rectangle);
                    }

                e.Graphics.DrawRectangle(collide ? new Pen(Color.Red, 2) : new Pen(Color.Green, 2), place);
                //}
                //else
                e.Graphics.DrawRectangle(new Pen(Color.Gray, 2), item.Rectangle);
                //if (item == ((Form1)ParentForm).editPartControll1.Current)
                //e.Graphics.DrawRectangle(dashed_magnet, new Rectangle(item.X + item.Rectangle.Width / 2 - 3, item.Y + item.Rectangle.Height / 2 - 3, 6, 6));
            }

            e.Graphics.DrawRectangle(dashed, 0, 0, atlas.Width, atlas.Height);
            e.Graphics.FillPie(Brushes.Red, atlas.Width - 5, atlas.Height - 5, 10, 10, 180, -270);
            e.Graphics.DrawString(atlas.Height.ToString(), font, Brushes.Red, atlas.Width, atlas.Height / 2 - e.Graphics.MeasureString(atlas.Height.ToString(), font).Width / 2, new StringFormat(StringFormatFlags.DirectionVertical));
            e.Graphics.DrawString(atlas.Width.ToString(), font, Brushes.Red, atlas.Width / 2 - e.Graphics.MeasureString(atlas.Width.ToString(), font).Width / 2, atlas.Height);
        }
    }
}