using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;
using CountdownV2.Common;

namespace CountdownV2.Controls
{
    public class ConfettiCanvas : Control
    {
        private readonly List<List<ConfettiParticle>> _allParticles = [];
        private const int MaxParticleSets = 7;
        private readonly DispatcherTimer _animationTimer = new() { Interval = TimeSpan.FromMilliseconds(16) };
        private long _lastTime;

        public ConfettiCanvas()
        {
            var stopwatch = Stopwatch.StartNew();
            _animationTimer.Tick += (_, _) => Animate(stopwatch);
        }

        private void Animate(Stopwatch stopwatch)
        {
            var now = stopwatch.ElapsedMilliseconds;
            var deltaTime = (now - _lastTime) / 1000.0;
            _lastTime = now;
            Update(deltaTime);
            InvalidateVisual();
        }

        public void StartAnimation(Point origin)
        {
            if (_allParticles.Count >= MaxParticleSets)
                _allParticles.RemoveAt(0);

            _allParticles.Add(GenerateParticles(origin));
            _animationTimer.Start();
        }

        private static List<ConfettiParticle> GenerateParticles(Point origin)
        {
            var newParticles = new List<ConfettiParticle>();
            var rnd = new Random();
            const double spreadAngleDegrees = 360;
            const double minAngle = -spreadAngleDegrees / 2;

            for (var i = 0; i < 100; i++)
            {
                var angleDegrees = minAngle + rnd.NextDouble() * spreadAngleDegrees;
                var angleRadians = angleDegrees * Math.PI / 180;
                var speed = rnd.NextDouble() * 100 + 50;
                var velocity = new Point(Math.Cos(angleRadians) * speed, Math.Sin(angleRadians) * speed);
                var rotationSpeed = rnd.NextDouble() * 360 - 180;
                var color = Color.FromArgb(255, (byte)rnd.Next(256), (byte)rnd.Next(256), (byte)rnd.Next(256));
                newParticles.Add(new ConfettiParticle(origin, velocity, rotationSpeed, color, true));
            }

            return newParticles;
        }

        public override void Render(DrawingContext context)
        {
            base.Render(context);
            foreach (var particle in _allParticles.SelectMany(particleSet => particleSet))
            {
                using (context.PushTransform(GetTransformMatrix(particle)))
                {
                    context.DrawRectangle(new SolidColorBrush(particle.Color), null, new Rect(particle.Position, new Size(10, 10)));
                }
                particle.Update(0.016);
            }
        }

        private static Matrix GetTransformMatrix(ConfettiParticle particle)
        {
            var angleRad = particle.Rotation * (Math.PI / 180.0);
            var sin = Math.Sin(angleRad);
            var cos = Math.Cos(angleRad);
            return Matrix.CreateTranslation(-particle.Position.X, -particle.Position.Y) *
                   new Matrix(cos, sin, -sin, cos, 0, 0) *
                   Matrix.CreateTranslation(particle.Position.X, particle.Position.Y);
        }

        private void Update(double deltaTime)
        {
            foreach (var particleSet in _allParticles)
            {
                foreach (var particle in particleSet)
                {
                    particle.Update(deltaTime);
                    if (particle.Position.Y > Bounds.Height)
                        particle.IsActive = false;
                }
                particleSet.RemoveAll(p => !p.IsActive);
            }
            _allParticles.RemoveAll(pSet => pSet.Count == 0);
        }
    }
}