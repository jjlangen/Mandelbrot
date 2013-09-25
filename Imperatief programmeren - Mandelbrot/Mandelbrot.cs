using System;
using System.Drawing;
using System.Windows.Forms;

namespace Mandelbrot
{
    public partial class Mandelbrot : Form
    {
        // Declaration of the globals
        Bitmap mandelbrot;
        double CenterX;
        double CenterY;
        double scale;
        double LeftTopX;
        double LeftTopY;
        int max;

        // Constructor
        public Mandelbrot() 
        {
            InitializeComponent();

            mandelbrot = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            DrawMandelbrot();
            pictureBox1.MouseClick += new MouseEventHandler(Zoom);
        }

        private void Zoom(object sender, MouseEventArgs e)
        {
            textBox1.Text = (this.LeftTopX + e.X * this.scale).ToString();
            textBox2.Text = (this.LeftTopY + e.Y * this.scale).ToString();
            
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                textBox3.Text = (this.scale / 2).ToString();
            }
            else if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                textBox3.Text = (this.scale * 2).ToString();
            }
            DrawMandelbrot();
        }

        // Calculate and draw the mandelbrot figure
        private void DrawMandelbrot()
        {
            int n;

            this.CenterX = Convert.ToDouble(textBox1.Text);
            this.CenterY = Convert.ToDouble(textBox2.Text);
            this.scale = Convert.ToDouble(textBox3.Text);
            this.LeftTopX = CenterX - (pictureBox1.Width / 2 * scale);
            this.LeftTopY = CenterY - (pictureBox1.Height / 2 * scale);
            this.max = Convert.ToInt32(textBox4.Text);

            // For every x and y run the mandelbrot calculation
            for (int x = 0; x < pictureBox1.Width; x++)
            {
                for (int y = 0; y < pictureBox1.Height; y++)
                {
                    n = CalculateMandelNumber(this.LeftTopX + x * this.scale, this.LeftTopY + y * this.scale);
                    mandelbrot.SetPixel(x, y, (n % 2 == 0) ? 
                        Color.White : Color.Black);
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

        private void button1_Click(object sender, EventArgs e)
        {
            DrawMandelbrot();
        }
    }
}
