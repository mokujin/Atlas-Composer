using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using Shared;

namespace Shared
{
    public partial class AtlasCanvas : UserControl
    {
        #region Drawing stuff

        protected Font font;
        protected Pen dashed;
        protected Pen dashed_magnet;

        bool draw_top_magnet_line;
        bool draw_bottom_magnet_line;
        bool draw_left_magnet_line;
        bool draw_right_magnet_line;

        protected bool move_grab = false;
        protected Point view_shift;
        protected Point mouse_start;
        protected Point mouse_shift;

        #endregion Drawing stuff

        //int area_width = 300, area_height = 300;

        protected bool resize_grab = false;
        protected Point grab_displacement;

        protected AtlasPart grabbed_element;
        protected Point magnet_shift;

        protected byte magnet_distance = 7;

        public int AreaWidth { get { return atlas.Width; } set { atlas.Width = value; } }

        public int AreaHeight { get { return atlas.Height; } set { atlas.Height = value; } }

        protected AtlasPart selected;

        public AtlasPart SelectedPart { get { return selected; } }

        protected Atlas atlas;

        public AtlasCanvas()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            UpdateStyles();
            font = new Font("courier new", 14);
            dashed = new Pen(Color.Red);
            dashed.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
            dashed_magnet = new Pen(Color.Gray);
            dashed_magnet.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
            this.AllowDrop = true;

            atlas = new Atlas(300, 300);
        }

        public virtual void Reset()
        {
            atlas.Reset(300, 300);
            view_shift = Point.Empty;
            Refresh();
        }

