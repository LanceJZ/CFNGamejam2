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

        public Gateway(Game game, GameLogic gameLogic) : base(game)
        {
            RefGameLogic = gameLogic;

            for (int i = 0; i < 2; i++)
            {
                Doors[i] = new AModel(game);
            }
        }

        public override void Initialize()
        {
            base.Initialize();

        }

        public override void LoadContent()
        {
            LoadModel("GateFrame");

            for (int i = 0; i < 2; i++)
            {
                Doors[i].LoadModel("GateDoor");
            }
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

            //open door
            //Doors[0].Rotation.Y -= MathHelper.PiOver2;
            //Doors[1].Rotation.Y += MathHelper.PiOver2;

            for (int i = 0; i < 2; i++)
            {
                Doors[i].AddAsChildOf(this, true, false);
            }

        }

        public override void Update(GameTime gameTime)
        {
            CheckCollide();

            base.Update(gameTime);
        }

        public override void Spawn(Vector3 position)
        {
            base.Spawn(position);

        }

        void CheckCollide()
        {
            if (SphereIntersect(RefGameLogic.RefPlayer))
            {
                RefGameLogic.RefPlayer.Bumped(Position);
            }
        }
    }
}
