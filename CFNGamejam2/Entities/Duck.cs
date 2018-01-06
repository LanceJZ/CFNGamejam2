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
        AModel[] Wings = new AModel[2];

        Timer FlapTimer;
        Timer GlideTimer;

        Mode CurrentMode;

        float Speed = 100;

        public Duck(Game game) : base(game)
        {
            for (int i = 0; i < 2; i++)
               Wings[i] = new AModel(game);

            FlapTimer = new Timer(game, 3);
            GlideTimer = new Timer(game, 5);

            LoadContent();
            BeginRun();
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
            Rotation.Y = MathHelper.PiOver2;

            for (int i = 0; i < 2; i++)
                Wings[i].AddAsChildOf(this, true, false);

            Wings[0].Position.Z = -4;
            Wings[1].Position.Z = 4;

            Wings[0].Rotation.X = -MathHelper.PiOver4;
            Wings[1].Rotation.X = MathHelper.PiOver4;

            Wings[0].RotationVelocity.X = 6;
            Wings[1].RotationVelocity.X = -6;

            RotationVelocity.Y = 1;

            base.BeginRun();
        }

        public override void Update(GameTime gameTime)
        {

            if (GlideTimer.Elapsed)
            {

            }

            switch(CurrentMode)
            {
                case Mode.Flap:
                    FlapWings();
                    break;
                case Mode.Glide:
                    if (GlideTimer.Elapsed)
                    {
                        FlapTimer.Reset(Services.RandomMinMax(2.5f, 5.5f));
                        CurrentMode = Mode.Flap;
                        Wings[0].RotationVelocity.X = 6;
                        Wings[1].RotationVelocity.X = -6;
                    }

                    break;
            }

            Velocity.X = VelocityFromAngle(Rotation.Y, Speed).X;
            Velocity.Z = -VelocityFromAngle(Rotation.Y, Speed).Y;

            base.Update(gameTime);
        }

        void FlapWings()
        {
            if (FlapTimer.Elapsed)
            {
                GlideTimer.Reset(Services.RandomMinMax(4.5f, 8.5f));
                CurrentMode = Mode.Glide;

                for (int i = 0; i < 2; i++)
                {
                    Wings[i].Rotation.X = 0;
                    Wings[i].RotationVelocity.X = 0;
                }
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
