using System;
using System.Drawing;
using System.Windows.Forms;

namespace Mandelbrot
{
    public partial class Mandelbrot : Form
    {
        // Declaration of the globals
        Bitmap mandelbrot;

        // Constructor
        public Mandelbrot() 
        {
            InitializeComponent();
            mandelbrot = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            DrawMandelbrot();

        }

        // Calculate and draw the mandelbrot figure
        private void DrawMandelbrot()
        {
            int n;
            double scale;
            scale = 0.004;

            // For every x and y run the mandelbrot calculation
            for (int x = 0; x < pictureBox1.Width; x++)
            {
                for (int y = 0; y < pictureBox1.Height; y++)
                {
                    n = CalculateMandelNumber(x * scale, y * scale);
                    mandelbrot.SetPixel(x, y, (n % 2 == 0) ? 
                        Color.White : Color.Black);
                }
            }

            pictureBox1.Image = mandelbrot;
        }

        // Method that calculates the Mandelnumber for a given x and y using 
        // the Mandelbrot algorithm
        private static int CalculateMandelNumber(double x, double y)
        {
            double a, b, ra, rb;
            int result;
            a = 0;
            b = 0;
            result = 0;

            while (DistanceToOrigin(a, b) < 2.0)
            {
                if (result == 100)
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
    }
}
