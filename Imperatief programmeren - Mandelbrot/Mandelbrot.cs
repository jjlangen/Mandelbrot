using System;
using System.Drawing;
using System.Windows.Forms;

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

        // Constructor
        public Mandelbrot() 
        {
            InitializeComponent();
            
            mandelbrot = new Bitmap(pictureBox1.Width, pictureBox1.Height);

            GetFormValues();
            DrawMandelbrot();

            pictureBox1.MouseClick += new MouseEventHandler(Zoom);
        }

        // Calculate and draw the mandelbrot figure
        private void DrawMandelbrot()
        {
            int n;

            this.leftTopX = centerX - (pictureBox1.Width / 2 * this.scale);
            this.leftTopY = centerY - (pictureBox1.Height / 2 * this.scale);

            // For every x and y run the mandelbrot calculation
            for (int x = 0; x < pictureBox1.Width; x++)
            {
                for (int y = 0; y < pictureBox1.Height; y++)
                {
                    n = CalculateMandelNumber(this.leftTopX + x * this.scale, this.leftTopY + y * this.scale);
                    //mandelbrot.SetPixel(x, y, (n % 2 == 0) ? Color.White : Color.Black);
                    if (n % 5 == 0)
                        mandelbrot.SetPixel(x, y, Color.White);
                    else if (n % 5 == 1)
                        mandelbrot.SetPixel(x, y, Color.LightBlue);
                    else if (n % 5 == 2)
                        mandelbrot.SetPixel(x, y, Color.Blue);
                    else if (n % 5 == 3)
                        mandelbrot.SetPixel(x, y, Color.DarkBlue);
                    else if (n % 5 == 4)
                        mandelbrot.SetPixel(x, y, Color.Black);
                    
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
                this.max = 300;
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

        

        
    }
}
