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
        Bitmap mandelbrot = new Bitmap(375, 406);
        double centerX,  centerY;
        double leftTopX, leftTopY;
        double scale;
        int max;

        // Class Constructor
        public Mandelbrot()
        {
            InitializeComponent();
            GetFormValues();
            LoopPictureBox();

            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 0;
            pictureBox1.MouseClick += Zoom;
        }

        // Speech based commands
        private void InitializeSpeechRecognition()
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

        // Loop through all pixels in the picturebox
        public void LoopPictureBox()
        {
            int n;
            int selectedIndex = comboBox2.SelectedIndex;

            leftTopX = centerX - (pictureBox1.Width / 2 * scale);
            leftTopY = centerY - (pictureBox1.Height / 2 * scale);

            // For every x and y run the mandelbrot calculation
            for (int x = 0; x < pictureBox1.Width; x++)
            {
                for (int y = 0; y < pictureBox1.Height; y++)
                {
                    n = CalculateMandelNumber(leftTopX + x * scale, leftTopY + y * scale);
                    DrawMandelbrot(x, y, n, comboBox2.SelectedIndex);
                }
            }

            pictureBox1.Image = mandelbrot;
        }

        // Draw the mandelbrot figure
        private void DrawMandelbrot(int x, int y, int n, int userChoice)
        {
            // Pastel colors
            if      (userChoice == 0)
                    mandelbrot.SetPixel(x, y, Color.FromArgb(255 / ((n % 3) + 1), 255 / ((n % 3) + 1), 255));
            // Candyland colors
            else if (userChoice == 1)
                    mandelbrot.SetPixel(x, y, Color.FromArgb(250, n % 2 * 255, 250 / ((n % 5) + 1)));
            // Lion colors
            else if (userChoice == 2)
                    mandelbrot.SetPixel(x, y, Color.FromArgb(255, 255 / ((n % 4) + 1), 0));
            // Checkmate colors
            else if (userChoice == 3)
                    mandelbrot.SetPixel(x, y, (n % 2 == 0) ? Color.White : Color.Black);
            // Rastafari colors
            else if (userChoice == 4)
            {
                    if      (n % 3 == 1)
                            mandelbrot.SetPixel(x, y, Color.Red);
                    else if (n % 3 == 2)
                            mandelbrot.SetPixel(x, y, Color.Green);
                    else
                            mandelbrot.SetPixel(x, y, Color.Yellow);
            }
            // Rainbow colors
            else if (userChoice == 5)
            {

                    if      (n % 8 == 1)
                            mandelbrot.SetPixel(x, y, Color.FromArgb(255, 0, 0));
                    else if (n % 8 == 2)
                            mandelbrot.SetPixel(x, y, Color.FromArgb(255, 127, 0));
                    else if (n % 8 == 3)
                            mandelbrot.SetPixel(x, y, Color.FromArgb(255, 255, 0));
                    else if (n % 8 == 4)
                            mandelbrot.SetPixel(x, y, Color.FromArgb(0, 255, 0));
                    else if (n % 8 == 5)
                            mandelbrot.SetPixel(x, y, Color.FromArgb(0, 0, 255));
                    else if (n % 8 == 6)
                            mandelbrot.SetPixel(x, y, Color.FromArgb(75, 0, 130));
                    else if (n % 8 == 7)
                            mandelbrot.SetPixel(x, y, Color.FromArgb(143, 0, 255));
                    else
                            mandelbrot.SetPixel(x, y, Color.FromArgb(255, 255, 255));
            }
        }

        // Method that calculates the Mandelnumber for a given x and y using 
        // the Mandelbrot algorithm
        private int CalculateMandelNumber(double x, double y)
        {
            double a, b, ra, rb;
            int result;
            a = 0;
            b = 0;
            result = 0;

            while (DistanceToOrigin(a, b) < 2.0)
            {
                if (result == this.max)
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
        private static double DistanceToOrigin(double x, double y)
        {
            return Math.Sqrt(x * x + y * y);
        }

        private void GetFormValues()
        {
            this.centerX = Convert.ToDouble(textBox1.Text);
            this.centerY = Convert.ToDouble(textBox2.Text);
            this.scale = Convert.ToDouble(textBox3.Text);
            this.max = Convert.ToInt32(textBox4.Text);
        }

        private void SetFormValues()
        {
            textBox1.Text = this.centerX.ToString();
            textBox2.Text = this.centerY.ToString();
            textBox3.Text = this.scale.ToString();
            textBox4.Text = this.max.ToString();
        }

        private void ChangeSettings(double centerX, double centerY, double scale, int max)
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

            textBox5.Text = srea.Result.Text;
            SetFormValues();
            LoopPictureBox();
        }


        private void Zoom(object sender, MouseEventArgs mea)
        {
            this.centerX = this.leftTopX + mea.X * this.scale;
            this.centerY = this.leftTopY + mea.Y * this.scale;

            if (mea.Button == MouseButtons.Left)
                this.scale = this.scale / 2;
            else if (mea.Button == MouseButtons.Right)
                this.scale = this.scale * 2;

            SetFormValues();
            LoopPictureBox();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            GetFormValues();
            LoopPictureBox();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int selectedIndex = comboBox1.SelectedIndex;

            if (selectedIndex == 0)
                ChangeSettings(-0.6, 0, 0.007, 200);
            else if (selectedIndex == 1)
                ChangeSettings(0.3634638671875, -0.589392578125, 1.708984375E-06, 295);
            else if (selectedIndex == 2)
                ChangeSettings(-1.76391357421875, 0.0282250976562505, 2.44140625E-06, 400);
            else if (selectedIndex == 3)
                ChangeSettings(-1.26814331054687, 0.414217453002929, 1.52587890625E-07, 250);
            else if (selectedIndex == 4)
                ChangeSettings(-1.98165582847596, 2.53677368206578E-07, 2.53677368206578E-07, 200);
            SetFormValues();
            LoopPictureBox();
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoopPictureBox();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                InitializeSpeechRecognition();
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
