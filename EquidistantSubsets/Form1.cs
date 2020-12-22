using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;


/* TEST:


7
-15
8
-9
6
-33
10
12
6
27
27
21
6
-21
27
6


*/

namespace EquidistantSubsets
{
    public partial class Form1 : Form
    {
        Graphics drawArea;
        SolidBrush brush;
        Pen pen;

        Point Center;
        Point Mid;
        Point Right; /* == */ Point Point2;
        Point Left; /* == Point1*/
        Point Point3;
        int Ymin;
        int Ymax;

        //массив всех точек
        List<Point> allPoints = new List<Point>() { };
        
        public Form1()
        {
            InitializeComponent();
            drawArea = pictureBox1.CreateGraphics();
        }

        public void DrawGrid()
        {
            brush = new SolidBrush(Color.Gray);
            drawArea.FillRectangle(brush, 0, 0, pictureBox1.Width, pictureBox1.Height);

            //нарисуем координатную сетку
            pen = new Pen(Color.Black);

            //вертикальные
            for (int i = 0; i < pictureBox1.Width; i += 5)
            {
                drawArea.DrawLine(pen, i, 0, i, pictureBox1.Height);
            }

            //горизонтальные
            for (int i = 0; i < pictureBox1.Height; i += 5)
            {
                drawArea.DrawLine(pen, 0, i, pictureBox1.Width, i);
            }

            //деление на четверти
            pen = new Pen(Color.Red, 2);
            drawArea.DrawLine(pen, Center.X, 0, Center.X, pictureBox1.Height);
            drawArea.DrawLine(pen, 0, Center.Y, pictureBox1.Width, Center.Y);
        }

        public void DrawPoints()
        {
            //чтение из файла
            string content = System.IO.File.ReadAllText(@"input.txt");
            textBox1.Text = content;

            //отметим точки
            pen = new Pen(Color.Orange, 2);
            string[] lines_points = textBox1.Lines;
            Point currentPoint;
            for (int i = 0; i < lines_points.Length; i += 2)
            {
                currentPoint = new Point(int.Parse(lines_points[i]), int.Parse(lines_points[i + 1]));
                allPoints.Add(currentPoint);
                drawArea.DrawEllipse(pen, Center.X + currentPoint.X * 5 - 1, Center.Y - currentPoint.Y * 5 - 1, 2, 2);

                if (Ymin > currentPoint.Y)
                    Ymin = currentPoint.Y;

                if (Ymax < currentPoint.Y)
                    Ymax = currentPoint.Y;
            }

            //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!отладка!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            Console.WriteLine("Ymin: " + Ymin);
            Console.WriteLine("Ymax: " + Ymax);
        }

        public void SortPoints()
        {
            //упорядочиваем список точек по возрастанию Х (если Х совпадает, то по У)
            //allPoints = allPoints.OrderBy(p => p.X).ThenBy(p => p.Y).ToList();
            BubbleSort();

            //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!отладка!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            Console.WriteLine("All Points:");
            for (int i = 0; i < allPoints.Count(); i++)
            {
                Console.WriteLine(allPoints[i].X + "; " + allPoints[i].Y);
            }
        }

        public void FindMid()
        {
            //находим среднюю точку
            Mid = allPoints[allPoints.Count() / 2];

            //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!отладка!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            Console.WriteLine("Mid: " + Mid);
        }

        public void FindLeft()
        {
            Point currentPoint;
            //находим X1
            currentPoint = Mid;
            int space = 1;
            while (currentPoint.X == Mid.X && allPoints.Count() / 2 >= space)
            {
                currentPoint = allPoints[allPoints.Count() / 2 - space];
                space++;
            }
            Left = currentPoint;
            //если пробежали весь список до начала
            if (allPoints.Count() / 2 - space < 0)
                Left = new Point(Mid.X - 1, Mid.Y - 1);

            //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!отладка!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            Console.WriteLine("Left == Point1: " + Left);
        }

        public void FindRight()
        {
            Point currentPoint;
            //находим X2
            currentPoint = Mid;
            int space = 1;
            while (currentPoint.X == Mid.X && allPoints.Count() / 2 + space < allPoints.Count())
            {
                currentPoint = allPoints[allPoints.Count() / 2 + space];
                space++;
            }
            Right = currentPoint;
            //если пробежали весь список до конца
            if (allPoints.Count() / 2 + space == allPoints.Count())
                Right = new Point(Mid.X + 1, Mid.Y + 1);

            //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!отладка!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            Console.WriteLine("Right: " + Right);
        }

        public void FindSegment()
        {
            //вычисляем последние две точки
            int Z = Math.Min(Mid.X - Left.X, Right.X - Mid.X);
            /* Если количество точек нечетно,
             * то искомая линия проходит через среднюю точку,
             * иначе над средней точкой */
            if (allPoints.Count() / 2 == 0)
            {
                if ((Mid.Y + Left.Y) % 2 == 0)
                    Point2 = new Point(Mid.X, (Mid.Y + Left.Y) / 2);
                else
                    Point2 = new Point(Mid.X, (Mid.Y + Left.Y) / 2 + 1);
                if (Z % 2 == 0)
                    Point3 = new Point(Mid.X + Z / 2, Mid.Y - Ymax + Ymin);
                else
                    Point3 = new Point(Mid.X + Z / 2 + 1, Mid.Y - Ymax + Ymin);
            }
            else
            {
                Point2 = new Point(Mid.X, Mid.Y);
                if (Z % 2 == 0)
                    Point3 = new Point(Mid.X + Z / 2, Mid.Y - Ymax + Ymin);
                else
                    Point3 = new Point(Mid.X + Z / 2 + 1, Mid.Y - Ymax + Ymin);
            }

            //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!отладка!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            Console.WriteLine("Point2: " + Point2);
            Console.WriteLine("Point3: " + Point3);
        }

        public void DrawSegment()
        {
            //рисуем нашу прямую
            pen = new Pen(Color.LimeGreen, 2);
            drawArea.DrawLine(pen, Center.X + Point2.X * 5, Center.Y - Point2.Y * 5, Center.X + Point3.X * 5, Center.Y - Point3.Y * 5);
        }

        public void Initialize()
        {
            Center = new Point(pictureBox1.Width / 2, pictureBox1.Height / 2);
            Ymin = pictureBox1.Height;
            Ymax = 0;
            allPoints = new List<Point>() { };

            DrawGrid();

            DrawPoints();
        }

        public void BubbleSort()
        {
            Point temp;
            for (int i = 0; i < allPoints.Count(); i++)
            {
                for (int j = i + 1; j < allPoints.Count(); j++)
                {
                    if (allPoints[i].X > allPoints[j].X)
                    {
                        temp = allPoints[i];
                        allPoints[i] = allPoints[j];
                        allPoints[j] = temp;
                    }
                    else
                        if (allPoints[i].X == allPoints[j].X && allPoints[i].Y > allPoints[j].Y)
                        {
                            temp = allPoints[i];
                            allPoints[i] = allPoints[j];
                            allPoints[j] = temp;
                        }
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Initialize();

            SortPoints();

            FindMid();

            FindLeft();

            FindRight();

            FindSegment();

            DrawSegment();
        }
    }
}
