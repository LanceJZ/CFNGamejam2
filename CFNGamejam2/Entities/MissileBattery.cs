using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using XnaModel = Microsoft.Xna.Framework.Graphics.Model;
using System.Collections.Generic;
using System.Linq;
using System;
using Engine;

namespace CFNGamejam2.Entities
{
    public class MissileBattery : AModel
    {
        GameLogic RefGameLogic;
        List<Missile> Missiles;
        Smoke TheSmoke;
        Explode TheExplosion;
        AModel HealthBar;
        AModel HealthBack;
        AModel Turret;
        Timer ChangeTargetTimer;
        Timer FireTimer;

        SoundEffect FireSound;
        SoundEffect ExplosionSound;

        Vector3 CurrentTarget = Vector3.Zero;
        int Health;

        public List<Missile> MissilesRef { get => Missiles; }

        public MissileBattery(Game game, GameLogic gameLogic) : base(game)
        {
            RefGameLogic = gameLogic;
            Missiles = new List<Missile>();
            TheSmoke = new Smoke(game);
            TheExplosion = new Explode(game);
            ChangeTargetTimer = new Timer(game);
            FireTimer = new Timer(game, 2);
            Turret = new AModel(Game);
            HealthBar = new AModel(Game);
            HealthBack = new AModel(Game);
            LoadContent();
        }

        public override void Initialize()
        {

            base.Initialize();

        }

        public void LoadContent()
        {
            LoadModel("MissileBatteryBase");
            Turret.LoadModel("MissileBatteryTurret");
            HealthBar.LoadModel("Core/Cube");
            HealthBack.LoadModel("Core/Cube");
            FireSound = LoadSoundEffect("MissileFire");
            ExplosionSound = LoadSoundEffect("GateExplode");

            BeginRun();
        }

        public override void BeginRun()
        {
            base.BeginRun();

            Turret.Position.Y = 20 + 19;
            Turret.AddAsChildOf(this, false, false);

            HealthBar.DefuseColor = new Vector3(0, 2, 0);
            HealthBar.ModelScale.X = 2;
            HealthBar.ModelScale.Z = 2;
            HealthBar.AddAsChildOf(this, true, false);
            HealthBack.Position.Y = 64;
            HealthBack.ModelScale.Y = 5;
            HealthBack.ModelScale.Z = 1;
            HealthBack.ModelScale.X = 1;
            HealthBack.DefuseColor = new Vector3(0.1f, 0.1f, 0.1f);
            HealthBack.AddAsChildOf(this, true, false);
            TheExplosion.DefuseColor = new Vector3(0.5f, 0.5f, 0.6f);
        }

        public override void Update(GameTime gameTime)
        {
            if (Turret.Active)
            {
                if (!RefGameLogic.RefPlayer.Active)
                {
                    CurrentTarget = new Vector3(0, 0, 2000);
                }

                if (AimedAtTargetY(CurrentTarget, Turret.WorldRotation.Y, 0.05f))
                {
                    Turret.RotationVelocity.Y = 0;
                    if (FireTimer.Elapsed)
                    {
                        if (Vector3.Distance(Position, CurrentTarget) < 500)
                            FireMissiles();
                    }
                }
                else
                {
                    Turret.RotationVelocity.Y = AimAtTargetY(CurrentTarget,
                        Turret.WorldRotation.Y, MathHelper.PiOver4 * 0.25f);
                }

                if (ChangeTargetTimer.Elapsed)
                {
                    ChangeTarget();
                    ChangeTargetTimer.Reset(Core.RandomMinMax(1.25f, 3.25f));
                }
            }

            CheckCollusion();

            base.Update(gameTime);
        }

        public override void Spawn(Vector3 position)
        {
            base.Spawn(position);
            Rotation.Y = -MathHelper.PiOver2;
            Turret.Active = true;
            TheSmoke.Kill();
            Health = 10;
            HealthBar.Active = true;
            HealthBack.Active = true;
            HealthBar.ModelScale.Y = Health / 2;
            HealthBar.Position.Y = 59.5f + HealthBar.ModelScale.Y;
        }

        void CheckCollusion()
        {
            foreach (TankShot shot in RefGameLogic.RefPlayer.ShotsRef)
            {
                if (shot.Active)
                {
                    if (SphereIntersect(shot))
                    {
                        Health -= 1;
                        HealthBar.ModelScale.Y = (Health / 2f);
                        HealthBar.Position.Y = 59.5f + HealthBar.ModelScale.Y;
                        shot.HitTarget();

                        if (Health <= 0)
                        {
                            Vector3 pos = Position;
                            pos.Y += 20;
                            TheExplosion.Spawn(Turret.WorldPosition, 10, 500);
                            TheSmoke.Spawn(pos, 10, 50);
                            Turret.Active = false;
                            RefGameLogic.AddToScore(25);
                            HealthBar.Active = false;
                            HealthBack.Active = false;
                            ExplosionSound.Play();
                        }

                        break;
                    }
                }
            }

            if (SphereIntersect(RefGameLogic.RefPlayer) ||
                SphereIntersect(RefGameLogic.RefPlayer.GunRef))
            {
                RefGameLogic.RefPlayer.Bumped(Position);
            }
        }

        void FireMissiles()
        {
            FireTimer.Reset();
            int[] thisOne = new int[2];
            Vector3[] launchTubes = new Vector3[2];
            Vector3 target = VelocityFromVectorsY(Turret.WorldPosition,
                CurrentTarget + new Vector3(0, 0, 0), 100);

            Vector3 tube = VelocityFromAngleY(Turret.WorldRotation.Y + MathHelper.PiOver2, 23);
            launchTubes[0] = Turret.WorldPosition + tube;
            launchTubes[1] = Turret.WorldPosition - tube;

            for (int i = 0; i < 2; i++)
            {
                bool spawnNew = true;
                thisOne[i] = 0;

                foreach (Missile missile in Missiles)
                {
                    if (!missile.Active)
                    {
                        spawnNew = false;
                        break;
                    }

                    thisOne[i]++;
                }

                if (spawnNew)
                {
                    thisOne[i] = Missiles.Count;
                    Missiles.Add(new Missile(Game, RefGameLogic));
                }

                Missiles[thisOne[i]].Spawn(launchTubes[i], Turret.WorldRotation, target, CurrentTarget);
                FireSound.Play();
            }
        }

        void ChangeTarget()
        {
            CurrentTarget = RefGameLogic.RefPlayer.Position;
        }

    }
}
