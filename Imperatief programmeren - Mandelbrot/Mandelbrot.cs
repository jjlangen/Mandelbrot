using System;
using System.Drawing;
using System.Windows.Forms;
using System.Speech.Recognition;

namespace Mandelbrot
{
    public partial class Mandelbrot : Form
    {

        // Declaration of the globals
        SpeechRecognitionEngine recognitionEngine = new SpeechRecognitionEngine();
        Bitmap mandelbrot = new Bitmap(500, 500);
        double centerX,  centerY;
        double leftTopX, leftTopY;
        double scale;
        int max;

        // Mandelbrot Class Constructor
        public Mandelbrot()
        {
            InitializeComponent();
            getFormValues();
            paintPicture();

            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 0;
            pictureBox1.MouseClick += zoom;
        }

        // Loop through all pixels in the picturebox
        private void paintPicture()
        {
            int mandelNumber;
            int selectedColorSet = comboBox2.SelectedIndex;
            int imageWidth = pictureBox1.Width;
            int imageHeight = pictureBox1.Height;

            leftTopX = centerX - (imageWidth / 2 * scale);
            leftTopY = centerY - (imageHeight / 2 * scale);

            // For every individual pixel in the picturebox calculate the mandelbrot
            // number and draw it in the mandelbrot Bitmap
            for (int x = 0; x < imageWidth; x++)
            {
                for (int y = 0; y < imageHeight; y++)
                {
                    mandelNumber = calculateMandelNumber(leftTopX + x * scale, leftTopY + y * scale);
                    drawMandelbrot(x, y, mandelNumber, selectedColorSet);
                }
            }

            // Show the mandelbrot Bitmap in the Picturebox
            pictureBox1.Image = mandelbrot;
        }

        // Speech based commands
        private void initializeSpeechRecognition()
        {
            string[] words = new string[] { "left", "right", "top", "bottom", "zoom in", "zoom out" };
            
            foreach (string s in words)
            {
                recognitionEngine.RequestRecognizerUpdate();
                recognitionEngine.LoadGrammar(new Grammar(new GrammarBuilder(s)));
            }

            recognitionEngine.SpeechRecognized += recognitionEngine_SpeechRecognized;
            recognitionEngine.SetInputToDefaultAudioDevice();
            recognitionEngine.RecognizeAsync(RecognizeMode.Multiple);
        }

        // Draw the mandelbrot figure
        private void drawMandelbrot(int x, int y, int mandelNumber, int selectedColorSet)
        {
            // Pastel colors
            if (selectedColorSet == 0)
                mandelbrot.SetPixel(x, y, 
                    Color.FromArgb(255 / ((mandelNumber % 3) + 1), 255 / ((mandelNumber % 3) + 1), 255));
            // Candyland colors
            else if (selectedColorSet == 1)
                mandelbrot.SetPixel(x, y, 
                    Color.FromArgb(250, mandelNumber % 2 * 255, 250 / ((mandelNumber % 5) + 1)));
            // Lion colors
            else if (selectedColorSet == 2)
                mandelbrot.SetPixel(x, y, 
                    Color.FromArgb(255, 255 / ((mandelNumber % 4) + 1), 0));
            // Checkmate colors
            else if (selectedColorSet == 3)
                mandelbrot.SetPixel(x, y, 
                    (mandelNumber % 2 == 0) ? Color.White : Color.Black);
            // Rastafari colors
            else if (selectedColorSet == 4)
            {
                if (mandelNumber % 3 == 1)
                    mandelbrot.SetPixel(x, y, Color.Red);
                else if (mandelNumber % 3 == 2)
                    mandelbrot.SetPixel(x, y, Color.Green);
                else
                    mandelbrot.SetPixel(x, y, Color.Yellow);
            }
            // Rainbow colors
            else if (selectedColorSet == 5)
            {

                if (mandelNumber % 8 == 1)
                    mandelbrot.SetPixel(x, y, Color.FromArgb(255, 0, 0));
                else if (mandelNumber % 8 == 2)
                    mandelbrot.SetPixel(x, y, Color.FromArgb(255, 127, 0));
                else if (mandelNumber % 8 == 3)
                    mandelbrot.SetPixel(x, y, Color.FromArgb(255, 255, 0));
                else if (mandelNumber % 8 == 4)
                    mandelbrot.SetPixel(x, y, Color.FromArgb(0, 255, 0));
                else if (mandelNumber % 8 == 5)
                    mandelbrot.SetPixel(x, y, Color.FromArgb(0, 0, 255));
                else if (mandelNumber % 8 == 6)
                    mandelbrot.SetPixel(x, y, Color.FromArgb(75, 0, 130));
                else if (mandelNumber % 8 == 7)
                    mandelbrot.SetPixel(x, y, Color.FromArgb(143, 0, 255));
                else
                    mandelbrot.SetPixel(x, y, Color.FromArgb(255, 255, 255));
            }
        }

