using System.Numerics;

public class Camera
{
    public static readonly float zOffset = 10; // Увеличьте zOffset, чтобы камера была видна
    public static readonly Vector3 defaultTarget = new Vector3(0, 0, 0);
    public static readonly Vector3 defaultUp = Vector3.UnitY;
    public Matrix4x4 ViewMatrix { get; private set; }
    public Matrix4x4 ProjectionMatrix { get; private set; }
    public Vector3 Position { get; set; }
    public Vector3 Target { get; set; }
    public Vector3 Up { get; private set; }
    public float FieldOfView { get; set; }
    public float AspectRatio { get; set; }
    public float NearClip { get; set; }
    public float FarClip { get; set; }

    private float rotationAngle = 0; // угол поворота в плоскости XZ

    public Camera(Vector3 position, Vector3 target, Vector3 up, float fieldOfView, float aspectRatio, float nearClip, float farClip)
    {
        Position = position;
        Target = target;
        Up = up;
        FieldOfView = fieldOfView;
        AspectRatio = aspectRatio;
        NearClip = nearClip;
        FarClip = farClip;
        UpdateViewMatrix();
        UpdateProjectionMatrix();
    }

    public void UpdateViewMatrix()
    {
        ViewMatrix = Matrix4x4.CreateLookAt(Position, Target, Up);
    }

    public void UpdateProjectionMatrix()
    {
        ProjectionMatrix = Matrix4x4.CreatePerspectiveFieldOfView(FieldOfView, AspectRatio, NearClip, FarClip);
    }

    public void Rotate(float angle)
    {
        // Обновление угла поворота
        rotationAngle += angle;

        // Вычисление нового направления камеры
        var direction = Target - Position;

        // Создание матрицы вращения вокруг оси Y
        var rotationMatrix = Matrix4x4.CreateRotationY(angle);

        // Применение вращения к направлению
        direction = Vector3.Transform(direction, rotationMatrix);

        // Установка новой целевой точки
        Target = Position + direction;

        // Обновление матрицы вида
        UpdateViewMatrix();
    }

    public void SetInitialView()
    {
        // Устанавливаем начальное положение камеры под углом 45 градусов
        Position = new Vector3(0, 5, 10); // Позиция выше и дальше от цели
        Target = defaultTarget;
        Up = Vector3.UnitY;
        UpdateViewMatrix();
    }

    public void SetViewFront()
    {
        Position = new Vector3(0, 0, -zOffset);
        Target = defaultTarget;
        Up = defaultUp;
        UpdateViewMatrix();
    }

    public void SetViewBack()
    {
        Position = new Vector3(0, 0, zOffset);
        Target = defaultTarget;
        Up = defaultUp;
        UpdateViewMatrix();
    }

    public void SetViewLeft()
    {
        Position = new Vector3(-zOffset, 0, 0);
        Target = defaultTarget;
        Up = defaultUp;
        UpdateViewMatrix();
    }

    public void SetViewRight()
    {
        Position = new Vector3(zOffset, 0, 0);
        Target = defaultTarget;
        Up = defaultUp;
        UpdateViewMatrix();
    }

    public void SetViewTop()
    {
        Position = new Vector3(0, zOffset, 0);
        Target = defaultTarget;
        Up = defaultUp;
        UpdateViewMatrix();
    }

    public void SetViewBottom()
    {
        Position = new Vector3(0, -zOffset, 0);
        Target = defaultTarget;
        Up = defaultUp;
        UpdateViewMatrix();
    }
}
