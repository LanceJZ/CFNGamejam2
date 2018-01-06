using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using XnaModel = Microsoft.Xna.Framework.Graphics.Model;
using System.Collections.Generic;
using System;
using Engine;
using CFNGamejam2.Entities;

namespace CFNGamejam2
{
    public enum GameState
    {
        Over,
        InPlay,
        HighScore,
        MainMenu
    };

    public class GameLogic : GameComponent, IBeginable, IUpdateableComponent, ILoadContent
    {
        Player ThePlayer;
        Ground TheGround;
        EnemyControl TheEnemy;

        GameState GameMode = GameState.MainMenu;

        float TheGameScale = 1.0f;
        int Score = 0;

        public Ground GroundRef { get => TheGround; }
        public EnemyControl EnemyRef { get => TheEnemy; }
        public GameState CurrentMode { get => GameMode; }

        public GameLogic(Game game) : base(game)
        {
            ThePlayer = new Player(game, this);
            TheGround = new Ground(game);
            TheEnemy = new EnemyControl(game, this);

            // Screen resolution is 1200 X 900.
            // Y positive on top of window. So down is negative.
            // X positive is right of window. So to the left is negative.
            // Z positive is towards the front. So to place objects behind other objects, put them in the negative.
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
            Services.Camera.Target = new Vector3(0, 50, 0);

        }

        public override void Update(GameTime gameTime)
        {
            GameStateSwitch();

            base.Update(gameTime);
        }

        public void SwitchToAttract()
        {
            GameMode = GameState.MainMenu;
        }

        public void GameOver()
        {
        }


        void GameStateSwitch()
        {
            switch (GameMode)
            {
                case GameState.InPlay:
                    GamePlay();
                    break;
                case GameState.MainMenu:
                    MainMenu();
                    break;
            }
        }

        public void NewGame()
        {
            Score = 0;
            GameMode = GameState.InPlay;
        }

        void MainMenu()
        {

        }

        void GamePlay()
        {

        }
    }
}
