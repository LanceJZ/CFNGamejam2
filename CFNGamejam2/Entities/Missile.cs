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
    public class Missile : AModel
    {
        GameLogic RefGameLogic;
        Explode Explosion;
        Timer LifeTimer;

        SoundEffect HitSound;

        Vector3 Target;

        public Missile(Game game, GameLogic gameLogic) : base(game)
        {
            RefGameLogic = gameLogic;
            LifeTimer = new Timer(game);
            Explosion = new Explode(game);

            LoadContent();
        }

        public override void Initialize()
        {

            base.Initialize();
        }

        public void LoadContent()
        {
            LoadModel("Missile");
            HitSound = LoadSoundEffect("MissileHit");

            BeginRun();
        }

        public override void BeginRun()
        {
            base.BeginRun();

            Radius = 2.5f;

        }

        public override void Update(GameTime gameTime)
        {
            if (LifeTimer.Elapsed)
                Active = false;

            if (Position.Y < -4)
            {
                HitTarget();
            }

            Acceleration.Y = -VelocityFromAngleY(Rotation.Z, 20).X;

            CheckCollusion();

            base.Update(gameTime);
        }

        public void Spawn(Vector3 position, Vector3 rotation, Vector3 velocity, Vector3 target)
        {
            Target = target;
            LifeTimer.Reset(3);
            base.Spawn(position, rotation, velocity);
            Acceleration = VelocityFromAngleY(rotation.Y, 80);
            RotationAcceleration.Z = -Vector3.Distance(position, target) * 0.00001f;
        }

        public void HitTarget()
        {
            Active = false;
            Explosion.Spawn(Position, Radius, 50);
            HitSound.Play();
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
