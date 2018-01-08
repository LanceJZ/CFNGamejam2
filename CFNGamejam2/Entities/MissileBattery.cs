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
        GameLogic GameLogicRef;
        AModel Turret;
        List<Missile> Missiles;
        Timer ChangeTargetTimer;
        Timer FireTimer;

        Vector3 CurrentTarget = Vector3.Zero;

        public MissileBattery(Game game, GameLogic gameLogic) : base(game)
        {
            GameLogicRef = gameLogic;
            Turret = new AModel(game);
            Missiles = new List<Missile>();
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
            Turret.AddAsChildOf(this, true, false);
            Turret.MatrixUpdate();
        }

        public override void Update(GameTime gameTime)
        {
            if (AimedAtTargetY(CurrentTarget, Turret.WorldRotation.Y, 0.05f))
            {
                Turret.RotationVelocity.Y = 0;
                if (FireTimer.Elapsed)
                    FireMissiles();
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


            base.Update(gameTime);
        }

        public override void Spawn(Vector3 position)
        {
            base.Spawn(position);

            Turret.MatrixUpdate();
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
                    Missiles.Add(new Missile(Game));
                    thisOne[i] = Missiles.Count - 1;
                }

                Missiles[thisOne[i]].Spawn(launchTubes[i], Turret.WorldRotation, target, CurrentTarget);
            }
        }

        void ChangeTarget()
        {
            CurrentTarget = GameLogicRef.PlayerRef.Position;
        }

    }
}
