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
    public class EnemyControl : GameComponent, IBeginable, IUpdateableComponent, ILoadContent
    {
        GameLogic GameLogicRef;
        Duck TheDuck;

        public EnemyControl(Game game, GameLogic gameLogic) : base(game)
        {
            GameLogicRef = gameLogic;
            TheDuck = new Duck(game);

            game.Components.Add(this);
        }

        public override void Initialize()
        {

            base.Initialize();
            Services.AddLoadable(this);
            Services.AddBeginable(this);
        }

        public void LoadContent()
        {

        }

        public void BeginRun()
        {
            TheDuck.Position.Y = 100;
            TheDuck.Position.Z = 100;
            TheDuck.Position.X = 100;
        }

        public override void Update(GameTime gameTime)
        {

            base.Update(gameTime);
        }
    }
}
