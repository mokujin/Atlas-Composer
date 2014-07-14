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
    public partial class EditPartControll : UserControl
    {
        AtlasPart current;

        public AtlasPart Current { get { return current; } }

        Pen dashed;

        public EditPartControll()
        {
            InitializeComponent();
            dashed = new Pen(Color.Gray, 2);
            dashed.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
        }

        public void Update(AtlasPart part)
        {
            current = part;
            if (part != null)
            {
                textBox1.Text = part.Name;
                numericUpDown1.Value = part.Border;
            }
            else
            {
                textBox1.Text = "";
                numericUpDown1.Value = 0;
            }
            pictureBox1.Refresh();
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            if (current != null)
            {
                e.Graphics.DrawImage(current.Image, pictureBox1.Width / 2 - current.Image.Width / 2, pictureBox1.Height / 2 - current.Image.Height / 2, current.Image.Width, current.Image.Height);
                Rectangle rect = current.Rectangle;
                rect.X = pictureBox1.Width / 2 - current.Rectangle.Width / 2;
                rect.Y = pictureBox1.Height / 2 - current.Rectangle.Height / 2;
                e.Graphics.DrawRectangle(dashed, rect);
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (current != null)
                current.Name = textBox1.Text;
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            if (current != null)
                current.Border = (int)numericUpDown1.Value;
            ((Form1)ParentForm).canvas1.Refresh();
        }
    }
}