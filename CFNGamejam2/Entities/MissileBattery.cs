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
        AModel Turret;
        Timer ChangeTargetTimer;
        Timer FireTimer;

        Vector3 CurrentTarget = Vector3.Zero;

        public MissileBattery(Game game, GameLogic gameLogic) : base(game)
        {
            RefGameLogic = gameLogic;
            Missiles = new List<Missile>();
            TheSmoke = new Smoke(game);
            Turret = new AModel(game);
            ChangeTargetTimer = new Timer(game);
            FireTimer = new Timer(game, 2);
        }

        public override void Initialize()
        {

            base.Initialize();
        }

        public override void LoadContent()
        {
            LoadModel("MissileBatteryBase");
            Turret.LoadModel("MissileBatteryTurret");
        }

        public override void BeginRun()
        {
            base.BeginRun();

            Turret.Position.Y = 20 + 19;
            Turret.AddAsChildOf(this, false, false);
            Turret.MatrixUpdate();
            Rotation.Y = -MathHelper.PiOver2;
        }

        public override void Update(GameTime gameTime)
        {
            if (Turret.Active)
            {
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
                    ChangeTargetTimer.Reset(Services.RandomMinMax(1.25f, 3.25f));
                }
            }

            CheckCollusion();

            base.Update(gameTime);
        }

        public override void Spawn(Vector3 position)
        {
            base.Spawn(position);

            Turret.MatrixUpdate();
        }

        void CheckCollusion()
        {
            foreach (TankShot shot in RefGameLogic.RefPlayer.ShotsRef)
            {
                if (shot.Active)
                {
                    if (SphereIntersect(shot))
                    {
                        Vector3 pos = Position;
                        pos.Y += 20;
                        TheSmoke.Spawn(pos, 10, 50);
                        shot.Active = false;
                        Turret.Active = false;
                        break;
                    }
                }
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
            }
        }

        void ChangeTarget()
        {
            CurrentTarget = RefGameLogic.RefPlayer.Position;
        }

    }
}
