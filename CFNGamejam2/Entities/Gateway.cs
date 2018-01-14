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
    public class Gateway : AModel
    {
        GameLogic RefGameLogic;
        AModel[] Doors = new AModel[2];
        Explode[] DoorExplosions = new Explode[2];
        AModel HealthBar;
        AModel HealthBack;

        SoundEffect ExplodeSound;

        int Health;

        public Gateway(Game game, GameLogic gameLogic) : base(game)
        {
            RefGameLogic = gameLogic;
            HealthBar = new AModel(game);
            HealthBack = new AModel(game);

            for (int i = 0; i < 2; i++)
            {
                Doors[i] = new AModel(game);
                DoorExplosions[i] = new Explode(game);
            }

            LoadContent();
        }

        public override void Initialize()
        {
            base.Initialize();

        }

        public void LoadContent()
        {
            LoadModel("GateFrame");
            HealthBar.LoadModel("Core/Cube");
            HealthBack.LoadModel("Core/Cube");

            for (int i = 0; i < 2; i++)
            {
                Doors[i].LoadModel("GateDoor");
            }

            ExplodeSound = LoadSoundEffect("GateExplode");

            BeginRun();
        }

        public override void BeginRun()
        {
            base.BeginRun();

            Doors[0].Position.X = -18;
            Doors[0].Position.Z = -6.5f;
            Doors[0].Position.Y = -6.5f;

            Doors[1].Position.X = 18;
            Doors[1].Position.Z = -6.5f;
            Doors[1].Position.Y = -6.5f;

            Doors[1].Rotation.Y = MathHelper.Pi;
            Doors[1].Rotation.X = MathHelper.Pi;

            for (int i = 0; i < 2; i++)
            {
                Doors[i].AddAsChildOf(this, true, false);
                DoorExplosions[i].DefuseColor = new Vector3(0.4f, 0.4f, 0.5f);
            }

            HealthBar.DefuseColor = new Vector3(0, 2, 0);
            HealthBar.ModelScale.X = 2;
            HealthBar.ModelScale.Z = 2;
            HealthBar.AddAsChildOf(this, true, false);
            HealthBack.Position.Y = 26;
            HealthBack.ModelScale.Y = 5;
            HealthBack.ModelScale.Z = 1;
            HealthBack.ModelScale.X = 1;
            HealthBack.DefuseColor = new Vector3(0.1f, 0.1f, 0.1f);
            HealthBack.AddAsChildOf(this, true, false);
        }

        public override void Update(GameTime gameTime)
        {
            if (HealthBar.Active)
            {
                CheckCollide();
            }
            else if (SphereIntersect(RefGameLogic.RefPlayer))
            {
                Doors[0].Rotation.Y = MathHelper.PiOver2;
                Doors[1].Rotation.Y = -MathHelper.PiOver2 + MathHelper.Pi;
            }

            base.Update(gameTime);
        }

        public override void Spawn(Vector3 position)
        {
            base.Spawn(position);

            Health = 10;
            HealthBar.ModelScale.Y = Health / 2;
            HealthBar.Position.Y = 20.5f + HealthBar.ModelScale.Y;
            HealthBar.Active = true;
            HealthBack.Active = true;

            foreach (AModel door in Doors)
            {
                door.Active = true;
            }

            Doors[0].Rotation.Y = 0;
            Doors[1].Rotation.Y = MathHelper.Pi;
            Doors[1].Rotation.X = MathHelper.Pi;
        }

        void CheckCollide()
        {
            if (SphereIntersect(RefGameLogic.RefPlayer))
            {
                RefGameLogic.RefPlayer.Bumped(Position);
            }

            foreach(TankShot shot in RefGameLogic.RefPlayer.ShotsRef)
            {
                if (shot.Active)
                {
                    foreach(AModel door in Doors)
                    {
                        if (door.SphereIntersect(shot))
                        {
                            Health -= 1;
                            HealthBar.ModelScale.Y = (Health / 2f);
                            HealthBar.Position.Y = 20.5f + HealthBar.ModelScale.Y;
                            shot.HitTarget();
                        }
                    }

                    if (Health <= 0)
                    {
                        int i = 0;

                        HealthBar.Active = false;
                        HealthBack.Active = false;

                        foreach(AModel door in Doors)
                        {
                            DoorExplosions[i].Spawn(door.Position, 10, 100);
                            i++;
                        }

                        //open door
                        Doors[0].Rotation.Y = MathHelper.PiOver4;
                        Doors[1].Rotation.Y = -MathHelper.PiOver4 + MathHelper.Pi;
                        ExplodeSound.Play();
                    }
                }
            }
        }
    }
}
