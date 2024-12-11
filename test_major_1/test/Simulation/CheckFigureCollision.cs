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

            Vector3 figurePosition = mesh.Position;
            bool isInIndentation = false;

            foreach (var indentation in indentations)
            {
                // Compute indentation boundaries in world units
                float indentationXStart = indentation.GridX * GPO.cellSize;
                float indentationXEnd = (indentation.GridX + indentation.Width) * GPO.cellSize;
                float indentationZStart = indentation.GridZ * GPO.cellSize;
                float indentationZEnd = (indentation.GridZ + indentation.Depth) * GPO.cellSize;
                float oneLineSize = mesh.RadiusInCells * GPO.cellSize;

                if (figurePosition.X - oneLineSize >= indentationXStart && figurePosition.X + oneLineSize <= indentationXEnd &&
                    figurePosition.Z - oneLineSize >= indentationZStart && figurePosition.Z + oneLineSize <= indentationZEnd)
                {
                    isInIndentation = true;
                    stopTimer();

                    // Check if the figure type matches the indentation type
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
                            figureHeight = mesh.HeightInCells * 2;
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
                        bool sizesMatch = Math.Abs(figureWidth - indentation.Width) < 0.1f &&
                                          Math.Abs(figureDepth - indentation.Depth) < 0.1f;

                        // Only compare heights if the indentation has a non-zero height
                        if (indentation.Height > 0)
                        {
                            sizesMatch = sizesMatch && Math.Abs(figureHeight/2 - indentation.Height) < 0.1f;
                        }

                        Console.WriteLine($"{figureWidth} {indentation.Width} {figureDepth} {indentation.Depth} {figureHeight} {indentation.Height}");

                        if (sizesMatch)
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
                    return true; // Figure processed and should be removed
                }
            }

            if (!isInIndentation)
            {
                stopTimer();
                MessageBox.Show($"Фигура {mesh.Name} не попала в лунку.");
                startTimer();
                return true; // Figure processed and should be removed
            }

            return false; // Figure has not yet reached the indentation
        }
    }
}
