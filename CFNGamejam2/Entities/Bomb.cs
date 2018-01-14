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
        GameLogic RefGameLogic;
        Explode Explosion;
        Timer LifeTimer;

        SoundEffect ExplodeSound;

        public Bomb(Game game, GameLogic gameLogic) : base(game)
        {
            RefGameLogic = gameLogic;
            Explosion = new Explode(game);
            LifeTimer = new Timer(game, 5);

            LoadContent();
        }

        public override void Initialize()
        {

            base.Initialize();
        }

        public void LoadContent()
        {
            LoadModel("Bomb");
            ExplodeSound = LoadSoundEffect("BombExplode");

            BeginRun();
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

            CheckCollusion();

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
            ExplodeSound.Play();
        }

        void CheckCollusion()
        {
            if (RefGameLogic.RefPlayer.Active)
            {
                if (SphereIntersect(RefGameLogic.RefPlayer))
                {
                    HitTarget();
                    RefGameLogic.RefPlayer.HitDamage(1);
                }
            }
        }
    }
}
