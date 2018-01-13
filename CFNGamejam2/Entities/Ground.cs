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

        Vector3 Position;
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

            for (int i = 0; i < 10; i++)
            {
                ExtraLargeRocks.Add(new Rock(Game, RefGameLogic));
                ExtraLargeRocks.Last().SetModel(ExtraLargeRockModel);
                SpawnExtraLargeRock(ExtraLargeRocks.Last());
            }

            for (int i = 0; i < 30; i++)
            {
                LargeRocks.Add(new Rock(Game, RefGameLogic));
                LargeRocks.Last().SetModel(LargeRockModel);
                SpawnLargeRock(LargeRocks.Last());

                MedRocks.Add(new Rock(Game, RefGameLogic));
                MedRocks.Last().SetModel(MedRockModel);
                SpawnMedRock(MedRocks.Last());

                SmallRocks.Add(new Rock(Game, RefGameLogic));
                SmallRocks.Last().SetModel(SmallRockModel);
                SpawnSmallRock(SmallRocks.Last());
            }

            Vector3[] path = new Vector3[2];
            path[0] = new Vector3(0, -11, -Border);
            path[1] = new Vector3(0, -11, Border);

            ClearPath(path);
        }

        public override void Update(GameTime gameTime)
        {

            base.Update(gameTime);
        }

        public void ClearPath(Vector3[] path)
        {
            bool working = true;
            float radius = 150;

            while(working)
            {
                working = false;
                Position = path[0];

                foreach (Vector3 spot in path)
                {
                    while (!MoveTowardsPoint(spot))
                    {
                        foreach (Rock rock in ExtraLargeRocks)
                        {
                            if (rock.CirclesIntersect(spot, radius))
                            {
                                working = true;
                                SpawnExtraLargeRock(rock);
                            }
                        }

                        foreach (Rock rock in LargeRocks)
                        {
                            if (rock.CirclesIntersect(spot, radius))
                            {
                                working = true;
                                SpawnLargeRock(rock);
                            }
                        }

                        foreach (Rock rock in MedRocks)
                        {
                            if (rock.CirclesIntersect(spot, radius))
                            {
                                working = true;
                                SpawnMedRock(rock);
                            }
                        }

                        foreach (Rock rock in SmallRocks)
                        {
                            if (rock.CirclesIntersect(spot, radius))
                            {
                                working = true;
                                SpawnSmallRock(rock);
                            }
                        }
                    }
                }
            }
        }

        bool MoveTowardsPoint(Vector3 goal)
        {
            if (Position == goal)
                return true;

            Vector3 direction = Vector3.Normalize(goal - Position);
            Position += direction * 0.1f;

            //if (Math.Abs(Vector3.Dot(direction, Vector3.Normalize(goal - Position)) + 1) < 0.1f)
            //    Position = goal; //Does the same as below, but would take more CPU.

            if (Vector3.Distance(direction, Vector3.Normalize(Position - goal)) < 0.1f)
                Position = goal;

            return Position == goal;
        }

        void SpawnExtraLargeRock(Rock rock)
        {
            SpawnRock(rock, -30, -10, Border / 2);
        }

        void SpawnLargeRock(Rock rock)
        {
            SpawnRock(rock, -20, -10, Border - 100);
        }

        void SpawnMedRock(Rock rock)
        {
            SpawnRock(rock, -15, -5, Border - 60);
        }

        void SpawnSmallRock(Rock rock)
        {
            SpawnRock(rock, -10, -2, Border - 20);
        }

        void SpawnRock(Rock rock, int hightMin, int hightMax, int border)
        {
            rock.Spawn(new Vector3(Services.RandomMinMax(-border, border),
                Services.RandomMinMax(hightMin, hightMax), Services.RandomMinMax(-border, border)),
                new Vector3(Services.RandomMinMax(0, MathHelper.Pi),
                Services.RandomMinMax(0, MathHelper.Pi),
                Services.RandomMinMax(0, MathHelper.Pi)));
        }
    }
}
