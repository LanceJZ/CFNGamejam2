﻿using Microsoft.Xna.Framework;
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
        GameLogic RefGameLogic;
        List<Bomb> Bombs;
        AModel[] Wings = new AModel[2];
        Explode Explosion;

        Timer FlapTimer;
        Timer GlideTimer;
        Timer DropBombTimer;
        Timer ChangeHeadingTimer;
        Timer QuackTimer;

        SoundEffect QuackSound;
        SoundEffect DropBombSound;

        Mode CurrentMode;

        Vector3 CurrentHeading = Vector3.Zero;
        float Speed = 100;

        public Duck(Game game, GameLogic gameLogic) : base(game)
        {
            RefGameLogic = gameLogic;

            for (int i = 0; i < 2; i++)
                Wings[i] = new AModel(Game);

            Bombs = new List<Bomb>();
            Explosion = new Explode(game);

            FlapTimer = new Timer(game, 3);
            GlideTimer = new Timer(game, 5);
            DropBombTimer = new Timer(game, 2);
            ChangeHeadingTimer = new Timer(game);
            QuackTimer = new Timer(game);

            LoadContent();
        }

        public override void Initialize()
        {

            base.Initialize();
        }

        public void LoadContent()
        {
            LoadModel("DuckBody");

            Wings[0].LoadModel("DuckLWing");
            Wings[1].LoadModel("DuckRWing");

            QuackSound = LoadSoundEffect("Duck");
            DropBombSound = LoadSoundEffect("DropBomb");

            BeginRun();
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
                    if (Position.Y > 100)
                    {
                        ChangeToGlide();
                        return;
                    }

                    FlapWings();
                    break;
                case Mode.Glide:
                    if (Position.Y < 30)
                    {
                        ChangeToFlap();
                        return;
                    }

                    if (GlideTimer.Elapsed && Position.Y < 60)
                    {
                        ChangeToFlap();
                        return;
                    }

                    if (RefGameLogic.CurrentMode == GameState.InPlay)
                    {
                        if (QuackTimer.Elapsed)
                        {
                            QuackTimer.Reset(Core.RandomMinMax(6.5f, 15.1f));
                            QuackSound.Play(0.25f, 1, 1);
                        }
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
                ChangeHeadingTimer.Reset(Core.RandomMinMax(3.25f, 6.25f));
            }

            CheckOffMap();
            CheckHit();

            base.Update(gameTime);
        }

        public override void Spawn(Vector3 position)
        {
            base.Spawn(position);

            QuackTimer.Reset(Core.RandomMinMax(3.5f, 15.1f));
        }

        void CheckHit()
        {
            foreach(TankShot shot in RefGameLogic.RefPlayer.ShotsRef)
            {
                if (shot.Active)
                {
                    if (SphereIntersect(shot))
                    {
                        Active = false;
                        Explosion.DefuseColor = new Vector3(0.713f, 0.149f, 0.286f);
                        Explosion.Spawn(Position, 5, 30);
                        shot.HitTarget();
                        RefGameLogic.AddToScore(100);
                    }
                }
            }
        }

        void ChangeToFlap()
        {
            FlapTimer.Reset(Core.RandomMinMax(2.5f, 8.5f));
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
            GlideTimer.Reset(Core.RandomMinMax(5.5f, 10.5f));
            CurrentMode = Mode.Glide;

            for (int i = 0; i < 2; i++)
            {
                Wings[i].Rotation.X = 0;
                Wings[i].RotationVelocity.X = 0;
            }
        }

        void ChangeHeading()
        {
            if (RefGameLogic.RefPlayer.Active)
            {
                CurrentHeading = RefGameLogic.RefPlayer.Position;
            }
            else
            {
                int border = RefGameLogic.RefGround.TheBorder;
                CurrentHeading = new Vector3(Core.RandomMinMax(-border, border),
                    0, Core.RandomMinMax( border - 500, border + 500));
            }
        }

        void CheckOffMap()
        {
            if (Vector2.Distance(new Vector2(Position.X, Position.Z), Vector2.Zero) >
                RefGameLogic.RefGround.TheBorder)
            {
                ChangeHeading();
            }
        }

        void DoesDuckDropBomb()
        {
            if (Vector3.Distance(Position, RefGameLogic.RefPlayer.Position) < 50 + Position.Y)
            {
                DropBombTimer.Reset(Core.RandomMinMax(2.5f, 5.5f));
                DropBomb();
            }
        }

        void DropBomb()
        {
            if (!RefGameLogic.RefPlayer.Active)
                return;

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
                thisOne = Bombs.Count;
                Bombs.Add(new Bomb(Game, RefGameLogic));
            }

            Bombs[thisOne].Spawn(Position, Rotation, Velocity / 4);
            DropBombSound.Play(0.5f, 1, 1);
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
