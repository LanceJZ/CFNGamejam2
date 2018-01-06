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
    public class TankShot : AModel
    {
        Explode Explosion;
        Timer LifeTimer;

        public TankShot(Game game) : base(game)
        {
            LifeTimer = new Timer(game);
            Explosion = new Explode(game);

            LoadContent();
            BeginRun();
        }

        public override void Initialize()
        {

            base.Initialize();
        }

        public override void LoadContent()
        {
            LoadModel("Core/Cube");
        }

        public override void BeginRun()
        {
            Radius = 1;
            base.BeginRun();
        }

        public override void Update(GameTime gameTime)
        {
            if (Active)
            {
                if (LifeTimer.Elapsed)
                    Active = false;

                if (Position.Y < -4)
                {
                    HitTarget();
                }
            }

            base.Update(gameTime);
        }

        public void Spawn(Vector3 position, Vector3 velocity)
        {
            LifeTimer.Reset(3);
            Position = position;
            Velocity = velocity;
            Acceleration.Y = -30;
            MatrixUpdate();
            Active = true;
        }

        public void HitTarget()
        {
            Active = false;
            Explosion.Spawn(Position, Radius * 0.25f, 30);
        }


    }
}
