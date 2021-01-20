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
        static OpenFileDialog od = new OpenFileDialog();
        static Graphics g = null;
        static bool isGrayScale = false;
        static Bitmap image = null;
        byte[,,] imageArray = null;
        byte[,,] ditheredArray = null;
        static int factor;
        //static int factor;
        static Bitmap bmp;
        //static Bitmap image;
        #endregion
        //
        //Bitmap dithered;
        // bool openFile = false;

        public Form1() => InitializeComponent();

        private void Form1_Load(object sender, EventArgs e)
        {
            od.Filter = "Image Files(*.bmp;*.jpg;*.png)|*.BMP;*.JPG;*.PNG";
            od.DefaultExt = ".png";
            factor = factorSlider.Value;
            SetTitle(factor);
        }

        private void SetTitle(int factor)
        {
            this.Text = TITLE + "Factor: " + factor.ToString();
        }

        private void DitherThreadMethod()
        {
            ditheredArray = Dithering.Make(imageArray, factor, isGrayScale);
            if (ditheredArray != null)
            {
                new Thread(Draw).Start();
            }
        }

        private void Draw()
        {
            g = Graphics.FromImage(image);
            g.Clear(Color.White);
            bmp = Dithering.GetBitmapFromArray(ditheredArray);
            g.DrawImage(bmp, 0, 0, image.Width, image.Height); //print original here
            pictureBoxView.Image = bmp;
            g.Dispose();
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

        // EVENTS
        private void ButtonLoad_Click(object sender, EventArgs e)
        {
            if (od.ShowDialog() == DialogResult.OK)
            {
                image = new Bitmap(od.FileName);
                if (image != null)
                {
                    this.pictureBoxView.Image = image;
                    this.imageArray = Dithering.GetImageArray(image);
                    buttonDither.Enabled = true;
                }
            }
        }

        private void ButtonDither_Click(object sender, EventArgs e)
        {
            buttonDither.Enabled = false;
            buttonSave.Enabled = true;
            new Thread(DitherThreadMethod).Start();
        }

        private void ButtonSave_Click(object sender, EventArgs e)
        {
            new Thread(SaveFile).Start();
            this.buttonSave.Enabled = false;
        }

        private void FactorSlider_Scroll(object sender, EventArgs e)
        {
            buttonDither.Enabled = true;
            SetTitle(factorSlider.Value);
            factor = factorSlider.Value;
        }

        private void IsGrayScale_Changed(object sender, EventArgs e)
        {
            isGrayScale = this.isGrayScaleCheckbox.Checked;
        }
    }
}