        // Method that calculates the mandelnumber for a given x and y using 
        // the Mandelbrot algorithm
        private int calculateMandelNumber(double x, double y)
        {
            double a, b, ra, rb;
            int result;
            a = 0;
            b = 0;
            result = 0;

            while (distanceToOrigin(a, b) < 2.0)
            {
                if (result == max)
                    break;

                // The Mandelbrot algorithm
                ra = a * a - b * b + x;
                rb = 2 * a * b + y;

                a = ra;
                b = rb;
                result++;
            }

            return result;
        }

        // Calculate the shortest distance from point (x, y) to point (0, 0)
        // using Pythagoras
        private static double distanceToOrigin(double x, double y)
        {
            return Math.Sqrt(x * x + y * y);
        }

        private void getFormValues()
        {
            centerX = Convert.ToDouble(textBox1.Text);
            centerY = Convert.ToDouble(textBox2.Text);
            scale   = Convert.ToDouble(textBox3.Text);
            max     = Convert.ToInt32 (textBox4.Text);
        }

        private void setFormValues()
        {
            textBox1.Text = centerX.ToString();
            textBox2.Text = centerY.ToString();
            textBox3.Text = scale.ToString();
            textBox4.Text = max.ToString();
        }

        private void changeSettings(double centerX, double centerY, double scale, int max)
        {
            this.centerX = centerX;
            this.centerY = centerY;
            this.scale = scale;
            this.max = max;
        }

        #region EventHandlers

        // Speech recognition for the commands
        private void recognitionEngine_SpeechRecognized(object sender, SpeechRecognizedEventArgs srea)
        {
            string command = srea.Result.Text;

            if (command == "left")
                centerX -= (125 * scale);
            else if (command == "right")
                centerX += (125 * scale);
            else if (command == "top")
                centerY -= (125 * scale);
            else if (command == "bottom")
                centerY += (125 * scale);
            else if (command == "zoom in")
                scale /= 2;
            else if (command == "zoom out")
                scale *= 2;

            textBox5.Text = command;
            setFormValues();
            paintPicture();
        }

        private void zoom(object sender, MouseEventArgs mea)
        {
            this.centerX = this.leftTopX + mea.X * this.scale;
            this.centerY = this.leftTopY + mea.Y * this.scale;

            if (mea.Button == MouseButtons.Left)
                this.scale = this.scale / 2;
            else if (mea.Button == MouseButtons.Right)
                this.scale = this.scale * 2;

            setFormValues();
            paintPicture();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            getFormValues();
            paintPicture();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int selectedIndex = comboBox1.SelectedIndex;

            if (selectedIndex == 0)
                changeSettings(-0.6, 0, 0.007, 200);
            else if (selectedIndex == 1)
                changeSettings(0.3634638671875, -0.589392578125, 1.708984375E-06, 295);
            else if (selectedIndex == 2)
                changeSettings(-1.76391357421875, 0.0282250976562505, 2.44140625E-06, 400);
            else if (selectedIndex == 3)
                changeSettings(-1.26814331054687, 0.414217453002929, 1.52587890625E-07, 250);
            else if (selectedIndex == 4)
                changeSettings(-1.98165582847596, 2.53677368206578E-07, 2.53677368206578E-07, 200);
            else if (selectedIndex == 5)
                changeSettings(-1.98165634573997, 1.0424554349739E-06, 4.95463609778473E-10, 150);
            setFormValues();
            paintPicture();
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            paintPicture();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                initializeSpeechRecognition();
                MessageBox.Show("The following voice commands are supported: \ntop, bottom, left, right, zoom in, zoom out.",
                    "Voice control activated", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                recognitionEngine.RecognizeAsyncCancel();
                textBox5.Text = "";
            }
        }

        #endregion

    }
}