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
        SoundEffect HitSound;

        public TankShot(Game game, SoundEffect hitSound) : base(game)
        {
            LifeTimer = new Timer(game);
            Explosion = new Explode(game);
            HitSound = hitSound;

            LoadContent();
        }

        public override void Initialize()
        {

            base.Initialize();
        }

        public void LoadContent()
        {
            LoadModel("Core/Cube");

            BeginRun();
        }

        public override void BeginRun()
        {
            base.BeginRun();

            Radius = 1;
        }

        public override void Update(GameTime gameTime)
        {
            if (LifeTimer.Elapsed)
                Active = false;

            if (Position.Y < -4)
            {
                HitTarget();
            }

            base.Update(gameTime);
        }

        public override void Spawn(Vector3 position, Vector3 rotation, Vector3 velocity)
        {
            LifeTimer.Reset(1.75f);
            base.Spawn(position, rotation, velocity);
            Acceleration.Y = -30;
        }

        public void HitTarget()
        {
            Active = false;
            Explosion.Spawn(Position, Radius * 0.25f, 30);
            HitSound.Play();
        }


    }
}
