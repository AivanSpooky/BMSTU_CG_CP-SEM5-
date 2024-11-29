using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace test
{
    public partial class Form1 : Form
    {
        private Random random = new Random();
        private void DisableUI()
        {
            btn_simulate.Enabled = false;
            btn_add_fig.Enabled = false;
            btn_add_ident.Enabled = false;
            btnGridSettings.Enabled = false;
        }
        private void EnableUI()
        {
            btn_simulate.Enabled = true;
            btn_add_fig.Enabled = true;
            btn_add_ident.Enabled = true;
            btnGridSettings.Enabled = true;
        }
        private async void StartResearch()
        {
            if (researchStart)
            {
                DisableUI();
                await Task.Run(() => ConductResearch());
                EnableUI();
                researchStart = false;
            }
        }

        private void ConductResearch()
        {
            FigureType[] figureTypes = new FigureType[]
            {
                FigureType.Cube,
                FigureType.Sphere,
                FigureType.HexPrism,
                FigureType.Cylinder
            };
            IndentationType[] indentationTypes = new IndentationType[]
            {
                IndentationType.Cube,
                IndentationType.Sphere,
                IndentationType.HexPrism,
                IndentationType.Cylinder
            };

            // Папка для сохранения результатов
            string resultsFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ResearchResults");
            try
            {
                Directory.CreateDirectory(resultsFolder);
            }
            catch (Exception ex)
            {
                Invoke(new Action(() =>
                {
                    MessageBox.Show($"Не удалось создать папку для результатов: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }));
                return;
            }
            if (firstPart)
                // Первое исследование: добавление объектов без лунок
                foreach (var figureType in figureTypes)
                {
                    string fileName = Path.Combine(resultsFolder, $"Objects_{figureType}.txt");
                    using (StreamWriter writer = new StreamWriter(fileName))
                    {
                        for (int n = 1; n <= 100; n += 5)
                        {
                            // Очищаем сцену от всех объектов, кроме площадки
                            Invoke(new Action(() => scene.RemoveAllObjectsExcept("Ground")));

                            // Добавляем n объектов
                            for (int i = 0; i < n; i++)
                            {
                                Mesh mesh = CreateFigure(figureType, i);
                                Invoke(new Action(() => scene.AddObject(mesh)));
                            }

                            // Перестраиваем площадку после добавления объектов
                            Invoke(new Action(() =>
                            {
                                scene.RemoveObjectByName("Ground");
                                Mesh ground = Mesh.CreateGridPlane(
                                    new Vector3(0, 0, 0),
                                    GPO.gridWidth,
                                    GPO.gridDepth,
                                    GPO.cellSize,
                                    Color.Green,
                                    indentations,
                                    indentationEdges
                                );
                                ground.Name = "Ground";
                                scene.AddObject(ground);
                            }));

                            // Измеряем время отрисовки
                            Stopwatch stopwatch = Stopwatch.StartNew();
                            Invoke(new Action(() => Render()));
                            stopwatch.Stop();

                            // Записываем результаты в микросекундах
                            double elapsedTimeUs = stopwatch.Elapsed.TotalMilliseconds * 1000;
                            writer.WriteLine($"{n} {elapsedTimeUs}");
                        }
                    }
                }
            if (secondPart)
                // Второе исследование: добавление лунок
                foreach (var indentationType in indentationTypes)
                {
                    string fileName = Path.Combine(resultsFolder, $"Indentations_{indentationType}.txt");
                    using (StreamWriter writer = new StreamWriter(fileName))
                    {
                        for (int n = 1; n <= 20; n += 3)
                        {
                            // Очищаем сцену и список лунок
                            Invoke(new Action(() =>
                            {
                                scene.RemoveAllObjectsExcept("Ground");
                                ClearAllIndentations();
                            }));

                            // Инициализируем сетку 30x30 с размером клетки 0.25
                            Invoke(new Action(() =>
                            {
                                InitializeGrid(30, 30, 0.25f); // Изменено с 100x100 на 30x30
                            }));

                            // Добавляем n лунок последовательно
                            PopulateIndentationsSequentially(n, indentationType);

                            // Измеряем время отрисовки
                            Stopwatch stopwatch = Stopwatch.StartNew();
                            Invoke(new Action(() => Render()));
                            stopwatch.Stop();

                            // Записываем результаты в микросекундах
                            double elapsedTimeUs = stopwatch.Elapsed.TotalMilliseconds * 1000;
                            Console.WriteLine($"{n} {elapsedTimeUs}");
                            writer.WriteLine($"{n} {elapsedTimeUs}");
                        }
                    }
                }
        }

        private void ClearAllIndentations()
        {
            foreach (var indentation in indentations.ToList())
                FreeOccupiedCells(indentation);
            indentations.Clear();
        }
        #region creation funcs
        private Mesh CreateFigure(FigureType type, int index)
        {
            Vector3 position = new Vector3(
                (float)(random.NextDouble() * GPO.gridWidth * GPO.cellSize),
                5f + index, // Чтобы объекты не перекрывались
                (float)(random.NextDouble() * GPO.gridDepth * GPO.cellSize)
            );
            Color color = Color.FromArgb(random.Next(256), random.Next(256), random.Next(256));

            Mesh mesh = null;
            float size = GPO.cellSize;
            float height = GPO.cellSize;

            switch (type)
            {
                case FigureType.Cube:
                    mesh = Mesh.CreateCube(position, size, color);
                    break;
                case FigureType.Sphere:
                    mesh = Mesh.CreateSphere(position, size / 2, GPO.SPCPA, GPO.SPCPA, color);
                    break;
                case FigureType.HexPrism:
                    mesh = Mesh.CreateHexPrism(position, size / 2, height, color);
                    break;
                case FigureType.Cylinder:
                    mesh = Mesh.CreateCylinder(position, size / 2, height, GPO.SPCPA, color);
                    break;
            }

            mesh.Name = $"{type}_{index}";
            return mesh;
        }

        private void PopulateIndentationsSequentially(int totalIndentations, IndentationType specificType)
        {
            // Список типов лунок для разнообразия (используем specificType)
            IndentationType[] indentationTypes = new IndentationType[]
            {
        specificType
            };

            // Список возможных размеров лунок (ширина, глубина, высота)
            var possibleSizes = new List<(int width, int depth, int height)>
    {
        (1, 1, 1),
        (2, 2, 1),
        (3, 3, 2)
    };

            int indentationsAdded = 0;

            foreach (var size in possibleSizes)
            {
                for (int z = 0; z <= GPO.gridDepth - size.depth && indentationsAdded < totalIndentations; z++)
                {
                    for (int x = 0; x <= GPO.gridWidth - size.width && indentationsAdded < totalIndentations; x++)
                    {
                        if (CanPlaceIndentation(x, z, size.width, size.depth))
                        {
                            // Выбираем тип лунки (используем specificType)
                            IndentationType type = indentationTypes[random.Next(indentationTypes.Length)];

                            // Добавляем лунку
                            if (type == IndentationType.Cube || type == IndentationType.Sphere)
                            {
                                AddIndentation(x, z, size.width, size.depth, type);
                            }
                            else
                            {
                                AddIndentation(x, z, size.width, size.depth, size.height, type);
                            }

                            indentationsAdded++;

                            // Обновляем карту занятых клеток
                            for (int i = x; i < x + size.width; i++)
                            {
                                for (int j = z; j < z + size.depth; j++)
                                {
                                    gridOccupied[i, j] = true;
                                }
                            }

                            // Перестраиваем площадку после добавления лунки
                            Invoke(new Action(() =>
                            {
                                scene.RemoveObjectByName("Ground");
                                Mesh ground = Mesh.CreateGridPlane(
                                    new Vector3(0, 0, 0),
                                    GPO.gridWidth,
                                    GPO.gridDepth,
                                    GPO.cellSize,
                                    Color.Green,
                                    indentations,
                                    indentationEdges
                                );
                                ground.Name = "Ground";
                                scene.AddObject(ground);
                                Render();
                            }));

                            if (indentationsAdded >= totalIndentations)
                                break;
                        }
                    }
                }
            }

            if (indentationsAdded < totalIndentations)
            {
                Invoke(new Action(() =>
                {
                    MessageBox.Show($"Добавлено {indentationsAdded} лунок из запрошенных {totalIndentations}. Возможно, не хватило места для всех лунок.", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }));
            }
        }
        #endregion
    }
}
