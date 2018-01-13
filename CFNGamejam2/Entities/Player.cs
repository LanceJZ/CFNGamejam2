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
    public class Player : AModel
    {
        GameLogic RefGameLogic;
        List<TankShot> Shots;
        AModel HealthBar;
        AModel HealthBack;
        AModel Turret;
        AModel Gun;
        AModel[] Treads = new AModel[2];

        Timer PerShotTimer;

        KeyboardState LastKeyState;

        int Speed = 50;
        int Health = 10;
        bool WasBumped;

        public List<TankShot> ShotsRef { get => Shots; }

        public Player(Game game, GameLogic gameLogic) : base(game)
        {
            RefGameLogic = gameLogic;
            Shots = new List<TankShot>();
            Turret = new AModel(game);
            Gun = new AModel(game);
            HealthBar = new AModel(game);
            HealthBack = new AModel(game);

            for (int i = 0; i < 2; i++)
            {
                Treads[i] = new AModel(game);
            }

            PerShotTimer = new Timer(game, 1);
        }

        public override void Initialize()
        {

            base.Initialize();
        }

        public override void LoadContent()
        {
            LoadModel("TankBody");
            Turret.LoadModel("TankTurret");
            Gun.LoadModel("TankGun");
            HealthBar.LoadModel("Core/Cube");

            for (int i = 0; i < 2; i++)
            {
                Treads[i].LoadModel("TankTreads");
            }
        }

        public override void BeginRun()
        {
            base.BeginRun();

            Turret.Position.Y = 11.5f;
            Gun.Position.X = 13.5f;
            Gun.Position.Y = -2.25f;
            HealthBar.DefuseColor = new Vector3(0, 200, 0);
            HealthBack.Position.Y = 24;
            //HealthBack.Position.Z = -1; //If facing camera;
            HealthBack.ModelScale.Y = 10;
            HealthBack.ModelScale.Z = 0.5f;
            HealthBack.ModelScale.X = 0.5f;

            for (int i = 0; i < 2; i++)
            {
                Treads[i].Position.Y = -2.5f;
                Treads[i].Position.Z = 12 - (24 * i);
                Treads[i].AddAsChildOf(this, true, false);
            }

            Turret.AddAsChildOf(this, true, false);
            Gun.AddAsChildOf(Turret, true, false);
            HealthBar.AddAsChildOf(this, true, false);
            HealthBack.AddAsChildOf(this, true, false);

            GameOver();
        }

        public override void Update(GameTime gameTime)
        {
            InPlay();
            base.Update(gameTime);
        }

        public override void Draw()
        {
            base.Draw();
        }

        public void Bumped(Vector3 position)
        {
            Velocity = (Velocity * 0.1f) * -1;
            Velocity += VelocityFromVectorsY(position, Position, 75);
            WasBumped = true;
        }

        public void HitDamage(int damage)
        {
            Health -= damage;
            HealthBar.ModelScale.Y = Health / 2;
            HealthBar.Position.Y = 19.5f + HealthBar.ModelScale.Y;

            if (Health <= 0)
            {
                GameOver();
            }
        }

        public void NewGame()
        {
            Active = true;
            Gun.Rotation.Z = 0.1f;
            Turret.Rotation.Y = 0;
            Position.Y = 0;
            Rotation.Y = MathHelper.PiOver2;
            Health = 10;
            HitDamage(0);
        }

        void InPlay()
        {
            if (!WasBumped)
                Input();

            KeepInBorders();

            if (Gun.Rotation.Z < 0)
                Gun.Rotation.Z = 0;

            if (Gun.Rotation.Z > 0.333f)
                Gun.Rotation.Z = 0.333f;

            Services.Camera.Position.Z = 600 + Position.Z;
            Services.Camera.Target = Position;

            WasBumped = false;
        }

        void GameOver()
        {
            RefGameLogic.RefUI.GameOver();
            Position.Z = RefGameLogic.RefGround.TheBorder - 30;
            Services.Camera.Position.Z = 600 + Position.Z;
            Services.Camera.Target = Position;
            RefGameLogic.GameOver();
            Active = false;
        }

        void KeepInBorders()
        {
            int edge = 150;

            if (Position.X > RefGameLogic.RefGround.TheBorder - edge)
            {
                Position.X = RefGameLogic.RefGround.TheBorder - edge;
            }

            if (Position.X < -RefGameLogic.RefGround.TheBorder + edge)
            {
                Position.X = -RefGameLogic.RefGround.TheBorder + edge;
            }

            if (Position.Z > RefGameLogic.RefGround.TheBorder - 20)
            {
                Position.Z = RefGameLogic.RefGround.TheBorder - 20;
            }

            if (Position.Z < -RefGameLogic.RefGround.TheBorder)
            {
                Position.Z = -RefGameLogic.RefGround.TheBorder;
            }
        }

        void Input()
        {
            KeyboardState theKeyboard = Keyboard.GetState();

            if (theKeyboard.IsKeyDown(Keys.W))
                MoveForward();
            else if (theKeyboard.IsKeyDown(Keys.S))
                MoveBackward();
            else
                StopMovement();

            if (theKeyboard.IsKeyDown(Keys.Left))
                RotateTurretCW();
            else if (theKeyboard.IsKeyDown(Keys.Right))
                RotateTurretCC();
            else
                StopTurretRotation();

            if (theKeyboard.IsKeyDown(Keys.Up))
                PointGunUp();
            else if (theKeyboard.IsKeyDown(Keys.Down))
                PointGunDown();
            else
                StopGunRotation();

            if (theKeyboard.IsKeyDown(Keys.D))
                RotateCC();
            else if (theKeyboard.IsKeyDown(Keys.A))
                RotateCW();
            else
                StopRotation();

            if (!LastKeyState.IsKeyDown(Keys.Space) && theKeyboard.IsKeyDown(Keys.Space))
            {
                if (PerShotTimer.Elapsed)
                {
                    PerShotTimer.Reset();
                    FireShot();
                }
            }

            LastKeyState = theKeyboard;
        }

        void FireShot()
        {
            bool makeNew = true;
            int thisOne = 0;


            foreach (TankShot shot in Shots)
            {
                if (!shot.Active)
                {
                    makeNew = false;
                    break;
                }

                thisOne++;
            }

            if (makeNew)
            {
                Shots.Add(new TankShot(Game));
                thisOne = Shots.Count - 1;
            }

            Vector3 vel = VelocityFromAngle(new Vector2(Turret.WorldRotation.Y,
                Gun.Rotation.Z), 250);
            Shots[Shots.Count - 1].Spawn(Gun.TheWoldMatrix.Translation + (vel * 0.2f),
                Gun.Rotation, vel);
        }

        void MoveForward()
        {
            Velocity = VelocityFromAngleY(Rotation.Y, Speed);
        }

        void MoveBackward()
        {
            Velocity = VelocityFromAngleY(Rotation.Y, -Speed);
        }

        void RotateCW()
        {
            RotationVelocity.Y = 0.5f;
        }

        void RotateCC()
        {
            RotationVelocity.Y = -0.5f;
        }

        void StopRotation()
        {
            RotationVelocity = Vector3.Zero;
        }

        void StopMovement()
        {
            Velocity = Vector3.Zero;
        }

        void RotateTurretCW()
        {
            Turret.RotationVelocity.Y = 0.35f;
        }

        void RotateTurretCC()
        {
            Turret.RotationVelocity.Y = -0.35f;
        }

        void StopTurretRotation()
        {
            Turret.RotationVelocity.Y = 0;
        }

        void PointGunUp()
        {
            if (Gun.Rotation.Z < 0.3f)
                Gun.RotationVelocity.Z = 0.25f;
        }

        void PointGunDown()
        {
            if (Gun.Rotation.Z > 0)
                Gun.RotationVelocity.Z = -0.5f;
        }

        void StopGunRotation()
        {
            Gun.RotationVelocity.Z = 0;
        }
    }
}
