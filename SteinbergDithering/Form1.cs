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
using System.Threading;

namespace SteinbergDithering
{
    public partial class Form1 : Form
    {

        //
        #region GLOBAL VARIABLES
        static readonly string TITLE = "JellyBeanci Steinberg Dithering ";
        static int factor;
        static OpenFileDialog od = new OpenFileDialog();
        static Bitmap bmp;
        static Bitmap image;
        static bool isGrayScale = false;
        #endregion
        //

        public Form1() => InitializeComponent();

   
        Bitmap dithered;
        // bool openFile = false;


        private void Form1_Load(object sender, EventArgs e)
        {
            od.Filter = "Image Files(*.bmp;*.jpg;*.png)|*.BMP;*.JPG;*.PNG";
            od.DefaultExt = ".png";
            factor = factorSlider.Value;
            this.Text = TITLE + "Factor: " + factor.ToString();
        }

        private void ButtonLoad_Click(object sender, EventArgs e)
        {
            if (od.ShowDialog() == DialogResult.OK)
            {
                image = new Bitmap(od.FileName);
                if (image != null)
                {
                    this.pictureBoxView.Image = image;
                    //openFile = true; //idk
                    buttonDither.Enabled = true;
                    dithered = (Bitmap)image.Clone();
                    bmp = (Bitmap)image.Clone();
                }
            }
        }

        private void ButtonDither_Click(object sender, EventArgs e)
        {
            buttonDither.Enabled = false;
            new Thread(DitherThreadMethod).Start();

            buttonSave.Enabled = true;
        }

        private void DitherThreadMethod()
        {
            dithered = Dithering.Make(image, factor, isGrayScale);
            if (dithered != null)
            {
                //pictureBoxView.Image = dithered;
                new Thread(Draw).Start();
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

        private void ButtonSave_Click(object sender, EventArgs e)
        {
            new Thread(SaveFile).Start();
            this.buttonSave.Enabled = false;
        }
        
        private void SaveFile()
        {
            if (bmp != null)
            {
                string name = DateTime.Now.ToLongTimeString().Replace('.', ' ').Replace(':', ' ');
                bmp.Save(name + ".bmp", ImageFormat.Bmp);
                MessageBox.Show("Dosya " + name + " ismi ile kayıt edildi", "Başarılı!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void FactorSlider_Scroll(object sender, EventArgs e)
        {
            factor = factorSlider.Value;
            buttonDither.Enabled = true;
            this.Text = TITLE + "Factor: " + factor.ToString();
        }

        private void IsGrayScale_Changed(object sender, EventArgs e)
        {
            isGrayScale = this.isGrayScaleCheckbox.Checked;
        }
    }
}
