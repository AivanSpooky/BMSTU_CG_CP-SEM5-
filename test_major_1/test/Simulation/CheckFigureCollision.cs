using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace test
{
    public partial class Form1: Form
    {
        private void stopTimer()
        {
            timer.Stop();
            isMessageBoxShown = true;
        }
        private void startTimer()
        {
            isMessageBoxShown = false;
            timer.Start();
        }
        private bool CheckFigureCollision(Mesh mesh, List<Mesh> meshesToRemove, List<Indentation> indentationsToRemove)
        {
            if (isMessageBoxShown)
                return false;
            // Получаем позицию фигуры
            Vector3 figurePosition = mesh.Position;

            // Флаг, показывающий, попала ли фигура в лунку
            bool isInIndentation = false;

            foreach (var indentation in indentations)
            {
                // Вычисляем границы лунки
                float indentationXStart = indentation.GridX * GPO.cellSize;
                float indentationXEnd = (indentation.GridX + indentation.Width) * GPO.cellSize;
                float indentationZStart = indentation.GridZ * GPO.cellSize;
                float indentationZEnd = (indentation.GridZ + indentation.Depth) * GPO.cellSize;

                // Проверяем, находится ли фигура внутри границ лунки
                if (figurePosition.X >= indentationXStart && figurePosition.X <= indentationXEnd &&
                    figurePosition.Z >= indentationZStart && figurePosition.Z <= indentationZEnd)
                {
                    isInIndentation = true;
                    stopTimer();

                    // Проверяем тип фигуры и тип лунки
                    if ((int)mesh.Type == (int)indentation.Type)
                    {
                        // Retrieve figure dimensions in cells
                        float figureWidth, figureDepth, figureHeight;
                        if (mesh.Type == FigureType.Cube)
                        {
                            figureWidth = figureDepth = mesh.SizeInCells;
                            figureHeight = mesh.SizeInCells;
                        }
                        else if (mesh.Type == FigureType.Cylinder || mesh.Type == FigureType.HexPrism)
                        {
                            figureWidth = figureDepth = mesh.RadiusInCells * 2;
                            figureHeight = mesh.HeightInCells;
                        }
                        else if (mesh.Type == FigureType.Sphere)
                        {
                            figureWidth = figureDepth = mesh.RadiusInCells * 2;
                            figureHeight = mesh.RadiusInCells * 2;
                        }
                        else
                        {
                            // Default values
                            figureWidth = figureDepth = figureHeight = 1;
                        }

                        // Compare figure and indentation sizes
                        if (Math.Abs(figureWidth - indentation.Width) < 0.1f &&
                            Math.Abs(figureDepth - indentation.Depth) < 0.1f &&
                            Math.Abs(figureHeight - indentation.Height) < 0.1f)
                        {
                            MessageBox.Show($"Фигура {mesh.Name} попала в свою лунку.");
                            indentationsToRemove.Add(indentation);
                        }
                        else
                        {
                            MessageBox.Show($"Фигура {mesh.Name} попала в лунку не своего размера.");
                        }
                    }
                    else
                    {
                        MessageBox.Show($"Фигура {mesh.Name} попала в чужую лунку.");
                    }
                    startTimer();
                    // Возвращаем true, чтобы указать, что фигура обработана и должна быть удалена
                    return true;
                }
            }

            if (!isInIndentation)
            {
                stopTimer();
                // Фигура не попала в лунку
                MessageBox.Show($"Фигура {mesh.Name} не попала в лунку.");
                // Возвращаем true, чтобы указать, что фигура обработана и должна быть удалена

                startTimer();
                return true;
            }

            // Если фигура не достигла площадки или не требует удаления
            return false;
        }
    }
}
