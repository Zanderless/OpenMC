using System;
using System.Numerics;
using static System.Net.Mime.MediaTypeNames;

namespace OpenMC
{
    public class Camera
    {
        public Vector3 Position { get; set; }
        public Vector3 Forward { get; set; }
        public Vector3 Up { get; set; }
        public Vector3 Right { get; set; }

        public float Yaw { get; set; } = -90f;
        public float Pitch { get; set; }

        public float AspectRatio { get; set; }

        private float fov = 60f;

        public Camera(Vector3 position, Vector3 forward, Vector3 up, float aspectRatio)
        {
            Position = position;
            Forward = forward;
            Up = up;
            AspectRatio = aspectRatio;
        }

        public void ModifyDirection(float xOffset, float yOffset)
        {
            Yaw += xOffset;
            Pitch -= yOffset;

            Pitch = Math.Clamp(Pitch, -89f, 89f);

            var camDirection = Vector3.Zero;
            camDirection.X = MathF.Cos(MathUtils.DegreesToRadians(Yaw)) * MathF.Cos(MathUtils.DegreesToRadians(Pitch));
            camDirection.Y = MathF.Sin(MathUtils.DegreesToRadians(Pitch));
            camDirection.Z = MathF.Sin(MathUtils.DegreesToRadians(Yaw)) * MathF.Cos(MathUtils.DegreesToRadians(Pitch));
            Forward = Vector3.Normalize(camDirection);
            Right = Vector3.Normalize(Vector3.Cross(Forward, Up));

        }

        public Matrix4x4 GetViewMatrix()
        {
            return Matrix4x4.CreateLookAt(Position, Position + Forward, Up);
        }

        public Matrix4x4 GetProjectionMatrix()
        {
            return Matrix4x4.CreatePerspectiveFieldOfView(MathUtils.DegreesToRadians(fov), AspectRatio, 0.1f, 100.0f);
        }
    }
}
