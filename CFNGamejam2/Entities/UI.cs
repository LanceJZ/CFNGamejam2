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
    public class UI : GameComponent, IBeginable, IUpdateableComponent, ILoadContent
    {
        GameLogic RefGameLogic;
        Words TheGameOverWords;
        Words TheStartANewGameWords;
        Words TheScoreWords;
        Numbers TheScore;

        public UI(Game game, GameLogic gameLogic) : base(game)
        {
            RefGameLogic = gameLogic;
            TheGameOverWords = new Words(game);
            TheStartANewGameWords = new Words(game);
            TheScoreWords = new Words(game);
            TheScore = new Numbers(game);

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
            string gameOver = "GAME OVER";
            string scoreTXT = "SCORE";
            string pressEnter = "PRESS ENTER TO START GAME";

            TheGameOverWords.ProcessWords(gameOver,
                new Vector3(-gameOver.Length * 20, 100, 1000), 4);
            TheStartANewGameWords.ProcessWords(pressEnter,
                new Vector3(-pressEnter.Length * 10, 50, 1000), 2);

            TheScoreWords.ProcessWords(scoreTXT, Vector3.Zero, 1);
            TheScore.ProcessNumber(0, Vector3.Zero, 1);

        }

        public override void Update(GameTime gameTime)
        {

            base.Update(gameTime);

            Vector3 viewPos = new Vector3();
            viewPos.X = RefGameLogic.RefPlayer.Position.X;
            viewPos.Z = RefGameLogic.RefPlayer.Position.Z;
            viewPos.Y = 35;

            Vector3 camRot = new Vector3();
            camRot.Y = -Services.Camera.View.Rotation.Y;
            camRot.X = -Services.Camera.View.Rotation.X;
            TheScore.Change(viewPos + new Vector3(50, 0, 0), camRot);
            TheScoreWords.Change(viewPos + new Vector3(-50, 0, 0), camRot);
        }

        public void ScoreUpdate(int score)
        {
            TheScore.ChangeNumber(score);
        }

        public void GameOver()
        {
            TheGameOverWords.ShowWords(true);
            TheStartANewGameWords.ShowWords(true);
        }

        public void NewGame()
        {
            TheGameOverWords.ShowWords(false);
            TheStartANewGameWords.ShowWords(false);
        }
    }
}
