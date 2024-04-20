using Avalonia;
using Avalonia.Media;

namespace CountdownV2.Common;

public class ConfettiParticle(Point position, Point velocity, double rotationSpeed, Color color, bool isActive)
{
    public Point Position { get; set; } = position;
    public Point Velocity { get; set; } = velocity;
    public double Rotation { get; set; } = 0;
    public double RotationSpeed { get; set; } = rotationSpeed;
    public Color Color { get; set; } = color;

    public bool IsActive { get; set; } = isActive;

    public void Update(double deltaTime)
    {
        Position = new Point(Position.X + Velocity.X * deltaTime, Position.Y + Velocity.Y * deltaTime);
        Rotation += RotationSpeed * deltaTime;
    }
}