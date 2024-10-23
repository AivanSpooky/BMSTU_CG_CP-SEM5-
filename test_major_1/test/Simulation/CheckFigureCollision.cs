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
                float indentationXStart = indentation.GridX * cellSize;
                float indentationXEnd = (indentation.GridX + indentation.Width) * cellSize;
                float indentationZStart = indentation.GridZ * cellSize;
                float indentationZEnd = (indentation.GridZ + indentation.Depth) * cellSize;

                // Проверяем, находится ли фигура внутри границ лунки
                if (figurePosition.X >= indentationXStart && figurePosition.X <= indentationXEnd &&
                    figurePosition.Z >= indentationZStart && figurePosition.Z <= indentationZEnd)
                {
                    isInIndentation = true;
                    stopTimer();

                    // Проверяем тип фигуры и тип лунки
                    if ((int)mesh.Type == (int)indentation.Type)
                    {
                        // Проверяем размер
                        float figureSize = (mesh.Type == FigureType.Cube) ? 1f : 1f; // Предполагаемый размер фигуры
                        float indentationSizeX = indentation.Width * cellSize;
                        float indentationSizeZ = indentation.Depth * cellSize;

                        if (Math.Abs(figureSize - indentationSizeX) < 0.1f &&
                            Math.Abs(figureSize - indentationSizeZ) < 0.1f)
                        {
                            // Фигура попала в свою лунку
                            MessageBox.Show($"Фигура {mesh.Name} попала в свою лунку.");
                            indentationsToRemove.Add(indentation);
                        }
                        else
                        {
                            // Фигура попала в лунку не своего размера
                            MessageBox.Show($"Фигура {mesh.Name} попала в лунку не своего размера.");
                        }
                    }
                    else
                    {
                        // Фигура попала в чужую лунку
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
