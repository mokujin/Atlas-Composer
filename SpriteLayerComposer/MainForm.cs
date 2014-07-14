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
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedTab.Controls.Count == 0)
                if (openAtlas.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    string file = openAtlas.FileName;
                    if (file.EndsWith(".xml") || file.EndsWith(".png"))
                        file = file.Remove(file.Length - 4, 4);
                    Atlas atlas = new Atlas();
                    atlas.Load(file);
                    AtlasBrowser b = new AtlasBrowser(atlas);
                    tabControl1.SelectedTab.Controls.Add(b);
                    b.Dock = DockStyle.Fill;
                }
        }
    }
}