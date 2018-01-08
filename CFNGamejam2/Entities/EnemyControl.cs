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
        MissileBattery TheBattery;

        public EnemyControl(Game game, GameLogic gameLogic) : base(game)
        {
            GameLogicRef = gameLogic;
            TheDuck = new Duck(game, gameLogic);
            TheBattery = new MissileBattery(game, gameLogic);

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
            TheDuck.Position.Y = 200;
            TheDuck.Position.Z = -600;
            TheDuck.Position.X = 300;

            TheBattery.Position.Y = 20;
            TheBattery.Position.Z = -400;
            TheBattery.Position.X = -200;

            TheBattery.Rotation.Y = -MathHelper.PiOver2;
        }

        public override void Update(GameTime gameTime)
        {

            base.Update(gameTime);
        }
    }
}
// Ducks control turrets, when destroyed they fly out.
// Tired of being hunted, they tank revenge.