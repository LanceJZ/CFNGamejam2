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
    public class Ground : GameComponent, IBeginable, IUpdateableComponent, ILoadContent
    {
        List<AModel> GroundTiles;
        List<AModel> ExtraLargeRocks;
        List<AModel> LargeRocks;
        List<AModel> MedRocks;
        List<AModel> SmallRocks;
        XnaModel ExtraLargeRockModel;
        XnaModel LargeRockModel;
        XnaModel MedRockModel;
        XnaModel SmallRockModel;
        XnaModel GroundTileModel;

        int Border;

        public int TheBorder { get => Border; }

        public Ground(Game game) : base(game) //Water facing the player, mountains on sides and back.
        {
            GroundTiles = new List<AModel>();
            ExtraLargeRocks = new List<AModel>();
            LargeRocks = new List<AModel>();
            MedRocks = new List<AModel>();
            SmallRocks = new List<AModel>();

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
            GroundTileModel = Services.LoadModel("Ground");
            ExtraLargeRockModel = Services.LoadModel("ExtraLargeRock");
            LargeRockModel = Services.LoadModel("LargeRock");
            MedRockModel = Services.LoadModel("MedRock");
            SmallRockModel = Services.LoadModel("SmallRock");
        }

        public void BeginRun()
        {
            int numberTiles = 22;
            Border = 63 * numberTiles;

            for (int x = 0; x < numberTiles; x++)
            {
                for (int z = 0; z < numberTiles; z++)
                {
                    GroundTiles.Add(new AModel(Game));
                    GroundTiles.Last().SetModel(GroundTileModel);
                    GroundTiles.Last().Position.X = -Border + (126 * (x + 1));
                    GroundTiles.Last().Position.Z = -Border + (126 * (z + 1));
                    GroundTiles.Last().Position.Y = -11;
                }
            }

            for (int i = 0; i < 30; i++)
            {
                Vector3 loc;
                Vector3 rot;
                LargeRocks.Add(new AModel(Game));
                LargeRocks.Last().SetModel(LargeRockModel);
                loc = new Vector3(Services.RandomMinMax(-Border - 100, Border - 100),
                    Services.RandomMinMax(-20, -10), Services.RandomMinMax(-Border - 100, Border - 100));
                rot = new Vector3(Services.RandomMinMax(0, MathHelper.Pi),
                    Services.RandomMinMax(0, MathHelper.Pi), Services.RandomMinMax(0, MathHelper.Pi));
                LargeRocks.Last().Position = loc;
                LargeRocks.Last().Rotation = rot;
                MedRocks.Add(new AModel(Game));
                MedRocks.Last().SetModel(MedRockModel);
                loc = new Vector3(Services.RandomMinMax(-Border - 60, Border - 60),
                    Services.RandomMinMax(-15, -5), Services.RandomMinMax(-Border - 60, Border - 60));
                rot = new Vector3(Services.RandomMinMax(0, MathHelper.Pi),
                    Services.RandomMinMax(0, MathHelper.Pi), Services.RandomMinMax(0, MathHelper.Pi));
                MedRocks.Last().Position = loc;
                MedRocks.Last().Rotation = rot;
                SmallRocks.Add(new AModel(Game));
                SmallRocks.Last().SetModel(SmallRockModel);
                loc = new Vector3(Services.RandomMinMax(-Border, Border),
                    Services.RandomMinMax(-10, -2), Services.RandomMinMax(-Border, Border));
                rot = new Vector3(Services.RandomMinMax(0, MathHelper.Pi),
                    Services.RandomMinMax(0, MathHelper.Pi), Services.RandomMinMax(0, MathHelper.Pi));
                SmallRocks.Last().Position = loc;
                SmallRocks.Last().Rotation = rot;
            }

            for (int i = 0; i < 10; i++)
            {
                Vector3 loc;
                Vector3 rot;
                ExtraLargeRocks.Add(new AModel(Game));
                ExtraLargeRocks.Last().SetModel(ExtraLargeRockModel);
                loc = new Vector3(Services.RandomMinMax(-Border / 2, Border / 2),
                    Services.RandomMinMax(-30, -10), Services.RandomMinMax(-Border / 2, Border / 2));
                rot = new Vector3(Services.RandomMinMax(0, MathHelper.Pi),
                    Services.RandomMinMax(0, MathHelper.Pi), Services.RandomMinMax(0, MathHelper.Pi));
                ExtraLargeRocks.Last().Position = loc;
                ExtraLargeRocks.Last().Rotation = rot;
            }
        }

        public override void Update(GameTime gameTime)
        {

            base.Update(gameTime);
        }
    }
}
