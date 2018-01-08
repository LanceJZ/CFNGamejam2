using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using XnaModel = Microsoft.Xna.Framework.Graphics.Model;
using System.Collections.Generic;
using System;
using Engine;

namespace CFNGamejam2.Entities
{
    public class Bomb : AModel
    {
        Explode Explosion;
        Timer LifeTimer;


        public Bomb(Game game) : base(game)
        {
            Explosion = new Explode(game);
            LifeTimer = new Timer(game, 5);

        }

        public override void Initialize()
        {

            base.Initialize();
        }

        public override void LoadContent()
        {
            LoadModel("Bomb");
        }

        public override void BeginRun()
        {
            base.BeginRun();

        }

        public override void Update(GameTime gameTime)
        {
            if (LifeTimer.Elapsed)
            {
                Active = false;
            }

            if (Position.Y < -4)
            {
                HitTarget();
            }

            if (Rotation.Z < -MathHelper.PiOver2)
            {
                RotationAcceleration.Z = 0;
                RotationVelocity.Z = 0;
            }

            base.Update(gameTime);
        }

        public override void Spawn(Vector3 position, Vector3 rotation, Vector3 velocity)
        {
            position.Y -= 5;
            base.Spawn(position, rotation, velocity);
            Acceleration.Y = -50;
            RotationAcceleration.Z = -MathHelper.Pi;
            LifeTimer.Reset();
        }

        public void HitTarget()
        {
            Active = false;
            Velocity.Y = 0;
            Acceleration.Y = 0;
            Explosion.Spawn(Position, Radius * 0.5f, 100);
        }
    }
}
