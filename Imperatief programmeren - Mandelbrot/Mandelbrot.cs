using System;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

class Mandelbrot : Form
{
    public Mandelbrot()
    {
        int l = CalculateMandelNumber(0.5, 0.8);
        Console.WriteLine(l.ToString());
    }

    private int CalculateMandelNumber(double x, double y)
    {
        double a, b, ra, rb;
        int result;
        a      = 0;
        b      = 0;
        result = 0;

        while (DistanceToOrigin(a, b) < 2.0)
        {
            ra = a * a - b * b + x;
            rb = 2 * a * b + y;
            a = ra;
            b = rb;
            result++;
        }

        return result;
    }

    private double DistanceToOrigin(double a, double b)
    {
        double result;
        result = Math.Sqrt(a * a + b * b);
        return result;
    }

    static void Main()
    {
        Application.Run(new Mandelbrot());
    }
}