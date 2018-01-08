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
    public class ExplodeParticle : AModel
    {
        Timer LifeTimer;

        public ExplodeParticle(Game game) : base(game)
        {
            LifeTimer = new Timer(game);
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void LoadContent()
        {

        }

        public override void BeginRun()
        {

            base.BeginRun();
        }

        public override void Update(GameTime gameTime)
        {
            if (LifeTimer.Elapsed)
                Active = false;

            base.Update(gameTime);
        }

        public override void Spawn(Vector3 position)
        {
            Velocity = RandomVelocity(100);
            base.Spawn(position);
            Scale = Services.RandomMinMax(1, 2);
            LifeTimer.Reset(Services.RandomMinMax(0.1f, 1));
        }
    }
}
