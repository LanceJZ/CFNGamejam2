using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using XnaModel = Microsoft.Xna.Framework.Graphics.Model;
using System.Collections.Generic;
using System;
using Engine;

namespace Engine
{
    public class SmokeParticle : AModel
    {
        Timer LifeTimer;

        public SmokeParticle(Game game) : base(game)
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
            base.Spawn(position);

            Scale = Services.RandomMinMax(1, 2);
            LifeTimer.Reset(Services.RandomMinMax(3.1f, 15.5f));
            Velocity.Y = Services.RandomMinMax(1.1f, 4.5f);
            float drift = 1.5f;
            Velocity.X = Services.RandomMinMax(-drift, drift);
            Velocity.Z = Services.RandomMinMax(-drift, drift);
        }
    }
}
