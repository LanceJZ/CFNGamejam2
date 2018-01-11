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
        GameLogic RefGameLogic;
        List<AModel> GroundTiles;
        List<AModel> MountainTiles;
        List<AModel> WaterTiles;
        List<Rock> ExtraLargeRocks;
        List<Rock> LargeRocks;
        List<Rock> MedRocks;
        List<Rock> SmallRocks;
        XnaModel ExtraLargeRockModel;
        XnaModel LargeRockModel;
        XnaModel MedRockModel;
        XnaModel SmallRockModel;
        XnaModel GroundTileModel;
        XnaModel MountainTilesModel;
        XnaModel WaterTileModel;

        int Border;

        public int TheBorder { get => Border; }

        public Ground(Game game, GameLogic gameLogic) : base(game) //Water facing the player, mountains on sides and back.
        {
            RefGameLogic = gameLogic;
            GroundTiles = new List<AModel>();
            MountainTiles = new List<AModel>();
            WaterTiles = new List<AModel>();
            ExtraLargeRocks = new List<Rock>();
            LargeRocks = new List<Rock>();
            MedRocks = new List<Rock>();
            SmallRocks = new List<Rock>();

            game.Components.Add(this);
        }

        public override void Initialize()
        {

            base.Initialize();
            LoadContent();
            BeginRun();
        }

        public void LoadContent()
        {
            GroundTileModel = Services.LoadModel("Ground");
            MountainTilesModel = Services.LoadModel("Mountain");
            WaterTileModel = Services.LoadModel("Water");
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
                    GroundTiles.Last().Position.X = -Border + (126 * (x + 0.5f));
                    GroundTiles.Last().Position.Z = -Border + (126 * (z + 0.5f));
                    GroundTiles.Last().Position.Y = -11;
                }
            }

            for (int i = 0; i < numberTiles * 0.25f; i++)
            {
                MountainTiles.Add(new AModel(Game));
                MountainTiles.Last().SetModel(MountainTilesModel);
                MountainTiles.Last().Position.X = -Border + (504 * i);
                MountainTiles.Last().Position.Y = 106;
                MountainTiles.Last().Position.Z = -Border + 40;
                MountainTiles.Last().ModelScale = new Vector3(4);
            }

            for (int i = 0; i < numberTiles * 0.25f + 2; i++)
            {
                MountainTiles.Add(new AModel(Game));
                MountainTiles.Last().SetModel(MountainTilesModel);
                MountainTiles.Last().Position.X = -Border + 40;
                MountainTiles.Last().Position.Y = 106;
                MountainTiles.Last().Position.Z = -Border + (504 * i);
                MountainTiles.Last().Rotation.Y = MathHelper.PiOver2;
                MountainTiles.Last().ModelScale = new Vector3(4);
            }

            for (int i = 0; i < numberTiles * 0.25f + 2; i++)
            {
                MountainTiles.Add(new AModel(Game));
                MountainTiles.Last().SetModel(MountainTilesModel);
                MountainTiles.Last().Position.X = Border - 40;
                MountainTiles.Last().Position.Y = 106;
                MountainTiles.Last().Position.Z = -Border + (504 * i);
                MountainTiles.Last().Rotation.Y = -MathHelper.PiOver2;
                MountainTiles.Last().ModelScale = new Vector3(4);
            }

            for (int z = 0; z < 7; z++)
            {
                for (int x = 0; x < numberTiles; x++)
                {
                    WaterTiles.Add(new AModel(Game));
                    WaterTiles.Last().SetModel(WaterTileModel);
                    WaterTiles.Last().Position.X = -Border + (126 * (x + 0.5f));
                    WaterTiles.Last().Position.Z = Border + (126 * (z + 0.5f));
                    WaterTiles.Last().Position.Y = -11;
                }
            }

            for (int i = 0; i < 30; i++)
            {
                Vector3 loc;
                Vector3 rot;
                LargeRocks.Add(new Rock(Game, RefGameLogic));
                LargeRocks.Last().SetModel(LargeRockModel);
                loc = new Vector3(Services.RandomMinMax(-Border - 100, Border - 100),
                    Services.RandomMinMax(-20, -10), Services.RandomMinMax(-Border - 100, Border - 100));
                rot = new Vector3(Services.RandomMinMax(0, MathHelper.Pi),
                    Services.RandomMinMax(0, MathHelper.Pi), Services.RandomMinMax(0, MathHelper.Pi));
                LargeRocks.Last().Spawn(loc, rot);
                MedRocks.Add(new Rock(Game, RefGameLogic));
                MedRocks.Last().SetModel(MedRockModel);
                loc = new Vector3(Services.RandomMinMax(-Border - 60, Border - 60),
                    Services.RandomMinMax(-15, -5), Services.RandomMinMax(-Border - 60, Border - 60));
                rot = new Vector3(Services.RandomMinMax(0, MathHelper.Pi),
                    Services.RandomMinMax(0, MathHelper.Pi), Services.RandomMinMax(0, MathHelper.Pi));
                MedRocks.Last().Spawn(loc, rot);
                SmallRocks.Add(new Rock(Game, RefGameLogic));
                SmallRocks.Last().SetModel(SmallRockModel);
                loc = new Vector3(Services.RandomMinMax(-Border, Border),
                    Services.RandomMinMax(-10, -2), Services.RandomMinMax(-Border, Border));
                rot = new Vector3(Services.RandomMinMax(0, MathHelper.Pi),
                    Services.RandomMinMax(0, MathHelper.Pi), Services.RandomMinMax(0, MathHelper.Pi));
                SmallRocks.Last().Spawn(loc, rot);
            }

            for (int i = 0; i < 10; i++)
            {
                Vector3 loc;
                Vector3 rot;
                ExtraLargeRocks.Add(new Rock(Game, RefGameLogic));
                ExtraLargeRocks.Last().SetModel(ExtraLargeRockModel);
                loc = new Vector3(Services.RandomMinMax(-Border / 2, Border / 2),
                    Services.RandomMinMax(-30, -10), Services.RandomMinMax(-Border / 2, Border / 2));
                rot = new Vector3(Services.RandomMinMax(0, MathHelper.Pi),
                    Services.RandomMinMax(0, MathHelper.Pi), Services.RandomMinMax(0, MathHelper.Pi));
                ExtraLargeRocks.Last().Spawn(loc, rot);
            }
        }

        public override void Update(GameTime gameTime)
        {

            base.Update(gameTime);
        }

        public void ClearPath(Vector3 start, Vector3 end)
        {

        }
    }
}
