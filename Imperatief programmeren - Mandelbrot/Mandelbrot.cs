using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

class Mandelbrot : Form
{
    public Mandelbrot()
    {
        const int width = 300;
        const int height = 300;
        int n;
        double scale;
        Bitmap mandelbrot;
        PictureBox pictureBox;

        mandelbrot = new Bitmap(width, height);
        pictureBox = new PictureBox();
        pictureBox.Size = new Size(width, height);
        pictureBox.Image = mandelbrot;
        scale = 0.004;

        // For every x and y run the mandelbrot calculation
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                n = CalculateMandelNumber(x * scale, y * scale);
                mandelbrot.SetPixel(x, y, (n % 2 == 0) ? Color.White : Color.Black);
            }
        }

        this.Controls.Add(pictureBox);
    }

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

    private static double DistanceToOrigin(double x, double y)
    {
        // Calculate the shortest distance from point (x, y) to point (0, 0) using Pythagoras
        return Math.Sqrt(x * x + y * y);
    }

    public static void Main()
    {
        Application.Run(new Mandelbrot());
    }
}