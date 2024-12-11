using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace ChartsAppCGCP
{
    public partial class Form1 : Form
    {
        // ChatGPT Catharsis
        private string cubesFilePath = @"Objects_Cube.txt";
        private string spheresFilePath = @"Objects_Sphere.txt";
        private string hexPrismsFilePath = @"Objects_HexPrism.txt";
        private string cylindersFilePath = @"Objects_Cylinder.txt";
        private bool objs = true;
        private string cubesIndsFilePath = @"Indentations_Cube.txt";
        private string spheresIndsFilePath = @"Indentations_Sphere.txt";
        private string hexPrismsIndsFilePath = @"Indentations_HexPrism.txt";
        private string cylindersIndsFilePath = @"Indentations_Cylinder.txt";

        public Form1()
        {
            InitializeComponent();
            InitializeChart();
            LoadAndPlotData();
        }
        /// <summary>
        /// Инициализация настроек графика в зависимости от флага objs.
        /// </summary>
        private void InitializeChart()
        {
            chart1.Series.Clear();
            chart1.ChartAreas.Clear();

            var chartArea = new ChartArea("MainArea");
            chart1.ChartAreas.Add(chartArea);

            // Настройка осей в зависимости от флага objs
            if (objs)
            {
                chartArea.AxisX.Title = "Количество тел";
                chartArea.AxisY.Title = "Время (мкс)";
            }
            else
            {
                chartArea.AxisX.Title = "Количество лунок";
                chartArea.AxisY.Title = "Время (мкс)";
            }

            chartArea.AxisX.Interval = 5;
            chartArea.AxisY.Minimum = 0;

            chartArea.AxisX.Minimum = 0;
            chartArea.AxisX.Maximum = objs ? 100 : 20;

            chart1.Legends.Clear();
            var legend = new Legend("Legend")
            {
                Docking = Docking.Top
            };
            chart1.Legends.Add(legend);
        }

        /// <summary>
        /// Загрузка и отображение данных на графике в зависимости от флага objs.
        /// </summary>
        private void LoadAndPlotData()
        {
            if (objs)
            {
                AddSeries("Кубы", cubesFilePath, Color.Red);
                AddSeries("Сферы", spheresFilePath, Color.Blue);
                AddSeries("Шестигранные призмы", hexPrismsFilePath, Color.Green);
                AddSeries("Цилиндры", cylindersFilePath, Color.Purple);
            }
            else
            {
                AddSeries("Лунки-Кубы", cubesIndsFilePath, Color.Red);
                AddSeries("Лунки-Сферы", spheresIndsFilePath, Color.Blue);
                AddSeries("Лунки-Шестигранные призмы", hexPrismsIndsFilePath, Color.Green);
                AddSeries("Лунки-Цилиндры", cylindersIndsFilePath, Color.Purple);
            }
        }

        /// <summary>
        /// Добавление серии данных на график из указанного файла.
        /// </summary>
        /// <param name="seriesName">Название серии</param>
        /// <param name="filePath">Путь к файлу с данными</param>
        /// <param name="seriesColor">Цвет серии</param>
        private void AddSeries(string seriesName, string filePath, Color seriesColor)
        {
            if (!File.Exists(filePath))
            {
                MessageBox.Show($"Файл {filePath} не найден.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var series = new Series(seriesName)
            {
                ChartType = SeriesChartType.Line,
                BorderWidth = 2,
                Color = seriesColor,
                MarkerStyle = MarkerStyle.Circle,
                MarkerSize = 7
            };

            var lines = File.ReadAllLines(filePath);
            foreach (var line in lines)
            {
                var parts = line.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length != 2)
                    continue;

                string timeStr = parts[1].Replace(',', '.');
                if (int.TryParse(parts[0], out int count) &&
                    double.TryParse(timeStr, NumberStyles.Any, CultureInfo.InvariantCulture, out double time))
                {
                    Console.WriteLine($"Фигура: {seriesName}, Количество: {count}, Время: {time} мкс");
                    series.Points.AddXY(count, time);
                }
            }

            chart1.Series.Add(series);
        }

        /// <summary>
        /// Пример использования: переключение между графиками объектов и лунок.
        /// Добавьте, например, кнопку на форму и привяжите к ней этот метод.
        /// </summary>
        private void ToggleGraph(bool showObjects)
        {
            objs = showObjects;
            InitializeChart();
            LoadAndPlotData();
        }

        private void btnShowObjects_Click(object sender, EventArgs e)
        {
            ToggleGraph(true);
        }

        private void btnShowIndentations_Click(object sender, EventArgs e)
        {
            ToggleGraph(false);
        }
    }
}
