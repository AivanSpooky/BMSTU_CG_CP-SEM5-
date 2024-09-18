using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace src
{
    using System;
    using System.Numerics;

    public class SimpleCamera
    {
        private float rotationAngle = 0; // Угол поворота камеры
        private float distance = 10; // Расстояние от центра сцены

        public Vector3 Position { get; private set; }
        public Vector3 Target { get; private set; }
        public Vector3 Up { get; private set; }

        public Matrix4x4 ViewMatrix { get; private set; }
        public Matrix4x4 ProjectionMatrix { get; private set; }

        public SimpleCamera(float distance, float fieldOfView, float aspectRatio, float nearClip, float farClip)
        {
            this.distance = distance;
            Target = Vector3.Zero;
            Up = Vector3.UnitY;

            // Установка начального положения камеры
            UpdatePosition();

            // Установка матриц проекции и вида
            ProjectionMatrix = Matrix4x4.CreatePerspectiveFieldOfView(fieldOfView, aspectRatio, nearClip, farClip);
            UpdateViewMatrix();
        }

        public void UpdateViewMatrix()
        {
            ViewMatrix = Matrix4x4.CreateLookAt(Position, Target, Up);
        }

        public void Rotate(float angle)
        {
            // Обновление угла поворота
            rotationAngle += angle;
            UpdatePosition();
            UpdateViewMatrix();
        }

        private void UpdatePosition()
        {
            // Вычисление новой позиции камеры по круговой траектории
            float x = (float)(distance * Math.Cos(rotationAngle));
            float z = (float)(distance * Math.Sin(rotationAngle));
            Position = new Vector3(x, 5, z); // Высота камеры фиксирована на уровне 5
        }
    }

}
