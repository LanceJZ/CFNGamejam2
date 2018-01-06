using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using XnaModel = Microsoft.Xna.Framework.Graphics.Model;
using System.Collections.Generic;
using System.Linq;
using System;
using Engine;

namespace CFNGamejam2.Entities
{
    public class Explode : GameComponent, IBeginable, IUpdateableComponent, ILoadContent
    {
        List<ExplodeParticle> Particles;
        XnaModel Cube;
        bool IsActive;

        public bool Active { get => IsActive; }

        public Explode(Game game) : base(game)
        {
            Particles = new List<ExplodeParticle>();

            game.Components.Add(this);
            LoadContent();
            BeginRun();
        }

        public override void Initialize()
        {

            base.Initialize();
        }

        public void LoadContent()
        {
            Cube = Services.LoadModel("Core/Cube");
        }

        public void BeginRun()
        {

        }

        public override void Update(GameTime gameTime)
        {
            if (IsActive)
            {
                bool done = true;

                foreach (ExplodeParticle particle in Particles)
                {
                    if (particle.Active)
                    {
                        done = false;
                        break;
                    }
                }

                if (done)
                    IsActive = false;

                base.Update(gameTime);
            }
        }

        public void Spawn(Vector3 position, float radius, int minCount)
        {
            IsActive = true;
            int count = Services.RandomMinMax(minCount, (int)(minCount + radius * 2));

            if (count > Particles.Count)
            {
                int more = count - Particles.Count;

                for (int i = 0; i < more; i++)
                {
                    Particles.Add(new ExplodeParticle(Game));
                    Particles.Last().SetModel(Cube);
                }
            }

            foreach (ExplodeParticle particle in Particles)
            {
                position += new Vector3(Services.RandomMinMax(-radius, radius),
                    Services.RandomMinMax(-radius, radius), 0);

                particle.Spawn(position);
            }
        }

        public void Kill()
        {
            foreach (ExplodeParticle particle in Particles)
            {
                particle.Active = false;
                IsActive = false;
            }
        }
    }
}
