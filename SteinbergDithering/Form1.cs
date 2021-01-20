using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace SteinbergDithering
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        string title = "JellyBeanci Steinberg Dithering ";
        int factor;
        OpenFileDialog od = new OpenFileDialog();
        Bitmap bmp;
        Bitmap image;
        Bitmap dithered;
        bool openFile = false;
        private void Form1_Load(object sender, EventArgs e)
        {
            od.Filter = "Image Files(*.bmp;*.jpg;*.png)|*.BMP;*.JPG;*.PNG";
            od.DefaultExt = ".png";
            this.factor = trackBar1.Value;
            this.Text = title + "Factor: " + factor.ToString();
        }

        private void buttonLoad_Click(object sender, EventArgs e)
        {
            if (od.ShowDialog() == DialogResult.OK)
            {
                image = new Bitmap(od.FileName);
                if (image != null)
                {
                    this.pictureBoxView.Image = image;
                    openFile = true;
                    buttonDither.Enabled = true;
                    dithered = (Bitmap)image.Clone();
                    bmp = (Bitmap)image.Clone();
                }
            }
        }

        private void buttonDither_Click(object sender, EventArgs e)
        {
            buttonDither.Enabled = false;
            dithered = Dithering.Make(image, factor, checkBox1.Checked);
            if (dithered != null)
            {
                //pictureBoxView.Image = dithered;
                Draw();
                buttonSave.Enabled = true;
            }
        }
        Graphics g;
        private void Draw()
        {
            g = Graphics.FromImage(bmp);
            g.Clear(Color.White);
            //g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias; //AA
            g.DrawImage(dithered, 0, 0, dithered.Width, dithered.Height); //print original here
            pictureBoxView.Image = bmp;
            g.Dispose();
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            if (bmp != null)
            {
                string name = DateTime.Now.ToLongTimeString().Replace('.', ' ').Replace(':', ' ');
                bmp.Save(name + ".bmp", ImageFormat.Bmp);
                buttonSave.Enabled = false;
                MessageBox.Show("Dosya " + name + " ismi ile kayıt edildi", "Başarılı!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            this.factor = trackBar1.Value;
            this.Text = title + "Factor: " + factor.ToString();
        }
    }
}
