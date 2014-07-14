using System;
using System.Drawing;
using System.Windows.Forms;

namespace AtlasComposer
{
    public partial class Form1 : Form
    {
        string sfile = null;

        public Form1()
        {
            InitializeComponent();
        }

        private void canvas_Paint(object sender, PaintEventArgs e)
        {
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            if (this.sfile != null)
                canvas1.SaveToFile(sfile);
            else if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string file = saveFileDialog1.FileName;
                if (file.EndsWith(".xml") || file.EndsWith(".png"))
                    file = file.Remove(file.Length - 4, 4);
                canvas1.SaveToFile(file);
            }
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            if (!canvas1.Saved)
                if (MessageBox.Show("There are unsaved changes... want to save it?", "Save or discard", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                {
                    toolStripButton1_Click(null, null);
                }
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string file = openFileDialog1.FileName;
                if (file.EndsWith(".xml") || file.EndsWith(".png"))
                    file = file.Remove(file.Length - 4, 4);
                canvas1.LoadFromFile(file);
                this.sfile = file;
            }
            canvas1.Refresh();
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string file = saveFileDialog1.FileName;
                if (file.EndsWith(".xml") || file.EndsWith(".png"))
                    file = file.Remove(file.Length - 4, 4);
                canvas1.SaveToFile(file);
            }
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            if (!canvas1.Saved)
                if (MessageBox.Show("There are unsaved changes... want to save it?", "Save or discard", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                {
                    toolStripButton1_Click(null, null);
                }
            canvas1.Reset();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.V)
            {
                if (Clipboard.GetDataObject().GetDataPresent(DataFormats.Bitmap))
                {
                    Bitmap a = (Bitmap)Clipboard.GetDataObject().GetData(DataFormats.Bitmap);
                    a.MakeTransparent();
                    canvas1.Add(a);
                }
            }
            if (e.KeyCode == Keys.Delete)
            {
                canvas1.Delete(editPartControll1.Current);
                editPartControll1.Update(null);
            }
        }
    }
}