        private void Canvas_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.TranslateTransform(view_shift.X + mouse_shift.X, view_shift.Y + mouse_shift.Y);
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
                if (item == grabbed_element)
                {
                    Rectangle place = item.Rectangle;
                    place.Offset(magnet_shift);

                    bool collide = false;
                    foreach (var i in atlas.Parts)
                        if (item.Rectangle != i.Rectangle && place.IntersectsWith(i.Rectangle))
                        {
                            collide = true;
                            e.Graphics.DrawRectangle(new Pen(Color.Red, 2), i.Rectangle);
                        }

                    e.Graphics.DrawRectangle(collide ? new Pen(Color.Red, 2) : new Pen(Color.Green, 2), place);
                    if (draw_bottom_magnet_line)
                        e.Graphics.DrawLine(dashed_magnet, 0, place.Bottom, atlas.Width, place.Bottom);
                    if (draw_left_magnet_line)
                        e.Graphics.DrawLine(dashed_magnet, place.Left, 0, place.Left, atlas.Height);
                    if (draw_top_magnet_line)
                        e.Graphics.DrawLine(dashed_magnet, 0, place.Top, atlas.Width, place.Top);
                    if (draw_right_magnet_line)
                        e.Graphics.DrawLine(dashed_magnet, place.Right, 0, place.Right, atlas.Height);
                }
                else
                    e.Graphics.DrawRectangle(new Pen(Color.Gray, 2), item.Rectangle);
                if (item == selected)
                    e.Graphics.DrawRectangle(dashed_magnet, new Rectangle(item.X + item.Rectangle.Width / 2 - 3, item.Y + item.Rectangle.Height / 2 - 3, 6, 6));
            }

            e.Graphics.DrawRectangle(dashed, 0, 0, atlas.Width, atlas.Height);
            e.Graphics.FillPie(Brushes.Red, atlas.Width - 5, atlas.Height - 5, 10, 10, 180, -270);
            e.Graphics.DrawString(atlas.Height.ToString(), font, Brushes.Red, atlas.Width, atlas.Height / 2 - e.Graphics.MeasureString(atlas.Height.ToString(), font).Width / 2, new StringFormat(StringFormatFlags.DirectionVertical));
            e.Graphics.DrawString(atlas.Width.ToString(), font, Brushes.Red, atlas.Width / 2 - e.Graphics.MeasureString(atlas.Width.ToString(), font).Width / 2, atlas.Height);
        }

        private void Canvas_MouseDown(object sender, MouseEventArgs e)
        {
            Point mouse = e.Location;
            mouse.Offset(-view_shift.X, -view_shift.Y);
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                if (new Rectangle(atlas.Width - 5, atlas.Height - 5, 10, 10).Contains(mouse))
                {
                    resize_grab = true;
                    grab_displacement = new Point(mouse.X - atlas.Width, mouse.Y - atlas.Height);
                }
                else
                    foreach (var item in atlas.Parts.Reverse<AtlasPart>())
                    {
                        if (item.Rectangle.Contains(mouse))
                        {
                            grab_displacement = new Point(mouse.X - item.Rectangle.X, mouse.Y - item.Rectangle.Y);
                            grabbed_element = item;
                            selected = item;// ((Form1)ParentForm).editPartControll1.Update(item);
                        }
                    }
            }
            if (e.Button == System.Windows.Forms.MouseButtons.Middle)
            {
                move_grab = true;
                mouse_start.X = e.X - view_shift.X;
                mouse_start.Y = e.Y - view_shift.Y;
                mouse_shift = Point.Empty;
            }
            Refresh();
        }

        private void Canvas_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
                if (grabbed_element != null)
                {
                    Point n = grabbed_element.Position;
                    n.Offset(magnet_shift);
                    grabbed_element.Position = n;
                }
            if (e.Button == System.Windows.Forms.MouseButtons.Middle && move_grab)
            {
                view_shift.Offset(mouse_shift);
                mouse_shift = Point.Empty;
                move_grab = false;
            }
            magnet_shift = Point.Empty;
            resize_grab = false;
            grabbed_element = null;
            Refresh();
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (move_grab)
            {
                mouse_shift.X = e.Location.X - mouse_start.X - view_shift.X;
                mouse_shift.Y = e.Location.Y - mouse_start.Y - view_shift.Y;
            }

            if (resize_grab)
            {
                atlas.Width = e.X - view_shift.X - grab_displacement.X;
                atlas.Height = e.Y - view_shift.Y - grab_displacement.Y;
            }
            if (grabbed_element != null)
            {
                Rectangle n = grabbed_element.Rectangle;
                n.X = e.X - view_shift.X - grab_displacement.X;
                n.Y = e.Y - view_shift.Y - grab_displacement.Y;

                magnet_shift = Point.Empty;

                int top = magnet_distance;
                int bottom = magnet_distance;
                int left = magnet_distance;
                int right = magnet_distance;

                foreach (var item in atlas.Parts)
                {
                    if (item == grabbed_element)
                        continue;
                    Magnet(n, item.Rectangle, ref top, ref bottom, ref right, ref left);
                }
                Magnet(n, new Rectangle(0, 0, atlas.Width, atlas.Height), ref top, ref bottom, ref right, ref left);

                draw_bottom_magnet_line = false;
                draw_left_magnet_line = false;
                draw_right_magnet_line = false;
                draw_top_magnet_line = false;
                if (Math.Abs(top) < magnet_distance)
                {
                    magnet_shift.Y -= (int)top;
                    draw_top_magnet_line = true;
                }
                else if (Math.Abs(bottom) < magnet_distance)
                {
                    magnet_shift.Y += (int)bottom;
                    draw_bottom_magnet_line = true;
                }
                if (Math.Abs(left) < magnet_distance)
                {
                    magnet_shift.X -= (int)left;
                    draw_left_magnet_line = true;
                }
                else if (Math.Abs(right) < magnet_distance)
                {
                    magnet_shift.X += (int)right;
                    draw_right_magnet_line = true;
                }
                grabbed_element.Position = n.Location;
            }
            Refresh();
        }

        private void Magnet(Rectangle source, Rectangle test, ref int top, ref int bottom, ref int right, ref int left)
        {
            int tmp;
            tmp = test.Left - source.Right;
            if (Math.Abs(right) > Math.Abs(tmp))
                right = tmp;

            tmp = test.Right - source.Right;
            if (Math.Abs(right) > Math.Abs(tmp))
                right = tmp;

            tmp = source.Left - test.Left;
            if (Math.Abs(left) > Math.Abs(tmp))
                left = tmp;

            tmp = source.Left - test.Right;
            if (Math.Abs(left) > Math.Abs(tmp))
                left = tmp;

            tmp = test.Top - source.Bottom;
            if (Math.Abs(bottom) > Math.Abs(tmp))
                bottom = tmp;

            tmp = test.Bottom - source.Bottom;
            if (Math.Abs(bottom) > Math.Abs(tmp))
                bottom = tmp;

            tmp = source.Top - test.Top;
            if (Math.Abs(top) > Math.Abs(tmp))
                top = tmp;

            tmp = source.Top - test.Bottom;
            if (Math.Abs(top) > Math.Abs(tmp))
                top = tmp;
        }


    }
}