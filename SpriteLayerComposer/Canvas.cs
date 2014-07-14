using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SpriteLayerComposer
{
    public partial class Canvas : UserControl
    {
        public Canvas()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            UpdateStyles();
        }

        private void Canvas_Paint(object sender, PaintEventArgs e)
        {

        }

        private void Canvas_DragEnter(object sender, DragEventArgs e)
        {

        }

        private void Canvas_DragDrop(object sender, DragEventArgs e)
        {

        }
    }
}