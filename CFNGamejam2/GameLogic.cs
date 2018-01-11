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

        public Ground RefGround { get => TheGround; }
        public EnemyControl RefEnemy { get => TheEnemy; }
        public Player RefPlayer { get => ThePlayer; }
        public GameState CurrentMode { get => GameMode; }

        public GameLogic(Game game) : base(game)
        {
            TheGround = new Ground(game, this);
            ThePlayer = new Player(game, this);
            TheEnemy = new EnemyControl(game, this);

            // Screen resolution is 1200 X 900.
            // Y positive is Up.
            // X positive is right of window when camera is at rotation zero.
            // Z positive is towards the camera when at rotation zero.
            // Y positive rotation rotates CCW. Zero is facing X positive. Pi/2 faces Z negative.
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
        }

        public void BeginRun()
        {
            Services.Camera.Target = new Vector3(0, 50, 0);
            NewGame();
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
/*
 Make a clear path that is covered by the missile batteries. Make a "road less traveled"
 that goes around them, where most of the ducks are flying. The flying ducks will drop a bomb
 on the player, if they can.
*/

