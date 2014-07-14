using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Shared;

namespace AtlasComposer
{
    public partial class EditableCanvas : AtlasCanvas
    {
        bool saved = true;

        public bool Saved { get { return saved; } }

        public EditableCanvas()
        {
            InitializeComponent();
            this.DragEnter += new DragEventHandler(Canvas_DragEnter);
            this.DragDrop += new DragEventHandler(Canvas_DragDrop);
            this.Paint += new PaintEventHandler(EditableCanvas_Paint);
            MouseDown += new MouseEventHandler(EditableCanvas_MouseDown);
        }

        void EditableCanvas_MouseDown(object sender, MouseEventArgs e)
        {
            saved = false;
        }

        private void EditableCanvas_Paint(object sender, PaintEventArgs e)
        {
            foreach (var item in atlas.Parts)
            {
                if (item == ((Form1)ParentForm).editPartControll1.Current)
                    e.Graphics.DrawRectangle(dashed_magnet, new Rectangle(item.X + item.Rectangle.Width / 2 - 3, item.Y + item.Rectangle.Height / 2 - 3, 6, 6));
            }
        }

        public void Add(Bitmap image)
        {
            Point pos = new Point(Width / 2 - view_shift.X, Height / 2 - view_shift.Y);
            AtlasPart item = new AtlasPart(image, atlas.Parts.Count.ToString(),
                new Rectangle(pos.X - image.Width / 2, pos.Y - image.Height / 2, image.Width, image.Height));
            item.Border = (int)((Form1)ParentForm).DefaultBorder.Value;
            atlas.Parts.Add(item);
            ((Form1)ParentForm).editPartControll1.Update(item);
            Refresh();
        }

        public void Delete(AtlasPart part)
        {
            atlas.Parts.Remove(part);
            Refresh();
        }

        private void Canvas_DragDrop(object sender, DragEventArgs e)
        {
            // Extract the data from the DataObject-Container into a string list
            string[] fileList = (string[])e.Data.GetData(DataFormats.FileDrop, false);

            Point pos = this.PointToClient(new Point(e.X - view_shift.X, e.Y - view_shift.Y));
            foreach (string file in fileList)
            {
                Image temp;
                try
                {
                    temp = Bitmap.FromFile(file);
                    AtlasPart item = new AtlasPart(new Bitmap(temp), atlas.Parts.Count.ToString(),
                        new Rectangle(pos.X - temp.Width / 2, pos.Y - temp.Height / 2, temp.Width, temp.Height));
                    item.Border = (int)((Form1)ParentForm).DefaultBorder.Value;
                    atlas.Parts.Add(item);
                    ((Form1)ParentForm).editPartControll1.Update(item);
                }
                catch (Exception)
                { }
            }
            Refresh();
        }

        private void Canvas_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy; // Okay
            else
                e.Effect = DragDropEffects.None; // Unknown data, ignore it
            Refresh();
        }

        public override void Reset()
        {
            base.Reset();
            saved = true;
        }
        public void SaveToFile(string file)
        {
            atlas.Save(file);
            saved = true;
        }

        public void LoadFromFile(string file)
        {
            atlas.Load(file);
            saved = true;
        }
    }
}