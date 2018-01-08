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
    enum Mode
    {
        Flap,
        Glide
    }

    public class Duck : AModel
    {
        GameLogic GameLogicRef;
        List<Bomb> Bombs;
        AModel[] Wings = new AModel[2];

        Timer FlapTimer;
        Timer GlideTimer;
        Timer DropBombTimer;
        Timer ChangeHeadingTimer;

        Mode CurrentMode;

        Vector3 CurrentHeading = Vector3.Zero;
        float Speed = 100;

        public Duck(Game game, GameLogic gameLogic) : base(game)
        {
            GameLogicRef = gameLogic;

            for (int i = 0; i < 2; i++)
               Wings[i] = new AModel(game);

            Bombs = new List<Bomb>();

            FlapTimer = new Timer(game, 3);
            GlideTimer = new Timer(game, 5);
            DropBombTimer = new Timer(game, 2);
            ChangeHeadingTimer = new Timer(game);
        }

        public override void Initialize()
        {

            base.Initialize();
        }

        public override void LoadContent()
        {
            LoadModel("DuckBody");
            Wings[0].LoadModel("DuckLWing");
            Wings[1].LoadModel("DuckRWing");
        }

        public override void BeginRun()
        {
            base.BeginRun();

            for (int i = 0; i < 2; i++)
                Wings[i].AddAsChildOf(this, true, false);

            Wings[0].Position.Z = -4;
            Wings[1].Position.Z = 4;

            Wings[0].Rotation.X = -MathHelper.PiOver4;
            Wings[1].Rotation.X = MathHelper.PiOver4;

            Wings[0].RotationVelocity.X = 6;
            Wings[1].RotationVelocity.X = -6;
        }

        public override void Update(GameTime gameTime)
        {
            switch (CurrentMode)
            {
                case Mode.Flap:
                    if (Position.Y > 130)
                    {
                        ChangeToGlide();
                        return;
                    }

                    FlapWings();
                    break;
                case Mode.Glide:
                    if (Position.Y < 20)
                    {
                        ChangeToFlap();
                        return;
                    }

                    if (GlideTimer.Elapsed && Position.Y < 100)
                    {
                        ChangeToFlap();
                    }

                    break;
            }

            Velocity.X = VelocityFromAngleY(Rotation.Y, Speed).X;
            Velocity.Z = VelocityFromAngleY(Rotation.Y, Speed).Z;

            RotationVelocity.Y = AimAtTargetY(CurrentHeading, Rotation.Y, MathHelper.PiOver2);

            if (DropBombTimer.Elapsed)
            {
                DoesDuckDropBomb();
            }

            if (ChangeHeadingTimer.Elapsed)
            {
                ChangeHeading();
                ChangeHeadingTimer.Reset(Services.RandomMinMax(3.25f, 6.25f));
            }

            CheckOffMap();

            base.Update(gameTime);
        }

        void ChangeToFlap()
        {
            FlapTimer.Reset(Services.RandomMinMax(2.5f, 8.5f));
            CurrentMode = Mode.Flap;
            Wings[0].RotationVelocity.X = 6;
            Wings[1].RotationVelocity.X = -6;
            Acceleration.Y = 5;
            Velocity.Y = 0;
        }

        void ChangeToGlide()
        {
            Acceleration.Y = -1.5f;
            Velocity.Y = 0;
            GlideTimer.Reset(Services.RandomMinMax(5.5f, 10.5f));
            CurrentMode = Mode.Glide;

            for (int i = 0; i < 2; i++)
            {
                Wings[i].Rotation.X = 0;
                Wings[i].RotationVelocity.X = 0;
            }
        }

        void ChangeHeading()
        {
            CurrentHeading = GameLogicRef.PlayerRef.Position;
        }

        void CheckOffMap()
        {
            if (Vector2.Distance(new Vector2(Position.X, Position.Z), Vector2.Zero) >
                GameLogicRef.GroundRef.TheBorder)
            {
                ChangeHeading();
            }
        }

        void DoesDuckDropBomb()
        {
            if (Vector3.Distance(Position, GameLogicRef.PlayerRef.Position) < 50 + Position.Y)
            {
                DropBombTimer.Reset(Services.RandomMinMax(2.5f, 5.5f));
                DropBomb();
            }
        }

        void DropBomb()
        {
            bool spawnNew = true;
            int thisOne = 0;

            foreach(Bomb bomb in Bombs)
            {
                if (!bomb.Active)
                {
                    spawnNew = false;
                    break;
                }

                thisOne++;
            }

            if (spawnNew)
            {
                Bombs.Add(new Bomb(Game));
                thisOne = Bombs.Count - 1;
            }

            Bombs[thisOne].Spawn(Position, Rotation, Velocity / 4);
        }

        void FlapWings()
        {
            if (FlapTimer.Elapsed)
            {
                ChangeToGlide();
                return;
            }

            for (int i = 0; i < 2; i++)
            {
                if (Wings[i].Rotation.X > MathHelper.PiOver4 || Wings[i].Rotation.X < -MathHelper.PiOver4)
                {
                    Wings[i].RotationVelocity.X *= -1;
                }

                if (Wings[i].Rotation.X > MathHelper.PiOver4)
                {
                    Wings[i].Rotation.X = MathHelper.PiOver4;
                }

                if (Wings[i].Rotation.X < -MathHelper.PiOver4)
                {
                    Wings[i].Rotation.X = -MathHelper.PiOver4;
                }
            }
        }
    }
}
