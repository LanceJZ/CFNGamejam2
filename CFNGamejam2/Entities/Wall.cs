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
    public class Wall : AModel
    {
        GameLogic RefGameLogic;

        public Wall(Game game, GameLogic gameLogic) : base(game)
        {
            RefGameLogic = gameLogic;

            LoadContent();
        }

        public override void Initialize()
        {
            base.Initialize();


        }

        public void LoadContent()
        {

            BeginRun();
        }

        public override void BeginRun()
        {
            base.BeginRun();


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
