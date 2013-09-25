using System;
using System.Drawing;
using System.Windows.Forms;

// needed for circumventing XP - SAPI interaction bug 
using System.Threading;

// additionally needed libraries
using System.Speech.Synthesis;
using System.Speech.Recognition;
using System.Net;
using System.IO;

namespace Mandelbrot
{
    public partial class Mandelbrot : Form
    {
        // Declaration of the globals
        Bitmap mandelbrot;

        double centerX;
        double centerY;
        double scale;
        int max;

        double leftTopX;
        double leftTopY;

        SpeechRecognitionEngine recognitionEngine = new SpeechRecognitionEngine();
        SpeechSynthesizer synthesizer = new SpeechSynthesizer();

        // Constructor
        public Mandelbrot() 
        {
            InitializeComponent(); 
            SpeechRecognition();
            
            mandelbrot = new Bitmap(pictureBox1.Width, pictureBox1.Height);

            GetFormValues();
            DrawMandelbrot();

            pictureBox1.MouseClick += new MouseEventHandler(Zoom);
        }

        private void SpeechRecognition()
        {
            string[] words = new string[] { "zoom in", "zoom out" };
            // start new thread to control for XP - SAPI bug
            Thread t1 = new Thread(delegate()
            {
                // call methods of SpeechRecognitionEngine object here
                foreach (string s in words)
                {
                    recognitionEngine.RequestRecognizerUpdate();
                    recognitionEngine.LoadGrammar(new Grammar(new GrammarBuilder(s)));
                }
                recognitionEngine.SpeechRecognized += recognitionEngine_SpeechRecognized;
                recognitionEngine.SetInputToDefaultAudioDevice();
                recognitionEngine.RecognizeAsync(RecognizeMode.Multiple);
            });

            // set right state for thread and start
            t1.SetApartmentState(ApartmentState.MTA);
            t1.Start();
        }

        void recognitionEngine_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            String word = e.Result.Text;
            if (e.Result.Text == "zoom in")
            {
                this.scale = this.scale / 2;
                DrawMandelbrot();
            }
            else if (e.Result.Text == "zoom out")
            {
                this.scale = this.scale * 2;
               DrawMandelbrot();
            }
        }

        // Calculate and draw the mandelbrot figure
        private void DrawMandelbrot()
        {
            int n;
            int selectedIndex = comboBox2.SelectedIndex;

            this.leftTopX = centerX - (pictureBox1.Width / 2 * this.scale);
            this.leftTopY = centerY - (pictureBox1.Height / 2 * this.scale);

            // For every x and y run the mandelbrot calculation
            for (int x = 0; x < pictureBox1.Width; x++)
            {
                for (int y = 0; y < pictureBox1.Height; y++)
                {
                    n = CalculateMandelNumber(this.leftTopX + x * this.scale, this.leftTopY + y * this.scale);
                    if (selectedIndex == 0)
                    {
                        mandelbrot.SetPixel(x, y, Color.FromArgb(255 / ((n % 3) + 1), 255 / ((n % 3) + 1), 255)); 
                    }
                    else if (selectedIndex == 1)
                    {
                        mandelbrot.SetPixel(x, y, Color.FromArgb(250, n % 2 * 255, 250 / ((n % 5) + 1)));
                    }
                    else if (selectedIndex == 2)
                    {
                        mandelbrot.SetPixel(x, y, Color.FromArgb(255, 255 / ((n % 4) + 1), 0));
                    }
                    else if (selectedIndex == 3)
                    {
                        mandelbrot.SetPixel(x, y, (n % 2 == 0) ? Color.White : Color.Black);
                    }


                    //mandelbrot.SetPixel(x, y, Color.FromArgb(0, 250/((n%5)+1), 0));
                    //mandelbrot.SetPixel(x, y, Color.FromArgb(250/((n%5)+1), 0, 0));
                    //mandelbrot.SetPixel(x, y, Color.FromArgb(246 / ((n % 3) + 1), 246 / ((n % 3) + 1), 246));
                    //mandelbrot.SetPixel(x, y, Color.FromArgb(250, n % 2 * 255, 250 / ((n % 5) + 1)));
                    /*
                    if (n % 3 == 1)
                        mandelbrot.SetPixel(x, y, Color.Green);
                    else if (n % 3 == 2)
                        mandelbrot.SetPixel(x, y, Color.Red);
                    else
                        mandelbrot.SetPixel(x, y, Color.Yellow);
                    */
                    else if (selectedIndex == 4)
                    {
                        if (n % 8 == 1)
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
            }

            pictureBox1.Image = mandelbrot;
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
                {
                    break;
                }

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

        private void Zoom(object sender, MouseEventArgs e)
        {
            this.centerX = this.leftTopX + e.X * this.scale;
            this.centerY = this.leftTopY + e.Y * this.scale;

            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                this.scale = this.scale / 2;
            }
            else if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                this.scale = this.scale * 2;
            }

            SetFormValues();
            DrawMandelbrot();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            GetFormValues();
            DrawMandelbrot();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int selectedIndex = comboBox1.SelectedIndex;

            if (selectedIndex == 0)
            {
                this.centerX = 0;
                this.centerY = 0;
                this.scale = 0.01;
                this.max = 200;
            }
            else if (selectedIndex == 1)
            {
                this.centerX = 0.3634638671875;
                this.centerY = -0.589392578125;
                this.scale = 1.708984375E-06;
                this.max = 295;
            }
            else if (selectedIndex == 2)
            {
                this.centerX = -1.76391357421875;
                this.centerY = 0.0282250976562505;
                this.scale = 2.44140625E-06;
                this.max = 400;
            }
            else if (selectedIndex == 3)
            {
                this.centerX = -1.26814331054687;
                this.centerY = 0.414217453002929;
                this.scale = 1.52587890625E-07;
                this.max = 250;
            }

            SetFormValues();
            DrawMandelbrot();
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

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            DrawMandelbrot();
        }

        

        
    }
}
