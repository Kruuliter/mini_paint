using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MiniPaint
{
    public partial class Form1 : Form
    {
        Color CurrentColor = Color.Black;//стандартно цвет кисти будет чёрным
        bool isPresset;//флаг, зажата мышь или нет
        Point CurrentPoint;//x2y2
        Point PrevPoint;//x1y1
        Graphics graph;//создаёт область, в которой можно будет рисовать
        Pen p;//цвет ручки
        string figures = "Кисть";//определение, какая фигура используется, стандартно кисть
        int x1, x2, y1, y2, width, heigth, x, y;//координаты фигуры, высота и длина
        List<Uniforms> uniforms = new List<Uniforms>();//сохраняет координаты, высоту и длину фигуры

        private void button6_Click(object sender, EventArgs e)
        {
            //очищаем холст
            graph.Clear(Color.White);
            uniforms.Clear();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            figures = ((Button)sender).Text;//забираем текст кнопки
        }

        private void кистьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            figures = ((ToolStripMenuItem)sender).Text;//забираем текст меню, как у кнопки
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            pictureBox3.BackColor = CurrentColor;
            p = new Pen(CurrentColor);
            graph = panel1.CreateGraphics();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult D = colorDialog1.ShowDialog();//вызываем окно выбора цвета
            if (System.Windows.Forms.DialogResult.OK == D) //если была нажата кнопка "ОК"
            {
                CurrentColor = colorDialog1.Color; //забираем выбранный цвет
                pictureBox3.BackColor = CurrentColor;//рисуем задний фон полученным цветом в маленькой картинке
                p = new Pen(CurrentColor);//изменяем цвет линий
            }
        }
        /// <summary>
        /// Линия
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        public void lins(int x1, int y1, int x2, int y2)
        {
            this.x1 = x1;
            this.y1 = y1;
            this.x2 = x2;
            this.y2 = y2;
        }
        /// <summary>
        /// рисует фигуры, которые уже были нарисованы
        /// </summary>
        private void Otris()
        {
            foreach (Uniforms uni in uniforms)
            {
                switch (uni.str)
                {
                    case "Кисть":
                        {
                            brushes(uni.X1, uni.Y1, uni.X2, uni.Y2);
                            graph.DrawLine(new Pen(uni.pens), x1, y1, x2, y2);
                        }
                        break;
                    case "Круг":
                        {
                            Circls(uni.X1, uni.Y1, uni.X2, uni.Y2);
                            graph.DrawEllipse(new Pen(uni.pens), x, y, width, heigth);
                        }
                        break;
                    case "Квадрат":
                        {
                            squars(uni.X1, uni.Y1, uni.X2, uni.Y2);
                            graph.DrawRectangle(new Pen(uni.pens), x, y, width, heigth);
                        }
                        break;
                    case "Линия":
                        {
                            lins(uni.X1, uni.Y1, uni.X2, uni.Y2);
                            graph.DrawLine(new Pen(uni.pens), x1, y1, x2, y2);
                        }
                        break;
                }
            }
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            Otris();
        }

        private void panel1_MouseUp(object sender, MouseEventArgs e)
        {
            isPresset = false;//запрещаем рисовать когда кнопка мыши не зажата
            uniforms.Add(new Uniforms
            {
                X1 = PrevPoint.X,
                Y1 = PrevPoint.Y,
                X2 = CurrentPoint.X,
                Y2 = CurrentPoint.Y,
                pens = CurrentColor,
                str = figures
            });
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            //кнопка мыши зажата
            if (isPresset == true)
            {
                if (figures == "Кисть")
                {
                    PrevPoint = CurrentPoint;//записываем координаты, с которой была зажата кнопка мыши
                }

                Drow(new Pen(Color.White));

                CurrentPoint = e.Location;//записываем новые координаты

                Drow();

                if (figures == "Кисть")
                {
                    uniforms.Add(new Uniforms
                    {
                        X1 = PrevPoint.X,
                        Y1 = PrevPoint.Y,
                        X2 = CurrentPoint.X,
                        Y2 = CurrentPoint.Y,
                        pens = CurrentColor,
                        str = figures
                    });
                }
                Otris();
            }
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            isPresset = true;//если зажали кнопку мыши, то рисуем
            if (figures == "Кисть")
                CurrentPoint = e.Location;
            else
                PrevPoint = e.Location;//записывает координаты х,у на котором была зажата кнопка мыши
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            DialogResult D = colorDialog1.ShowDialog();//вызываем окно выбора цвета
            if (System.Windows.Forms.DialogResult.OK == D) //если была нажата кнопка "ОК"
            {
                CurrentColor = colorDialog1.Color; //забираем выбранный цвет
                pictureBox3.BackColor = CurrentColor;//рисуем задний фон полученным цветом в маленькой картинке
                p = new Pen(CurrentColor);//изменяем цвет линий
            }
        }
        /// <summary>
        /// Сохранение файла в .xml формате
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void сохранитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Filter = ".xml | *.xml";
            saveFileDialog1.ShowDialog();
            if (saveFileDialog1.FileName != "")
            {
                //в открытом файле на каждой новой строчке сохраняет фигуру
                using (StreamWriter sw = new StreamWriter(saveFileDialog1.FileName, true, System.Text.Encoding.Default))
                {
                    foreach (Uniforms uni in uniforms)
                    {
                        sw.Write(uni.pens.Name + " ");
                        sw.Write(uni.str + " ");
                        sw.Write(uni.X1 + " ");
                        sw.Write(uni.Y1 + " ");
                        sw.Write(uni.X2 + " ");
                        sw.WriteLine(uni.Y2);
                    }
                }
            }
        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            graph.Dispose();
            this.Close();
        }
        /// <summary>
        /// Открытие файла .xml формата
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void открытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = ".xml | *.xml";
            openFileDialog1.ShowDialog();
            using (StreamReader sr = new StreamReader(openFileDialog1.FileName, System.Text.Encoding.Default))
            {
                //в открытом файле читает строчку и записывает новую фигуру
                string rst;
                while ((rst = sr.ReadLine()) != null)
                {
                    string[] fig = rst.Split(' ');
                    uniforms.Add(new Uniforms
                    {
                        pens = Color.FromName(fig[0]),
                        str = fig[1],
                        X1 = int.Parse(fig[2]),
                        Y1 = int.Parse(fig[3]),
                        X2 = int.Parse(fig[4]),
                        Y2 = int.Parse(fig[5])

                    });
                }
            }
            //после записи всех фигур в uniforms очищает холст и рисует фигуры, которые записао
            graph.Clear(Color.White);
            Otris();
        }
        /// <summary>
        /// Фигура круг
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        public void Circls(int x1, int y1, int x2, int y2)
        {
            this.x1 = x1;
            this.y1 = y1;
            this.x2 = x2;
            this.y2 = y2;
            x = x1;
            y = y1;
            if (x > x2) x = x2;
            if (y > y2) y = y2;
            width = Math.Abs(x2 - x1);
            heigth = Math.Abs(y2 - y1);
        }
        /// <summary>
        /// Фигура кисть
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        public void brushes(int x1, int y1, int x2, int y2)
        {
            this.x1 = x1;
            this.y1 = y1;
            this.x2 = x2;
            this.y2 = y2;
        }
        /// <summary>
        /// Фигура квадрат
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        public void squars(int x1, int y1, int x2, int y2)
        {
            x = x1;
            y = y1;
            if (x > x2) x = x2;
            if (y > y2) y = y2;
            width = Math.Abs(x2 - x1);
            heigth = Math.Abs(y2 - y1);
        }

        public Form1()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Рисует фигуру, которую выбрал пользователь
        /// </summary>
        private void Drow()
        {
            switch (figures)
            {
                case "Кисть":
                    {
                        brushes(PrevPoint.X, PrevPoint.Y, CurrentPoint.X, CurrentPoint.Y);
                        graph.DrawLine(p, x1, y1, x2, y2);
                    }
                    break;
                case "Круг":
                    {
                        Circls(PrevPoint.X, PrevPoint.Y, CurrentPoint.X, CurrentPoint.Y);
                        graph.DrawEllipse(p, x, y, width, heigth);
                    }
                    break;
                case "Квадрат":
                    {
                        squars(PrevPoint.X, PrevPoint.Y, CurrentPoint.X, CurrentPoint.Y);
                        graph.DrawRectangle(p, x, y, width, heigth);
                    }
                    break;
                case "Линия":
                    {
                        lins(PrevPoint.X, PrevPoint.Y, CurrentPoint.X, CurrentPoint.Y);
                        graph.DrawLine(p, x1, y1, x2, y2);
                    }
                    break;
            }
        }
        /// <summary>
        /// Рисует фигуру, которую выбрал пользователь определённым цветом
        /// </summary>
        /// <param name="pens"></param>
        private void Drow(Pen pens)
        {
            switch (figures)
            {
                case "Круг":
                    {
                        Circls(PrevPoint.X, PrevPoint.Y, CurrentPoint.X, CurrentPoint.Y);
                        graph.DrawEllipse(pens, x, y, width, heigth);
                    }
                    break;
                case "Квадрат":
                    {
                        squars(PrevPoint.X, PrevPoint.Y, CurrentPoint.X, CurrentPoint.Y);
                        graph.DrawRectangle(pens, x, y, width, heigth);
                    }
                    break;
                case "Линия":
                    {
                        lins(PrevPoint.X, PrevPoint.Y, CurrentPoint.X, CurrentPoint.Y);
                        graph.DrawLine(pens, x1, y1, x2, y2);
                    }
                    break;
            }
        }
    }
}
