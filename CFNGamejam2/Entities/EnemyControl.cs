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
    public class EnemyControl : GameComponent, IBeginable, IUpdateableComponent, ILoadContent
    {
        GameLogic RefGameLogic;
        List<Duck> TheDucks;
        List<MissileBattery> TheBatterys;
        List<Wall> TheWall;
        XnaModel WallPiece;
        Gateway TheGateway;

        public List<MissileBattery> BatterysRef { get => TheBatterys; }

        public EnemyControl(Game game, GameLogic gameLogic) : base(game)
        {
            RefGameLogic = gameLogic;
            TheDucks = new List<Duck>();
            TheBatterys = new List<MissileBattery>();
            TheWall = new List<Wall>();
            TheGateway = new Gateway(game, gameLogic);

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
            WallPiece = Services.LoadModel("GateWall");
        }

        public void BeginRun()
        {
            TheGateway.Position.Z = -1000;
            TheGateway.Position.Y = 20;

            for (int i = 0; i < 10; i++)
            {
                Vector3 pos = new Vector3();
                pos.Z = TheGateway.Position.Z - 5;
                pos.Y = TheGateway.Position.Y;
                pos.X = (-30 + -63 + (i * -126));
                SpawnWall(pos);
                pos.X = (30 + 63 + (i * 126));
                SpawnWall(pos);
            }

            NewGame();
            GameOver();
        }

        public override void Update(GameTime gameTime)
        {

            base.Update(gameTime);
        }

        public void GameOver()
        {

        }

        public void NewGame()
        {
            foreach (Duck duck in TheDucks)
            {
                duck.Active = false;
            }

            int border = RefGameLogic.RefGround.TheBorder;

            for (int i = 0; i < 4; i++)
                SpawnDuck(new Vector3(Services.RandomMinMax(-border, border), 200, border + 500));

            foreach(MissileBattery battery in TheBatterys)
            {
                battery.Active = false;
            }

            for (int i = 0; i < 4; i++)
            {
                SpawnBattery(new Vector3(-150, 20, -700 + (i * 500)));
                SpawnBattery(new Vector3(150, 20, -700 + (i * 500)));
            }

            TheGateway.Spawn(TheGateway.Position);
        }

        void SpawnWall(Vector3 position)
        {
            TheWall.Add(new Wall(Game, RefGameLogic));
            TheWall.Last().SetModel(WallPiece);
            TheWall.Last().Position = position;
        }

        void SpawnDuck(Vector3 position)
        {
            bool newOne = true;
            int thisOne = 0;

            foreach(Duck duck in TheDucks)
            {
                if (!duck.Active)
                {
                    newOne = false;
                    break;
                }

                thisOne++;
            }

            if (newOne)
            {
                thisOne = TheDucks.Count;
                TheDucks.Add(new Duck(Game, RefGameLogic));
            }

            TheDucks[thisOne].Spawn(position);
        }

        void SpawnBattery(Vector3 position)
        {
            bool newOne = true;
            int thisOne = 0;

            foreach(MissileBattery battery in TheBatterys)
            {
                if (!battery.Active)
                {
                    newOne = false;
                    break;
                }

                thisOne++;
            }

            if (newOne)
            {
                thisOne = TheBatterys.Count;
                TheBatterys.Add(new MissileBattery(Game, RefGameLogic));
            }

            TheBatterys[thisOne].Spawn(position);
        }
    }
}
// Ducks control turrets, when destroyed they fly out.
// Tired of being hunted, they tank revenge.